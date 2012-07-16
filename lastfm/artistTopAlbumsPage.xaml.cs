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
using System.Threading.Tasks;

namespace lastfm
{
    public partial class artistTopAlbumsPage : PhoneApplicationPage
    {
        public artistTopAlbumsPage()
        {
            InitializeComponent();
            SystemTray.ProgressIndicator = prog;
        }

        List<albumInfo> albums;
        ProgressIndicator prog = new ProgressIndicator();

        private async void GetAlbums(string artistName)
        {
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Loading items...";

            try { albums = await artist.getTopAlbums(artistName); }
            catch (TaskCanceledException) { };

            prog.IsVisible = false;
            prog.IsIndeterminate = false;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string artistName;
            if (NavigationContext.QueryString.TryGetValue("artistName", out artistName)) GetAlbums(artistName);
            else NavigationService.GoBack();
        }
    }
}