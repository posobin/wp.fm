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
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace lastfm
{
    public partial class artistInfoPage : PhoneApplicationPage
    {
        public artistInfoPage()
        {
            InitializeComponent();
            this.DataContext = currArtist;
            prog = new ProgressIndicator();
        }

        ProgressIndicator prog;

        artistInfo currArtist = new artistInfo();

        const string NotifyScript = @"<script type='text/javascript' language='javascript'>
                                            window.onload = function(){
                                                var linkArray = document.getElementsByTagName('a');
                                                for(var i=0; i < linkArray.length; i++){
                                                    linkArray[i].onclick = function() { 
                                                        var str = this.href + "" "" + this.getAttribute(""className""); 
                                                        window.external.Notify(str); }
                                                }
                                            }
                                        </script>";

        private string makeHtml(string str)
        {
            string ret;
            string bgcolor = "#" + ((Color)App.Current.Resources["PhoneBackgroundColor"]).ToString().Substring(3);
            string fcolor = "#" + ((Color)App.Current.Resources["PhoneForegroundColor"]).ToString().Substring(3);
            ret = "<html>" + 
                    "<head>" +
                        "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0\" />" +
                        NotifyScript +
                    "</head>" +
                    "<body " + "bgcolor=" + bgcolor + " style=\"color:" + fcolor + "\"" + ">" +
                        str +
                    "</body>" +
                  "</html>";
            return ret;
        }

        private void processLink(string str)
        {
            if (str.EndsWith(" "))
            {
                WebBrowserTask browser = new WebBrowserTask();
                browser.Uri = new Uri(str, UriKind.Absolute);
                browser.Show();
            }
            else if (str.EndsWith(" bbcode_artist"))
            {
                string navigateTo = str.Remove(str.Length - " bbcode_artist".Length).Split( new char[] {'/'}).Last();
                NavigationService.Navigate(new Uri("/artistInfoPage.xaml?artistName=" + navigateTo, UriKind.Relative));
            }
            //MessageBox.Show("Not implemented yet.");
        }

        private void ScriptNotify(object sender, NotifyEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value)) processLink(e.Value);
        }

        private async void getArtistInfo(string artistName)
        {
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            SystemTray.SetProgressIndicator(this, prog);
            prog.Text = "Loading...";
            currArtist = await artist.getInfo(artistName);
            this.DataContext = currArtist;
            webBrowser1.NavigateToString(makeHtml(currArtist.bio));
            prog.IsIndeterminate = false;
            prog.IsVisible = false;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string artistName = "";
            if (NavigationContext.QueryString.TryGetValue("artistName", out artistName) && string.Equals(panArtist.Title, artistName))
            {
                panArtist.Title = artistName;
                getArtistInfo(artistName);
            }
        }

        private void webBrowser1_Navigating(object sender, NavigatingEventArgs e)
        {
            if (((WebBrowser)sender).Opacity != 0)
                e.Cancel = true;
        }

        private void webBrowser1_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            ((WebBrowser)sender).Opacity = 1;
        }
    }
}