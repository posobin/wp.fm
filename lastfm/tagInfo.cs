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
using System.Linq;
using System.Text;

namespace lastfm
{
    public class tagInfo
    {
        public string name { get; set; }
        public Uri url { get; set; }
        public string wiki { get; set; }

        public tagInfo() { }
        public tagInfo(XElement element)
        {
            this.name = element.Element("name").Value.ToString();
            string url_str = element.Element("url").Value.ToString();
            if (url_str.StartsWith("www.")) url_str = @"http://" + url_str;
            this.url = new Uri(url_str);
            if (element.Element("wiki") != null && element.Element("wiki").Element("content") != null)
            {
                XCData cdata = element.Element("wiki").Element("content").DescendantNodes().OfType<XCData>().First();
                this.wiki = cdata.Value;
            }
        }
    }
}
