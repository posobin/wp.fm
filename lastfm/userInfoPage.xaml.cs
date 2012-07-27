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
        bool HookedResultsScrolling = false;
        int NextTracksPage = 0;

        public userInfoPage()
        {
            InitializeComponent();
            recentTrackslb.Loaded += new RoutedEventHandler(recentTrackslb_Loaded);
        }

        void recentTrackslb_Loaded(object sender, RoutedEventArgs e)
        {
            if (HookedResultsScrolling == true)
                return;
            HookedResultsScrolling = true;
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
            tracksLoading.VerticalAlignment = VerticalAlignment.Top;
            tracksLoading.Visibility = Visibility.Visible;
            List<trackInfo> lst = new List<trackInfo>();
            try { lst = await user.getRecentTracks(Session.CurrentSession.UserName, 50, NextTracksPage++); }
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
            InfoGrid.DataContext = currUser;
            InfoProg.Visibility = Visibility.Collapsed;
            InfoGrid.Visibility = Visibility.Visible;
        }

        private void userPan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as Panorama).SelectedIndex == 1 && recentTrackslb.Items.Count == 0)
                GetRecentTracks();
        }

        private async void GetRecentTracks()
        {
            tracksLoading.VerticalAlignment = VerticalAlignment.Center;
            tracksLoading.Visibility = Visibility.Visible;
            recentTrackslb.Visibility = Visibility.Collapsed;
            List<trackInfo> lst = new List<trackInfo>();
            try { lst = await user.getRecentTracks(Session.CurrentSession.UserName); }
            catch (TaskCanceledException) { }
            foreach (trackInfo item in lst)
                RecentTracks.Add(item);
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
    }
}