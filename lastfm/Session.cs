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
        private string _sk;
        private string _UserName;

        public string UserName
        {
            get { return _UserName; }
        }

        public string SessionKey
        {
            get { return _sk; }
        }
        public Session(string skey, string userName)
        {
            _sk = skey;
            _UserName = userName;
        }
    }
}
