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
using System.Collections.ObjectModel;
using Microsoft.Phone.Controls;
using System.Windows.Data;
using Microsoft.Phone.Shell;

namespace lastfm
{
    public partial class artistSearchPage : PhoneApplicationPage
    {
        ObservableCollection<artistInfo> lstResults = new ObservableCollection<artistInfo>();
        ProgressIndicator prog;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string searchText = "";
            if (NavigationContext.QueryString.TryGetValue("searchText", out searchText))
                getList(searchText);
        }
        public artistSearchPage()
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
            foreach (artistInfo info in await artist.search(searchText))
                lstResults.Add(info);
            prog.IsIndeterminate = false;
            prog.IsVisible = false;
        }

        private void txtSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                lstResults.Clear();
                getList(txtSearchBox.Text);
            }
        }

        private void searchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex != -1)
                NavigationService.Navigate(new Uri("/artistInfoPage.xaml?artistName=" + ((artistInfo)((ListBox)sender).SelectedItem).name, UriKind.Relative));
        }
    }
}