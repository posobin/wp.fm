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
using System.IO;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;

namespace lastfm
{
    public partial class LoginPage : PhoneApplicationPage
    {
        ProgressIndicator prog;
        public LoginPage()
        {
            InitializeComponent();
            prog = new ProgressIndicator();
        }

        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Logging in...";
            SystemTray.SetProgressIndicator(this, prog);

            try { Session.CurrentSession = await auth.authorize(txtUsername.Text, txtPassword.Text); }
            catch (TaskCanceledException) { }

            if (Session.CurrentSession != null)
                NavigationService.GoBack();
            prog.IsIndeterminate = false;
            prog.IsVisible = false;
        }
    }
}