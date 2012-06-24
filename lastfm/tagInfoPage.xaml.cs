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
    public partial class tagInfoPage : PhoneApplicationPage
    {
        public tagInfoPage()
        {
            InitializeComponent();
            SystemTray.SetProgressIndicator(this, prog);
        }

        tagInfo currTag;
        ProgressIndicator prog = new ProgressIndicator();

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string tagName = "";
            if (NavigationContext.QueryString.TryGetValue("tagName", out tagName) && !string.Equals(tagName, PageTitle.Text))
            {
                PageTitle.Text = tagName;
                getTagInfo(tagName);
            }
        }

        private async void getTagInfo(string tagName)
        {
            prog.IsVisible = true;
            prog.IsIndeterminate = true;

            try { currTag = await tag.getInfo(tagName); }
            catch (TaskCanceledException) { }
            
            webBrowser1.NavigateToString(utilities.makeHtmlFromCdata(currTag.wiki));
            this.DataContext = currTag;
            prog.IsVisible = false;
            prog.IsIndeterminate = false;
        }

        private void ScriptNotify(object sender, NotifyEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value))
            {
                string navigateTo = utilities.processBBcodeLink(e.Value);
                if (navigateTo != "")
                    NavigationService.Navigate(new Uri(navigateTo, UriKind.Relative));
            }
        }

        private void webBrowser1_Navigating(object sender, NavigatingEventArgs e)
        {
            if (((WebBrowser)sender).Opacity != 0)
                e.Cancel = true;
        }

        private void webBrowser1_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            ((WebBrowser)sender).Opacity = 1;
        }
    }
}