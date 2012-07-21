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

namespace lastfm
{
    public class user
    {
        public static async Task<userInfo> getInfo(string user)
        {
            RequestParameters rParams = new RequestParameters();
            rParams.Add("method", "user.getInfo");
            rParams.Add("user", user);
            XDocument ReturnedXML = await Request.MakeRequest(rParams);
            if (Request.CheckStatus(ReturnedXML) == 0)
                return new userInfo(ReturnedXML.Element("user"));
            else
                throw new LastFmAPIException(Request.GetErrorMessage(ReturnedXML), Request.CheckStatus(ReturnedXML));
        }
    }
}
