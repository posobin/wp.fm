using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;

namespace lastfm
{
    public class RequestParameters : Dictionary<string, string>
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string key in this.Keys)
                sb.Append(key+'='+this[key]+'&');
            string ret = sb.ToString().Substring(0, sb.Length-1);
            return ret;
        }

        public byte[] ToBytes()
        {
            return Encoding.UTF8.GetBytes(ToString());
        }
    }
}
