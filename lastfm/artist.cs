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
        public static Task<artistInfo> getinfo()
        {
            throw new NotImplementedException();
        }

        public static async Task<List<artistInfo>> search(string text, int page = 0, int limit = 30)
        {
            RequestParameters rParams = new RequestParameters();
            rParams.Add("artist", text);
            rParams.Add("method", "artist.search");
            XDocument returnedXml = await Request.MakeRequest(rParams);
            if (Request.CheckStatus(returnedXml) == 0)
            {
                //MessageBox.Show(returnedXml.Descendants("artist"));
                List<artistInfo> artists = new List<artistInfo>((from item in returnedXml.Descendants("artistmatches").Elements() select new artistInfo(item)));
                return artists;
            }
            else
                MessageBox.Show("Sorry, there was some error while executing your request." + Request.CheckStatus(returnedXml).ToString());
            return null;
        }
    }
}
