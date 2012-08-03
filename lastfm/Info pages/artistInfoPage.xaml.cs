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
using Microsoft.Phone.Tasks;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Collections;
using System.IO.IsolatedStorage;
using System.IO;

namespace lastfm
{
    public partial class artistInfoPage : PhoneApplicationPage
    {
        ProgressIndicator prog;
        artistInfo currArtist = new artistInfo();
        bool alreadyHookedScrollEvents = false;
        private ScrollBar sb = null;
        private ScrollViewer sv = null;
        WebBrowserHelper wbh = null;

        public artistInfoPage()
        {
            InitializeComponent();
            this.DataContext = currArtist;
            prog = new ProgressIndicator();
            SystemTray.SetProgressIndicator(this, prog);
            this.Loaded += new RoutedEventHandler(artistInfoPage_Loaded);
            wbh = new WebBrowserHelper(artistDescription);
            wbh.ScrollDisabled = false;
        }

        #region Methods for infinite scroll
        //taken from http://blogs.msdn.com/b/slmperf/archive/2011/06/30/windows-phone-mango-change-listbox-how-to-detect-compression-end-of-scroll-states.aspx
        private void artistInfoPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (alreadyHookedScrollEvents)
                return;

            alreadyHookedScrollEvents = true;
            sb = (ScrollBar)FindElementRecursive(similarArtists, typeof(ScrollBar));
            sv = (ScrollViewer)FindElementRecursive(similarArtists, typeof(ScrollViewer));

            if (sv != null)
            {
                FrameworkElement element = VisualTreeHelper.GetChild(sv, 0) as FrameworkElement;
                if (element != null)
                {
                    VisualStateGroup vgroup = FindVisualState(element, "VerticalCompression");
                    if (vgroup != null)
                        vgroup.CurrentStateChanging += new EventHandler<VisualStateChangedEventArgs>(vgroup_CurrentStateChanging);
                }
            }
        }

        private UIElement FindElementRecursive(FrameworkElement parent, Type targetType)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            UIElement returnElement = null;
            if (childCount > 0)
            {
                for (int i = 0; i < childCount; ++i)
                {
                    Object element = VisualTreeHelper.GetChild(parent, i);
                    if (element.GetType() == targetType)
                        return element as UIElement;
                    else
                        returnElement = FindElementRecursive(VisualTreeHelper.GetChild(parent, i) as FrameworkElement, targetType);
                }
            }
            return returnElement;
        }

        private VisualStateGroup FindVisualState(FrameworkElement element, string name)
        {
            if (element == null)
                return null;

            IList groups = VisualStateManager.GetVisualStateGroups(element);
            foreach (VisualStateGroup group in groups)
                if (group.Name == name)
                    return group;

            return null;
        }

        private void vgroup_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState.Name == "CompressionBottom")
            {
                //Load more items to the list
            }
        }

        #endregion

        private void ScriptNotify(object sender, NotifyEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value))
            {
                string navigateTo = utilities.processBBcodeLink(e.Value);
                if (navigateTo != "")
                    NavigationService.Navigate(new Uri(navigateTo, UriKind.Relative));
            }
        }

        private async void getArtistInfo(string artistName)
        {
            SystemTray.IsVisible = true;
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Loading...";

            try { currArtist = await artist.getInfo(artistName); }
            catch (TaskCanceledException) { }

            this.DataContext = currArtist;

            //NavigateToString method works bad with encodings, that's why I am using this stuff
            var store = IsolatedStorageFile.GetUserStoreForApplication();
            using (var stream = new IsolatedStorageFileStream("artist.html", FileMode.Create, FileAccess.Write, store))
            {
                using (var sw = new StreamWriter(stream))
                        sw.Write(utilities.makeHtmlFromCdata(currArtist.bio, currArtist.extralargeImage));
            }
            artistDescription.Navigate(new Uri("artist.html", UriKind.Relative));

            prog.IsIndeterminate = false;
            prog.IsVisible = false;
            SystemTray.IsVisible = false;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string artistName = "";
            if (NavigationContext.QueryString.TryGetValue("artistName", out artistName) && !string.Equals(panArtist.Title, artistName))
            {
                panArtist.Title = artistName;
                getArtistInfo(artistName);
            }
        }

        private void artistDescription_Navigating(object sender, NavigatingEventArgs e)
        {
            if (((WebBrowser)sender).Opacity != 0)
                e.Cancel = true;
        }

        private void artistDescription_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            ((WebBrowser)sender).Opacity = 1;
        }

        private void tags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex != -1)
            {
                NavigationService.Navigate(new Uri("/Info pages/tagInfoPage.xaml?tagName=" + HttpUtility.UrlEncode(((tagInfo)((ListBox)sender).SelectedItem).name), UriKind.Relative));
                ((ListBox)sender).SelectedIndex = -1;
            }
        }

        private void similarArtists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex != -1)
            {
                NavigationService.Navigate(new Uri("/Info pages/artistInfoPage.xaml?artistName=" + HttpUtility.UrlEncode(((artistInfo)((ListBox)sender).SelectedItem).name), UriKind.Relative));
                ((ListBox)sender).SelectedIndex = -1;
            }
        }
    }
}