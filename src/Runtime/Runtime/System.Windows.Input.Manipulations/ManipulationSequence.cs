using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	internal class ManipulationSequence
	{
		public event EventHandler<Manipulation2DStartedEventArgs> Started;

		public event EventHandler<Manipulation2DDeltaEventArgs> Delta;

		public event EventHandler<Manipulation2DCompletedEventArgs> Completed;

		public void ProcessManipulators(long timestamp, IEnumerable<Manipulator2D> manipulators, ManipulationSequence.ISettings settings)
		{
			if (this.processorState != ManipulationSequence.ProcessorState.Waiting)
			{
				if (timestamp - this.currentManipulationState.Timestamp < 0L)
				{
					throw Exceptions.InvalidTimestamp("timestamp", timestamp);
				}
			}
			this.OnProcessManipulators(timestamp, manipulators, settings);
		}

		public void CompleteManipulation(long timestamp)
		{
			if (this.processorState != ManipulationSequence.ProcessorState.Waiting)
			{
				if (timestamp - this.currentManipulationState.Timestamp < 0L)
				{
					throw Exceptions.InvalidTimestamp("timestamp", timestamp);
				}
			}
			this.OnCompleteManipulation(timestamp);
			if (this.manipulatorStates != null)
			{
				this.manipulatorStates.Clear();
			}
		}

		private bool IsPinned(ManipulationSequence.ISettings settings)
		{
			return !settings.SupportedManipulations.SupportsAny(Manipulations2D.Translate) && settings.Pivot != null && settings.Pivot.HasPosition;
		}

		private float GetVelocityX()
		{
			return ManipulationSequence.GetVelocity(this.history, (ManipulationSequence.ManipulationState item) => item.Position.X);
		}

		private float GetVelocityY()
		{
			return ManipulationSequence.GetVelocity(this.history, (ManipulationSequence.ManipulationState item) => item.Position.Y);
		}

		private float GetExpansionVelocity()
		{
			return ManipulationSequence.GetVelocity(this.history, (ManipulationSequence.ManipulationState item) => item.Expansion);
		}

		private float GetAngularVelocity()
		{
			ManipulationSequence.ManipulationState firstSample = (this.history.Count > 0) ? this.history.Peek() : default(ManipulationSequence.ManipulationState);
			return ManipulationSequence.GetVelocity(this.history, (ManipulationSequence.ManipulationState item) => ManipulationSequence.AdjustOrientation(item.Orientation, firstSample.Orientation));
		}

		private ManipulationVelocities2D GetVelocities()
		{
			return new ManipulationVelocities2D(new Func<float>(this.GetVelocityX), new Func<float>(this.GetVelocityY), new Func<float>(this.GetAngularVelocity), new Func<float>(this.GetExpansionVelocity));
		}

		private float GetSmoothOrientation()
		{
			float result;
			if (this.smoothing.Count > 1)
			{
				result = ManipulationSequence.CalculateMovingAverage(this.smoothing, (ManipulationSequence.ManipulationState item) => item.Orientation, 0f);
			}
			else
			{
				result = this.cumulativeRotation;
			}
			return result;
		}

		private float GetSmoothExpansion()
		{
			float result;
			if (this.smoothing.Count > 1)
			{
				result = ManipulationSequence.CalculateMovingAverage(this.smoothing, (ManipulationSequence.ManipulationState item) => item.Expansion, 0f);
			}
			else
			{
				result = this.cumulativeExpansion;
			}
			return result;
		}

		private float GetSmoothScale()
		{
			float num;
			if (this.smoothing.Count > 1)
			{
				num = ManipulationSequence.CalculateMovingAverage(this.smoothing, (ManipulationSequence.ManipulationState item) => item.Scale, 1f);
				if (float.IsInfinity(num))
				{
					num = this.cumulativeScale;
				}
			}
			else
			{
				num = this.cumulativeScale;
			}
			Debug.Assert(!float.IsNaN(num) && !float.IsInfinity(num));
			return num;
		}

		private void ExtractAndUpdateManipulators(IEnumerable<Manipulator2D> manipulators, long timestamp, out List<ManipulationSequence.ManipulatorState> addedManipulatorList, out List<int> removedManipulatorIds, out int currentManipulatorCount, out int updatedManipulatorCount)
		{
			updatedManipulatorCount = 0;
			currentManipulatorCount = 0;
			addedManipulatorList = null;
			removedManipulatorIds = ((this.manipulatorStates != null && this.manipulatorStates.Count > 0) ? new List<int>(this.manipulatorStates.Keys) : null);
			if (manipulators != null)
			{
				foreach (Manipulator2D manipulator2D in manipulators)
				{
					if (removedManipulatorIds != null)
					{
						removedManipulatorIds.Remove(manipulator2D.Id);
					}
					currentManipulatorCount++;
					ManipulationSequence.ManipulatorState manipulatorState;
					if (this.manipulatorStates == null || !this.manipulatorStates.TryGetValue(manipulator2D.Id, ref manipulatorState) || manipulatorState == null)
					{
						manipulatorState = ManipulationSequence.CreateManipulatorState(manipulator2D);
						if (addedManipulatorList == null)
						{
							addedManipulatorList = new List<ManipulationSequence.ManipulatorState>(20);
						}
						addedManipulatorList.Add(manipulatorState);
					}
					else if (manipulatorState.CurrentManipulatorSnapshot.X != manipulator2D.X || manipulatorState.CurrentManipulatorSnapshot.Y != manipulator2D.Y)
					{
						manipulatorState.CurrentManipulatorSnapshot = manipulator2D;
						this.log.AppendLine(string.Concat(new object[]
						{
							timestamp.ToString(CultureInfo.InvariantCulture),
							"\t",
							manipulatorState.CurrentManipulatorSnapshot.Id,
							"\tChanged\t",
							new PointF(manipulatorState.CurrentManipulatorSnapshot.X, manipulatorState.CurrentManipulatorSnapshot.Y)
						}));
						updatedManipulatorCount++;
					}
				}
			}
		}

		private void OnProcessManipulators(long timestamp, IEnumerable<Manipulator2D> manipulators, ManipulationSequence.ISettings settings)
		{
			if (this.log.Length >= 100000)
			{
				this.log.Length = 0;
			}
			VectorF previousTranslation = this.cumulativeTranslation;
			float previousSmoothedScale = this.smoothedCumulativeScale;
			float previousSmoothedExpansion = this.smoothedCumulativeExpansion;
			float previousSmoothedRotation = this.smoothedCumulativeRotation;
			List<ManipulationSequence.ManipulatorState> list;
			List<int> list2;
			int num;
			int num2;
			this.ExtractAndUpdateManipulators(manipulators, timestamp, out list, out list2, out num, out num2);
			if (num2 == 0 && (list == null || list.Count == 0) && (list2 == null || list2.Count == 0))
			{
				if (this.processorState != ManipulationSequence.ProcessorState.Waiting && num > 0 && this.history.Count > 0)
				{
					float scale;
					float expansion;
					float orientation;
					this.GetSmoothedDeltas(timestamp, settings, out scale, out expansion, out orientation);
					this.history.Enqueue(new ManipulationSequence.ManipulationState(ManipulationSequence.ZeroPoint, scale, expansion, orientation, timestamp), true);
					this.RaiseEvents(this.cumulativeTranslation, previousSmoothedScale, previousSmoothedExpansion, previousSmoothedRotation);
				}
			}
			else
			{
				if (list != null && list.Count > 0)
				{
					this.EnsureReadyToProcessManipulators(timestamp);
				}
				else
				{
					Debug.Assert(this.processorState == ManipulationSequence.ProcessorState.Manipulating, "Wrong state.");
				}
				if (num2 > 0)
				{
					this.CalculateTransforms(timestamp, settings);
				}
				this.ProcessAddsAndRemoves(timestamp, list, list2);
				if ((list != null && list.Count > 0) || (list2 != null && list2.Count > 0))
				{
					if (this.manipulatorStates == null || this.manipulatorStates.Count <= 0)
					{
						this.OnCompleteManipulation(timestamp);
						return;
					}
					this.OverwriteManipulationState(this.GetAveragePoint(), this.currentManipulationState.Scale, this.currentManipulationState.Expansion, this.currentManipulationState.Orientation, timestamp);
					this.SetVectorsFromPoint(this.currentManipulationState.Position, settings);
				}
				this.RaiseEvents(previousTranslation, previousSmoothedScale, previousSmoothedExpansion, previousSmoothedRotation);
			}
		}

		private void ProcessAddsAndRemoves(long timestamp, List<ManipulationSequence.ManipulatorState> addedManipulatorList, List<int> removedManipulatorIds)
		{
			if (addedManipulatorList != null && addedManipulatorList.Count > 0)
			{
				foreach (ManipulationSequence.ManipulatorState manipulatorState in addedManipulatorList)
				{
					this.log.AppendLine(string.Concat(new object[]
					{
						timestamp.ToString(CultureInfo.InvariantCulture),
						"\t",
						manipulatorState.Id,
						"\tAdded\t",
						new PointF(manipulatorState.InitialManipulatorSnapshot.X, manipulatorState.InitialManipulatorSnapshot.Y)
					}));
					this.AddManipulator(manipulatorState);
				}
			}
			if (removedManipulatorIds != null && removedManipulatorIds.Count > 0)
			{
				foreach (int num in removedManipulatorIds)
				{
					this.RemoveManipulator(num);
					this.log.AppendLine(string.Concat(new object[]
					{
						timestamp.ToString(CultureInfo.InvariantCulture),
						"\t",
						num,
						"\tRemoved"
					}));
				}
			}
		}

		private void RaiseEvents(VectorF previousTranslation, float previousSmoothedScale, float previousSmoothedExpansion, float previousSmoothedRotation)
		{
			if (this.processorState == ManipulationSequence.ProcessorState.Waiting)
			{
				this.processorState = ManipulationSequence.ProcessorState.Manipulating;
				Debug.Assert(this.Started != null, "Processor hasn't registered for Started event");
				this.Started.Invoke(this, new Manipulation2DStartedEventArgs(this.currentManipulationState.Position.X, this.currentManipulationState.Position.Y));
			}
			else
			{
				Debug.Assert(this.Delta != null, "Processor hasn't registered for Delta event");
				float num = this.smoothedCumulativeScale / previousSmoothedScale;
				float num2 = this.smoothedCumulativeExpansion - previousSmoothedExpansion;
				ManipulationDelta2D delta = new ManipulationDelta2D(this.cumulativeTranslation.X - previousTranslation.X, this.cumulativeTranslation.Y - previousTranslation.Y, this.smoothedCumulativeRotation - previousSmoothedRotation, num, num, num2, num2);
				ManipulationDelta2D cumulative = new ManipulationDelta2D(this.cumulativeTranslation.X, this.cumulativeTranslation.Y, this.smoothedCumulativeRotation, this.smoothedCumulativeScale, this.smoothedCumulativeScale, this.smoothedCumulativeExpansion, this.smoothedCumulativeExpansion);
				Manipulation2DDeltaEventArgs manipulation2DDeltaEventArgs = new Manipulation2DDeltaEventArgs(this.currentManipulationState.Position.X, this.currentManipulationState.Position.Y, this.GetVelocities(), delta, cumulative);
				this.Delta.Invoke(this, manipulation2DDeltaEventArgs);
			}
		}

		private void OnCompleteManipulation(long timestamp)
		{
			if (this.processorState != ManipulationSequence.ProcessorState.Waiting)
			{
				PointF position = this.currentManipulationState.Position;
				VectorF vectorF = this.cumulativeTranslation;
				float num = this.smoothedCumulativeScale;
				float num2 = this.smoothedCumulativeExpansion;
				float num3 = this.smoothedCumulativeRotation;
				this.processorState = ManipulationSequence.ProcessorState.Waiting;
				this.log.AppendLine(string.Concat(new object[]
				{
					timestamp.ToString(CultureInfo.InvariantCulture),
					"\tManipulation\tCompleted\t",
					position,
					"\t",
					num,
					"\t",
					num2,
					"\t",
					num3
				}));
				Debug.Assert(this.Completed != null, "Processor hasn't registered for Completed event");
				ManipulationDelta2D total = new ManipulationDelta2D(vectorF.X, vectorF.Y, num3, num, num, num2, num2);
				Manipulation2DCompletedEventArgs manipulation2DCompletedEventArgs = new Manipulation2DCompletedEventArgs(position.X, position.Y, this.GetVelocities(), total);
				this.Completed.Invoke(this, manipulation2DCompletedEventArgs);
				Debug.Assert(this.log.Length > 0);
			}
		}

		private void AddManipulator(ManipulationSequence.ManipulatorState initialState)
		{
			if (this.manipulatorStates == null)
			{
				this.manipulatorStates = new Dictionary<int, ManipulationSequence.ManipulatorState>();
			}
			if (!this.manipulatorStates.ContainsKey(initialState.Id))
			{
				this.manipulatorStates[initialState.Id] = initialState;
			}
		}

		private bool RemoveManipulator(int manipulatorId)
		{
			return this.manipulatorStates != null && this.manipulatorStates.Remove(manipulatorId);
		}

		private void EnsureReadyToProcessManipulators(long timestamp)
		{
			if (this.processorState == ManipulationSequence.ProcessorState.Waiting)
			{
				this.log.Length = 0;
				this.history.Clear();
				this.smoothing.Clear();
				this.InitializeManipulationState(timestamp);
			}
		}

		private void CalculateTransforms(long timestamp, ManipulationSequence.ISettings settings)
		{
			Debug.Assert(this.processorState == ManipulationSequence.ProcessorState.Manipulating, "Invalid state.");
			PointF averagePoint = this.GetAveragePoint();
			VectorF vectorF = new VectorF(averagePoint.X - this.currentManipulationState.Position.X, averagePoint.Y - this.currentManipulationState.Position.Y);
			float num = 0f;
			float num2 = 0f;
			float num3 = 1f;
			this.averageRadius = 0f;
			if (this.manipulatorStates != null)
			{
				if (this.manipulatorStates.Count > 1 && settings.SupportedManipulations.SupportsAny(Manipulations2D.Scale | Manipulations2D.Rotate))
				{
					this.CalculateMultiManipulatorRotationAndScale(averagePoint, ref num, ref num3, ref num2, settings);
				}
				else if (this.manipulatorStates.Count == 1 && settings.SupportedManipulations.SupportsAny(Manipulations2D.Rotate) && settings.Pivot != null && settings.Pivot.HasPosition)
				{
					num = this.CalculateSingleManipulatorRotation(averagePoint, this.currentManipulationState.Position, settings);
				}
			}
			if (!settings.SupportedManipulations.SupportsAny(Manipulations2D.TranslateX))
			{
				vectorF.X = 0f;
			}
			if (!settings.SupportedManipulations.SupportsAny(Manipulations2D.TranslateY))
			{
				vectorF.Y = 0f;
			}
			this.cumulativeTranslation += vectorF;
			this.cumulativeScale *= num3;
			this.cumulativeRotation += num;
			this.cumulativeExpansion += num2;
			this.cumulativeTranslation.X = ManipulationSequence.ForceFinite(this.cumulativeTranslation.X);
			this.cumulativeTranslation.Y = ManipulationSequence.ForceFinite(this.cumulativeTranslation.Y);
			this.cumulativeRotation = ManipulationSequence.ForceFinite(this.cumulativeRotation);
			this.cumulativeScale = ManipulationSequence.ForceFinite(this.cumulativeScale);
			this.cumulativeScale = ManipulationSequence.ForcePositive(this.cumulativeScale);
			this.cumulativeExpansion = ManipulationSequence.ForceFinite(this.cumulativeExpansion);
			float scale;
			float expansion;
			float orientation;
			this.GetSmoothedDeltas(timestamp, settings, out scale, out expansion, out orientation);
			if (this.history.Count == 0)
			{
				this.history.Enqueue(new ManipulationSequence.ManipulationState(this.currentManipulationState.Timestamp));
			}
			this.history.Enqueue(new ManipulationSequence.ManipulationState((PointF)vectorF, scale, expansion, orientation, timestamp));
			this.OverwriteManipulationState(averagePoint, this.cumulativeScale, this.cumulativeExpansion, this.cumulativeRotation, timestamp);
		}

		private static float ForceFinite(float value)
		{
			Debug.Assert(!double.IsNaN((double)value));
			float result;
			if (float.IsInfinity(value))
			{
				result = (float.IsNegativeInfinity(value) ? float.MinValue : float.MaxValue);
			}
			else
			{
				result = value;
			}
			return result;
		}
		private static float ForcePositive(float value)
		{
			float result;
			if (value < 1.1920929E-07f)
			{
				result = 1.1920929E-07f;
			}
			else
			{
				result = value;
			}
			return result;
		}

		private void GetSmoothedDeltas(long timestamp, ManipulationSequence.ISettings settings, out float smoothedScale, out float smoothedExpansion, out float smoothedRotation)
		{
			Debug.Assert(this.manipulatorStates != null);
			float minimumScaleRotateRadius = settings.MinimumScaleRotateRadius;
			float num = 10f * settings.MinimumScaleRotateRadius;
			float num2;
			if (this.manipulatorStates.Count < 2 || this.averageRadius < minimumScaleRotateRadius || this.averageRadius >= num)
			{
				num2 = 0f;
			}
			else
			{
				num2 = 1f - (this.averageRadius - minimumScaleRotateRadius) / (num - minimumScaleRotateRadius);
			}
			this.smoothing.SetSmoothingLevel((double)num2);
			this.smoothing.Enqueue(new ManipulationSequence.ManipulationState((PointF)this.cumulativeTranslation, this.cumulativeScale, this.cumulativeExpansion, this.cumulativeRotation, timestamp));
			float num3 = this.smoothedCumulativeRotation;
			this.smoothedCumulativeRotation = this.GetSmoothOrientation();
			smoothedRotation = this.smoothedCumulativeRotation - num3;
			Debug.Assert(!float.IsNaN(smoothedRotation) && !float.IsInfinity(smoothedRotation));
			float num4 = this.smoothedCumulativeExpansion;
			this.smoothedCumulativeExpansion = this.GetSmoothExpansion();
			smoothedExpansion = this.smoothedCumulativeExpansion - num4;
			Debug.Assert(!float.IsNaN(smoothedExpansion) && !float.IsInfinity(smoothedExpansion));
			float num5 = this.smoothedCumulativeScale;
			this.smoothedCumulativeScale = this.GetSmoothScale();
			smoothedScale = this.smoothedCumulativeScale / num5;
			Debug.Assert(!float.IsNaN(smoothedScale) && !float.IsInfinity(smoothedScale));
		}

	
		private void CalculateMultiManipulatorRotationAndScale(PointF averagePoint, ref float rotation, ref float scaleFactor, ref float expansion, ManipulationSequence.ISettings settings)
		{
			double num = 0.0;
			int num2 = 0;
			double num3 = 0.0;
			double num4 = 0.0;
			int num5 = 0;
			bool flag = this.IsPinned(settings);
			float minimumScaleRotateRadius = settings.MinimumScaleRotateRadius;
			foreach (KeyValuePair<int, ManipulationSequence.ManipulatorState> keyValuePair in this.manipulatorStates)
			{
				VectorF vectorFromManipulationOrigin = keyValuePair.Value.VectorFromManipulationOrigin;
				VectorF vectorF = new PointF(keyValuePair.Value.CurrentManipulatorSnapshot.X, keyValuePair.Value.CurrentManipulatorSnapshot.Y) - averagePoint;
				VectorF vectorFromPivotPoint = keyValuePair.Value.VectorFromPivotPoint;
				VectorF vectorF2 = flag ? new VectorF(keyValuePair.Value.CurrentManipulatorSnapshot.X - settings.Pivot.X, keyValuePair.Value.CurrentManipulatorSnapshot.Y - settings.Pivot.Y) : ManipulationSequence.ZeroVector;
				double num6 = (double)vectorFromManipulationOrigin.Length;
				double num7 = (double)vectorF.Length;
				if (num6 >= (double)minimumScaleRotateRadius && num7 >= (double)minimumScaleRotateRadius)
				{
					num3 += num6;
					num4 += num7;
					if (settings.SupportedManipulations.SupportsAny(Manipulations2D.Rotate) && (!flag || (vectorFromPivotPoint.Length >= minimumScaleRotateRadius && vectorF2.Length >= minimumScaleRotateRadius)))
					{
						VectorF vectorF3 = flag ? vectorFromPivotPoint : vectorFromManipulationOrigin;
						VectorF vectorF4 = flag ? vectorF2 : vectorF;
						double num8 = Math.Atan2((double)vectorF3.Y, (double)vectorF3.X);
						double num9 = Math.Atan2((double)vectorF4.Y, (double)vectorF4.X);
						double num10 = num9 - num8;
						if (num10 > 3.141592653589793)
						{
							num10 -= 6.283185307179586;
						}
						if (num10 < -3.141592653589793)
						{
							num10 += 6.283185307179586;
						}
						num += num10;
						num2++;
					}
					if (settings.SupportedManipulations.SupportsAny(Manipulations2D.Scale))
					{
						num5++;
					}
				}
				keyValuePair.Value.VectorFromManipulationOrigin = vectorF;
				keyValuePair.Value.VectorFromPivotPoint = vectorF2;
			}
			if (num2 > 0)
			{
				rotation = (float)(num / (double)num2);
				this.averageRadius = (float)(num4 / (double)num2);
			}
			if (num5 > 0 && num3 > 0.0)
			{
				scaleFactor = (float)(num4 / num3);
				expansion = (float)(num4 - num3) / (float)num5;
				this.averageRadius = (float)(num4 / (double)num5);
			}
		}

		private float CalculateSingleManipulatorRotation(PointF currentPosition, PointF previousPosition, ManipulationSequence.ISettings settings)
		{
			Debug.Assert(settings.Pivot != null, "don't call unless we have a settings.Pivot");
			Debug.Assert(settings.Pivot.HasPosition, "don't call unless there's a settings.Pivot location");
			bool flag = !this.IsPinned(settings);
			PointF point = new PointF(settings.Pivot.X, settings.Pivot.Y);
			VectorF vector = previousPosition - point;
			VectorF vector2 = currentPosition - point;
			float num = 1f;
			if (flag && !float.IsNaN(settings.Pivot.Radius))
			{
				num = (float)Math.Min(1.0, Math.Pow((double)(vector.Length / settings.Pivot.Radius), 4.0));
			}
			float num2 = VectorF.AngleBetween(vector, vector2);
			float result;
			if (float.IsNaN(num2))
			{
				result = 0f;
			}
			else
			{
				result = num2 * num;
			}
			return result;
		}

		private void InitializeManipulationState(long timestamp)
		{
			this.initialManipulationState = new ManipulationSequence.ManipulationState(timestamp);
			this.currentManipulationState = this.initialManipulationState;
			this.cumulativeTranslation = new VectorF(0f, 0f);
			this.cumulativeScale = 1f;
			this.cumulativeRotation = 0f;
			this.cumulativeExpansion = 0f;
			this.smoothedCumulativeRotation = 0f;
			this.smoothedCumulativeExpansion = 0f;
			this.smoothedCumulativeScale = 1f;
			this.averageRadius = 0f;
			this.log.AppendLine(string.Concat(new object[]
			{
				timestamp.ToString(CultureInfo.InvariantCulture),
				"\tManipulation\tInitialized\t",
				this.initialManipulationState.Position,
				"\t",
				this.initialManipulationState.Expansion,
				"\t",
				this.initialManipulationState.Orientation
			}));
		}

		private void OverwriteManipulationState(PointF position, float scale, float expansion, float orientation, long timestamp)
		{
			this.currentManipulationState = new ManipulationSequence.ManipulationState(position, scale, expansion, orientation, timestamp);
			this.log.AppendLine(string.Concat(new object[]
			{
				timestamp.ToString(CultureInfo.InvariantCulture),
				"\tManipulation\tUpdated\t",
				position,
				"\t",
				scale,
				"\t",
				expansion,
				"\t",
				orientation
			}));
		}

		private PointF GetAveragePoint()
		{
			Debug.Assert(this.manipulatorStates != null && this.manipulatorStates.Count > 0);
			float num = 0f;
			float num2 = 0f;
			foreach (KeyValuePair<int, ManipulationSequence.ManipulatorState> keyValuePair in this.manipulatorStates)
			{
				num += keyValuePair.Value.CurrentManipulatorSnapshot.X;
				num2 += keyValuePair.Value.CurrentManipulatorSnapshot.Y;
			}
			PointF result = new PointF(num / (float)this.manipulatorStates.Count, num2 / (float)this.manipulatorStates.Count);
			return result;
		}

		private void SetVectorsFromPoint(PointF referenceOrigin, ManipulationSequence.ISettings settings)
		{
			Debug.Assert(this.manipulatorStates != null && this.manipulatorStates.Count > 0);
			foreach (KeyValuePair<int, ManipulationSequence.ManipulatorState> keyValuePair in this.manipulatorStates)
			{
				keyValuePair.Value.VectorFromManipulationOrigin = new PointF(keyValuePair.Value.CurrentManipulatorSnapshot.X, keyValuePair.Value.CurrentManipulatorSnapshot.Y) - referenceOrigin;
				keyValuePair.Value.VectorFromPivotPoint = (this.IsPinned(settings) ? new VectorF(keyValuePair.Value.CurrentManipulatorSnapshot.X - settings.Pivot.X, keyValuePair.Value.CurrentManipulatorSnapshot.Y - settings.Pivot.Y) : ManipulationSequence.ZeroVector);
			}
		}

		private static float GetVelocity(Queue<ManipulationSequence.ManipulationState> queue, ManipulationSequence.PropertyAccessor accessor)
		{
			float num = ManipulationSequence.CalculateWeightedMovingAverage(queue, accessor);
			return num * 10000f;
		}

		private static float CalculateWeightedMovingAverage(Queue<ManipulationSequence.ManipulationState> queue, ManipulationSequence.PropertyAccessor accessor)
		{
			Debug.Assert(queue != null);
			int count = queue.Count;
			float result;
			if (count <= 1)
			{
				result = 0f;
			}
			else
			{
				int num = 0;
				float num2 = 0f;
				long num3 = 0L;
				foreach (ManipulationSequence.ManipulationState item in queue)
				{
					float num4 = accessor(item);
					if (num > 0)
					{
						long num5 = item.Timestamp - num3;
						Debug.Assert(num5 >= 10000L, "the queue should not contain samples with timeDelta < 1 msec.");
						float num6 = (float)num / (float)num5;
						num2 += num6 * num4;
					}
					num3 = item.Timestamp;
					num++;
				}
				if (num <= 1)
				{
					result = 0f;
				}
				else
				{
					float num6 = 2f / (float)(num * (num - 1));
					num2 *= num6;
					result = num2;
				}
			}
			return result;
		}

		private static float CalculateMovingAverage(Queue<ManipulationSequence.ManipulationState> queue, ManipulationSequence.PropertyAccessor accessor, float defaultValue)
		{
			Debug.Assert(queue != null);
			int count = queue.Count;
			float result;
			if (count < 1)
			{
				result = defaultValue;
			}
			else
			{
				float num = 0f;
				foreach (ManipulationSequence.ManipulationState item in queue)
				{
					num += accessor(item);
				}
				result = num / (float)count;
			}
			return result;
		}

		private static float AdjustOrientation(float value, float baseValue)
		{
			float num = value + 6.2831855f;
			float num2 = value - 6.2831855f;
			float num3 = Math.Abs(baseValue - value);
			if (Math.Abs(baseValue - num) < num3)
			{
				value = num;
			}
			else if (Math.Abs(baseValue - num2) < num3)
			{
				value = num2;
			}
			return value;
		}

		private static ManipulationSequence.ManipulatorState CreateManipulatorState(Manipulator2D manipulator)
		{
			ManipulationSequence.ManipulatorState manipulatorState = new ManipulationSequence.ManipulatorState(manipulator.Id);
			manipulatorState.InitialManipulatorSnapshot = manipulator;
			manipulatorState.CurrentManipulatorSnapshot = manipulatorState.InitialManipulatorSnapshot;
			return manipulatorState;
		}

		private const double singleManipulatorTorqueFactor = 4.0;

		private static readonly PointF ZeroPoint = new PointF(0f, 0f);

		private static readonly VectorF ZeroVector = new VectorF(0f, 0f);

		private StringBuilder log = new StringBuilder();

		private Dictionary<int, ManipulationSequence.ManipulatorState> manipulatorStates;

		private ManipulationSequence.HistoryQueue history = new ManipulationSequence.HistoryQueue();

		private ManipulationSequence.SmoothingQueue smoothing = new ManipulationSequence.SmoothingQueue();

		private ManipulationSequence.ProcessorState processorState;

		private ManipulationSequence.ManipulationState initialManipulationState;

		private ManipulationSequence.ManipulationState currentManipulationState;

		private VectorF cumulativeTranslation;

		private float cumulativeScale;

		private float cumulativeExpansion;

		private float cumulativeRotation;

		private float smoothedCumulativeScale;

		private float smoothedCumulativeRotation;

		private float smoothedCumulativeExpansion;

		private float averageRadius;

		internal interface ISettings
		{
			Manipulations2D SupportedManipulations { get; }

			ManipulationPivot2D Pivot { get; }
			float MinimumScaleRotateRadius { get; }
		}

		private class HistoryQueue : Queue<ManipulationSequence.ManipulationState>
		{
			public HistoryQueue() : base(5)
			{
			}

			public void Enqueue(ManipulationSequence.ManipulationState item)
			{
				this.Enqueue(item, false);
			}

			public void Enqueue(ManipulationSequence.ManipulationState item, bool stopMark)
			{
				if (this.previousTimestamp == null || item.Timestamp - this.previousTimestamp.Value >= 10000L)
				{
					if (stopMark)
					{
						this.stopMarkCount++;
						if (this.stopMarkCount > 5)
						{
							this.Clear();
							return;
						}
					}
					else
					{
						this.stopMarkCount = 0;
					}
					if (base.Count >= 5)
					{
						base.Dequeue();
					}
					while (base.Count > 0)
					{
						ManipulationSequence.ManipulationState manipulationState = base.Peek();
						long num = item.Timestamp - manipulationState.Timestamp;
						if (num <= 2000000L)
						{
							break;
						}
						base.Dequeue();
					}
					base.Enqueue(item);
					this.previousTimestamp = new long?(item.Timestamp);
				}
			}

			public void Clear()
			{
				base.Clear();
				this.previousTimestamp = default(long?);
			}

			private const int MaxHistoryLength = 5;

			private const int MaxHistoryDuration = 200;

			private const long MaxTimestampDelta = 2000000L;

			private long? previousTimestamp;

			private int stopMarkCount;
		}

		private class SmoothingQueue : Queue<ManipulationSequence.ManipulationState>
		{
			public void SetSmoothingLevel(double smoothingLevel)
			{
				smoothingLevel = Math.Max(0.0, Math.Min(1.0, smoothingLevel));
				int num = (int)Math.Round(smoothingLevel * 9.0);
				if (num != this.historyLength)
				{
					this.historyLength = num;
					this.maxTimestampDelta = (long)(smoothingLevel * 200.0 * 10000.0);
					if (this.historyLength <= 1)
					{
						this.Clear();
					}
					else
					{
						while (base.Count > this.historyLength)
						{
							base.Dequeue();
						}
					}
				}
			}

			private void Fill(ManipulationSequence.ManipulationState item, long timestamp)
			{
				long num = 150000L;
				for (int i = -this.historyLength + 1; i < 0; i++)
				{
					item.Timestamp = timestamp + num * (long)i;
					base.Enqueue(item);
				}
			}

			public void Enqueue(ManipulationSequence.ManipulationState item)
			{
				if (this.historyLength > 1)
				{
					long timestamp = item.Timestamp;
					if (base.Count == 0)
					{
						this.Fill(this.lastItem, timestamp);
					}
					else if (timestamp - this.lastItem.Timestamp > this.maxTimestampDelta)
					{
						base.Clear();
						this.Fill(this.lastItem, timestamp);
					}
					else
					{
						if (base.Count >= this.historyLength)
						{
							base.Dequeue();
						}
						while (base.Count > 0)
						{
							long num = timestamp - base.Peek().Timestamp;
							if (num <= this.maxTimestampDelta)
							{
								break;
							}
							base.Dequeue();
						}
					}
					base.Enqueue(item);
				}
				this.lastItem = item;
			}

			public void Clear()
			{
				base.Clear();
				this.lastItem = new ManipulationSequence.ManipulationState(0L);
			}

			private const int MaxHistoryLength = 9;

			private const int MaxHistoryDuration = 200;

			private int historyLength;

			private long maxTimestampDelta;

			private ManipulationSequence.ManipulationState lastItem;
		}

		private class ManipulatorState
		{
			public ManipulatorState(int manipulatorId)
			{
				this.manipulatorId = manipulatorId;
			}
			public int Id
			{
				get
				{
					return this.manipulatorId;
				}
			}

			public Manipulator2D InitialManipulatorSnapshot
			{
				get
				{
					return this.initialManipulatorSnapshot;
				}
				set
				{
					this.initialManipulatorSnapshot = value;
				}
			}

			public Manipulator2D CurrentManipulatorSnapshot
			{
				get
				{
					return this.currentManipulatorSnapshot;
				}
				set
				{
					this.currentManipulatorSnapshot = value;
				}
			}

			public VectorF VectorFromManipulationOrigin
			{
				get
				{
					return this.vectorFromManipulationOrigin;
				}
				set
				{
					this.vectorFromManipulationOrigin = value;
				}
			}

			public VectorF VectorFromPivotPoint
			{
				get
				{
					return this.vectorFromPivotPoint;
				}
				set
				{
					this.vectorFromPivotPoint = value;
				}
			}

			private readonly int manipulatorId;

			private Manipulator2D initialManipulatorSnapshot;

			private Manipulator2D currentManipulatorSnapshot;

			private VectorF vectorFromManipulationOrigin;

			private VectorF vectorFromPivotPoint;
		}

		private struct ManipulationState
		{
			public ManipulationState(PointF position, float scale, float expansion, float orientation, long timestamp)
			{
				Debug.Assert(!float.IsNaN(position.X) && !float.IsNaN(position.Y));
				Debug.Assert(!float.IsInfinity(position.Y) && !float.IsInfinity(position.Y));
				Debug.Assert(!float.IsNaN(scale) && !float.IsInfinity(scale) && scale > 0f);
				Debug.Assert(!float.IsNaN(expansion) && !float.IsInfinity(expansion));
				Debug.Assert(!float.IsNaN(orientation) && !float.IsInfinity(orientation));
				this.Position = position;
				this.Scale = scale;
				this.Expansion = expansion;
				this.Orientation = orientation;
				this.Timestamp = timestamp;
			}

			public ManipulationState(long timestamp)
			{
				this = new ManipulationSequence.ManipulationState(ManipulationSequence.ZeroPoint, 1f, 0f, 0f, timestamp);
			}

			public readonly PointF Position;

			public readonly float Scale;

			public readonly float Expansion;

			public readonly float Orientation;

			public long Timestamp;
		}

		private enum ProcessorState
		{
			Waiting,
			Manipulating
		}

		private delegate float PropertyAccessor(ManipulationSequence.ManipulationState item);
	}
}