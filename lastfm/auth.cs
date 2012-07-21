using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using System.Linq;
using System.Threading.Tasks;
using JeffWilcox.Utilities.Silverlight;

namespace lastfm
{
    public class auth
    {
        public static async Task<Session> authorize(string username, string password)
        {
            string password_hash = MD5CryptoServiceProvider.GetMd5String(password);
            string authToken = MD5CryptoServiceProvider.GetMd5String(username.ToLower() + password_hash);
            RequestParameters rParams = new RequestParameters();
            rParams.Add("username", username);
            rParams.Add("authToken", authToken);
            rParams.Add("method", "auth.getMobileSession");
            XDocument ReturnedXML = await Request.MakeRequest(rParams, true);

            if (Request.CheckStatus(ReturnedXML) == 0)
            {
                string skey = (from el in ReturnedXML.Descendants("key") select el).First().Value;
                string user = (from el in ReturnedXML.Descendants("name") select el).First().Value;
                return new Session(skey, user);
            }
            else
            {
                MessageBox.Show("Sorry, something went wrong. Probably, you specified wrong password or username. Please, try again");
                return null;
            }
        }
    }
}
