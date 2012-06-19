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

        void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            UpdateNowPlaying();
        }

        private void UpdateNowPlaying()
        {
            Song NowPlaying = MediaPlayer.Queue.ActiveSong;
            if (NowPlaying != null)
            {
                SongTitle.Content = NowPlaying.Name;
                SongTitle.NavigateUri = new Uri("/trackSearchPage.xaml?searchText=" + HttpUtility.UrlEncode(NowPlaying.Name), UriKind.Relative);
                AlbumName.Content = NowPlaying.Album.Name;
                AlbumName.NavigateUri = new Uri("/albumSearchPage.xaml?searchText=" + HttpUtility.UrlEncode(NowPlaying.Album.Name), UriKind.Relative);
                ArtistName.Content = NowPlaying.Artist.Name;
                ArtistName.NavigateUri = new Uri("/artistSearchPage.xaml?searchText=" + HttpUtility.UrlEncode(NowPlaying.Artist.Name), UriKind.Relative);
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
            UpdateNowPlaying();
        }

        private void Login(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((Pivot)sender).SelectedIndex)
            {
                case 1:
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

        private void Search(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/searchPage.xaml", UriKind.Relative));
        }
    }
}