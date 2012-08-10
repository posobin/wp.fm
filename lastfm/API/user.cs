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
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace lastfm
{
    public class user
    {
        public static async Task<UserInfo> getInfo(string user)
        {
            RequestParameters rParams = new RequestParameters("user.getinfo");
            rParams.Add("user", user);
            XDocument ReturnedXML = await Request.MakeRequest(rParams);
            if (Request.CheckStatus(ReturnedXML) == 0)
                return new UserInfo(ReturnedXML.Element("lfm").Element("user"));
            else
                throw new LastFmAPIException(Request.GetErrorMessage(ReturnedXML), Request.CheckStatus(ReturnedXML));
        }

        public static async Task<List<trackInfo>> getRecentTracks(String user, int limit = 50, int page = 1)
        {
            RequestParameters rParams = new RequestParameters("user.getRecentTracks");
            rParams.Add("user", user);
            rParams.Add("limit", limit.ToString());
            rParams.Add("page", page.ToString());
            XDocument returnedXml = await Request.MakeRequest(rParams);
            if (Request.CheckStatus(returnedXml) == 0)
            {
                int totalPagesAttr = Int32.Parse(returnedXml.Element("lfm").Element("recenttracks").Attribute("totalPages").Value);
                if (page > totalPagesAttr)
                    throw new ArgumentOutOfRangeException("page", "Page number is greater than total amount of pages");
                List<trackInfo> tracks = new List<trackInfo>(from item in returnedXml.Element("lfm").Element("recenttracks").Elements() select new trackInfo(item));
                return tracks;
            }
            else
                throw new LastFmAPIException(Request.GetErrorMessage(returnedXml), Request.CheckStatus(returnedXml));
        }

        public static async Task<List<artistInfo>> getRecommendedArtists(int page = 1, int limit = 50)
        {
            RequestParameters rParams = new RequestParameters("user.getRecommendedArtists");
            rParams.Add("limit", limit.ToString());
            rParams.Add("page", page.ToString());
            rParams.Add("sk", Session.CurrentSession.SessionKey);
            XDocument returnedXml = await Request.MakeRequest(rParams, true);
            if (Request.CheckStatus(returnedXml) == 0)
            {
                int totalPagesAttr = Int32.Parse(returnedXml.Element("lfm").Element("recommendations").Attribute("totalPages").Value);
                if (page > totalPagesAttr)
                    throw new ArgumentOutOfRangeException("page", "Page number is greater than total amount of pages");
                List<artistInfo> artists = new List<artistInfo>(from item in returnedXml.Element("lfm").Element("recommendations").Elements() select new artistInfo(item));
                return artists;
            }
            else
                throw new LastFmAPIException(returnedXml);
        }
    }
}
