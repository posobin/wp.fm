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
using System.Threading;
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

        public static void MakeRequest(RequestParameters rParams, bool toSign = false, MethodToCallAfter method = null)
        {
            rParams.Add("api_key", api_key);
            if (toSign == true)
            {
                StringBuilder sb = new StringBuilder();
                //All arguments must be sorted in signature
                //Because silverlight doesn't support SortedDictionary, I used Linq instead
                var sortedList = from q in rParams orderby q.Key ascending select q.Key;
                foreach (string key in sortedList)
                {
                    sb.Append(key.ToString()+rParams[key]);
                }
                sb.Append(secret);
                MessageBox.Show(sb.ToString());
                rParams.Add("api_sig", MD5CryptoServiceProvider.GetMd5String(sb.ToString()));
            }
            string request_string = rParams.ToString();
            HttpWebRequest postRequest = (HttpWebRequest)WebRequest.Create(new Uri(root_url));
            postRequest.Method = "POST";
            postRequest.UserAgent = "last.fm scrobbler for WP";
            postRequest.ContentType = "application/x-www-form-urlencoded";
            postRequest.Headers["Accept-Charset"] = "utf-8";
            IAsyncResult aResult = postRequest.BeginGetRequestStream(new AsyncCallback((x) => { }), null);
            Stream postStream = postRequest.EndGetRequestStream(aResult);
            byte[] byteArray = rParams.ToBytes();
            MessageBox.Show(rParams.ToString());
            postRequest.Headers["Content-Length"] = byteArray.Length.ToString();
            postStream.Write(byteArray, 0, byteArray.Length);
            postStream.Close();
            string responseString = "";
            IAsyncResult postResult = postRequest.BeginGetResponse(new AsyncCallback((x) =>
            {
                HttpWebRequest request = (HttpWebRequest)x.AsyncState;
                try
                {
                    HttpWebResponse resp = (HttpWebResponse)postRequest.EndGetResponse(x);
                    using (StreamReader sr1 = new StreamReader(resp.GetResponseStream()))
                    {
                        responseString = sr1.ReadToEnd();
                    }
                }
                catch (WebException ex)
                {
                    using (StreamReader sr1 = new StreamReader(((HttpWebResponse)ex.Response).GetResponseStream()))
                    {
                        responseString = sr1.ReadToEnd();
                    }
                }
                finally
                {
                    method(new KeyValuePair<int, string>(0, responseString));
                }
            }), null);
        }
    }
}
