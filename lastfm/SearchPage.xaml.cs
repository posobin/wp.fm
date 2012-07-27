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
using System.Windows.Controls.Primitives;
using System.Collections;

namespace lastfm
{
    class pair<T, K>
    {
        public T EntriesNumber { get; set; }
        public K PageNumber { get; set; }
        public pair(T first, K second)
        {
            EntriesNumber = first;
            PageNumber = second;
        }
    }
    public partial class SearchPage : PhoneApplicationPage
    {
        ObservableCollection<artistInfo> lstArtistResults = new ObservableCollection<artistInfo>();
        ObservableCollection<albumInfo> lstAlbumResults = new ObservableCollection<albumInfo>();
        ObservableCollection<trackInfo> lstTrackResults = new ObservableCollection<trackInfo>();
        ObservableCollection<tagInfo> lstTagResults = new ObservableCollection<tagInfo>();
        ProgressIndicator prog;
        //last string searched for
        private string lastArtistString;
        private string lastAlbumString;
        private string lastTrackString;
        private string lastTagString;
        //overall number of search restults and number of the current page (starts with 1)
        private pair<int, int> artistNums = new pair<int, int>(0, 1);
        private pair<int, int> albumNums = new pair<int, int>(0, 1);
        private pair<int, int> trackNums = new pair<int, int>(0, 1);
        private pair<int, int> tagNums = new pair<int, int>(0, 1);

        bool HookedArtistScrolling = false;
        bool HookedAlbumScrolling = false;
        bool HookedTrackScrolling = false;
        bool HookedTagScrolling = false;

