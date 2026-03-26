using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace TestApplication.Tests
{
    public partial class TriggerTest : Page
    {
        private BooleanTriggerViewModel _booleanViewModel;
        private MultiConditionViewModel _multiConditionViewModel;

        public TriggerTest()
        {
            InitializeComponent();
            InitializeViewModels();
            SetupDataBindings();
            ApplyProgrammaticTriggerStyle();
        }

        private void InitializeViewModels()
        {
            _booleanViewModel = new BooleanTriggerViewModel { IsActive = true };
            _multiConditionViewModel = new MultiConditionViewModel { IsActive = false, IsPremium = false };
        }

        private void SetupDataBindings()
        {
            BooleanDataTriggerPanel.DataContext = _booleanViewModel;
            MultiDataTriggerPanel.DataContext = _multiConditionViewModel;

            // Setup Person list for DataTemplate triggers
            var people = new ObservableCollection<PersonViewModel>
            {
                new PersonViewModel { Name = "Alice Johnson", Age = 28, Status = "Active User", IsActive = true, IsPremium = true },
                new PersonViewModel { Name = "Bob Smith", Age = 35, Status = "Regular User", IsActive = true, IsPremium = false },
                new PersonViewModel { Name = "Charlie Brown", Age = 42, Status = "Inactive", IsActive = false, IsPremium = false },
                new PersonViewModel { Name = "Diana Prince", Age = 30, Status = "Premium Inactive", IsActive = false, IsPremium = true },
            };
            PersonList.ItemsSource = people;
        }

        private void ApplyProgrammaticTriggerStyle()
        {
            // Create a style with triggers programmatically
            Style style = new Style(typeof(Button));

            // Use a simple template that directly shows Background property
            // The default Button template has gradient overlays that obscure Background changes
            var simpleTemplate = (ControlTemplate)Resources["SimpleButtonTemplate"];
            style.Setters.Add(new Setter(Control.TemplateProperty, simpleTemplate));

            // Set default values (using distinctive colors)
            style.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Colors.LightGray)));
            style.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Colors.Black)));
            style.Setters.Add(new Setter(Control.BorderBrushProperty, new SolidColorBrush(Colors.Gray)));
            style.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(1)));
            style.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(15, 8, 15, 8)));

            // Create IsMouseOver trigger (IsMouseOverProperty is defined on UIElement)
            // Uses bright yellow to clearly show the trigger is working
            Trigger mouseOverTrigger = new Trigger
            {
                Property = UIElement.IsMouseOverProperty,
                Value = true
            };
            mouseOverTrigger.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Colors.Yellow)));
            mouseOverTrigger.Setters.Add(new Setter(Control.BorderBrushProperty, new SolidColorBrush(Colors.Orange)));
            mouseOverTrigger.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Colors.DarkOrange)));
            style.Triggers.Add(mouseOverTrigger);

            // Create IsPressed trigger (IsPressedProperty is defined on ButtonBase)
            // Uses orange/red to clearly show pressed state
            Trigger pressedTrigger = new Trigger
            {
                Property = ButtonBase.IsPressedProperty,
                Value = true
            };
            pressedTrigger.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Colors.Orange)));
            pressedTrigger.Setters.Add(new Setter(Control.BorderBrushProperty, new SolidColorBrush(Colors.Red)));
            pressedTrigger.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Colors.White)));
            style.Triggers.Add(pressedTrigger);

            ProgrammaticTriggerButton.Style = style;
        }

        private void ToggleIsActive_Click(object sender, RoutedEventArgs e)
        {
            _booleanViewModel.IsActive = !_booleanViewModel.IsActive;
        }

        private void ToggleMultiIsActive_Click(object sender, RoutedEventArgs e)
        {
            _multiConditionViewModel.IsActive = !_multiConditionViewModel.IsActive;
        }

        private void ToggleMultiIsPremium_Click(object sender, RoutedEventArgs e)
        {
            _multiConditionViewModel.IsPremium = !_multiConditionViewModel.IsPremium;
        }
    }

    #region ViewModels

    /// <summary>
    /// ViewModel for boolean DataTrigger tests
    /// </summary>
    public class BooleanTriggerViewModel : INotifyPropertyChanged
    {
        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// ViewModel for MultiDataTrigger tests with multiple conditions
    /// </summary>
    public class MultiConditionViewModel : INotifyPropertyChanged
    {
        private bool _isActive;
        private bool _isPremium;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        public bool IsPremium
        {
            get => _isPremium;
            set
            {
                if (_isPremium != value)
                {
                    _isPremium = value;
                    OnPropertyChanged(nameof(IsPremium));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// ViewModel for Person items in DataTemplate triggers
    /// </summary>
    public class PersonViewModel : INotifyPropertyChanged
    {
        private string _name;
        private int _age;
        private string _status;
        private bool _isActive;
        private bool _isPremium;

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public int Age
        {
            get => _age;
            set
            {
                if (_age != value)
                {
                    _age = value;
                    OnPropertyChanged(nameof(Age));
                }
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        public bool IsPremium
        {
            get => _isPremium;
            set
            {
                if (_isPremium != value)
                {
                    _isPremium = value;
                    OnPropertyChanged(nameof(IsPremium));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion
}
