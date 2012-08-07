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
                Uri navigateTo = utilities.processBBcodeLink(e.Value);
                if (navigateTo != null)
                    NavigationService.Navigate(navigateTo);
            }
        }

        private async void getTrackInfo(string artistName, string trackName)
        {
            // Notify user that request is being processed
            SystemTray.IsVisible = true;
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Loading...";

            // Download track information
            try 
            {
                if (Session.CanUseCurrentSession())
                    currTrack = await track.getInfo(artistName, trackName, Session.CurrentSession.UserName); 
                else
                    currTrack = await track.getInfo(artistName, trackName); 
            }
            catch (TaskCanceledException) { }

            // Show track information to the user
            this.DataContext = currTrack;

            // Show track description
            string htmlDescription = utilities.makeHtmlFromCdata(currTrack.description, currTrack.extralargeImage);
            string descriptionFileName = utilities.SaveStringToFile(htmlDescription, "track.html");
            trackDescription.Navigate(new Uri(descriptionFileName, UriKind.Relative));

            // Add album link
            if (currTrack.album != null)
                AlbumLink.NavigateUri = utilities.getAlbumInfoPageUri(artistName, currTrack.album.name);

            UpdateAppbar();

            // Notify user that request was completed
            prog.IsIndeterminate = false;
            prog.IsVisible = false;
            SystemTray.IsVisible = false;
        }

        private void UpdateAppbar()
        {
            // Show love/unlove button if user is logined
            if (Session.CanUseCurrentSession())
            {
                if (currTrack.userloved == trackInfo.LoveState.Loved)
                    this.ApplicationBar = this.Resources["unloveAppbar"] as ApplicationBar;
                else if (currTrack.userloved == trackInfo.LoveState.Unloved)
                    this.ApplicationBar = this.Resources["loveAppbar"] as ApplicationBar;
            }
            else // Else don't show appbar
                this.ApplicationBar.IsVisible = false;
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
                ArtistLink.NavigateUri = utilities.getArtistInfoPageUri(artistName);
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
                tagInfo selectedTag = ((ListBox)sender).SelectedItem as tagInfo;
                this.NavigationService.Navigate(utilities.getTagInfoPageUri(selectedTag.name));
                ((ListBox)sender).SelectedIndex = -1;
            }
        }

        private void LoveOrUnlove(object sender, EventArgs e)
        {
            // If track is loved send unlove request
            if (currTrack.userloved == trackInfo.LoveState.Loved)
                track.unlove(currTrack.name, currTrack.artist.name);
            // Else send love request
            else
                track.love(currTrack.name, currTrack.artist.name);
            // Change loved state of the track
            switch (currTrack.userloved)
            {
                case trackInfo.LoveState.Loved:
                    currTrack.userloved = trackInfo.LoveState.Unloved;
                    break;
                case trackInfo.LoveState.Unloved:
                    currTrack.userloved = trackInfo.LoveState.Loved;
                    break;
            }
            // Update appbar
            UpdateAppbar();
        }
    }
}