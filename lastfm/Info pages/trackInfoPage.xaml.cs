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
using System.Threading.Tasks;
using System.IO;
using System.IO.IsolatedStorage;

namespace lastfm
{
    public partial class trackInfoPage : PhoneApplicationPage
    {
        public trackInfoPage()
        {
            InitializeComponent();
            this.DataContext = currTrack;
            SystemTray.SetProgressIndicator(this, prog);
            wbh = new WebBrowserHelper(trackDescription);
            wbh.ScrollDisabled = false;
        }

        trackInfo currTrack;
        ProgressIndicator prog = new ProgressIndicator();
        WebBrowserHelper wbh = null;

        private void ScriptNotify(object sender, NotifyEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value))
            {
                string navigateTo = utilities.processBBcodeLink(e.Value);
                if (navigateTo != "")
                    NavigationService.Navigate(new Uri(navigateTo, UriKind.Relative));
            }
        }

        private async void getTrackInfo(string artistName, string trackName)
        {
            SystemTray.IsVisible = true;
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Loading...";

            try { currTrack = await track.getInfo(artistName, trackName); }
            catch (TaskCanceledException) { }

            this.DataContext = currTrack;

            //NavigateToString method works bad with encodings, that's why I am using this stuff
            var store = IsolatedStorageFile.GetUserStoreForApplication();
            using (var stream = new IsolatedStorageFileStream("track.html", FileMode.Create, FileAccess.Write, store))
            {
                using (var sw = new StreamWriter(stream))
                    sw.Write(utilities.makeHtmlFromCdata(currTrack.description, currTrack.extralargeImage));
            }
            trackDescription.Navigate(new Uri("track.html", UriKind.Relative));

            if (currTrack.album != null)
                AlbumLink.NavigateUri = new Uri("/albumInfoPage.xaml?artistName=" + HttpUtility.UrlEncode(artistName) +
                                                "&albumName=" + HttpUtility.UrlEncode(currTrack.album.name), UriKind.Relative);
            prog.IsIndeterminate = false;
            prog.IsVisible = false;
            SystemTray.IsVisible = false;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string trackName = "";
            string artistName = "";
            if (this.NavigationContext.QueryString.TryGetValue("trackName", out trackName) &&
                this.NavigationContext.QueryString.TryGetValue("artistName", out artistName) &&
                (!string.Equals(albumInfoPanorama.Title, trackName) || !string.Equals(ArtistLink.Content, artistName)))
            {
                ArtistLink.NavigateUri = new Uri("/artistInfoPage.xaml?artistName=" + HttpUtility.UrlEncode(artistName), UriKind.Relative);
                ArtistLink.Content = artistName;
                albumInfoPanorama.Title = trackName;
                getTrackInfo(artistName, trackName);
            }
        }

        private void trackDescription_Navigating(object sender, NavigatingEventArgs e)
        {
            if (((WebBrowser)sender).Opacity != 0)
                e.Cancel = true;
        }

        private void trackDescription_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            ((WebBrowser)sender).Opacity = 1;
        }

        private void tagsLst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex != -1)
            {
                this.NavigationService.Navigate(new Uri("/Info pages/tagInfoPage.xaml?tagName=" + HttpUtility.UrlEncode(((tagInfo)((ListBox)sender).SelectedItem).name), UriKind.Relative));
                ((ListBox)sender).SelectedIndex = -1;
            }
        }
    }
}