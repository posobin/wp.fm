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

        public static string makeHtmlFromCdata(string cdata)
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
                    <body " + "bgcolor=" + bgcolor + " style=\"color:" + fcolor + "\"" + ">" +
                            ((string.IsNullOrEmpty(cdata)) ? "Sorry, no description found" : cdata.Replace("\n", "<br>")) +
                    "</body>" +
                  "</html>";
            return ret;
        }

        public static string processBBcodeLink(string str)
        {
            string navigateTo = "";
            if (str.EndsWith(" "))
            {
                WebBrowserTask browser = new WebBrowserTask();
                browser.Uri = new Uri(str, UriKind.Absolute);
                browser.Show();
            }
            else if (str.EndsWith(" bbcode_artist"))
            {
                navigateTo = str.Remove(str.Length - " bbcode_artist".Length).Split(new char[] { '/' }).Last();
                navigateTo = "/artistInfoPage.xaml?artistName=" + navigateTo;
            }
            else if (str.EndsWith(" bbcode_tag"))
            {
                navigateTo = str.Remove(str.Length - " bbcode_tag".Length).Split(new char[] { '/' }).Last();
                navigateTo = "/tagInfoPage.xaml?tagName=" + navigateTo;
            }
            else if (str.EndsWith(" bbcode_album"))
            {
                string[] lst = str.Remove(str.Length - " bbcode_album".Length).Split(new char[] { '/' });
                navigateTo = "/albumInfoPage.xaml?albumName=" + lst.Last() + "&artistName=" + lst[lst.Length - 2];
            }
            else if (str.EndsWith(" bbcode_track"))
            {
                string[] lst = str.Remove(str.Length - " bbcode_track".Length).Split(new char[] { '/' });
                navigateTo = "/trackInfoPage.xaml?trackName=" + lst.Last() + "&artistName=" + lst[lst.Length - 3];
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
    }
}
