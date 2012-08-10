using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Tasks;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.IO.IsolatedStorage;
using System.IO;

namespace lastfm
{
    public class utilities
    {
        const string NotifyScript = @"<script type='text/javascript' language='javascript'>
                                            window.onload = function(){
                                                var linkArray = document.getElementsByTagName('a');
                                                for(var i=0; i < linkArray.length; i++){
                                                    linkArray[i].onclick = function() { 
                                                        var str = this.href + "" "" + this.getAttribute(""className""); 
                                                        window.external.Notify(str); }
                                                }
                                            }
                                        </script>";

        public static string makeHtmlFromCdata(string cdata, Uri image = null)
        {
            string ret;
            string bgcolor = "#" + ((Color)App.Current.Resources["PhoneBackgroundColor"]).ToString().Substring(3);
            string fcolor = "#" + ((Color)App.Current.Resources["PhoneForegroundColor"]).ToString().Substring(3);
            ret = @"<html>
                        <head>
                        <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0"" />" +
                        NotifyScript +
                    @"</head>
                    <body " + "bgcolor=" + bgcolor + " style=\"color:" + fcolor + "\"" + ">";
            if (image != null) ret += String.Format("<p align=\"center\"><img src=\"{0}\" style=\"margin:auto;display:block;\" /></p>", image.ToString());
            ret +=      ((string.IsNullOrEmpty(cdata)) ? "Sorry, no description found" : cdata.Replace("\n", "<br>")) +
                    "</body>" +
                  "</html>";
            return ret;
        }

        public static Uri processBBcodeLink(string str)
        {
            Uri navigateTo = null;
            if (str.EndsWith(" "))
            {
                WebBrowserTask browser = new WebBrowserTask();
                browser.Uri = new Uri(str, UriKind.Absolute);
                browser.Show();
            }
            else if (str.EndsWith(" bbcode_artist"))
            {
                string artistName = str.Remove(str.Length - " bbcode_artist".Length).Split(new char[] { '/' }).Last();
                artistName = HttpUtility.UrlDecode(artistName);
                navigateTo = utilities.getArtistInfoPageUri(artistName);
            }
            else if (str.EndsWith(" bbcode_tag"))
            {
                string tagName = str.Remove(str.Length - " bbcode_tag".Length).Split(new char[] { '/' }).Last();
                tagName = HttpUtility.UrlDecode(tagName);
                navigateTo = utilities.getTagInfoPageUri(tagName);
            }
            else if (str.EndsWith(" bbcode_album"))
            {
                string[] lst = str.Remove(str.Length - " bbcode_album".Length).Split(new char[] { '/' });
                string artistName = HttpUtility.UrlDecode(lst[lst.Length - 2]);
                string albumName = HttpUtility.UrlDecode(lst[lst.Length - 1]);
                navigateTo = utilities.getAlbumInfoPageUri(artistName, albumName);
            }
            else if (str.EndsWith(" bbcode_track"))
            {
                string[] lst = str.Remove(str.Length - " bbcode_track".Length).Split(new char[] { '/' });
                string artistName = HttpUtility.UrlDecode(lst[lst.Length - 3]);
                string trackName = HttpUtility.UrlDecode(lst[lst.Length - 1]);
                navigateTo = utilities.getTrackInfoPageUri(artistName, trackName);
            }
            else
            {
                int index = str.LastIndexOf(" ");
                WebBrowserTask browser = new WebBrowserTask();
                browser.Uri = new Uri(str.Substring(0, index), UriKind.Absolute);
                browser.Show();
            }
            return navigateTo;
        }

        public static UIElement FindElementRecursive(FrameworkElement parent, Type targetType)
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

        public static VisualStateGroup FindVisualState(FrameworkElement element, string name)
        {
            if (element == null)
                return null;

            IList groups = VisualStateManager.GetVisualStateGroups(element);
            foreach (VisualStateGroup group in groups)
                if (group.Name == name)
                    return group;

            return null;
        }

