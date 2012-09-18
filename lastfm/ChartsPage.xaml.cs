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
using System.Collections.ObjectModel;

namespace lastfm.API
{
    public partial class ChartsPage : PhoneApplicationPage
    {
        public ChartsPage()
        {
            InitializeComponent();
        }

        ObservableCollection<artistInfo> TopArtistsList = new ObservableCollection<artistInfo>();
        ProgressIndicator prog = new ProgressIndicator();

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            topArtistslb.DataContext = TopArtistsList;
            SystemTray.SetProgressIndicator(this, prog);
            getCharts();
        }

        private async void getCharts()
        {
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Loading...";

            try
            {
                List<artistInfo> lst = await chart.getTopArtists();

                foreach (artistInfo info in lst)
                    TopArtistsList.Add(info);
            }
            finally
            {
                prog.IsVisible = false;
            }
        }
    }
}