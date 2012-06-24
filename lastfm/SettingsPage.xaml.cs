using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace lastfm
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ScrobblingToggle.IsChecked = Session.AutoScrobbling;
            SessionToggle.IsChecked = Session.RememberSession;
        }
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void ScrobblingToggle_Checked(object sender, RoutedEventArgs e)
        {
            Session.AutoScrobbling = true;
        }

        private void ScrobblingToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            Session.AutoScrobbling = false;
        }

        private void SessionToggle_Checked(object sender, RoutedEventArgs e)
        {
            Session.RememberSession = true;
        }

        private void SessionToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            Session.RememberSession = false;
        }
    }
}