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
            currAlbum = await album.getInfo(artistName, albumName);
            this.DataContext = currAlbum;
            albumDescription.NavigateToString(utilities.makeHtmlFromCdata(currAlbum.description));
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
                ArtistLink.NavigateUri = new Uri("/artistInfoPage.xaml?artistName=" + artistName, UriKind.Relative);
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
                this.NavigationService.Navigate(new Uri("/tagInfoPage.xaml?tagName=" + ((tagInfo)((ListBox)sender).SelectedItem).name, UriKind.Relative));
        }

        private void tracksLst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex != -1)
                this.NavigationService.Navigate(new Uri("/trackInfoPage.xaml?trackName=" + ((trackInfo)((ListBox)sender).SelectedItem).name, UriKind.Relative));
        }
    }
}