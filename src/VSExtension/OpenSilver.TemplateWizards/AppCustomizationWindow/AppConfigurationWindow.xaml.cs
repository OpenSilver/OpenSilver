﻿using System;
using System.Windows;

namespace OpenSilver.TemplateWizards.AppCustomizationWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AppConfigurationWindow : Window
    {
        public OpenSilverBuildType OpenSilverBuildType
        {
            get
            {
                switch (BuildTypeComboBox.SelectedIndex)
                {
                    case 0:
                        return OpenSilverBuildType.Stable;
                    case 1:
                        return OpenSilverBuildType.WorkInProgress;
                    default:
                        throw new InvalidOperationException("Error retrieving selected OpenSilver build type");
                }
            }
        }

        public BlazorVersion BlazorVersion
        {
            get
            {
                switch (BlazorVersionComboBox.SelectedIndex)
                {
                    case 0:
                        return BlazorVersion.Net5;
                    case 1:
                        return BlazorVersion.Net6;
                    default:
                        throw new InvalidOperationException("Error retrieving selected blazor version");
                }
            }
        }

        public AppConfigurationWindow(string openSilverType)
        {
            InitializeComponent();

            if (openSilverType == "Library")
            {
                BlazorVersionStackPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void ButtonContinue_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
