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
            SystemTray.SetProgressIndicator(this, prog);
        }

        ProgressIndicator prog;
        artistInfo currArtist = new artistInfo();

        private void ScriptNotify(object sender, NotifyEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value))
            {
                string navigateTo = utilities.processBBcodeLink(e.Value);
                if (navigateTo != "")
                    NavigationService.Navigate(new Uri(navigateTo, UriKind.Relative));
            }
        }

        private async void getArtistInfo(string artistName)
        {
            SystemTray.IsVisible = true;
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Loading...";
            currArtist = await artist.getInfo(artistName);
            this.DataContext = currArtist;
            webBrowser1.NavigateToString(utilities.makeHtmlFromCdata(currArtist.bio));
            prog.IsIndeterminate = false;
            prog.IsVisible = false;
            SystemTray.IsVisible = false;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string artistName = "";
            if (NavigationContext.QueryString.TryGetValue("artistName", out artistName) && !string.Equals(panArtist.Title, artistName))
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

        private void tags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex != -1)
                NavigationService.Navigate(new Uri("/tagInfoPage.xaml?tagName=" + HttpUtility.UrlEncode(((tagInfo)((ListBox)sender).SelectedItem).name), UriKind.Relative));
        }

        private void similarArtists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex != -1)
                NavigationService.Navigate(new Uri("/artistInfoPage.xaml?artistName=" + HttpUtility.UrlEncode(((artistInfo)((ListBox)sender).SelectedItem).name), UriKind.Relative));
        }
    }
}