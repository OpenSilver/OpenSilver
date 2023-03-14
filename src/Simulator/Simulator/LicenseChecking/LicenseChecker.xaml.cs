

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Simulator (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Simulator (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/



using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DotNetBrowser.Wpf;
using OpenSilver;
using DotNetBrowser.Browser;
using DotNetBrowser.Navigation.Events;
using DotNetBrowser.Net;
using DotNetBrowser.Engine;
using DotNetBrowser.Cookies;

namespace DotNetForHtml5.EmulatorWithoutJavascript.LicenseChecking
{
    /// <summary>
    /// Interaction logic for LicenseChecker.xaml
    /// </summary>
    public partial class LicenseChecker : UserControl
    {
        const string USERNAME_TAG = "<username>";
        const string USERNAME_TAG_END = "</username>";

        const bool local = false; // true; //to test in local
        const string LOCAL_PREFIX = "https://localhost:44358/";
        const string PREFIX = "https://www.cshtml5.com/";
        const string LOGIN_SUFFIX = "simulator-authentication/login.aspx";
        const string LOGOUT_SUFFIX = "simulator-authentication/logout.aspx";
        const string NAME_FOR_STORING_COOKIES = "ms_cookies_for_simulator_login"; // This is an arbitrary name used to store the cookies in the registry

        const string CSHTML5_COOKIES_URL = "https://www.cshtml5.com/"; // "https://localhost:44358"; //to test in local

#if BRIDGE && SILVERLIGHTCOMPATIBLEVERSION
        const string _trialFeatureID = Constants.SL_MIGRATION_EDITION_FEATURE_ID;
#else
        const string _trialFeatureID = Constants.COMMERCIAL_EDITION_S_FEATURE_ID;
#endif

        const string _trialFeatureFriendlyName = Constants.COMMERCIAL_EDITION_S_FRIENDLY_NAME;

        protected MainWindow _mainWindow;
        protected IBrowser _browser;
        protected BrowserView _browserView;
        protected string _username;
        protected string _editionFriendlyName;
        protected int _numberOfTrialDaysLeft;
        protected bool _enable;
        protected string _activatedFeatureID;

        public IBrowser Browser
        {
            get
            {
                return _browser;
            }
            set
            {
                _browser = value;
            }
        }

        public BrowserView BrowserView
        {
            get
            {
                return _browserView;
            }
            set
            {
                _browserView = value;
            }
        }

        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
            }
        }

        public bool Enable
        {
            get
            {
                return _enable;
            }
            set
            {
                _enable = value;
                MainWindow.Dispatcher.BeginInvoke((Action)(() =>
                {
                    Visibility = value ? Visibility.Visible : Visibility = Visibility.Collapsed;
                }), DispatcherPriority.ApplicationIdle);
            }
        }

        public int NumberOfTrialDaysLeft
        {
            get
            {
                return _numberOfTrialDaysLeft;
            }
            set
            {
                _numberOfTrialDaysLeft = value;
            }
        }

        public string TrialFeatureID
        {
            get
            {
                return _trialFeatureID;
            }
        }

        public MainWindow MainWindow
        {
            get
            {
                return _mainWindow;
            }

            set
            {
                _mainWindow = value;
            }
        }

        public string ActivatedFeatureID
        {
            get
            {
                return _activatedFeatureID;
            }

            set
            {
                _activatedFeatureID = value;
            }
        }


        public string LoginURL
        {
            get
            {
                return (local ? LOCAL_PREFIX : PREFIX) + LOGIN_SUFFIX;
            }
        }

        public string LogoutURL
        {
            get
            {
                return (local ? LOCAL_PREFIX : PREFIX) + LOGOUT_SUFFIX;
            }
        }

        public LicenseChecker()
        {
            InitializeComponent();

            MainWindow = (MainWindow)Application.Current.MainWindow;

            Loaded += LicenseChecker_Loaded;
            KeyDown += LicenseChecker_Keydown; // used to close the license checker in case of dead end
        }

        private void LicenseChecker_Keydown(object sender, KeyEventArgs e)
        {
            KeyboardDevice keyboard = e.KeyboardDevice;
            if ((keyboard.IsKeyDown(Key.LeftCtrl) || keyboard.IsKeyDown(Key.RightCtrl)) && keyboard.IsKeyDown(Key.W))
                ButtonContinue_Click(this, new RoutedEventArgs());
        }

        private void LicenseChecker_Loaded(object sender, RoutedEventArgs e)
        {

            ToolbarForLicenseChecker.Visibility = Visibility.Collapsed;
            ButtonContinue.Visibility = Visibility.Collapsed;
            ButtonKeyActivation.Visibility = Visibility.Collapsed;
            ButtonHobbyist.Visibility = Visibility.Collapsed;
            ToolbarForLicenseChecker.Visibility = Visibility.Collapsed;
            ButtonGoToLoginPage.Visibility = Visibility.Collapsed;
            LicenseCheckerBrowserContainer.Visibility = Visibility.Collapsed;

            IEngine engine = EngineFactory.Create(new EngineOptions.Builder
            {
                IncognitoEnabled = true,
                RenderingMode = RenderingMode.OffScreen,
                UserDataDirectory = "data-dir-license"
            }.Build());

            Browser = engine.CreateBrowser();

            CookiesHelper.LoadCookies(Browser, CSHTML5_COOKIES_URL, NAME_FOR_STORING_COOKIES);
            CookiesHelper.LoadMicrosoftCookies(Browser, NAME_FOR_STORING_COOKIES);

            BrowserView = new BrowserView();
            BrowserView.InitializeFrom(Browser);
            LicenseCheckerBrowserContainer.Child = BrowserView;
            
            // we check if a commercial key is  activated and we add it to the cookie before loading the login URL
            if (IsCommercialKeyActivated())
                SetKeyGuidAsCookie(); // we set the key guid as a session cookie so the website know we have an activated key

            // we add an handler for browser error (eg: internet down, website down, page not found...)
            Browser.Navigation.FrameLoadFailed += OnFailLoadingFrameEvent; //To Do: find a better way of managing those error

            Browser.Navigation.LoadUrl(LoginURL);

            Enable = true;

            Browser.Navigation.FrameDocumentLoadFinished += (s1, e1) =>
            {
                // We use a dispatcher to go back to thread in charge of the UI.
                MainWindow.Dispatcher.BeginInvoke((Action)(() =>
                {
                    //if (_javaScriptExecutionHandler == null)
                    //    _javaScriptExecutionHandler = new JavaScriptExecutionHandler(MainWebBrowser);

                    //dynamic rootElement = _javaScriptExecutionHandler.ExecuteJavaScriptWithResult(@"document.getElementByIdSafe(""cshtml5-root"");");

                    //MessageBox.Show(rootElement.ToString());


                    //todo: verify that we are not on an outside page (eg. Azure Active Directory login page)
                    OnLoaded();
                }), DispatcherPriority.ApplicationIdle);
            };
        }

        bool _needToRedirectAfterLogout = false;
        bool _wasLogged;

        public void OnFailLoadingFrameEvent(object sender, FrameLoadFailedEventArgs e)
        {
            if (e.ErrorCode != NetError.Aborted &&
                e.ErrorCode != NetError.ConnectionAborted &&
                e.ErrorCode != (NetError)(-27))
                OnNetworkNotAvailable();
        }

        public void OnLoaded()
        {
            //LicenseCheckerBrowserContainer.Visibility = Visibility.Visible;
            bool onLoginPage = Browser.Url == LoginURL;

            if (_needToRedirectAfterLogout)
            {
                if (onLoginPage)
                {
                    _needToRedirectAfterLogout = false;
                    return;
                }
                Browser.Navigation.LoadUrl(LoginURL);
                return;
            }

            if (onLoginPage)
            {
                _wasLogged = IsLogged();
                ButtonGoToLoginPage.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainWindow.ButtonProfil.Visibility = Visibility.Collapsed;
                ButtonKeyActivation.Visibility = Visibility.Collapsed;
                ButtonHobbyist.Visibility = Visibility.Collapsed;
                ButtonGoToLoginPage.Visibility = Visibility.Visible;
            }

            UpdateUsername();
            if (IsLogged())
            {
                // we check activated keys
                if (IsCommercialKeyActivated())
                {
                    OnCommercialKeyActivated();
                    Enable = false;
                }
                // we check for the community key
                else if (IsCommunityKeyActivated())
                {
                    Enable = false;
                }
                // we check for trial versions
                else
                {
                    TrialHelpers.TrialStatus trialStatus;
                    do
                    {
                        trialStatus = TrialHelpers.IsTrial(TrialFeatureID, out _numberOfTrialDaysLeft);
                        switch (trialStatus)
                        {
                            case TrialHelpers.TrialStatus.NotStarted:
                                StartTrial(TrialFeatureID);
                                break;
                            case TrialHelpers.TrialStatus.Running:
                                OnTrialRunning(TrialFeatureID);
                                break;
                            case TrialHelpers.TrialStatus.Expired:
                                OnTrialExpired(TrialFeatureID);
                                break;
                            default:
                                break;
                        }
                    } while (trialStatus == TrialHelpers.TrialStatus.NotStarted);
                }
            }
            else if (_wasLogged)
            {
                if (onLoginPage)
                {
                    LogOut();
                    _needToRedirectAfterLogout = true;
                }

            }
        }

        string GetFriendlyName(string featureID)
        {
            switch (featureID)
            {
                case Constants.COMMERCIAL_EDITION_S_FEATURE_ID:
                    return Constants.COMMERCIAL_EDITION_S_FRIENDLY_NAME;
                case Constants.COMMERCIAL_EDITION_L_FEATURE_ID:
                    return Constants.COMMERCIAL_EDITION_L_FRIENDLY_NAME;
                case Constants.SL_MIGRATION_EDITION_FEATURE_ID:
                    return Constants.SL_MIGRATION_EDITION_FRIENDLY_NAME;
                case Constants.ENTERPRISE_EDITION_FEATURE_ID:
                    return Constants.ENTERPRISE_EDITION_FRIENDLY_NAME;
                case Constants.PROFESSIONAL_EDITION_FEATURE_ID:
                    return Constants.PROFESSIONAL_EDITION_FRIENDLY_NAME;
                default:
                    return "N/A";
            }
        }

        private void OnTrialRunning(string featureID)
        {
            MainWindow.ProfilDetailEdition.Text = GetFriendlyName(featureID) + " (trial version)";

            // skip the toolbar if already displayed today
            if (TrialHelpers_MoreMethods.WasTheTrialMessageAlreadyDisplayedToday)
            {
                Enable = false;
                return;
            }
            ToolbarForLicenseChecker.Visibility = Visibility.Visible;
            StateRun.Text = "Your trial of CSHTML5 (" + GetFriendlyName(featureID) + ") expires in " + NumberOfTrialDaysLeft + " days.";
            ButtonContinue.Visibility = Visibility.Visible;
            ButtonHobbyist.Visibility = Visibility.Visible;
            ButtonKeyActivation.Visibility = Visibility.Visible;
            MainWindow.ButtonProfil.Visibility = Visibility.Visible;
        }

        private void OnTrialExpired(string featureID)
        {
            ToolbarForLicenseChecker.Visibility = Visibility.Visible;
            StateRun.Text = "Your trial of CSHTML5 (" + GetFriendlyName(featureID) + ") has expired.";
            ButtonContinue.Visibility = Visibility.Collapsed;
            ButtonHobbyist.Visibility = Visibility.Visible;
            ButtonKeyActivation.Visibility = Visibility.Visible;
            MainWindow.ButtonProfil.Visibility = Visibility.Visible;
        }

        public void UpdateUsername()
        {
            string html = Browser.MainFrame?.Html;

            if (html != null)
            {
                int start = html.IndexOf(USERNAME_TAG);
                int end = html.IndexOf(USERNAME_TAG_END);
                if (start >= 0 && end >= 0)
                {
                    int length = end - (start + USERNAME_TAG.Length);
                    Username = html.Substring(start + USERNAME_TAG.Length, length);
                }
                else
                    Username = null;
            }
            else
            {
                Username = null;
            }

            StateRun.Text = "";
            if (Username != null)
            {
                MainWindow.ButtonProfil.Visibility = Visibility.Visible;
                MainWindow.ProfilDetailEmail.Text = Username;


                ToolbarForLicenseChecker.Visibility = Visibility.Visible;
                WelcomeRun.Text = "Welcome " + Username + "!";
                LicenseCheckerBrowserContainer.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainWindow.ButtonProfil.Visibility = Visibility.Collapsed;
                MainWindow.ProfilDetailEmail.Text = "";
                LicenseCheckerBrowserContainer.Visibility = Visibility.Visible;
            }
        }

        public bool IsLogged()
        {
            return Username != null;
        }

        public bool IsCommercialKeyActivated()
        {
            if (ActivatedFeatureID != null)
                return true;

#if BRIDGE
#if SILVERLIGHTCOMPATIBLEVERSION
            if (ActivationHelpers.IsFeatureEnabled(Constants.SL_MIGRATION_EDITION_FEATURE_ID))
                ActivatedFeatureID = Constants.SL_MIGRATION_EDITION_FEATURE_ID;
#else
            if (ActivationHelpers.IsFeatureEnabled(Constants.ENTERPRISE_EDITION_FEATURE_ID))
                ActivatedFeatureID = Constants.ENTERPRISE_EDITION_FEATURE_ID;
            else if (ActivationHelpers.IsFeatureEnabled(Constants.PROFESSIONAL_EDITION_FEATURE_ID))
                ActivatedFeatureID = Constants.PROFESSIONAL_EDITION_FEATURE_ID;
            else if (ActivationHelpers.IsFeatureEnabled(Constants.COMMERCIAL_EDITION_S_FEATURE_ID))
                ActivatedFeatureID = Constants.COMMERCIAL_EDITION_S_FEATURE_ID;
            else if (ActivationHelpers.IsFeatureEnabled(Constants.COMMERCIAL_EDITION_L_FEATURE_ID))
                ActivatedFeatureID = Constants.COMMERCIAL_EDITION_L_FEATURE_ID;
            else if (ActivationHelpers.IsFeatureEnabled(Constants.PREMIUM_SUPPORT_EDITION_FEATURE_ID))
                ActivatedFeatureID = Constants.PREMIUM_SUPPORT_EDITION_FEATURE_ID;
#endif
#else
            if (ActivationHelpers.IsFeatureEnabled(Constants.SL_MIGRATION_EDITION_FEATURE_ID))
                ActivatedFeatureID = Constants.SL_MIGRATION_EDITION_FEATURE_ID;
            else if (ActivationHelpers.IsFeatureEnabled(Constants.ENTERPRISE_EDITION_FEATURE_ID))
                ActivatedFeatureID = Constants.ENTERPRISE_EDITION_FEATURE_ID;
            else if (ActivationHelpers.IsFeatureEnabled(Constants.PROFESSIONAL_EDITION_FEATURE_ID))
                ActivatedFeatureID = Constants.PROFESSIONAL_EDITION_FEATURE_ID;
            else if (ActivationHelpers.IsFeatureEnabled(Constants.COMMERCIAL_EDITION_S_FEATURE_ID))
                ActivatedFeatureID = Constants.COMMERCIAL_EDITION_S_FEATURE_ID;
            else if (ActivationHelpers.IsFeatureEnabled(Constants.COMMERCIAL_EDITION_L_FEATURE_ID))
                ActivatedFeatureID = Constants.COMMERCIAL_EDITION_L_FEATURE_ID;
            else if (ActivationHelpers.IsFeatureEnabled(Constants.PREMIUM_SUPPORT_EDITION_FEATURE_ID))
                ActivatedFeatureID = Constants.PREMIUM_SUPPORT_EDITION_FEATURE_ID;
#endif
            bool foundKey = ActivatedFeatureID != null;
            return foundKey;
        }

        public void OnCommercialKeyActivated()
        {
            MainWindow.ProfilDetailEdition.Text = GetFriendlyName(ActivatedFeatureID);
        }

        public bool IsCommunityKeyActivated()
        {
            bool foundKey = RegistryHelpers.GetSetting("IsCommunity", null) != null;
            if (foundKey)
                MainWindow.ProfilDetailEdition.Text = Constants.COMMUNITY_EDITION_FRIENDLY_NAME;
            return foundKey;
        }

        public void StartTrial(string featureID)
        {
            TrialHelpers_MoreMethods.StartTrial(featureID);
        }

        public void LogOut()
        {
            Enable = true;
            Username = null;
            ActivatedFeatureID = null;
            MainWindow.ButtonProfil.Visibility = Visibility.Collapsed;
            ButtonKeyActivation.Visibility = Visibility.Collapsed;
            ButtonHobbyist.Visibility = Visibility.Collapsed;
            ButtonContinue.Visibility = Visibility.Collapsed;
            ToolbarForLicenseChecker.Visibility = Visibility.Collapsed;
            Browser.Navigation.LoadUrl(LogoutURL);
        }

        public void SetHobbyist()
        {
            RegistryHelpers.SaveSetting("IsCommunity", "yes");
            MainWindow.ProfilDetailEdition.Text = Constants.COMMUNITY_EDITION_FRIENDLY_NAME;
            Enable = false;
        }

        public void LaunchKeyActivation()
        {
            ActivationHelpers.DisplayActivationApp(ActivationHelpers.GetActivationAppPath(), TrialFeatureID, "Register your keys");
            Browser.Navigation.LoadUrl(LoginURL);
        }

        const string LICENSE_KEY_GUID_COOKIE = "keyGuid";

        private void SetKeyGuidAsCookie()
        {
            Guid keyGuid;

            if (Guid.TryParse(RegistryHelpers.GetSetting("Feature_" + ActivatedFeatureID, null), out keyGuid))
            {
                var builder = new Cookie.Builder("localhost");
                builder.Name = LICENSE_KEY_GUID_COOKIE;
                builder.Value = keyGuid.ToString();
                builder.Path = "/";
                builder.Secure = true;
                builder.HttpOnly = true;
                Browser.Engine.Profiles.Default.CookieStore.SetCookie(builder.Build()).Wait();

                foreach (var cookie in Browser.Engine.Profiles.Default.CookieStore.GetAllCookies(CSHTML5_COOKIES_URL).Result)
                    Debug.WriteLine(cookie);
            }
        }

        public void Dispose()
        {
            CookiesHelper.SaveCookies(Browser, NAME_FOR_STORING_COOKIES, CSHTML5_COOKIES_URL);
            CookiesHelper.SaveMicrosoftCookies(Browser, NAME_FOR_STORING_COOKIES);
            Browser.Dispose();
        }

        public void OnNetworkAvailabilityChanged(bool available)
        {
            if (!available)
                OnNetworkNotAvailable();
        }

        public void OnNetworkNotAvailable()
        {
            MainWindow.Dispatcher.BeginInvoke((Action)(() =>
            {
                LicenseCheckerBrowserContainer.Visibility = Visibility.Collapsed;
                BrowserView.Visibility = Visibility.Collapsed;
                ToolbarForLicenseChecker.Visibility = Visibility.Visible;
                WelcomeRun.Text = "Please check your internet connection.";
                StateRun.Text = "Contact support@cshtml5.com if the error persists.";
                MessagePath.Data = Geometry.Parse("M11,15H13V17H11V15M11,7H13V13H11V7M12,2C6.47,2 2,6.5 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M12,20A8,8 0 0,1 4,12A8,8 0 0,1 12,4A8,8 0 0,1 20,12A8,8 0 0,1 12,20Z");
                ButtonContinue.Visibility = Visibility.Visible;
                ButtonGoToLoginPage.Visibility = Visibility.Collapsed;
                ButtonHobbyist.Visibility = Visibility.Collapsed;
                ButtonKeyActivation.Visibility = Visibility.Collapsed;
            }), DispatcherPriority.ApplicationIdle);
        }

        private void ButtonContinue_Click(object sender, RoutedEventArgs e)
        {
            Enable = false;
            TrialHelpers_MoreMethods.RememberThatTheTrialMessageWasDisplayedToday();
        }

        private void ButtonHobbyist_Click(object sender, RoutedEventArgs e)
        {
            SetHobbyist();
        }

        private void ButtonKeyActivation_Click(object sender, RoutedEventArgs e)
        {
            LaunchKeyActivation();
        }

        private void ButtonProfile_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((FrameworkElement)sender).ContextMenu.IsOpen = true;
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            TrialHelpers_MoreMethods.Reset(TrialFeatureID);
        }

        private void ButtonGoToLoginPage_Click(object sender, RoutedEventArgs e)
        {
            Browser.Navigation.LoadUrl(LoginURL);
        }
    }
}
