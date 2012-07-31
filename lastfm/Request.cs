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
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Collections.Generic;
using JeffWilcox.Utilities.Silverlight;

namespace lastfm
{
    public class Request : secret_data
    {
        private const string root_url = "http://ws.audioscrobbler.com/2.0/";

        /// <summary>
        /// Returns response status, contained in the lfm tag attribute
        /// </summary>
        /// <param name="xml">XDocument object to be checked</param>
        /// <returns>Status of the response</returns>
        public static int CheckStatus(XDocument xml)
        {
            try 
            {
                string status = xml.Element("lfm").Attribute("status").Value; 
                if (status.ToLower() == "ok")
                    return 0;
                else
                {
                    string code = xml.Element("lfm").Element("error").Attribute("code").Value;
                    return Int32.Parse(code);
                }
            }
            catch (NullReferenceException ex) { throw new ArgumentException("Wrong XML tree. Can't parse.", ex); }
        }

        /// <summary>
        /// Gets the error message contained in each screwed xml returned by the last.fm server
        /// </summary>
        /// <param name="xml">xml to find error message in</param>
        /// <returns>Error message text</returns>
        public static string GetErrorMessage(XDocument xml)
        {
            if (Request.CheckStatus(xml) != 0)
                return xml.Element("lfm").Element("error").Value;
            else
                throw new ArgumentException("XML has no errors");
        }

        /// <summary>
        /// Compiles all the arguments in one request, sends it to the last.fm and returns response xml
        /// </summary>
        /// <param name="rParams">List of parameters</param>
        /// <param name="toSign">Indicates, whether request should be signed or not</param>
        /// <returns>XDocument, containing the whole response from the server</returns>
        public async static Task<XDocument> MakeRequest(RequestParameters rParams, bool toSign = false)
        {
            try { rParams.Add("api_key", secret_data.api_key); }
            catch (ArgumentException) { }
            if (toSign == true)
            {
                StringBuilder sb = new StringBuilder();
                //All arguments must be sorted in signature
                //Because silverlight doesn't support SortedDictionary, I used Linq instead
                var sortedList = from q in rParams orderby q.Key ascending select q.Key;

                foreach (string key in sortedList)
                    sb.Append(key.ToString() + rParams[key]);

                sb.Append(secret_data.secret);
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
            return XDocument.Load(new XmlSanitizingStream(toReadFrom)); 
        }
    }
}
