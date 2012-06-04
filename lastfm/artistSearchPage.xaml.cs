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
using System.Windows.Data;
using Microsoft.Phone.Shell;

namespace lastfm
{
    public partial class artistSearchPage : PhoneApplicationPage
    {
        List<artistInfo> lstResults;
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
            prog = new ProgressIndicator();
        }
        private async void getList(string searchText)
        {
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Loading...";
            lstResults = await artist.search(searchText);
            txtSearch.Text = searchText;
            this.DataContext = lstResults;
            prog.IsIndeterminate = false;
            prog.IsVisible = false;
        }
    }
}