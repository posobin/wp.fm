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

namespace lastfm
{
    public partial class LoginPage : PhoneApplicationPage
    {
        ProgressIndicator prog;
        Session mainSession;
        public LoginPage()
        {
            InitializeComponent();
            prog = new ProgressIndicator();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            mainSession = new Session();
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Click me...";
            SystemTray.SetProgressIndicator(this, prog);
            auth.authorize(txtUsername.Text, txtPassword.Text, mainSession);
            prog.IsIndeterminate = false;
            prog.IsVisible = false;
        }
    }
}