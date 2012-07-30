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
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace lastfm
{
    public partial class userInfoPage : PhoneApplicationPage
    {
        private UserInfo currUser { get; set; }
        private ObservableCollection<trackInfo> RecentTracks { get; set; }
        private ObservableCollection<artistInfo> RecommendedArtists { get; set; }
        bool HookedTracksScrolling = false;
        bool HookedArtistsScrolling = false;
        int NextTracksPage = 0;
        int NextArtistsPage = 0;

        public userInfoPage()
        {
            InitializeComponent();
            recentTrackslb.Loaded += new RoutedEventHandler(recentTrackslb_Loaded);
            recomArtistslb.Loaded += new RoutedEventHandler(recomArtistslb_Loaded);
        }

        void recomArtistslb_Loaded(object sender, RoutedEventArgs e)
        {
            if (HookedArtistsScrolling == true)
                return;
            HookedArtistsScrolling = true;
            ScrollViewer sv_artists = (ScrollViewer)utilities.FindElementRecursive(recomArtistslb, typeof(ScrollViewer));

            if (sv_artists != null)
            {
                FrameworkElement element = VisualTreeHelper.GetChild(sv_artists, 0) as FrameworkElement;
                if (element != null)
                {
                    VisualStateGroup vgroup = utilities.FindVisualState(element, "VerticalCompression");
                    if (vgroup != null)
                        vgroup.CurrentStateChanging += new EventHandler<VisualStateChangedEventArgs>(vgroupArtists_CurrentStateChanging);
                }
            }
        }

        void vgroupArtists_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState.Name == "CompressionBottom")
                loadMoreRecommendedArtists();
        }

        private async void loadMoreRecommendedArtists()
        {
            recomArtistsLoading.VerticalAlignment = VerticalAlignment.Bottom;
            recomArtistsLoading.Visibility = Visibility.Visible;
            List<artistInfo> lst = new List<artistInfo>();
            try { lst = await user.getRecommendedArtists(NextTracksPage++, 50); }
            catch (ArgumentOutOfRangeException) { }
            catch (TaskCanceledException) { }

            foreach (artistInfo item in lst)
                RecommendedArtists.Add(item);

            recomArtistsLoading.Visibility = Visibility.Collapsed;
            recomArtistsLoading.VerticalAlignment = VerticalAlignment.Center;
        }

        void recentTrackslb_Loaded(object sender, RoutedEventArgs e)
        {
            if (HookedTracksScrolling == true)
                return;
            HookedTracksScrolling = true;
            ScrollViewer sv_tracks = (ScrollViewer)utilities.FindElementRecursive(recentTrackslb, typeof(ScrollViewer));

            if (sv_tracks != null)
            {
                FrameworkElement element = VisualTreeHelper.GetChild(sv_tracks, 0) as FrameworkElement;
                if (element != null)
                {
                    VisualStateGroup vgroup = utilities.FindVisualState(element, "VerticalCompression");
                    if (vgroup != null)
                        vgroup.CurrentStateChanging += new EventHandler<VisualStateChangedEventArgs>(vgroupTracks_CurrentStateChanging);
                }
            }
        }

        private void vgroupTracks_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState.Name == "CompressionBottom")
            {
                //Load more items to the recent track list
                loadMoreTracks();
            }
        }

        private async void loadMoreTracks()
        {
            tracksLoading.VerticalAlignment = VerticalAlignment.Bottom;
            tracksLoading.Visibility = Visibility.Visible;
            List<trackInfo> lst = new List<trackInfo>();
            try { lst = await user.getRecentTracks(Session.CurrentSession.UserName, 50, NextTracksPage++); }
            catch (ArgumentOutOfRangeException) { }
            catch (TaskCanceledException) { }

            foreach (trackInfo item in lst)
                RecentTracks.Add(item);

            tracksLoading.Visibility = Visibility.Collapsed;
            tracksLoading.VerticalAlignment = VerticalAlignment.Center;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            InfoGrid.Visibility = Visibility.Collapsed;
            if (Session.CurrentSession != null && Session.CurrentSession.UserName != null)
            {
                UpdateUser();
                InfoGrid.DataContext = currUser;
            }
            else
                MessageBox.Show("Log in to be able to view user info page");
        }

        private async void UpdateUser()
        {
            InfoProg.Visibility = Visibility.Visible;
            RecentTracks = new ObservableCollection<trackInfo>();
            try { currUser = await user.getInfo(Session.CurrentSession.UserName); }
            catch (TaskCanceledException) { }
            this.DataContext = currUser;
            InfoGrid.DataContext = currUser;
            InfoProg.Visibility = Visibility.Collapsed;
            InfoGrid.Visibility = Visibility.Visible;
        }

        private void userPan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as Panorama).SelectedIndex == 1 && recentTrackslb.Items.Count == 0)
                GetRecentTracks();
            else if ((sender as Panorama).SelectedIndex == 2 && recomArtistslb.Items.Count == 0)
                GetRecommendedArtists();
        }

        private async void GetRecommendedArtists()
        {
            recomArtistsLoading.VerticalAlignment = VerticalAlignment.Center;
            recomArtistsLoading.Visibility = Visibility.Visible;
            recomArtistslb.Visibility = Visibility.Collapsed;
            List<artistInfo> lst = new List<artistInfo>();
            try { lst = await user.getRecommendedArtists(); }
            catch (TaskCanceledException) { }
            RecommendedArtists = new ObservableCollection<artistInfo>(lst);
            recomArtistsLoading.Visibility = Visibility.Collapsed;
            recomArtistslb.ItemsSource = RecommendedArtists;
            recomArtistslb.Visibility = Visibility.Visible;
            NextArtistsPage = 2;
        }

        private async void GetRecentTracks()
        {
            tracksLoading.VerticalAlignment = VerticalAlignment.Center;
            tracksLoading.Visibility = Visibility.Visible;
            recentTrackslb.Visibility = Visibility.Collapsed;
            List<trackInfo> lst = new List<trackInfo>();
            try { lst = await user.getRecentTracks(Session.CurrentSession.UserName); }
            catch (TaskCanceledException) { }
            RecentTracks = new ObservableCollection<trackInfo>(lst);
            tracksLoading.Visibility = Visibility.Collapsed;
            recentTrackslb.ItemsSource = RecentTracks;
            recentTrackslb.Visibility = Visibility.Visible;
            NextTracksPage = 2;
        }

        private void recentTrackslb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex != -1)
            {
                trackInfo selected = (trackInfo)((ListBox)sender).SelectedItem;
                this.NavigationService.Navigate(new Uri("/trackInfoPage.xaml?trackName=" + HttpUtility.UrlEncode(selected.name) + "&artistName=" + HttpUtility.UrlEncode(selected.artist.name), UriKind.Relative));
                ((ListBox)sender).SelectedIndex = -1;
            }
        }

        private void recomArtistslb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex != -1)
            {
                artistInfo selected = (artistInfo)((ListBox)sender).SelectedItem;
                this.NavigationService.Navigate(new Uri("/artistInfoPage.xaml?artistName=" + HttpUtility.UrlEncode(selected.name), UriKind.Relative));
                ((ListBox)sender).SelectedIndex = -1;
            }
        }
    }
}