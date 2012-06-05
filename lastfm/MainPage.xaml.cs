﻿using System;
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

namespace lastfm
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string selectedItem = ((ListBoxItem) lstSearch.SelectedItem).Content.ToString();
            if (selectedItem == "artist") this.NavigationService.Navigate(new Uri("/artistSearchPage.xaml?searchText="+txtSearchText.Text, UriKind.Relative));
            else if (selectedItem == "album") this.NavigationService.Navigate(new Uri("/albumSearchPage.xaml?searchText=" + txtSearchText.Text, UriKind.Relative));
        }
    }
}