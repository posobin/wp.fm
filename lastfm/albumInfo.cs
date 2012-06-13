using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Xml.Linq;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace lastfm
{
    public class albumInfo
    {
        public string name { get; set; }
        public string artistName { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public List<trackInfo> tracks { get; set; }
        public List<tagInfo> tags { get; set; }
        public Uri url { get; set; }
        public Uri smallImage { get; set; }
        public Uri mediumImage { get; set; }
        public Uri largeImage { get; set; }
        public Uri extralargeImage { get; set; }

        public albumInfo() { }
        public albumInfo(XElement element)
        {
            this.name = element.Element("name").Value.ToString();
            this.artistName = element.Element("artist").Value.ToString();
            string url_str = element.Element("url").Value.ToString();
            if (url_str.StartsWith("www.")) url_str = @"http://" + url_str;
            this.url = new Uri(url_str);
            if (element.Element("wiki") != null && element.Element("wiki").Element("content") != null)
            {
                XCData cdata = element.Element("wiki").Element("content").DescendantNodes().OfType<XCData>().First();
                this.description = cdata.Value;
            }
            if (element.Element("tracks") != null)
                this.tracks = new List<trackInfo>(from el in element.Element("tracks").Elements() select new trackInfo(el));
            if (element.Element("toptags") != null)
                this.tags = new List<tagInfo>(from el in element.Element("toptags").Elements() select new tagInfo(el));
            try
            { this.smallImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "small" select el.Value.ToString()).First()); }
            catch (UriFormatException) { this.smallImage = null; }
            try
            { this.mediumImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "medium" select el.Value.ToString()).First()); }
            catch (UriFormatException) { this.mediumImage = null; }
            try
            { this.largeImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "large" select el.Value.ToString()).First()); }
            catch (UriFormatException) { this.largeImage = null; }
            try
            { this.extralargeImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "extralarge" select el.Value.ToString()).First()); }
            catch (UriFormatException) { this.extralargeImage = null; }
        }
    }
}
