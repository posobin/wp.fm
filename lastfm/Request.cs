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
using System.IO;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Collections.Generic;
using JeffWilcox.Utilities.Silverlight;

namespace lastfm
{
    public class Request
    {
        private const string api_key = "85fc9119f3928f1a2c2c547b0247eb6d";
        private const string secret = "b0158e565140df0529a9a1f4ce15d9b5";
        private const string root_url = "http://ws.audioscrobbler.com/2.0/";
        public delegate void MethodToCallAfter(KeyValuePair<int, string> response);

        public async static Task<XDocument> MakeRequest(RequestParameters rParams, bool toSign = false)
        {
            rParams.Add("api_key", api_key);
            if (toSign == true)
            {
                StringBuilder sb = new StringBuilder();
                //All arguments must be sorted in signature
                //Because silverlight doesn't support SortedDictionary, I used Linq instead
                var sortedList = from q in rParams orderby q.Key ascending select q.Key;

                foreach (string key in sortedList)
                    sb.Append(key.ToString() + rParams[key]);

                sb.Append(secret);
                rParams.Add("api_sig", MD5CryptoServiceProvider.GetMd5String(sb.ToString()));
            }
            string request_string = rParams.ToString();
            WebClient client = new WebClient();
            client.Headers["User-Agent"] = "last.fm scrobbler for WP";
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            client.Headers["Accept-Charset"] = "utf-8";
            client.Encoding = Encoding.UTF8;
            byte[] byteArray = rParams.ToBytes();
            Stream toReadFrom;
            try
            {
                string response = "response string not set";
                response = await client.UploadStringTaskAsync(new Uri(root_url), "POST", rParams.ToString());
                toReadFrom = new MemoryStream(Encoding.UTF8.GetBytes(response));
            }
            catch (WebException ex)
            {
                toReadFrom = ((HttpWebResponse)ex.Response).GetResponseStream();
            }
            return XDocument.Load(toReadFrom);
        }
    }
}
