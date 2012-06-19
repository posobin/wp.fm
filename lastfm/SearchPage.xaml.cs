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
using System.Collections.ObjectModel;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;

namespace lastfm
{
    public partial class SearchPage : PhoneApplicationPage
    {
        public SearchPage()
        {
            InitializeComponent();
            artistResults.DataContext = lstArtistResults;
            albumResults.DataContext = lstAlbumResults;
            trackResults.DataContext = lstTrackResults;
            tagResults.DataContext = lstTagResults;
            prog = new ProgressIndicator();
        }
        ObservableCollection<artistInfo> lstArtistResults = new ObservableCollection<artistInfo>();
        ObservableCollection<albumInfo> lstAlbumResults = new ObservableCollection<albumInfo>();
        ObservableCollection<trackInfo> lstTrackResults = new ObservableCollection<trackInfo>();
        ObservableCollection<tagInfo> lstTagResults = new ObservableCollection<tagInfo>();
        ProgressIndicator prog;

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string searchText = "";
            string searchType = "";
            if (NavigationContext.QueryString.TryGetValue("searchText", out searchText))
            {
                NavigationContext.QueryString.TryGetValue("searchType", out searchType);
                switch (searchType)
                {
                    case "album":
                        getAlbumList(searchText);
                        break;
                    case "track":
                        getTrackList(searchText);
                        break;
                    case "tag":
                        getTagList(searchText);
                        break;
                    case "artist":
                    default:
                        getArtistList(searchText);
                        break;
                }
            }
        }

        #region Downloading lists

        private async void getAlbumList(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return;
            txtSearchBox.Text = searchText;
            SystemTray.SetProgressIndicator(this, prog);
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Searching for albums...";

            List<albumInfo> lst = new List<albumInfo>();
            try
            { lst = new List<albumInfo>(await album.search(searchText)); }
            catch (TaskCanceledException) { }

            foreach (albumInfo info in lst)
                lstAlbumResults.Add(info);

            prog.IsIndeterminate = false;
            prog.IsVisible = false;
        }

        private async void getTrackList(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return;
            txtSearchBox.Text = searchText;
            SystemTray.SetProgressIndicator(this, prog);
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Loading...";

            List<trackInfo> lst = new List<trackInfo>();
            try
            {
                lst = new List<trackInfo>(await track.search(searchText));
            }
            catch (TaskCanceledException) { }

            foreach (trackInfo info in lst)
                lstTrackResults.Add(info);

            prog.IsIndeterminate = false;
            prog.IsVisible = false;
        }

        private async void getTagList(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return;
            txtSearchBox.Text = searchText;
            SystemTray.SetProgressIndicator(this, prog);
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Loading...";

            List<tagInfo> lst = new List<tagInfo>();
            try
            {
                lst = new List<tagInfo>(await tag.search(searchText));
            }
            catch (TaskCanceledException) { }

            foreach (tagInfo info in lst)
                lstTagResults.Add(info);

            prog.IsIndeterminate = false;
            prog.IsVisible = false;
        }

        private async void getArtistList(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return;
            txtSearchBox.Text = searchText;
            SystemTray.SetProgressIndicator(this, prog);
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Searching for artists...";
            List<artistInfo> lst = new List<artistInfo>();
            try
            {
                lst = await artist.search(searchText);
            }
            catch (TaskCanceledException) { }
            foreach (artistInfo info in lst)
                lstArtistResults.Add(info);
            prog.IsIndeterminate = false;
            prog.IsVisible = false;
            
        }
        /// <summary>
        /// Chooses which list (artist, track, album or tag) to download, depending on currently selected pivot page
        /// </summary>
        /// <param name="searchText">String to search for</param>
        private void getList(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return;
            switch (SearchPivot.SelectedIndex)
            {
                case 0:
                    //artists
                    getArtistList(searchText);
                    break;
                case 1:
                    //albums
                    getAlbumList(searchText);
                    break;
                case 2:
                    //tracks
                    getTrackList(searchText);
                    break;
                case 3:
                    //tags
                    getTagList(searchText);
                    break;
                default:
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Reacts when enter key is pressed in the search texbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Focus();
                lstArtistResults.Clear();
                lstAlbumResults.Clear();
                lstTrackResults.Clear();
                lstTagResults.Clear();
                getList(txtSearchBox.Text);
            }
        }

        #region Selection changes

        private void artistResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex != -1)
            {
                NavigationService.Navigate(new Uri("/artistInfoPage.xaml?artistName=" + HttpUtility.UrlEncode(((artistInfo)((ListBox)sender).SelectedItem).name), UriKind.Relative));
                ((ListBox)sender).SelectedIndex = -1;
            }
        }

        private void albumResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex != -1)
            {
                albumInfo selected = (albumInfo)((ListBox)sender).SelectedItem;
                this.NavigationService.Navigate(new Uri("/albumInfoPage.xaml?albumName=" + HttpUtility.UrlEncode(selected.name) + "&artistName=" + HttpUtility.UrlEncode(selected.artistName), UriKind.Relative));
                ((ListBox)sender).SelectedIndex = -1;
            }
        }

        private void trackResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex != -1)
            {
                trackInfo selected = (trackInfo)((ListBox)sender).SelectedItem;
                this.NavigationService.Navigate(new Uri("/trackInfoPage.xaml?trackName=" + HttpUtility.UrlEncode(selected.name) + "&artistName=" + HttpUtility.UrlEncode(selected.artist.name), UriKind.Relative));
                ((ListBox)sender).SelectedIndex = -1;
            }
        }

        private void tagResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex != -1)
            {
                this.NavigationService.Navigate(new Uri("/tagInfoPage.xaml?tagName=" + HttpUtility.UrlEncode(((tagInfo)((ListBox)sender).SelectedItem).name), UriKind.Relative));
                ((ListBox)sender).SelectedIndex = -1;
            }
        }

        private void SearchPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (SearchPivot.SelectedIndex)
            {
                case 0:
                    //artists
                    if (lstArtistResults.Count == 0)
                        getArtistList(txtSearchBox.Text);
                    break;
                case 1:
                    //albums
                    if (lstAlbumResults.Count == 0)
                        getAlbumList(txtSearchBox.Text);
                    break;
                case 2:
                    //tracks
                    if (lstTrackResults.Count == 0)
                        getTrackList(txtSearchBox.Text);
                    break;
                case 3:
                    //tags
                    if (lstTagResults.Count == 0)
                        getTagList(txtSearchBox.Text);
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}