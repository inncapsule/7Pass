﻿using System;
using System.Windows;
using System.Windows.Navigation;
using KeePass.Utils;

namespace KeePass
{
    public partial class Settings
    {
        public Settings()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(bool cancelled, NavigationEventArgs e)
        {
            if (cancelled)
                return;

            var settings = AppSettings.Instance;
            chkBrowser.IsChecked = settings.UseIntBrowser;
            chkRecycleBin.IsChecked = settings.HideRecycleBin;
        }

        private void chkBrowser_CheckedChanged(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.UseIntBrowser =
                chkBrowser.IsChecked == true;
        }

        private void chkRecycleBin_CheckedChanged(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.HideRecycleBin =
                chkRecycleBin.IsChecked == true;
        }
    }
}