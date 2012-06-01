using System;
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
using System.Threading.Tasks;
using JeffWilcox.Utilities.Silverlight;

namespace lastfm
{
    public class auth
    {
        public static void authorize(string username, string password, Session SessionName,Request.MethodToCallAfter method = null)
        {
            string password_hash = MD5CryptoServiceProvider.GetMd5String(password);
            string authToken = MD5CryptoServiceProvider.GetMd5String(username.ToLower() + password_hash);
            RequestParameters rParams = new RequestParameters();
            rParams.Add("username", username);
            rParams.Add("authToken", authToken);
            rParams.Add("method", "auth.getMobileSession");
            AutoResetEvent wh = new AutoResetEvent(false);
            string resp = "";
            Request.MakeRequest(rParams, true, x => 
            {
                resp = x.Value;
                SessionName = new Session(resp);
                method(x);
            });
            //wh.WaitOne();
        }
    }
}
