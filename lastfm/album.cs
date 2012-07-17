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
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace lastfm
{
    public class album
    {
        /// <summary>
        /// album.search last.fm API function
        /// </summary>
        /// <param name="albumName">Album name to search for</param>
        /// <param name="page">Number of page</param>
        /// <param name="limit">Maximum number of entries per one page</param>
        /// <returns>List of found albums, sorted by relevance</returns>
        public static async Task<List<albumInfo>> search(string albumName, int page = 0, int limit = 30)
        {
            RequestParameters rParams = new RequestParameters();
            rParams.Add("album", HttpUtility.UrlEncode(albumName));
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
                throw new LastFmAPIException(Request.GetErrorMessage(returnedXml), Request.CheckStatus(returnedXml));
        }

        /// <summary>
        /// album.getInfo last.fm API function
        /// </summary>
        /// <param name="artistName">Artist name</param>
        /// <param name="albumName">Album name</param>
        /// <param name="username">Username (if used, user's playcount included)</param>
        /// <param name="lang">Language to return response in</param>
        /// <returns>Album information</returns>
        public static async Task<albumInfo> getInfo(string artistName, string albumName, string username = "", string lang = "en")
        {
            RequestParameters rParams = new RequestParameters();
            rParams.Add("artist", HttpUtility.UrlEncode(artistName));
            rParams.Add("album", HttpUtility.UrlEncode(albumName));
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
                throw new LastFmAPIException(Request.GetErrorMessage(returnedXml), Request.CheckStatus(returnedXml));
        }
    }
}
