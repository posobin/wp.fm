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
            wbh = new WebBrowserHelper(albumDescription);
            wbh.ScrollDisabled = false;
        }

        albumInfo currAlbum;
        ProgressIndicator prog = new ProgressIndicator();
        WebBrowserHelper wbh = null;

        private void ScriptNotify(object sender, NotifyEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value))
            {
                Uri navigateTo = utilities.processBBcodeLink(e.Value);
                if (navigateTo != null)
                    NavigationService.Navigate(navigateTo);
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
                        sw.Write(utilities.makeHtmlFromCdata(currAlbum.description, currAlbum.extralargeImage));
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
                ArtistLink.NavigateUri = utilities.getArtistInfoPageUri(artistName);
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
                tagInfo selectedTag = ((ListBox)sender).SelectedItem as tagInfo;
                this.NavigationService.Navigate(utilities.getTagInfoPageUri(selectedTag.name));
                ((ListBox)sender).SelectedIndex = -1;
            }
        }

        private void tracksLst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex != -1)
            {
                trackInfo selectedTrack = (trackInfo)((ListBox)sender).SelectedItem;
                this.NavigationService.Navigate(utilities.getTrackInfoPageUri(selectedTrack.artist.name, selectedTrack.name));
                ((ListBox)sender).SelectedIndex = -1;
            }
        }
    }
}