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

        public userInfoPage()
        {
            InitializeComponent();
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
        }
    }
}