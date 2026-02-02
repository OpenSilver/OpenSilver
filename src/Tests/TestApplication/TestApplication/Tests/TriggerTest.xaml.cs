using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Navigation;

namespace TestApplication.Tests
{
    public partial class TriggerTest : Page
    {
        // ViewModels for various DataTrigger tests
        private BooleanTriggerViewModel _booleanViewModel;
        private StatusTriggerViewModel _statusViewModel;
        private QuantityTriggerViewModel _quantityViewModel;
        private MultiConditionViewModel _multiConditionViewModel;
        private EnterExitViewModel _enterExitViewModel;

        public TriggerTest()
        {
            InitializeComponent();

            // Initialize ViewModels
            InitializeViewModels();

            // Setup data bindings
            SetupDataBindings();

            // Apply programmatic trigger style
            ApplyProgrammaticTriggerStyle();
        }

        private void InitializeViewModels()
        {
            _booleanViewModel = new BooleanTriggerViewModel { IsActive = true };
            _statusViewModel = new StatusTriggerViewModel { Status = "Pending" };
            _quantityViewModel = new QuantityTriggerViewModel { Quantity = 10 };
            _multiConditionViewModel = new MultiConditionViewModel { IsActive = false, IsPremium = false };
            _enterExitViewModel = new EnterExitViewModel { IsActive = false };
        }

        private void SetupDataBindings()
        {
            // Bind DataTrigger test panels
            BooleanDataTriggerPanel.DataContext = _booleanViewModel;
            StatusDataTriggerPanel.DataContext = _statusViewModel;
            QuantityDataTriggerPanel.DataContext = _quantityViewModel;
            MultiDataTriggerPanel.DataContext = _multiConditionViewModel;
            EnterExitActionsPanel.DataContext = _enterExitViewModel;

            // Setup Person list for DataTemplate triggers
            var people = new ObservableCollection<PersonViewModel>
            {
                new PersonViewModel { Name = "Alice Johnson", Age = 28, Status = "Active User", IsActive = true, IsPremium = true },
                new PersonViewModel { Name = "Bob Smith", Age = 35, Status = "Regular User", IsActive = true, IsPremium = false },
                new PersonViewModel { Name = "Charlie Brown", Age = 42, Status = "Inactive", IsActive = false, IsPremium = false },
                new PersonViewModel { Name = "Diana Prince", Age = 30, Status = "Premium Inactive", IsActive = false, IsPremium = true },
            };
            PersonList.ItemsSource = people;

            // Setup Status list for DataTemplate string triggers
            var statuses = new ObservableCollection<StatusViewModel>
            {
                new StatusViewModel { Status = "Pending" },
                new StatusViewModel { Status = "Approved" },
                new StatusViewModel { Status = "Rejected" },
                new StatusViewModel { Status = "Pending" },
                new StatusViewModel { Status = "Approved" },
            };
            StatusList.ItemsSource = statuses;
        }

        private void ApplyProgrammaticTriggerStyle()
        {
            // Create a style with triggers programmatically
            Style style = new Style(typeof(Button));

            // Set default values (using the declaring class for each property)
            style.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Colors.LightGray)));
            style.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Colors.Black)));
            style.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(15, 8, 15, 8)));

            // Create IsMouseOver trigger (IsMouseOverProperty is defined on UIElement)
            Trigger mouseOverTrigger = new Trigger
            {
                Property = UIElement.IsMouseOverProperty,
                Value = true
            };
            mouseOverTrigger.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Colors.LightBlue)));
            mouseOverTrigger.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Colors.DarkBlue)));
            style.Triggers.Add(mouseOverTrigger);

            // Create IsPressed trigger (IsPressedProperty is defined on ButtonBase)
            Trigger pressedTrigger = new Trigger
            {
                Property = ButtonBase.IsPressedProperty,
                Value = true
            };
            pressedTrigger.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Colors.SteelBlue)));
            pressedTrigger.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Colors.White)));
            style.Triggers.Add(pressedTrigger);

            // Apply the style
            ProgrammaticTriggerButton.Style = style;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        #region Boolean DataTrigger Event Handlers

        private void ToggleIsActive_Click(object sender, RoutedEventArgs e)
        {
            _booleanViewModel.IsActive = !_booleanViewModel.IsActive;
        }

        #endregion

        #region Status DataTrigger Event Handlers

        private void SetStatusPending_Click(object sender, RoutedEventArgs e)
        {
            _statusViewModel.Status = "Pending";
        }

        private void SetStatusApproved_Click(object sender, RoutedEventArgs e)
        {
            _statusViewModel.Status = "Approved";
        }

        private void SetStatusRejected_Click(object sender, RoutedEventArgs e)
        {
            _statusViewModel.Status = "Rejected";
        }

        #endregion

        #region Quantity DataTrigger Event Handlers

        private void SetQuantityZero_Click(object sender, RoutedEventArgs e)
        {
            _quantityViewModel.Quantity = 0;
        }

        private void SetQuantityTen_Click(object sender, RoutedEventArgs e)
        {
            _quantityViewModel.Quantity = 10;
        }

        #endregion

        #region MultiDataTrigger Event Handlers

        private void ToggleMultiIsActive_Click(object sender, RoutedEventArgs e)
        {
            _multiConditionViewModel.IsActive = !_multiConditionViewModel.IsActive;
        }

        private void ToggleMultiIsPremium_Click(object sender, RoutedEventArgs e)
        {
            _multiConditionViewModel.IsPremium = !_multiConditionViewModel.IsPremium;
        }

        #endregion

        #region EnterExit Actions Event Handlers

        private void ToggleEnterExitIsActive_Click(object sender, RoutedEventArgs e)
        {
            _enterExitViewModel.IsActive = !_enterExitViewModel.IsActive;
        }

        #endregion
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
    /// ViewModel for string-based DataTrigger tests
    /// </summary>
    public class StatusTriggerViewModel : INotifyPropertyChanged
    {
        private string _status;

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// ViewModel for numeric DataTrigger tests
    /// </summary>
    public class QuantityTriggerViewModel : INotifyPropertyChanged
    {
        private int _quantity;

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
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
    /// ViewModel for EnterActions/ExitActions tests
    /// </summary>
    public class EnterExitViewModel : INotifyPropertyChanged
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

    /// <summary>
    /// Simple ViewModel for Status string-based triggers in DataTemplate
    /// </summary>
    public class StatusViewModel : INotifyPropertyChanged
    {
        private string _status;

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion
}
