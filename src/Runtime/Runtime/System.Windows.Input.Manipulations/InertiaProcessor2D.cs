using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	public class InertiaProcessor2D
	{
		public InertiaProcessor2D()
		{
			this.initialExpansion.MinBound = 1.0;
			this.initialExpansion.Value = 1.0;
			this.TranslationBehavior = new InertiaTranslationBehavior2D();
			this.RotationBehavior = new InertiaRotationBehavior2D();
			this.ExpansionBehavior = new InertiaExpansionBehavior2D();
		}

		public float InitialOriginX
		{
			get
			{
				return (float)this.initialTranslationX.Value;
			}
			set
			{
				this.CheckNotRunning("InitialOriginX");
				InertiaProcessor2D.CheckOriginalValue(value, "InitialOriginX");
				this.Reset();
				this.initialTranslationX.Value = (double)value;
			}
		}

		public float InitialOriginY
		{
			get
			{
				return (float)this.initialTranslationY.Value;
			}
			set
			{
				this.CheckNotRunning("InitialOriginY");
				InertiaProcessor2D.CheckOriginalValue(value, "InitialOriginY");
				this.Reset();
				this.initialTranslationY.Value = (double)value;
			}
		}

		public bool IsRunning
		{
			get
			{
				return this.processorState == InertiaProcessor2D.ProcessorState.Running;
			}
		}

		public InertiaTranslationBehavior2D TranslationBehavior
		{
			get
			{
				return this.translationBehavior;
			}
			set
			{
				this.SetBehavior<InertiaTranslationBehavior2D>(ref this.translationBehavior, value, new Action<InertiaParameters2D, string>(this.OnTranslationBehaviorChanged), "TranslationBehavior");
			}
		}

		public InertiaRotationBehavior2D RotationBehavior
		{
			get
			{
				return this.rotationBehavior;
			}
			set
			{
				this.SetBehavior<InertiaRotationBehavior2D>(ref this.rotationBehavior, value, new Action<InertiaParameters2D, string>(this.OnRotationBehaviorChanged), "RotationBehavior");
			}
		}

		public InertiaExpansionBehavior2D ExpansionBehavior
		{
			get
			{
				return this.expansionBehavior;
			}
			set
			{
				this.SetBehavior<InertiaExpansionBehavior2D>(ref this.expansionBehavior, value, new Action<InertiaParameters2D, string>(this.OnExpansionBehaviorChanged), "ExpansionBehavior");
			}
		}

		public event EventHandler<Manipulation2DDeltaEventArgs> Delta;

		public event EventHandler<Manipulation2DCompletedEventArgs> Completed;

		public bool Process(long timestamp)
		{
			return this.Process(timestamp, false);
		}

		public void Complete(long timestamp)
		{
			bool flag = this.Process(timestamp, true);
			Debug.Assert(!flag, "Complete method is supposed to raise Completed event.");
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetParameters(InertiaParameters2D parameters)
		{
			Validations.CheckNotNull(parameters, "parameters");
			InertiaTranslationBehavior2D inertiaTranslationBehavior2D = parameters as InertiaTranslationBehavior2D;
			if (inertiaTranslationBehavior2D != null)
			{
				this.TranslationBehavior = inertiaTranslationBehavior2D;
			}
			else
			{
				InertiaRotationBehavior2D inertiaRotationBehavior2D = parameters as InertiaRotationBehavior2D;
				if (inertiaRotationBehavior2D != null)
				{
					this.RotationBehavior = inertiaRotationBehavior2D;
				}
				else
				{
					InertiaExpansionBehavior2D inertiaExpansionBehavior2D = parameters as InertiaExpansionBehavior2D;
					if (inertiaExpansionBehavior2D != null)
					{
						this.ExpansionBehavior = inertiaExpansionBehavior2D;
					}
					else
					{
						Debug.Assert(false, "Unsupported parameters");
					}
				}
			}
		}

		private void SetInitialTimestamp(long timestamp)
		{
			Debug.Assert(!this.IsRunning);
			if (timestamp != this.initialTimestamp)
			{
				this.Reset();
				this.initialTimestamp = timestamp;
				this.previousTimestamp = timestamp;
			}
		}

		private void Reset()
		{
			if (this.processorState != InertiaProcessor2D.ProcessorState.NotInitialized)
			{
				this.processorState = InertiaProcessor2D.ProcessorState.NotInitialized;
				this.previousTimestamp = this.initialTimestamp - 1L;
			}
		}

		private void CheckNotRunning(string paramName)
		{
			if (this.IsRunning)
			{
				throw Exceptions.CannotChangeParameterDuringInertia(paramName);
			}
		}

		private void SetBehavior<TBehavior>(ref TBehavior currentBehavior, TBehavior newBehavior, Action<InertiaParameters2D, string> handler, string propertyName) where TBehavior : InertiaParameters2D
		{
			Debug.Assert(handler != null);
			if (!object.ReferenceEquals(newBehavior, currentBehavior))
			{
				if (currentBehavior != null)
				{
					currentBehavior.Changed -= handler;
					currentBehavior = default(TBehavior);
				}
				this.Reset();
				if (newBehavior != null)
				{
					currentBehavior = newBehavior;
					currentBehavior.Changed += handler;
				}
				handler.Invoke(currentBehavior, propertyName);
			}
		}

		private void OnTranslationBehaviorChanged(InertiaParameters2D parameters, string paramName)
		{
			this.CheckNotRunning(paramName);
			this.Reset();
			InertiaTranslationBehavior2D behavior = InertiaProcessor2D.GetBehavior<InertiaTranslationBehavior2D>(parameters);
			this.desiredDeceleration = (double)behavior.DesiredDeceleration;
			this.desiredDisplacement = (double)behavior.DesiredDisplacement;
			this.initialTranslationX.Velocity = (double)behavior.InitialVelocityX;
			this.initialTranslationY.Velocity = (double)behavior.InitialVelocityY;
		}

		private void OnRotationBehaviorChanged(InertiaParameters2D parameters, string paramName)
		{
			this.CheckNotRunning(paramName);
			this.Reset();
			InertiaRotationBehavior2D behavior = InertiaProcessor2D.GetBehavior<InertiaRotationBehavior2D>(parameters);
			this.initialOrientation.AbsoluteDeceleration = (double)behavior.DesiredDeceleration;
			this.initialOrientation.AbsoluteOffset = (double)behavior.DesiredRotation;
			this.initialOrientation.Velocity = (double)behavior.InitialVelocity;
		}

		private void OnExpansionBehaviorChanged(InertiaParameters2D parameters, string paramName)
		{
			this.CheckNotRunning(paramName);
			this.Reset();
			InertiaExpansionBehavior2D behavior = InertiaProcessor2D.GetBehavior<InertiaExpansionBehavior2D>(parameters);
			this.initialExpansion.Value = (double)behavior.InitialRadius;
			this.initialExpansion.AbsoluteDeceleration = (double)behavior.DesiredDeceleration;
			this.initialExpansion.AbsoluteOffset = (double)((behavior == null) ? float.NaN : behavior.DesiredExpansionX);
			this.initialExpansion.Velocity = (double)((behavior == null) ? float.NaN : behavior.InitialVelocityX);
		}

		private static TBehavior GetBehavior<TBehavior>(InertiaParameters2D parameters) where TBehavior : InertiaParameters2D, new()
		{
			TBehavior result;
			if (parameters == null)
			{
				result = Activator.CreateInstance<TBehavior>();
			}
			else
			{
				result = (TBehavior)((object)parameters);
			}
			return result;
		}

		private ManipulationVelocities2D GetVelocities()
		{
			ManipulationVelocities2D result;
			switch (this.processorState)
			{
				case InertiaProcessor2D.ProcessorState.Running:
				case InertiaProcessor2D.ProcessorState.Completing:
					{
						long num = this.previousTimestamp - this.initialTimestamp;
						if (num < 0L)
						{
							result = ManipulationVelocities2D.Zero;
						}
						else
						{
							result = new ManipulationVelocities2D(this.translationX.GetVelocity(num), this.translationY.GetVelocity(num), this.orientation.GetVelocity(num), this.expansion.GetVelocity(num));
						}
						break;
					}
				default:
					result = ManipulationVelocities2D.Zero;
					break;
			}
			return result;
		}

		private static ManipulationDelta2D GetIncrementalDelta(InertiaProcessor2D.ExtrapolatedValue translationX, InertiaProcessor2D.ExtrapolatedValue translationY, InertiaProcessor2D.ExtrapolatedValue orientation, InertiaProcessor2D.ExtrapolatedValue expansion, double scaleDelta)
		{
			return new ManipulationDelta2D((float)translationX.Delta, (float)translationY.Delta, (float)orientation.Delta, (float)scaleDelta, (float)scaleDelta, (float)expansion.Delta, (float)expansion.Delta);
		}

		private static ManipulationDelta2D GetCumulativeDelta(InertiaProcessor2D.ExtrapolatedValue translationX, InertiaProcessor2D.ExtrapolatedValue translationY, InertiaProcessor2D.ExtrapolatedValue orientation, InertiaProcessor2D.ExtrapolatedValue expansion, double totalScale)
		{
			return new ManipulationDelta2D((float)translationX.Total, (float)translationY.Total, (float)orientation.Total, (float)totalScale, (float)totalScale, (float)expansion.Total, (float)expansion.Total);
		}

		private void Prepare()
		{
			this.log.Length = 0;
			if (!double.IsNaN(this.desiredDisplacement))
			{
				Debug.Assert(double.IsNaN(this.desiredDeceleration), "desiredDisplacement and desiredDeceleration are mutually exclusive.");
				VectorD absoluteVector = InertiaProcessor2D.GetAbsoluteVector(this.desiredDisplacement, new VectorD(this.initialTranslationX.Velocity, this.initialTranslationY.Velocity));
				this.initialTranslationX.AbsoluteOffset = Math.Abs(absoluteVector.X);
				this.initialTranslationY.AbsoluteOffset = Math.Abs(absoluteVector.Y);
				this.initialTranslationX.AbsoluteDeceleration = double.NaN;
				this.initialTranslationY.AbsoluteDeceleration = double.NaN;
			}
			else if (!double.IsNaN(this.desiredDeceleration))
			{
				Debug.Assert(double.IsNaN(this.desiredDisplacement), "desiredDisplacement and desiredDeceleration are mutually exclusive.");
				VectorD absoluteVector2 = InertiaProcessor2D.GetAbsoluteVector(this.desiredDeceleration, new VectorD(this.initialTranslationX.Velocity, this.initialTranslationY.Velocity));
				this.initialTranslationX.AbsoluteDeceleration = Math.Abs(absoluteVector2.X);
				this.initialTranslationY.AbsoluteDeceleration = Math.Abs(absoluteVector2.Y);
				this.initialTranslationX.AbsoluteOffset = double.NaN;
				this.initialTranslationY.AbsoluteOffset = double.NaN;
			}
			this.translationX = this.Prepare(this.initialTranslationX, "translationX");
			this.translationY = this.Prepare(this.initialTranslationY, "translationY");
			this.orientation = this.Prepare(this.initialOrientation, "orientation");
			this.expansion = this.Prepare(this.initialExpansion, "expansion");
			if (this.translationX.ExtrapolationResult == InertiaProcessor2D.ExtrapolationResult.Skip && this.translationY.ExtrapolationResult == InertiaProcessor2D.ExtrapolationResult.Skip && this.orientation.ExtrapolationResult == InertiaProcessor2D.ExtrapolationResult.Skip && this.expansion.ExtrapolationResult == InertiaProcessor2D.ExtrapolationResult.Skip)
			{
				throw Exceptions.NoInertiaVelocitiesSpecified("TranslationBehavior.InitialVelocityX", "TranslationBehavior.InitialVelocityY", "RotationBehavior.InitialVelocity", "ExpansionBehavior.InitialVelocityX", "ExpansionBehavior.InitialVelocityY");
			}
		}

		private InertiaProcessor2D.ExtrapolationState Prepare(InertiaProcessor2D.InitialState initialState, string dimension)
		{
			this.LogLine("PREPARE: " + dimension);
			InertiaProcessor2D.ExtrapolationState extrapolationState = new InertiaProcessor2D.ExtrapolationState(initialState);
			if (extrapolationState.ExtrapolationResult != InertiaProcessor2D.ExtrapolationResult.Skip)
			{
				Debug.Assert(!double.IsNaN(extrapolationState.Offset) || !double.IsNaN(extrapolationState.AbsoluteDeceleration), "Either offset or deceleration should have been set by now");
				if (DoubleUtil.IsZero(extrapolationState.InitialVelocity))
				{
					extrapolationState.InitialVelocity = 0.0;
					extrapolationState.Duration = 0.0;
					extrapolationState.Offset = 0.0;
					extrapolationState.AbsoluteDeceleration = 0.0;
				}
				else
				{
					Debug.Assert(!DoubleUtil.IsZero(extrapolationState.InitialVelocity));
					if (!double.IsNaN(extrapolationState.Offset))
					{
						if (DoubleUtil.IsZero(extrapolationState.Offset))
						{
							extrapolationState.Offset = 0.0;
						}
						extrapolationState.Duration = 2.0 * Math.Abs(extrapolationState.Offset / extrapolationState.InitialVelocity);
						if (DoubleUtil.IsZero(extrapolationState.Duration))
						{
							extrapolationState.Duration = 0.0;
							extrapolationState.AbsoluteDeceleration = double.PositiveInfinity;
						}
						else
						{
							extrapolationState.AbsoluteDeceleration = Math.Abs(extrapolationState.InitialVelocity) / extrapolationState.Duration;
						}
					}
					if (DoubleUtil.IsZero(extrapolationState.AbsoluteDeceleration))
					{
						extrapolationState.AbsoluteDeceleration = 0.0;
						extrapolationState.Duration = double.PositiveInfinity;
						extrapolationState.Offset = ((extrapolationState.InitialVelocity > 0.0) ? double.PositiveInfinity : double.NegativeInfinity);
					}
					else if (double.IsNaN(extrapolationState.Offset))
					{
						extrapolationState.Duration = Math.Abs(extrapolationState.InitialVelocity) / extrapolationState.AbsoluteDeceleration;
						extrapolationState.Offset = extrapolationState.InitialVelocity * extrapolationState.Duration * 0.5;
					}
					Debug.Assert(extrapolationState.Duration >= 0.0);
					Debug.Assert(!double.IsNaN(extrapolationState.Deceleration));
					Debug.Assert(!double.IsNaN(extrapolationState.Offset));
				}
			}
			this.LogLine(extrapolationState.ToString());
			this.LogLine("");
			extrapolationState.AssertValid();
			return extrapolationState;
		}

		private bool ExtrapolateAndRaiseEvents(long timestamp, bool forceCompleted)
		{
			Debug.Assert(this.processorState == InertiaProcessor2D.ProcessorState.Running);
			long num = timestamp - this.initialTimestamp;
			if (num < 0L)
			{
				num = long.MaxValue;
				forceCompleted = true;
				Debug.WriteLine("Too long extrapolation, stop it.");
			}
			InertiaProcessor2D.ExtrapolatedValue extrapolatedValueAndUpdateState = InertiaProcessor2D.GetExtrapolatedValueAndUpdateState(this.translationX, (double)num);
			InertiaProcessor2D.ExtrapolatedValue extrapolatedValueAndUpdateState2 = InertiaProcessor2D.GetExtrapolatedValueAndUpdateState(this.translationY, (double)num);
			InertiaProcessor2D.ExtrapolatedValue extrapolatedValueAndUpdateState3 = InertiaProcessor2D.GetExtrapolatedValueAndUpdateState(this.orientation, (double)num);
			InertiaProcessor2D.ExtrapolatedValue extrapolatedValueAndUpdateState4 = InertiaProcessor2D.GetExtrapolatedValueAndUpdateState(this.expansion, (double)num);
			bool flag = forceCompleted || (extrapolatedValueAndUpdateState.ExtrapolationResult != InertiaProcessor2D.ExtrapolationResult.Continue && extrapolatedValueAndUpdateState2.ExtrapolationResult != InertiaProcessor2D.ExtrapolationResult.Continue && extrapolatedValueAndUpdateState3.ExtrapolationResult != InertiaProcessor2D.ExtrapolationResult.Continue && extrapolatedValueAndUpdateState4.ExtrapolationResult != InertiaProcessor2D.ExtrapolationResult.Continue);
			if (flag)
			{
				this.processorState = InertiaProcessor2D.ProcessorState.Completing;
				EventHandler<Manipulation2DCompletedEventArgs> completed = this.Completed;
				if (completed != null)
				{
					double num2 = this.initialScale;
					if (this.expansion.ExtrapolationResult != InertiaProcessor2D.ExtrapolationResult.Skip)
					{
						Debug.Assert(this.expansion.InitialValue > 0.0 && !double.IsInfinity(this.expansion.InitialValue) && !double.IsNaN(this.expansion.InitialValue), "Invalid initial expansion value.");
						num2 *= extrapolatedValueAndUpdateState4.Value / this.expansion.InitialValue;
					}
					ManipulationDelta2D cumulativeDelta = InertiaProcessor2D.GetCumulativeDelta(extrapolatedValueAndUpdateState, extrapolatedValueAndUpdateState2, extrapolatedValueAndUpdateState3, extrapolatedValueAndUpdateState4, num2);
					Manipulation2DCompletedEventArgs manipulation2DCompletedEventArgs = new Manipulation2DCompletedEventArgs((float)extrapolatedValueAndUpdateState.Value, (float)extrapolatedValueAndUpdateState2.Value, this.GetVelocities(), cumulativeDelta);
					completed.Invoke(this, manipulation2DCompletedEventArgs);
					this.LogLine(string.Concat(new object[]
					{
						"Completed event: timeDelta=",
						num,
						" OriginX=",
						manipulation2DCompletedEventArgs.OriginX,
						" OriginY=",
						manipulation2DCompletedEventArgs.OriginY,
						" TotalTranslationX=",
						manipulation2DCompletedEventArgs.Total.TranslationX,
						" TotalTranslationY=",
						manipulation2DCompletedEventArgs.Total.TranslationY,
						" TotalRotation=",
						manipulation2DCompletedEventArgs.Total.Rotation,
						" TotalScale=",
						manipulation2DCompletedEventArgs.Total.ScaleX
					}));
				}
			}
			else
			{
				EventHandler<Manipulation2DDeltaEventArgs> delta = this.Delta;
				if (delta != null)
				{
					double num2 = this.initialScale;
					double scaleDelta = 1.0;
					if (this.expansion.ExtrapolationResult != InertiaProcessor2D.ExtrapolationResult.Skip)
					{
						Debug.Assert(this.expansion.InitialValue > 0.0 && !double.IsInfinity(this.expansion.InitialValue) && !double.IsNaN(this.expansion.InitialValue), "Invalid initial expansion value.");
						num2 *= extrapolatedValueAndUpdateState4.Value / this.expansion.InitialValue;
						if (!DoubleUtil.IsZero(extrapolatedValueAndUpdateState4.Delta))
						{
							double num3 = extrapolatedValueAndUpdateState4.Value - extrapolatedValueAndUpdateState4.Delta;
							Debug.Assert(!DoubleUtil.IsZero(num3));
							scaleDelta = extrapolatedValueAndUpdateState4.Value / num3;
						}
					}
					ManipulationDelta2D incrementalDelta = InertiaProcessor2D.GetIncrementalDelta(extrapolatedValueAndUpdateState, extrapolatedValueAndUpdateState2, extrapolatedValueAndUpdateState3, extrapolatedValueAndUpdateState4, scaleDelta);
					ManipulationDelta2D cumulativeDelta = InertiaProcessor2D.GetCumulativeDelta(extrapolatedValueAndUpdateState, extrapolatedValueAndUpdateState2, extrapolatedValueAndUpdateState3, extrapolatedValueAndUpdateState4, num2);
					Manipulation2DDeltaEventArgs manipulation2DDeltaEventArgs = new Manipulation2DDeltaEventArgs((float)extrapolatedValueAndUpdateState.Value, (float)extrapolatedValueAndUpdateState2.Value, this.GetVelocities(), incrementalDelta, cumulativeDelta);
					delta.Invoke(this, manipulation2DDeltaEventArgs);
					this.LogLine(string.Concat(new object[]
					{
						"Delta event: timeDelta=",
						num,
						" OriginX=",
						manipulation2DDeltaEventArgs.OriginX,
						" OriginY=",
						manipulation2DDeltaEventArgs.OriginY,
						" DeltaX=",
						manipulation2DDeltaEventArgs.Delta.TranslationX,
						" DeltaY=",
						manipulation2DDeltaEventArgs.Delta.TranslationY,
						" RotationDelta=",
						manipulation2DDeltaEventArgs.Delta.Rotation,
						" ScaleDelta=",
						manipulation2DDeltaEventArgs.Delta.ScaleX,
						" CumulativeTranslationX=",
						manipulation2DDeltaEventArgs.Cumulative.TranslationX,
						" CumulativeTranslationY=",
						manipulation2DDeltaEventArgs.Cumulative.TranslationY,
						" CumulativeRotation=",
						manipulation2DDeltaEventArgs.Cumulative.Rotation,
						" CumulativeScale=",
						manipulation2DDeltaEventArgs.Cumulative.ScaleX
					}));
				}
			}
			return !flag;
		}

		private static double GetExtrapolatedValue(double initialValue, double initialVelocity, double deceleration, double timeDelta)
		{
			Debug.Assert(!double.IsNaN(initialVelocity) && !double.IsInfinity(initialValue));
			Debug.Assert(!double.IsNaN(initialVelocity) && !double.IsInfinity(initialVelocity));
			Debug.Assert(!double.IsNaN(deceleration) && !double.IsInfinity(deceleration));
			Debug.Assert(!double.IsNaN(timeDelta) && !double.IsInfinity(timeDelta) && timeDelta >= 0.0);
			return initialValue + (initialVelocity - deceleration * timeDelta * 0.5) * timeDelta;
		}

		private static InertiaProcessor2D.ExtrapolatedValue GetExtrapolatedValueAndUpdateState(InertiaProcessor2D.ExtrapolationState state, double timeDelta)
		{
			Debug.Assert(!double.IsNaN(timeDelta) && !double.IsInfinity(timeDelta) && timeDelta >= 0.0);
			InertiaProcessor2D.ExtrapolatedValue result;
			if (state.ExtrapolationResult == InertiaProcessor2D.ExtrapolationResult.Skip)
			{
				result = new InertiaProcessor2D.ExtrapolatedValue(state.InitialValue, 0.0, 0.0, InertiaProcessor2D.ExtrapolationResult.Skip);
			}
			else if (state.ExtrapolationResult == InertiaProcessor2D.ExtrapolationResult.Stop)
			{
				result = new InertiaProcessor2D.ExtrapolatedValue(state.PreviousValue, 0.0, state.PreviousValue - state.InitialValue, InertiaProcessor2D.ExtrapolationResult.Stop);
			}
			else
			{
				InertiaProcessor2D.ExtrapolationResult result2 = InertiaProcessor2D.ExtrapolationResult.Continue;
				double num = double.NaN;
				if (timeDelta >= state.Duration)
				{
					num = state.FinalValue;
					timeDelta = state.Duration;
					result2 = InertiaProcessor2D.ExtrapolationResult.Stop;
				}
				if (double.IsNaN(num))
				{
					num = InertiaProcessor2D.GetExtrapolatedValue(state.InitialValue, state.InitialVelocity, state.Deceleration, timeDelta);
					double num2 = state.LimitValue(num);
					if (num2 != num)
					{
						num = num2;
						result2 = InertiaProcessor2D.ExtrapolationResult.Stop;
					}
				}
				Debug.Assert(!double.IsNaN(num) && !double.IsInfinity(num), "Calculation error, value should be a finite number.");
				InertiaProcessor2D.ExtrapolatedValue extrapolatedValue = new InertiaProcessor2D.ExtrapolatedValue(num, num - state.PreviousValue, num - state.InitialValue, result2);
				state.PreviousValue = num;
				state.ExtrapolationResult = extrapolatedValue.ExtrapolationResult;
				state.AssertValid();
				result = extrapolatedValue;
			}
			return result;
		}

		private bool Process(long timestamp, bool forceCompleted)
		{
			switch (this.processorState)
			{
				case InertiaProcessor2D.ProcessorState.NotInitialized:
					if (this.translationBehavior != null)
					{
						this.translationBehavior.CheckValid();
					}
					if (this.expansionBehavior != null)
					{
						this.expansionBehavior.CheckValid();
					}
					if (this.rotationBehavior != null)
					{
						this.rotationBehavior.CheckValid();
					}
					if (this.previousTimestamp != this.initialTimestamp)
					{
						this.SetInitialTimestamp(timestamp);
					}
					this.Prepare();
					this.processorState = InertiaProcessor2D.ProcessorState.Running;
					break;
				case InertiaProcessor2D.ProcessorState.Running:
					break;
				case InertiaProcessor2D.ProcessorState.Completing:
				case InertiaProcessor2D.ProcessorState.Completed:
					return false;
				default:
					Debug.Assert(false);
					break;
			}
			if (timestamp - this.previousTimestamp < 0L)
			{
				throw Exceptions.InvalidTimestamp("timestamp", timestamp);
			}
			bool flag = this.ExtrapolateAndRaiseEvents(timestamp, forceCompleted);
			this.previousTimestamp = timestamp;
			if (!flag)
			{
				this.processorState = InertiaProcessor2D.ProcessorState.Completed;
			}
			return flag;
		}

		private static double ScaleValue(double value, double scale)
		{
			Debug.Assert(!double.IsInfinity(value));
			double result;
			if (double.IsNaN(value))
			{
				result = double.NaN;
			}
			else if (DoubleUtil.IsZero(value))
			{
				result = 0.0;
			}
			else
			{
				result = scale;
			}
			return result;
		}

		private static VectorD GetAbsoluteVector(double length, VectorD baseVector)
		{
			Debug.Assert(!double.IsNaN(length) && length >= 0.0 && !double.IsInfinity(length));
			Debug.Assert(!double.IsInfinity(baseVector.X));
			Debug.Assert(!double.IsInfinity(baseVector.Y));
			VectorD result;
			if (!double.IsNaN(baseVector.X) && !double.IsNaN(baseVector.Y) && !DoubleUtil.IsZero(baseVector.X) && !DoubleUtil.IsZero(baseVector.Y))
			{
				double num = length / baseVector.Length;
				result = new VectorD(Math.Abs(baseVector.X * num), Math.Abs(baseVector.Y * num));
			}
			else
			{
				result = new VectorD(InertiaProcessor2D.ScaleValue(baseVector.X, length), InertiaProcessor2D.ScaleValue(baseVector.Y, length));
			}
			return result;
		}

		private static void CheckOriginalValue(float value, string paramName)
		{
			Validations.CheckFinite(value, paramName);
		}

		private void LogLine(string msg)
		{
			if (this.log.Length < 1000000)
			{
				this.log.AppendLine(msg);
			}
		}

		private const double timestampTicksPerMillisecond = 10000.0;

		private const double millisecondsPerTimestampTick = 0.0001;

		private const double millisecondsPerTimestampTickSquared = 1E-08;

		private const string initialOriginXName = "InitialOriginX";

		private const string initialOriginYName = "InitialOriginY";

		private StringBuilder log = new StringBuilder();

		private long initialTimestamp;

		private long previousTimestamp = -1L;

		private InertiaTranslationBehavior2D translationBehavior;

		private InertiaRotationBehavior2D rotationBehavior;

		private InertiaExpansionBehavior2D expansionBehavior;

		private double initialScale = 1.0;

		private double desiredDisplacement = double.NaN;

		private double desiredDeceleration = double.NaN;

		private InertiaProcessor2D.InitialState initialTranslationX = new InertiaProcessor2D.InitialState();

		private InertiaProcessor2D.InitialState initialTranslationY = new InertiaProcessor2D.InitialState();

		private InertiaProcessor2D.InitialState initialOrientation = new InertiaProcessor2D.InitialState();

		private InertiaProcessor2D.InitialState initialExpansion = new InertiaProcessor2D.InitialState();

		private InertiaProcessor2D.ExtrapolationState translationX;

		private InertiaProcessor2D.ExtrapolationState translationY;

		private InertiaProcessor2D.ExtrapolationState orientation;

		private InertiaProcessor2D.ExtrapolationState expansion;

		private InertiaProcessor2D.ProcessorState processorState = InertiaProcessor2D.ProcessorState.NotInitialized;

		private class InitialState
		{
			public InertiaProcessor2D.InitialState Clone()
			{
				return (InertiaProcessor2D.InitialState)base.MemberwiseClone();
			}

			public override string ToString()
			{
				return string.Concat(new object[]
				{
					"Velocity=",
					this.Velocity,
					",\nValue=",
					this.Value,
					",\nAbsoluteOffset=",
					this.AbsoluteOffset,
					",\nAbsoluteDeceleration=",
					this.AbsoluteDeceleration,
					",\nMinBound=",
					this.MinBound,
					",\nMaxBound=",
					this.MaxBound
				});
			}

			public double Velocity = double.NaN;

			public double Value;

			public double AbsoluteOffset = double.NaN;

			public double MinBound = double.NegativeInfinity;

			public double MaxBound = double.PositiveInfinity;

			public double AbsoluteDeceleration = double.NaN;
		}

		private class ExtrapolationState
		{
			public double FinalValue
			{
				get
				{
					return this.LimitValue(this.InitialValue + this.Offset);
				}
			}

			public double LimitValue(double value)
			{
				return DoubleUtil.Limit(value, this.initialState.MinBound, this.initialState.MaxBound);
			}

			public double Deceleration
			{
				get
				{
					return (this.InitialVelocity < 0.0) ? (-this.AbsoluteDeceleration) : this.AbsoluteDeceleration;
				}
			}

			public ExtrapolationState(InertiaProcessor2D.InitialState initialState)
			{
				Debug.Assert(initialState != null);
				Debug.Assert(initialState.AbsoluteDeceleration >= 0.0 || double.IsNaN(initialState.AbsoluteDeceleration));
				Debug.Assert(initialState.AbsoluteOffset >= 0.0 || double.IsNaN(initialState.AbsoluteOffset));
				this.initialState = initialState.Clone();
				this.InitialVelocity = initialState.Velocity * 0.0001;
				this.InitialValue = initialState.Value;
				this.Offset = ((initialState.Velocity < 0.0) ? (-initialState.AbsoluteOffset) : initialState.AbsoluteOffset);
				this.AbsoluteDeceleration = initialState.AbsoluteDeceleration * 1E-08;
				this.PreviousValue = this.InitialValue;
				this.ExtrapolationResult = (double.IsNaN(this.InitialVelocity) ? InertiaProcessor2D.ExtrapolationResult.Skip : InertiaProcessor2D.ExtrapolationResult.Continue);
			}

			public float GetVelocity(long elapsedTimeSinceInitialTimestamp)
			{
				Debug.Assert(elapsedTimeSinceInitialTimestamp >= 0L);
				float result;
				if (this.ExtrapolationResult != InertiaProcessor2D.ExtrapolationResult.Continue)
				{
					result = 0f;
				}
				else
				{
					double num = this.InitialVelocity - this.Deceleration * (double)elapsedTimeSinceInitialTimestamp;
					num *= 10000.0;
					Debug.Assert(Validations.IsFinite((float)num));
					result = (float)num;
				}
				return result;
			}

			public override string ToString()
			{
				return string.Concat(new object[]
				{
					"initialState:\n{",
					this.initialState.ToString(),
					"}\n\nExtrapolationResult=",
					this.ExtrapolationResult,
					",\nInitialVelocity=",
					this.InitialVelocity,
					",\nInitialValue=",
					this.InitialValue,
					",\nFinalValue=",
					this.FinalValue,
					",\nDeceleration=",
					this.Deceleration,
					",\nOffset=",
					this.Offset,
					",\nDuration=",
					this.Duration
				});
			}

			public void AssertValid()
			{
				if (double.IsNaN(this.InitialVelocity))
				{
					Debug.Assert(this.ExtrapolationResult == InertiaProcessor2D.ExtrapolationResult.Skip);
				}
				else
				{
					Debug.Assert(!double.IsNaN(this.InitialValue) && !double.IsInfinity(this.InitialValue));
					Debug.Assert(!double.IsNaN(this.InitialVelocity) && !double.IsInfinity(this.InitialVelocity));
					Debug.Assert(!double.IsNaN(this.Offset));
					Debug.Assert(!double.IsNaN(this.AbsoluteDeceleration) && this.AbsoluteDeceleration >= 0.0 && !double.IsInfinity(this.InitialVelocity));
					Debug.Assert(!double.IsNaN(this.Duration) && this.Duration >= 0.0);
					Debug.Assert(!double.IsNaN(this.FinalValue));
				}
			}

			private readonly InertiaProcessor2D.InitialState initialState = new InertiaProcessor2D.InitialState();

			public double InitialVelocity = double.NaN;

			public readonly double InitialValue;

			public double Offset;

			public double AbsoluteDeceleration = double.NaN;

			public double Duration;

			public InertiaProcessor2D.ExtrapolationResult ExtrapolationResult = InertiaProcessor2D.ExtrapolationResult.Skip;

			public double PreviousValue;
		}

		private enum ExtrapolationResult
		{
			Skip,
			Continue,
			Stop
		}

		private struct ExtrapolatedValue
		{
			public ExtrapolatedValue(double value, double delta, double total, InertiaProcessor2D.ExtrapolationResult result)
			{
				Debug.Assert(!double.IsNaN(value) && !double.IsInfinity(value));
				Debug.Assert(!double.IsNaN(delta) && !double.IsInfinity(delta));
				Debug.Assert(!double.IsNaN(total) && !double.IsInfinity(total));
				this.Value = value;
				this.Delta = delta;
				this.Total = total;
				this.ExtrapolationResult = result;
			}

			public readonly double Value;

			public readonly double Delta;

			public readonly double Total;

			public readonly InertiaProcessor2D.ExtrapolationResult ExtrapolationResult;
		}

		private enum ProcessorState
		{
			NotInitialized,
			Running,
			Completing,
			Completed
		}
	}
}
