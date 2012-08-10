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
        private const int NOTHING_SELECTED_INDEX = -1;
        ObservableCollection<artistInfo> lstArtistResults = new ObservableCollection<artistInfo>();
        ObservableCollection<albumInfo> lstAlbumResults = new ObservableCollection<albumInfo>();
        ObservableCollection<trackInfo> lstTrackResults = new ObservableCollection<trackInfo>();
        ObservableCollection<tagInfo> lstTagResults = new ObservableCollection<tagInfo>();
        ProgressIndicator prog;
        //last strings searched for
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

        enum PivotItem
        {
            First = 0, Artist = 0, Album = 1, Track = 2, Tag = 3, Last = 3
        }

        public SearchPage()
        {
            InitializeComponent();
            artistResults.DataContext = lstArtistResults;
            albumResults.DataContext = lstAlbumResults;
            trackResults.DataContext = lstTrackResults;
            tagResults.DataContext = lstTagResults;
            artistResults.Loaded += new RoutedEventHandler(artistResults_Loaded);
            albumResults.Loaded += new RoutedEventHandler(albumResults_Loaded);
            trackResults.Loaded += new RoutedEventHandler(trackResults_Loaded);
            tagResults.Loaded += new RoutedEventHandler(tagResults_Loaded);
            prog = new ProgressIndicator();
            SystemTray.SetProgressIndicator(this, prog);
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
                    case "artist":
                        if (lstAlbumResults.Count == 0 || searchText != txtSearchBox.Text)
                        {
                            SearchPivot.SelectedIndex = (int)PivotItem.Artist;
                            getArtistList(searchText);
                        }
                        break;
                    case "album":
                        if (lstAlbumResults.Count == 0 || searchText != txtSearchBox.Text)
                        {
                            SearchPivot.SelectedIndex = (int)PivotItem.Album;
                            getAlbumList(searchText);
                        }
                        break;
                    case "track":
                        if (lstTrackResults.Count == 0 || searchText != txtSearchBox.Text)
                        {
                            SearchPivot.SelectedIndex = (int)PivotItem.Track;
                            getTrackList(searchText);
                        }
                        break;
                    case "tag":
                        if (lstTagResults.Count == 0 || searchText != txtSearchBox.Text)
                        {
                            SearchPivot.SelectedIndex = (int)PivotItem.Tag;
                            getTagList(searchText);
                        }
                        break;
                    default:
                        if (lstTrackResults.Count == 0 || searchText != txtSearchBox.Text)
                        {
                            SearchPivot.SelectedIndex = (int)PivotItem.Artist;
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
            ShowProgressIndicator("Searching for artists...");

            try
            {
                List<artistInfo> lst = new List<artistInfo>();
                lst = await artist.search(searchText);

                foreach (artistInfo info in lst)
                    lstArtistResults.Add(info);
                artistNums.PageNumber++;
            }
            finally
            {
                HideProgressIndicator();
                lastArtistString = searchText;
            }
        }

        private async void getAlbumList(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return;
            txtSearchBox.Text = searchText;
            ShowProgressIndicator("Searching for albums...");

            try
            {
                List<albumInfo> lst = new List<albumInfo>();
                lst = (await album.search(searchText));

                foreach (albumInfo info in lst)
                    lstAlbumResults.Add(info);
                albumNums.PageNumber++;
            }
            finally
            {
                HideProgressIndicator();
                lastAlbumString = searchText;
            }
        }

        private async void getTrackList(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return;
            txtSearchBox.Text = searchText;
            ShowProgressIndicator("Searching for tracks...");

            try
            {
                List<trackInfo> lst = new List<trackInfo>();
                lst = await track.search(searchText);

                foreach (trackInfo info in lst)
                    lstTrackResults.Add(info);
                trackNums.PageNumber++;
            }
            finally
            {
                HideProgressIndicator();
                lastTrackString = searchText;
            }
        }

        private async void getTagList(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return;
            txtSearchBox.Text = searchText;
            ShowProgressIndicator("Searching for tags...");

            try
            {
                List<tagInfo> lst = new List<tagInfo>();
                lst = await tag.search(searchText);

                foreach (tagInfo info in lst)
                    lstTagResults.Add(info);
                tagNums.PageNumber++;
            }
            finally
            {
                HideProgressIndicator();
                lastTagString = searchText;
            }
        }
        #endregion

        #region Load more...

        private async void loadMoreArtists()
        {
            if (String.IsNullOrEmpty(lastArtistString))
                return;
            ShowProgressIndicator("Loading more artists...");

            try
            {
                List<artistInfo> lst = new List<artistInfo>();
                lst = await artist.search(lastArtistString, artistNums.PageNumber);
                // If add increment inside tag.search call 
                // then if the exception occured page number would still increase, 
                // so we would have to decrement it inside of exception handler
                artistNums.PageNumber++;

                foreach (artistInfo info in lst)
                    lstArtistResults.Add(info);
            }
            catch (IndexOutOfRangeException) { prog.Text = "No more results"; }
            finally { HideProgressIndicator(); }
        }

        private async void loadMoreAlbums()
        {
            if (string.IsNullOrEmpty(lastAlbumString))
                return;
            ShowProgressIndicator("Loading more albums...");

            try
            {
                List<albumInfo> lst = new List<albumInfo>();
                lst = new List<albumInfo>(await album.search(lastAlbumString, albumNums.PageNumber));
                // If add increment inside tag.search call 
                // then if the exception occured page number would still increase, 
                // so we would have to decrement it inside of exception handler
                albumNums.PageNumber++;

                foreach (albumInfo info in lst)
                    lstAlbumResults.Add(info);
            }
            catch (IndexOutOfRangeException) { prog.Text = "No more results"; }
            finally { HideProgressIndicator(); }
        }

        private async void loadMoreTracks()
        {
            if (string.IsNullOrEmpty(lastTrackString))
                return;
            ShowProgressIndicator("Loading more tracks...");

            try
            {
                List<trackInfo> lst = new List<trackInfo>();
                lst = new List<trackInfo>(await track.search(lastTrackString, trackNums.PageNumber));
                // If add increment inside tag.search call 
                // then if the exception occured page number would still increase, 
                // so we would have to decrement it inside of exception handler
                trackNums.PageNumber++;

                foreach (trackInfo info in lst)
                    lstTrackResults.Add(info);
            }
            catch (IndexOutOfRangeException) { prog.Text = "No more results"; }
            finally { HideProgressIndicator(); }
        }

        private async void loadMoreTags()
        {
            if (string.IsNullOrEmpty(lastTagString))
                return;
            ShowProgressIndicator("Loading more tags...");

            try
            {
                List<tagInfo> lst = new List<tagInfo>();
                lst = new List<tagInfo>(await tag.search(lastTagString, tagNums.PageNumber));
                // If add increment inside tag.search call 
                // then if the exception occured page number would still increase, 
                // so we would have to decrement it inside of exception handler
                tagNums.PageNumber++;

                foreach (tagInfo info in lst)
                    lstTagResults.Add(info);
            }
            catch (IndexOutOfRangeException) { prog.Text = "No more results"; }
            finally { HideProgressIndicator(); }
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
            switch ((PivotItem)SearchPivot.SelectedIndex)
            {
                case PivotItem.Artist:
                    getArtistList(searchText);
                    break;
                case PivotItem.Album:
                    getAlbumList(searchText);
                    break;
                case PivotItem.Track:
                    getTrackList(searchText);
                    break;
                case PivotItem.Tag:
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
            if (((ListBox)sender).SelectedIndex != NOTHING_SELECTED_INDEX)
            {
                artistInfo selectedArtist = ((ListBox)sender).SelectedItem as artistInfo;
                NavigationService.Navigate(utilities.getArtistInfoPageUri(selectedArtist.name));
                ((ListBox)sender).SelectedIndex = NOTHING_SELECTED_INDEX;
            }
        }

        private void albumResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex != NOTHING_SELECTED_INDEX)
            {
                albumInfo selectedAlbum = ((ListBox)sender).SelectedItem as albumInfo;
                this.NavigationService.Navigate(utilities.getAlbumInfoPageUri(selectedAlbum.artistName, selectedAlbum.name));
                ((ListBox)sender).SelectedIndex = NOTHING_SELECTED_INDEX;
            }
        }

        private void trackResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex != NOTHING_SELECTED_INDEX)
            {
                trackInfo selectedTrack = (trackInfo)((ListBox)sender).SelectedItem;
                this.NavigationService.Navigate(utilities.getTrackInfoPageUri(selectedTrack.artist.name, selectedTrack.name));
                ((ListBox)sender).SelectedIndex = NOTHING_SELECTED_INDEX;
            }
        }

        private void tagResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex != NOTHING_SELECTED_INDEX)
            {
                tagInfo selectedTag = ((ListBox)sender).SelectedItem as tagInfo;
                this.NavigationService.Navigate(utilities.getTagInfoPageUri(selectedTag.name));
                ((ListBox)sender).SelectedIndex = NOTHING_SELECTED_INDEX;
            }
        }

        private void SearchPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((PivotItem)SearchPivot.SelectedIndex)
            {
                case PivotItem.Artist:
                    if (lstArtistResults.Count == 0)
                        getArtistList(txtSearchBox.Text);
                    break;
                case PivotItem.Album:
                    if (lstAlbumResults.Count == 0)
                        getAlbumList(txtSearchBox.Text);
                    break;
                case PivotItem.Track:
                    if (lstTrackResults.Count == 0)
                        getTrackList(txtSearchBox.Text);
                    break;
                case PivotItem.Tag:
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
                // Hide keyboard
                this.Focus();
                // Clear all the previous searches
                lstArtistResults.Clear();
                lstAlbumResults.Clear();
                lstTrackResults.Clear();
                lstTagResults.Clear();
                // Begin new search
                getList(txtSearchBox.Text);
            }
        }

        private void HideProgressIndicator()
        {
            SystemTray.ProgressIndicator = prog;
            prog.IsIndeterminate = false;
            prog.IsVisible = false;
        }

        private void ShowProgressIndicator(string text = "")
        {
            if (!string.IsNullOrEmpty(text))
                prog.Text = text;
            prog.IsIndeterminate = true;
            prog.IsVisible = true;
            SystemTray.ProgressIndicator = prog;
        }
    }
}