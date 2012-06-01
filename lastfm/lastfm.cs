using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using JeffWilcox.Utilities.Silverlight;

namespace lastfm
{
    public class lastfm
    {
        private const string api_key = "85fc9119f3928f1a2c2c547b0247eb6d";
        private const string secret = "b0158e565140df0529a9a1f4ce15d9b5";
        private const string root_url = "http://ws.audioscrobbler.com/2.0/";

        //using info from http://blogs.msdn.com/b/silverlight_sdk/archive/2008/04/01/using-webclient-and-httpwebrequest.aspx

        private static void GetRequestStreamCallback(IAsyncResult asynchronusResult, string request_string)
        {
            HttpWebRequest authRequest = (HttpWebRequest)asynchronusResult.AsyncState;
            Stream postStream = authRequest.EndGetRequestStream(asynchronusResult);
            byte[] byte1 = Encoding.UTF8.GetBytes(System.Uri.EscapeUriString(request_string));
            authRequest.Headers["Content-Length"] = byte1.Length.ToString();
            postStream.Write(byte1, 0, byte1.Length);
            postStream.Close();
            authRequest.BeginGetResponse(new AsyncCallback(ReadCallback), authRequest);
        }

        private static void ReadCallback(IAsyncResult asynchronusResult)
        {
            HttpWebRequest request = (HttpWebRequest)asynchronusResult.AsyncState;
            string response;
            try
            {
                HttpWebResponse resp = (HttpWebResponse)request.EndGetResponse(asynchronusResult);
                using (StreamReader streamReader1 = new StreamReader(resp.GetResponseStream()))
                {
                    response = streamReader1.ReadToEnd();
                }
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(response);
                });
            }
            catch (WebException ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    using (StreamReader streamReader1 = new StreamReader((((HttpWebResponse)ex.Response).GetResponseStream())))
                    {
                        MessageBox.Show(streamReader1.ReadToEnd());
                    }
                });
            }
        }
    }
}
