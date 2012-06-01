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

namespace lastfm
{
    public class Session
    {
        //session key returned after authorization
        private string sk;

        public string SessionKey
        {
            get { return sk; }
            set { sk = value; }
        }
        public Session(string skey) { SessionKey = skey; }
        public Session() { SessionKey = ""; }
    }
}
