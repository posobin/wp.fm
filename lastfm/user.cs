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
            RequestParameters rParams = new RequestParameters();
            rParams.Add("method", "user.getInfo");
            rParams.Add("user", user);
            XDocument ReturnedXML = await Request.MakeRequest(rParams);
            if (Request.CheckStatus(ReturnedXML) == 0)
                return new UserInfo(ReturnedXML.Element("lfm").Element("user"));
            else
                throw new LastFmAPIException(Request.GetErrorMessage(ReturnedXML), Request.CheckStatus(ReturnedXML));
        }

        public static async Task<List<trackInfo>> getRecentTracks(String user, int limit = 50, int page = 1)
        {
            RequestParameters rParams = new RequestParameters();
            rParams.Add("method", "user.getRecentTracks");
            rParams.Add("user", user);
            rParams.Add("limit", limit.ToString());
            rParams.Add("page", page.ToString());
            XDocument ReturnedXML = await Request.MakeRequest(rParams);
            if (Request.CheckStatus(ReturnedXML) == 0)
            {
                List<trackInfo> tracks = new List<trackInfo>((from item in ReturnedXML.Element("lfm").Element("recenttracks").Elements() select new trackInfo(item)));
                return tracks;
            }
            else
                throw new LastFmAPIException(Request.GetErrorMessage(ReturnedXML), Request.CheckStatus(ReturnedXML));
        }
    }
}