        public SearchPage()
        {
            InitializeComponent();
            artistResults.DataContext = lstArtistResults;
            albumResults.DataContext = lstAlbumResults;
            trackResults.DataContext = lstTrackResults;
            tagResults.DataContext = lstTagResults;
            prog = new ProgressIndicator();
            SystemTray.SetProgressIndicator(this, prog);
            artistResults.Loaded += new RoutedEventHandler(artistResults_Loaded);
            albumResults.Loaded += new RoutedEventHandler(albumResults_Loaded);
            trackResults.Loaded += new RoutedEventHandler(trackResults_Loaded);
            tagResults.Loaded += new RoutedEventHandler(tagResults_Loaded);
        }

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
                        if (lstAlbumResults.Count == 0 || searchText != txtSearchBox.Text)
                        {
                            SearchPivot.SelectedIndex = 1;
                            getAlbumList(searchText);
                        }
                        break;
                    case "track":
                        if (lstTrackResults.Count == 0 || searchText != txtSearchBox.Text)
                        {
                            SearchPivot.SelectedIndex = 2;
                            getTrackList(searchText);
                        }
                        break;
                    case "tag":
                        if (lstTagResults.Count == 0 || searchText != txtSearchBox.Text)
                        {
                            SearchPivot.SelectedIndex = 3;
                            getTagList(searchText);
                        }
                        break;
                    case "artist":
                    default:
                        if (lstTrackResults.Count == 0 || searchText != txtSearchBox.Text)
                        {
                            SearchPivot.SelectedIndex = 0;
                            getArtistList(searchText);
                        }
                        break;
                }
            }
            else if (String.IsNullOrEmpty(txtSearchBox.Text))
                Dispatcher.BeginInvoke(() => txtSearchBox.Focus());
        }

        #region Methods for infinite scroll

        #region Loaded events

        //taken from http://blogs.msdn.com/b/slmperf/archive/2011/06/30/windows-phone-mango-change-listbox-how-to-detect-compression-end-of-scroll-states.aspx
        void artistResults_Loaded(object sender, RoutedEventArgs e)
        {
            if (HookedArtistScrolling)
                return;
            HookedArtistScrolling = true;

            ScrollViewer sv_artists = (ScrollViewer)utilities.FindElementRecursive(artistResults, typeof(ScrollViewer));

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

        void albumResults_Loaded(object sender, RoutedEventArgs e)
        {
            if (HookedAlbumScrolling)
                return;
            HookedAlbumScrolling = true;

            ScrollViewer sv_albums = (ScrollViewer)utilities.FindElementRecursive(albumResults, typeof(ScrollViewer));

            if (sv_albums != null)
            {
                FrameworkElement element = VisualTreeHelper.GetChild(sv_albums, 0) as FrameworkElement;
                if (element != null)
                {
                    VisualStateGroup vgroup = utilities.FindVisualState(element, "VerticalCompression");
                    if (vgroup != null)
                        vgroup.CurrentStateChanging += new EventHandler<VisualStateChangedEventArgs>(vgroupAlbums_CurrentStateChanging);
                }
            }
        }

        void trackResults_Loaded(object sender, RoutedEventArgs e)
        {
            if (HookedTrackScrolling)
                return;
            HookedTrackScrolling = true;

            ScrollViewer sv_tracks = (ScrollViewer)utilities.FindElementRecursive(trackResults, typeof(ScrollViewer));

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

        void tagResults_Loaded(object sender, RoutedEventArgs e)
        {
            if (HookedTagScrolling)
                return;

            HookedTagScrolling = true;

            ScrollViewer sv_tags = (ScrollViewer)utilities.FindElementRecursive(tagResults, typeof(ScrollViewer));

            if (sv_tags != null)
            {
                FrameworkElement element = VisualTreeHelper.GetChild(sv_tags, 0) as FrameworkElement;
                if (element != null)
                {
                    VisualStateGroup vgroup = utilities.FindVisualState(element, "VerticalCompression");
                    if (vgroup != null)
                        vgroup.CurrentStateChanging += new EventHandler<VisualStateChangedEventArgs>(vgroupTags_CurrentStateChanging);
                }
            }
        }

        #endregion
        #region *CurrentStateChanging

        private void vgroupArtists_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState.Name == "CompressionBottom")
            {
                //Load more items to the artist list
                loadMoreArtists();
            }
        }

        private void vgroupAlbums_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState.Name == "CompressionBottom")
            {
                //Load more items to the album list
                loadMoreAlbums();
            }
        }

        private void vgroupTracks_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState.Name == "CompressionBottom")
            {
                //Load more items to the track list
                loadMoreTracks();
            }
        }

        private void vgroupTags_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState.Name == "CompressionBottom")
            {
                //Load more items to the tag list
                loadMoreTags();
            }
        }
        #endregion

        #endregion

        #region Downloading lists

        #region List downloaders

        private async void getArtistList(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return;
            txtSearchBox.Text = searchText;
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Searching for artists...";
            List<artistInfo> lst = new List<artistInfo>();
            try
            { lst = await artist.search(searchText); }
            catch (TaskCanceledException) { }

            foreach (artistInfo info in lst)
                lstArtistResults.Add(info);

            prog.IsIndeterminate = false;
            prog.IsVisible = false;
            lastArtistString = searchText;
            artistNums.PageNumber++;
        }

        private async void getAlbumList(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return;
            txtSearchBox.Text = searchText;
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Searching for albums...";

            List<albumInfo> lst = new List<albumInfo>();
            try
            { lst = (await album.search(searchText)); }
            catch (TaskCanceledException) { }

            foreach (albumInfo info in lst)
                lstAlbumResults.Add(info);

            prog.IsIndeterminate = false;
            prog.IsVisible = false;
            lastAlbumString = searchText;
            albumNums.PageNumber++;
        }

        private async void getTrackList(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return;
            txtSearchBox.Text = searchText;
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Searching for tracks...";

            List<trackInfo> lst = new List<trackInfo>();
            try
            { lst = await track.search(searchText); }
            catch (TaskCanceledException) { }

            foreach (trackInfo info in lst)
                lstTrackResults.Add(info);

            prog.IsIndeterminate = false;
            prog.IsVisible = false;
            lastTrackString = searchText;
            trackNums.PageNumber++;
        }

        private async void getTagList(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return;
            txtSearchBox.Text = searchText;
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Searching for tags...";

            List<tagInfo> lst = new List<tagInfo>();
            try
            { lst = await tag.search(searchText); }
            catch (TaskCanceledException) { }

            foreach (tagInfo info in lst)
                lstTagResults.Add(info);

            prog.IsIndeterminate = false;
            prog.IsVisible = false;
            lastTagString = searchText;
            tagNums.PageNumber++;
        }
        #endregion

        #region Load more...

        private async void loadMoreArtists()
        {
            if (String.IsNullOrEmpty(lastArtistString))
                return;
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Loading more artists...";

            List<artistInfo> lst = new List<artistInfo>();

            try
            { lst = await artist.search(lastArtistString, artistNums.PageNumber++); }
            catch (TaskCanceledException) { artistNums.PageNumber--; }
            catch (IndexOutOfRangeException) { prog.Text = "No more results"; }

            foreach (artistInfo info in lst)
                lstArtistResults.Add(info);

            prog.IsVisible = false;
            prog.IsIndeterminate = false;
        }

        private async void loadMoreAlbums()
        {
            if (string.IsNullOrEmpty(lastAlbumString))
                return;
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Loading more albums...";

            List<albumInfo> lst = new List<albumInfo>();

            try
            { lst = new List<albumInfo>(await album.search(lastAlbumString, albumNums.PageNumber++)); }
            catch (TaskCanceledException) { albumNums.PageNumber--; }
            catch (IndexOutOfRangeException) { prog.Text = "No more results"; }

            foreach (albumInfo info in lst)
                lstAlbumResults.Add(info);

            prog.IsIndeterminate = false;
            prog.IsVisible = false;
        }

        private async void loadMoreTracks()
        {
            if (string.IsNullOrEmpty(lastTrackString))
                return;
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Loading more tracks...";

            List<trackInfo> lst = new List<trackInfo>();
            try
            { lst = new List<trackInfo>(await track.search(lastTrackString, trackNums.PageNumber++)); }
            catch (TaskCanceledException) { trackNums.PageNumber--; }
            catch (IndexOutOfRangeException) { prog.Text = "No more results"; }

            foreach (trackInfo info in lst)
                lstTrackResults.Add(info);

            prog.IsIndeterminate = false;
            prog.IsVisible = false;
        }

        private async void loadMoreTags()
        {
            if (string.IsNullOrEmpty(lastTagString))
                return;
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Loading more tags...";

            List<tagInfo> lst = new List<tagInfo>();
            try
            { lst = new List<tagInfo>(await tag.search(lastTagString, tagNums.PageNumber++)); }
            catch (TaskCanceledException) { tagNums.PageNumber--; }
            catch (IndexOutOfRangeException) { prog.Text = "No more results"; }

            foreach (tagInfo info in lst)
                lstTagResults.Add(info);

            prog.IsIndeterminate = false;
            prog.IsVisible = false;
        }

        #endregion

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

        /// <summary>
        /// Reacts when enter key is pressed in the search texbox so as to hide the keyboard
        /// </summary>
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
    }
}