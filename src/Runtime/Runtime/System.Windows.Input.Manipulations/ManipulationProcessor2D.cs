using System.ComponentModel;
using System.Diagnostics;
#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	public class ManipulationProcessor2D : ManipulationSequence.ISettings
	{
		public ManipulationProcessor2D(Manipulations2D supportedManipulations) : this(supportedManipulations, null)
		{
		}

		public ManipulationProcessor2D(Manipulations2D supportedManipulations, ManipulationPivot2D pivot)
		{
			supportedManipulations.CheckValue("supportedManipulations");
			this.supportedManipulations = supportedManipulations;
			this.pivot = pivot;
		}

		public float MinimumScaleRotateRadius
		{
			get
			{
				return this.minimumScaleRotateRadius;
			}
			set
			{
				Validations.CheckFiniteNonNegative(value, "MinimumScaleRotateRadius");
				this.minimumScaleRotateRadius = value;
			}
		}

		public ManipulationPivot2D Pivot
		{
			get
			{
				return this.pivot;
			}
			set
			{
				this.pivot = value;
			}
		}

		public Manipulations2D SupportedManipulations
		{
			get
			{
				return this.supportedManipulations;
			}
			set
			{
				value.CheckValue("SupportedManipulations");
				this.supportedManipulations = value;
			}
		}

		public event EventHandler<Manipulation2DStartedEventArgs> Started;

		public event EventHandler<Manipulation2DDeltaEventArgs> Delta;

		public event EventHandler<Manipulation2DCompletedEventArgs> Completed;

		public void ProcessManipulators(long timestamp, IEnumerable<Manipulator2D> manipulators)
		{
			ManipulationSequence manipulationSequence = this.currentManipulation;
			if (manipulationSequence == null)
			{
				manipulationSequence = new ManipulationSequence();
				manipulationSequence.Started += new EventHandler<Manipulation2DStartedEventArgs>(this.OnManipulationStarted);
			}
			manipulationSequence.ProcessManipulators(timestamp, manipulators, this);
		}

		public void CompleteManipulation(long timestamp)
		{
			if (this.currentManipulation != null)
			{
				this.currentManipulation.CompleteManipulation(timestamp);
				this.currentManipulation = null;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetParameters(ManipulationParameters2D parameters)
		{
			Validations.CheckNotNull(parameters, "parameters");
			parameters.Set(this);
		}

		private void OnManipulationStarted(object sender, Manipulation2DStartedEventArgs args)
		{
			Debug.Assert(sender != null);
			Debug.Assert(this.currentManipulation == null, "Manipulation was already in progress");
			this.currentManipulation = (ManipulationSequence)sender;
			this.currentManipulation.Delta += new EventHandler<Manipulation2DDeltaEventArgs>(this.OnManipulationDelta);
			this.currentManipulation.Completed += new EventHandler<Manipulation2DCompletedEventArgs>(this.OnManipulationCompleted);
			if (this.Started != null)
			{
				this.Started.Invoke(this, args);
			}
		}

		private void OnManipulationDelta(object sender, Manipulation2DDeltaEventArgs args)
		{
			Debug.Assert(object.ReferenceEquals(sender, this.currentManipulation));
			if (this.Delta != null)
			{
				this.Delta.Invoke(this, args);
			}
		}

		private void OnManipulationCompleted(object sender, Manipulation2DCompletedEventArgs args)
		{
			Debug.Assert(object.ReferenceEquals(sender, this.currentManipulation));
			this.currentManipulation.Started -= new EventHandler<Manipulation2DStartedEventArgs>(this.OnManipulationStarted);
			this.currentManipulation.Delta -= new EventHandler<Manipulation2DDeltaEventArgs>(this.OnManipulationDelta);
			this.currentManipulation.Completed -= new EventHandler<Manipulation2DCompletedEventArgs>(this.OnManipulationCompleted);
			this.currentManipulation = null;
			if (this.Completed != null)
			{
				this.Completed.Invoke(this, args);
			}
		}

		private const double singleManipulatorTorqueFactor = 4.0;

		internal const long TimestampTicksPerMillisecond = 10000L;

		private static readonly PointF ZeroPoint = new PointF(0f, 0f);

		private static readonly VectorF ZeroVector = new VectorF(0f, 0f);

		private float minimumScaleRotateRadius = 20f;

		private Manipulations2D supportedManipulations;

		private ManipulationPivot2D pivot;

		private ManipulationSequence currentManipulation;
	}
}