        /// <summary>
        /// Saves string to the file. If file does already exist, rewrites it.
        /// </summary>
        /// <param name="str">String to write to the file</param>
        /// <param name="fileName">Name of the file. If not supplied, is temp.txt</param>
        /// <returns>Name of the file</returns>
        public static string SaveStringToFile(string str, string fileName = "temp.txt")
        {
            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName cannot be empty", "fileName");
            var store = IsolatedStorageFile.GetUserStoreForApplication();
            using (var stream = new IsolatedStorageFileStream(fileName, FileMode.Create, FileAccess.Write, store))
            {
                using (var sw = new StreamWriter(stream))
                    sw.Write(str);
            }
            return fileName;
        }

        /// <summary>
        /// Constructs uri for navigating to the artist information page. Parameters should NOT be urlencoded.
        /// </summary>
        /// <param name="artistName">Artist name to load info for.</param>
        /// <returns>Constructed uri</returns>
        public static Uri getArtistInfoPageUri(string artistName)
        {
            string baseString= @"/Info pages/artistInfoPage.xaml";
            if (string.IsNullOrEmpty(artistName))
                throw new ArgumentException("artistName cannot be empty", "artistName");
            return new Uri(string.Format("{0}?artistName={1}", baseString, HttpUtility.UrlEncode(artistName)), UriKind.Relative);
        }

        /// <summary>
        /// Constructs uri for navigating to the album information page. Parameters should NOT be urlencoded.
        /// </summary>
        /// <param name="artistName">Album's artist</param>
        /// <param name="albumName">Album's name</param>
        /// <returns>Constructed uri</returns>
        public static Uri getAlbumInfoPageUri(string artistName, string albumName)
        {
            string baseString = @"/Info pages/albumInfoPage.xaml";
            if (string.IsNullOrEmpty(artistName))
                throw new ArgumentException("artistName cannot be empty", "artistName");
            if (string.IsNullOrEmpty(albumName))
                throw new ArgumentException("albumName cannot be empty", "albumName");
            return new Uri(string.Format("{0}?artistName={1}&albumName={2}", 
                                        baseString, 
                                        HttpUtility.UrlEncode(artistName), 
                                        HttpUtility.UrlEncode(albumName)), 
                            UriKind.Relative);
        }

        /// <summary>
        /// Constructs uri for navigating to the track information page. Parameters should NOT be urlencoded.
        /// </summary>
        /// <param name="artistName">Track's artist</param>
        /// <param name="trackName">Track's name</param>
        /// <returns>Constructed uri</returns>
        public static Uri getTrackInfoPageUri(string artistName, string trackName)
        {
            string baseString = @"/Info pages/trackInfoPage.xaml";
            if (string.IsNullOrEmpty(artistName))
                throw new ArgumentException("artistName cannot be empty", "artistName");
            if (string.IsNullOrEmpty(trackName))
                throw new ArgumentException("trackName cannot be empty", "trackName");
            return new Uri(string.Format("{0}?artistName={1}&trackName={2}",
                                        baseString,
                                        HttpUtility.UrlEncode(artistName),
                                        HttpUtility.UrlEncode(trackName)),
                            UriKind.Relative);
        }

        /// <summary>
        /// Constructs uri for navigating to the tag information page. Parameters should NOT be urlencoded.
        /// </summary>
        /// <param name="tagName">Tag name</param>
        /// <returns>Constructed uri</returns>
        public static Uri getTagInfoPageUri(string tagName)
        {
            string baseString = @"/Info pages/tagInfoPage.xaml";
            if (string.IsNullOrEmpty(tagName))
                throw new ArgumentException("tagName connot be empty", "tagName");
            return new Uri(string.Format("{0}?tagName={1}", baseString, tagName), UriKind.Relative);
        }

        public static Uri NoImageBig { get{ return new Uri("/Images/NoImageBig.png", UriKind.Relative); } }
    }
}
