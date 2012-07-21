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
using System.Xml.Linq;

namespace lastfm
{
    public class userInfo
    {
        public int id { get; set; }
        public int PlayCount { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
        public string RealName { get; set; }
        public string Type { get; set; }
        public char Gender { get; set; }
        public Uri url { get; set; }
        public Uri Image { get; set; }
        public DateTime Registered { get; set; }

        public userInfo() { }
        public userInfo(XElement element)
        {
            this.Name = element.Element("name").Value;
            this.id = Int32.Parse(element.Element("id").Value);
            this.url = new Uri(element.Element("url").Value);

            if (element.Element("realname") != null)
                this.RealName = element.Element("realname").Value;
            if (element.Element("age") != null)
                this.Age = Int32.Parse(element.Element("age").Value);
            if (element.Element("playcount") != null)
                this.PlayCount = Int32.Parse(element.Element("playcount").Value);
        }
    }
}
