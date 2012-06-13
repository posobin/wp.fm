﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace lastfm
{
    public class album
    {
        public static async Task<List<albumInfo>> search(string text, int page = 0, int limit = 30)
        {
            RequestParameters rParams = new RequestParameters();
            rParams.Add("album", text);
            rParams.Add("method", "album.search");
            rParams.Add("limit", limit.ToString());
            rParams.Add("page", page.ToString());
            XDocument returnedXml = await Request.MakeRequest(rParams);
            if (Request.CheckStatus(returnedXml) == 0)
            {
                List<albumInfo> albums = new List<albumInfo>((from item in returnedXml.Descendants("albummatches").Elements() select new albumInfo(item)));
                return albums;
            }
            else
                MessageBox.Show("Sorry, there was some error while executing your request. " + Request.CheckStatus(returnedXml).ToString());
            return null;
        }

        public static async Task<albumInfo> getInfo(string artistName, string albumName, string username = "", string lang = "en")
        {
            RequestParameters rParams = new RequestParameters();
            rParams.Add("artist", artistName);
            rParams.Add("album", albumName);
            if (!string.IsNullOrEmpty(username))
                rParams.Add("username", username);
            rParams.Add("lang", lang);
            rParams.Add("method", "album.getinfo");
            XDocument returnedXml = await Request.MakeRequest(rParams);
            if (Request.CheckStatus(returnedXml) == 0)
            {
                albumInfo album = new albumInfo(returnedXml.Element("lfm").Element("album"));
                return album;
            }
            else
                MessageBox.Show("Sorry, there was some error while executing your request. " + Request.CheckStatus(returnedXml).ToString());
            return null;
        }
    }
}
