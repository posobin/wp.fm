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
using System.Windows.Controls.Primitives;

namespace lastfm
{
    public partial class MainPage : PhoneApplicationPage
    {
        ProgressIndicator prog = new ProgressIndicator();
        Song LastSong = null;
        DateTime LastSongBegan = default(DateTime);

        public MainPage()
        {
            InitializeComponent();

            // Add timer for ActiveSongChanged and MediaStateChanged
            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMilliseconds(33);
            dt.Tick += delegate { try { FrameworkDispatcher.Update(); } catch { } };
            dt.Start();

            MediaPlayer.MediaStateChanged += new EventHandler<EventArgs>(MediaStateChanged);
            MediaPlayer.ActiveSongChanged += new EventHandler<EventArgs>(ActiveSongChanged);
        }

        #region Scrobbling

        /// <summary>
        /// Event being fired when track was changed
        /// </summary>
        void ActiveSongChanged(object sender, EventArgs e)
        {
            // Update info about now playing song
            UpdateNowPlayingPivot();
            if (Session.AutoScrobbling == true)
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    // Scrobble song that ended playing 
                    // Last.fm recommends scrobbling at the end of the song
                    if (LastSong != null)
                    {
                        Scrobble(LastSong, LastSongBegan);
                        LastSong = MediaPlayer.Queue.ActiveSong;
                        LastSongBegan = DateTime.Now;
                    }
                    UpdateNowPlaying();
                }
                else
                {
                    // Offline scrobbling
                    trackInfo track = new trackInfo(MediaPlayer.Queue.ActiveSong, DateTime.Now);
                    if (Session.Scrobbles.Contains(track))
                        Scrobbling.Scrobble();
                    else
                        Session.Scrobbles.Add(new trackInfo(MediaPlayer.Queue.ActiveSong, DateTime.Now));
                }
            }
        }

        /// <summary>
        /// Event being fired when track is being flash-forwarded, stoped, etc.
        /// </summary>
        void MediaStateChanged(object sender, EventArgs e)
        {
            UpdateNowPlayingPivot();
            // ActiveSongChanged is not being fired when application launches and the song starts playing
            Song NowPlaying = MediaPlayer.Queue.ActiveSong;
            if (NowPlaying != null && LastSong == null)
                if (NetworkInterface.GetIsNetworkAvailable())
                    if (Session.AutoScrobbling == true)
                    {
                        UpdateNowPlaying();
                        LastSong = MediaPlayer.Queue.ActiveSong;
                        LastSongBegan = DateTime.Now;
                    }
        }

        /// <summary>
        /// Shows up info about currently playing song
        /// </summary>
        private void UpdateNowPlayingPivot()
        {
            Song NowPlaying = MediaPlayer.Queue.ActiveSong;
            if (NowPlaying != null)
            {
                // Show song title
                SongTitle.Content = NowPlaying.Name;
                SongTitle.NavigateUri = new Uri("/SearchPage.xaml?searchText=" + HttpUtility.UrlEncode(NowPlaying.Name) + "&searchType=track", UriKind.Relative);
                // Show album title
                AlbumName.Content = NowPlaying.Album.Name;
                AlbumName.NavigateUri = new Uri("/SearchPage.xaml?searchText=" + HttpUtility.UrlEncode(NowPlaying.Album.Name) + "&searchType=album", UriKind.Relative);
                // Show artist name
                ArtistName.Content = NowPlaying.Artist.Name;
                ArtistName.NavigateUri = new Uri("/SearchPage.xaml?searchText=" + HttpUtility.UrlEncode(NowPlaying.Artist.Name) + "&searchType=artist", UriKind.Relative);
                // If album has image then show it
                if (NowPlaying.Album.HasArt == true)
                {
                    BitmapImage AlbumImage = new BitmapImage();
                    AlbumImage.SetSource(NowPlaying.Album.GetAlbumArt());
                    AlbumCover.Source = AlbumImage;
                }
                // Else show that album has no image
                else
                    AlbumCover.Source = new BitmapImage(utilities.NoImageBig);
            }
            else
            {
                SongTitle.Content = "Nothing is playing";
                AlbumName.Content = "";
                ArtistName.Content = "";
                AlbumCover.Source = new BitmapImage(utilities.NoImageBig);
            }
        }

        /// <summary>
        /// Calls ScrobbleNowPlaying
        /// </summary>
        private void ScrobbleClick(object sender, EventArgs e)
        {
            try { Scrobble(MediaPlayer.Queue.ActiveSong, DateTime.Now); }
            catch (ArgumentException) { MessageBox.Show("Nothing is playing"); }
        }

        /// <summary>
        /// Scrobbles song currently being played
        /// </summary>
        private void Scrobble(Song song, DateTime songBegan)
        {
            if (song == null)
                throw new ArgumentNullException("song");
            if (songBegan == null)
                throw new ArgumentNullException("songBegan");
            // Notify user that request is being processed
            prog.IsIndeterminate = true;
            prog.IsVisible = true;
            prog.Text = "Scrobbling...";
            SystemTray.ProgressIndicator = prog;
            // If current session is valid, add song to list of the unscrobbled songs
            if (Session.CanUseCurrentSession())
                Session.Scrobbles.Add(new trackInfo(song, songBegan));
            // Else notify user that he must be logined
            else
                MessageBox.Show("Login to be able to use scrobbling");
            // Show that request has completed
            prog.IsIndeterminate = false;
            prog.IsVisible = false;
        }

        /// <summary>
        /// Updates Now Playing track on the server
        /// </summary>
        private void UpdateNowPlaying()
        {
            Song NowPlaying = MediaPlayer.Queue.ActiveSong;
            prog.IsIndeterminate = true;
            prog.IsVisible = true;
            prog.Text = "Updating now playing...";
            SystemTray.ProgressIndicator = prog;
            if (Session.CanUseCurrentSession())
                if (NowPlaying != null)
                {
                    try { track.updateNowPlaying(NowPlaying.Artist.Name, NowPlaying.Name, NowPlaying.Album.Name); }
                    catch (TaskCanceledException) { }
                }
            prog.IsIndeterminate = false;
            prog.IsVisible = false;
        }

        #endregion

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                UpdateAppBar();
                UpdateNowPlayingPivot();
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
                        if (Session.CanUseCurrentSession())
                            ApplicationBar = this.Resources["appbar_scrobble"] as ApplicationBar;
                        else
                            ApplicationBar = this.Resources["appbar_login"] as ApplicationBar;
                        ApplicationBar.IsVisible = true;
                        break;
                    default:
                        if (Session.CanUseCurrentSession())
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
        /// Navigates to the settings page
        /// </summary>
        private void LaunchSettingsPage(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Navigates to the user info page
        /// </summary>
        private void LaunchUserInfoPage(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Info pages/userInfoPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Navigates to the about page
        /// </summary>
        private void LaunchAboutPage(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }
    }
}