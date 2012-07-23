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
using System.IO.IsolatedStorage;
using System.IO;

namespace lastfm
{
    public partial class albumInfoPage : PhoneApplicationPage
    {
        public albumInfoPage()
        {
            InitializeComponent();
            this.DataContext = currAlbum;
            SystemTray.SetProgressIndicator(this, prog);
        }

        albumInfo currAlbum;
        ProgressIndicator prog = new ProgressIndicator();

        private void ScriptNotify(object sender, NotifyEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value))
            {
                string navigateTo = utilities.processBBcodeLink(e.Value);
                if (navigateTo != "")
                    NavigationService.Navigate(new Uri(navigateTo, UriKind.Relative));
            }
        }

        private async void getAlbumInfo(string artistName, string albumName)
        {
            SystemTray.IsVisible = true;
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Loading...";

            try { currAlbum = await album.getInfo(artistName, albumName); }
            catch (TaskCanceledException) { }

            this.DataContext = currAlbum;

            //NavigateToString method works bad with encodings, that's why I am using this stuff
            var store = IsolatedStorageFile.GetUserStoreForApplication();
            using (var stream = new IsolatedStorageFileStream("album.html", FileMode.Create, FileAccess.Write, store))
            {
                using (var sw = new StreamWriter(stream))
                    sw.Write(utilities.makeHtmlFromCdata(currAlbum.description));
            }
            albumDescription.Navigate(new Uri("album.html", UriKind.Relative));

            prog.IsIndeterminate = false;
            prog.IsVisible = false;
            SystemTray.IsVisible = false;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string albumName = "";
            string artistName = "";
            if (this.NavigationContext.QueryString.TryGetValue("albumName", out albumName) && 
                this.NavigationContext.QueryString.TryGetValue("artistName", out artistName) && 
                (!string.Equals(albumInfoPanorama.Title, albumName) || !string.Equals(ArtistLink.Content, artistName)))
            {
                ArtistLink.NavigateUri = new Uri("/artistInfoPage.xaml?artistName=" + HttpUtility.UrlEncode(artistName), UriKind.Relative);
                albumInfoPanorama.Title = albumName;
                ArtistLink.Content = artistName;
                getAlbumInfo(artistName, albumName);
            }
        }

        private void albumDescription_Navigating(object sender, NavigatingEventArgs e)
        {
            if (((WebBrowser)sender).Opacity != 0)
                e.Cancel = true;
        }

        private void albumDescription_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            ((WebBrowser)sender).Opacity = 1;
        }

        private void tagsLst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex != -1)
            {
                this.NavigationService.Navigate(new Uri("/tagInfoPage.xaml?tagName=" + HttpUtility.UrlEncode(((tagInfo)((ListBox)sender).SelectedItem).name), UriKind.Relative));
                ((ListBox)sender).SelectedIndex = -1;
            }
        }

        private void tracksLst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex != -1)
            {
                trackInfo selected = (trackInfo)((ListBox)sender).SelectedItem;
                ((ListBox)sender).SelectedIndex = -1;
                this.NavigationService.Navigate(new Uri("/trackInfoPage.xaml?trackName=" + HttpUtility.UrlEncode(selected.name) + "&artistName=" + HttpUtility.UrlEncode(selected.artist.name), UriKind.Relative));
            }
        }
    }
}