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
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lastfm
{
    public class track
    {
        public static async Task<List<trackInfo>> search(string trackName, int page = 0, int limit = 30)
        {
            RequestParameters rParams = new RequestParameters();
            rParams.Add("track", HttpUtility.UrlEncode(trackName));
            rParams.Add("method", "track.search");
            rParams.Add("limit", limit.ToString());
            rParams.Add("page", page.ToString());
            XDocument returnedXml = await Request.MakeRequest(rParams);
            if (Request.CheckStatus(returnedXml) == 0)
            {
                List<trackInfo> tracks = new List<trackInfo>((from item in returnedXml.Descendants("trackmatches").Elements() select new trackInfo(item)));
                XNamespace opensearch = @"http://a9.com/-/spec/opensearch/1.1/";
                IEnumerable<XElement> opensearch_ = from el in returnedXml.Element("lfm").Element("results").Elements()
                                                    where el.Name.Namespace == opensearch
                                                    select el;
                int totalResults = Int32.Parse((from el in opensearch_ where el.Name.LocalName == "totalResults" select el.Value).First());
                int startIndex = Int32.Parse((from el in opensearch_ where el.Name.LocalName == "startIndex" select el.Value).First());
                int itemsPerPage = Int32.Parse((from el in opensearch_ where el.Name.LocalName == "itemsPerPage" select el.Value).First());
                if (totalResults - startIndex < 0)
                    throw new IndexOutOfRangeException("Page being shown is the first page");
                return tracks;
            }
            else
                MessageBox.Show("Sorry, there was some error while executing your request. " + Request.CheckStatus(returnedXml).ToString());
            return null;
        }

        public static async Task<trackInfo> getInfo(string artistName, string trackName, string username = "")
        {
            RequestParameters rParams = new RequestParameters();
            rParams.Add("artist", HttpUtility.UrlEncode(artistName));
            rParams.Add("track", HttpUtility.UrlEncode(trackName));
            if (!string.IsNullOrEmpty(username))
                rParams.Add("username", username);
            rParams.Add("method", "track.getinfo");
            XDocument returnedXml = await Request.MakeRequest(rParams);
            if (Request.CheckStatus(returnedXml) == 0)
            {
                trackInfo track = new trackInfo(returnedXml.Element("lfm").Element("track"));
                return track;
            }
            else
                MessageBox.Show("Sorry, there was some error while executing your request. " + Request.CheckStatus(returnedXml).ToString());
            return null;
        }

        public static async void scrobble(string artistName, string trackName, DateTime timestamp = default(DateTime))
        {
            if (Session.CurrentSession == null || Session.CurrentSession.SessionKey == null)
                MessageBox.Show("This service requires authentication");
            int timeStamp;
            if (timestamp != default(DateTime))
               timeStamp  = (int)(timestamp - new DateTime(1970, 1, 1)).TotalSeconds;
            else
                timeStamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            RequestParameters rParams = new RequestParameters();
            rParams.Add("artist", artistName);
            rParams.Add("track", trackName);
            rParams.Add("timestamp", timeStamp.ToString());
            rParams.Add("method", "track.scrobble");
            rParams.Add("sk", Session.CurrentSession.SessionKey);
            XDocument returnedXml = await Request.MakeRequest(rParams, true);
            if (Request.CheckStatus(returnedXml) != 0)
                MessageBox.Show("Sorry, there was some error while executing your request. " + Request.CheckStatus(returnedXml).ToString());
        }

        public static async void updateNowPlaying(string artistName, string trackName, string albumName = null)
        {
            if (Session.CurrentSession == null || Session.CurrentSession.SessionKey == null)
                MessageBox.Show("This service requires authentication");
            RequestParameters rParams = new RequestParameters();
            rParams.Add("method", "track.updateNowPlaying");
            rParams.Add("artist", artistName);
            rParams.Add("track", trackName);
            rParams.Add("sk", Session.CurrentSession.SessionKey);
            if (!String.IsNullOrEmpty(albumName))
                rParams.Add("album", albumName);
            XDocument returnedXml = await Request.MakeRequest(rParams, true);
            if (Request.CheckStatus(returnedXml) != 0)
                MessageBox.Show("Sorry, there was some error while executing your request. " + Request.CheckStatus(returnedXml).ToString());
        }
    }
}
