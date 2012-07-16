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
using Microsoft.Devices;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;
using Microsoft.Phone.Net.NetworkInformation;

namespace lastfm
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            
            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMilliseconds(33);
            dt.Tick += delegate { try { FrameworkDispatcher.Update(); } catch { } };
            dt.Start();

            MediaPlayer.MediaStateChanged += new EventHandler<EventArgs>(MediaPlayer_MediaStateChanged);
        }

        /// <summary>
        /// Event being fired when track is being changed, stoped, etc.
        /// </summary>
        void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            UpdateNowPlaying();
            if (Session.LastSong != MediaPlayer.Queue.ActiveSong)
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    Session.LastSong = MediaPlayer.Queue.ActiveSong;
                    if (Session.AutoScrobbling == true)
                        ScrobbleNowPlaying();
                }
            }
        }

        /// <summary>
        /// Shows up info about currently playing song
        /// </summary>
        private void UpdateNowPlaying()
        {
            Song NowPlaying = MediaPlayer.Queue.ActiveSong;
            if (NowPlaying != null)
            {
                SongTitle.Content = NowPlaying.Name;
                SongTitle.NavigateUri = new Uri("/SearchPage.xaml?searchText=" + HttpUtility.UrlEncode(NowPlaying.Name) + "&searchType=track", UriKind.Relative);
                AlbumName.Content = NowPlaying.Album.Name;
                AlbumName.NavigateUri = new Uri("/SearchPage.xaml?searchText=" + HttpUtility.UrlEncode(NowPlaying.Album.Name) + "&searchType=album", UriKind.Relative);
                ArtistName.Content = NowPlaying.Artist.Name;
                ArtistName.NavigateUri = new Uri("/SearchPage.xaml?searchText=" + HttpUtility.UrlEncode(NowPlaying.Artist.Name) + "&searchType=artist", UriKind.Relative);
                if (NowPlaying.Album.HasArt == true)
                {
                    BitmapImage AlbumImage = new BitmapImage();
                    AlbumImage.SetSource(NowPlaying.Album.GetAlbumArt());
                    AlbumCover.Source = AlbumImage;
                }
            }
            else
            {
                SongTitle.Content = "Nothing is playing";
                AlbumName.Content = "";
                ArtistName.Content = "";
                AlbumCover.Source = new BitmapImage(new Uri("NoImageBig.png", UriKind.Relative));
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                UpdateAppBar();
                UpdateNowPlaying();
            }
            else
                MessageBox.Show("No internet connection is available");
        }

        /// <summary>
        /// Sends user to the login page
        /// </summary>
        private void Login(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Deletes info about last session
        /// </summary>
        private void Logout(object sender, EventArgs e)
        {
            Session.CurrentSession = null;
            UpdateAppBar();
        }

        /// <summary>
        /// Event for handling different AppBars for each PivotItem
        /// </summary>
        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAppBar();
        }

        /// <summary>
        /// Updates AppBar so as it is appropriate for the current PivotItem
        /// </summary>
        private void UpdateAppBar()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                switch (((Pivot)MainPivot).SelectedIndex)
                {
                    case 0:
                        if (Session.CurrentSession != null && Session.CurrentSession.SessionKey != null)
                            ApplicationBar = this.Resources["appbar_scrobble_l"] as ApplicationBar;
                        else
                            ApplicationBar = this.Resources["appbar_scrobble_nl"] as ApplicationBar;
                        ApplicationBar.IsVisible = true;
                        break;
                    default:
                        if (Session.CurrentSession != null && Session.CurrentSession.SessionKey != null)
                            ApplicationBar = this.Resources["appbar_logout"] as ApplicationBar;
                        else
                            ApplicationBar = this.Resources["appbar_login"] as ApplicationBar;
                        break;
                }
            }
            else
                ApplicationBar = this.Resources["appbar_empty"] as ApplicationBar;
        }

        /// <summary>
        /// Navigates user to the SearchPage
        /// </summary>
        private void Search(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/searchPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Calls ScrobbleNowPlaying
        /// </summary>
        private void Scrobble(object sender, EventArgs e)
        {
            ScrobbleNowPlaying();
        }

        /// <summary>
        /// Scrobbles song currently being played
        /// </summary>
        private void ScrobbleNowPlaying()
        {
            prog.IsIndeterminate = true;
            prog.IsVisible = true;
            prog.Text = "Scrobbling...";
            SystemTray.ProgressIndicator = prog;
            Song NowPlaying = MediaPlayer.Queue.ActiveSong;
            if (NowPlaying != null && Session.CurrentSession != null && Session.CurrentSession.SessionKey != null)
            {
                SongTitle.Content = NowPlaying.Name;
                ArtistName.Content = NowPlaying.Artist.Name;
                try { track.scrobble(NowPlaying.Artist.Name, NowPlaying.Name); }
                catch (TaskCanceledException) { }
            }
            else if (NowPlaying == null)
                MessageBox.Show("Sorry, but nothing is playing now");
            prog.IsIndeterminate = false;
            prog.IsVisible = false;
        }

        /// <summary>
        /// Navigates user to the settings page
        /// </summary>
        private void LaunchSettingsPage(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        ProgressIndicator prog = new ProgressIndicator();
    }
}