using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace lastfm
{
    public class artist
    {
        public static async Task<artistInfo> getInfo(string artistName)
        {
            RequestParameters rParams = new RequestParameters();
            rParams.Add("artist", HttpUtility.UrlEncode(artistName));
            rParams.Add("method", "artist.getinfo");
            XDocument returnedXml = await Request.MakeRequest(rParams);
            if (Request.CheckStatus(returnedXml) == 0)
            {
                artistInfo artist = new artistInfo(returnedXml.Element("lfm").Element("artist"));
                return artist;
            }
            else
                MessageBox.Show("Sorry, there was some error while executing your request. " + Request.CheckStatus(returnedXml).ToString());
            return null;
        }

        /// <summary>
        /// Function that requests top albums for an artist
        /// </summary>
        /// <param name="artistName">Name of the artist to find top albums for</param>
        /// <returns>List of top albums</returns>
        public static async Task<List<albumInfo>> getTopAlbums(string artistName, int page = 0, int limit = 0)
        {
            RequestParameters rParams = new RequestParameters();
            rParams.Add("artist", artistName);
            rParams.Add("method", "artist.getTopAlbums");
            rParams.Add("page", page.ToString());
            rParams.Add("limit", limit.ToString());
            XDocument returnedXml = await Request.MakeRequest(rParams);
            if (Request.CheckStatus(returnedXml) == 0)
            {
                List<albumInfo> albums = new List<albumInfo>((from item in returnedXml.Descendants("topalbums").Elements() select new albumInfo(item)));
                return albums;
            }
            else
                MessageBox.Show("Sorry, there was some error while executing your request. " + Request.CheckStatus(returnedXml).ToString());
            return null;
        }

        public static async Task<List<artistInfo>> search(string artistName, int page = 0, int limit = 30)
        {
            RequestParameters rParams = new RequestParameters();
            rParams.Add("artist", HttpUtility.UrlEncode(artistName));
            rParams.Add("method", "artist.search");
            rParams.Add("page", page.ToString());
            rParams.Add("limit", limit.ToString());
            XDocument returnedXml = await Request.MakeRequest(rParams);
            if (Request.CheckStatus(returnedXml) == 0)
            {
                List<artistInfo> artists = new List<artistInfo>((from item in returnedXml.Descendants("artistmatches").Elements() select new artistInfo(item)));
                return artists;
            }
            else
                MessageBox.Show("Sorry, there was some error while executing your request. " + Request.CheckStatus(returnedXml).ToString());
            return null;
        }
    }
}
