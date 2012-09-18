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
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace lastfm.API
{
    public class chart
    {
        public static async Task<List<artistInfo>> getTopArtists(int page = 1, int limit = 50)
        {
            RequestParameters rParams = new RequestParameters("chart.getTopArtists");
            rParams.Add("page", page.ToString());
            rParams.Add("limit", limit.ToString());
            XDocument returnedXml = await Request.MakeRequest(rParams);
            if (Request.CheckStatus(returnedXml) == 0)
            {
                List<artistInfo> artists = new List<artistInfo>(from item in returnedXml.Descendants("artists").Elements()
                                                                select new artistInfo(item));
                int totalResults = Int32.Parse(returnedXml.Element("lfm").Element("artists").Attribute("total").Value);
                int currentPage = Int32.Parse(returnedXml.Element("lfm").Element("artists").Attribute("page").Value);
                int itemsPerPage = Int32.Parse(returnedXml.Element("lfm").Element("artists").Attribute("perPage").Value);
                // chart.getTopArtists if passed page number is more than the overall number of pages does load the last page
                if (currentPage != page)
                    throw new IndexOutOfRangeException("No more pages to show");
                return artists;
            }
            else
                throw new LastFmAPIException(returnedXml);
        }
    }
}
