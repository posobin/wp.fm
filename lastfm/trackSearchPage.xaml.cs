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
    public partial class trackSearchPage : PhoneApplicationPage
    {
        ObservableCollection<trackInfo> lstResults = new ObservableCollection<trackInfo>();
        ProgressIndicator prog;

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string searchText = "";
            if (NavigationContext.QueryString.TryGetValue("searchText", out searchText))
                getList(searchText);
        }

        public trackSearchPage()
        {
            InitializeComponent();
            this.DataContext = lstResults;
            prog = new ProgressIndicator();
        }

        private async void getList(string searchText)
        {
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
                lstResults.Add(info);

            prog.IsIndeterminate = false;
            prog.IsVisible = false;
        }

        private void txtSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                getList(txtSearchBox.Text);
        }

        private void searchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex != -1)
            {
                trackInfo selected = (trackInfo)((ListBox)sender).SelectedItem;
                this.NavigationService.Navigate(new Uri("/trackInfoPage.xaml?trackName=" + HttpUtility.UrlEncode(selected.name) + "&artistName=" + HttpUtility.UrlEncode(selected.artist.name), UriKind.Relative));
            }
        }
    }
}