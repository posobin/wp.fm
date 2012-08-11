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
        /// <summary>
        /// artist.getInfo last.fm api function
        /// </summary>
        /// <param name="artistName">Artist name</param>
        /// <returns>Artist info object, containing artist desription</returns>
        public static async Task<artistInfo> getInfo(string artistName)
        {
            RequestParameters rParams = new RequestParameters("artist.getinfo");
            rParams.Add("artist", artistName);
            XDocument returnedXml = await Request.MakeRequest(rParams);
            if (Request.CheckStatus(returnedXml) == 0)
            {
                artistInfo artist = new artistInfo(returnedXml.Element("lfm").Element("artist"));
                return artist;
            }
            else
                throw new LastFmAPIException(returnedXml);
        }

        /// <summary>
        /// artist.getTopAlbums last.fm api function
        /// </summary>
        /// <param name="artistName">Name of the artist to find top albums for</param>
        /// <returns>List of top albums</returns>
        public static async Task<List<albumInfo>> getTopAlbums(string artistName, int page = 0, int limit = 0)
        {
            RequestParameters rParams = new RequestParameters("artist.getTopAlbums");
            rParams.Add("artist", artistName);
            rParams.Add("page", page.ToString());
            rParams.Add("limit", limit.ToString());
            XDocument returnedXml = await Request.MakeRequest(rParams);
            if (Request.CheckStatus(returnedXml) == 0)
            {
                List<albumInfo> albums = new List<albumInfo>((from item in returnedXml.Descendants("topalbums").Elements() select new albumInfo(item)));
                return albums;
            }
            else
                throw new LastFmAPIException(returnedXml);
        }

        /// <summary>
        /// astist.search last.fm api function
        /// </summary>
        /// <param name="artistName">Artist name</param>
        /// <param name="page">Number of page to download</param>
        /// <param name="limit">Number of entries per one page</param>
        /// <returns>List of artistInfos with returned artists data</returns>
        public static async Task<List<artistInfo>> search(string artistName, int page = 0, int limit = 30)
        {
            RequestParameters rParams = new RequestParameters("artist.search");
            rParams.Add("artist", artistName);
            rParams.Add("page", page.ToString());
            rParams.Add("limit", limit.ToString());
            XDocument returnedXml = await Request.MakeRequest(rParams);
            if (Request.CheckStatus(returnedXml) == 0)
            {
                List<artistInfo> artists = new List<artistInfo>((from item in returnedXml.Descendants("artistmatches").Elements() select new artistInfo(item)));
                XNamespace opensearch = @"http://a9.com/-/spec/opensearch/1.1/";
                IEnumerable<XElement> opensearch_ = from el in returnedXml.Element("lfm").Element("results").Elements()
                                                   where el.Name.Namespace == opensearch
                                                   select el;
                int totalResults = Int32.Parse((from el in opensearch_ where el.Name.LocalName == "totalResults" select el.Value).First());
                int startIndex = Int32.Parse((from el in opensearch_ where el.Name.LocalName == "startIndex" select el.Value).First());
                int itemsPerPage = Int32.Parse((from el in opensearch_ where el.Name.LocalName == "itemsPerPage" select el.Value).First());
                if (totalResults - startIndex < 0)
                    throw new IndexOutOfRangeException("Page being shown is the first page");
                return artists;
            }
            else
                throw new LastFmAPIException(returnedXml);
        }
    }
}
