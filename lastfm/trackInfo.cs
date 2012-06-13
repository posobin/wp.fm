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

namespace lastfm
{
    public class trackInfo
    {
        public string name { get; set; }
        public string artistName { get; set; }
        public string id { get; set; }
        public int duration { get; set; } //in milliseconds
        public int position { get; set; } //track number in album
        public Uri url { get; set; }
        public Uri smallImage { get; set; }
        public Uri mediumImage { get; set; }
        public Uri largeImage { get; set; }
        public Uri extralargeImage { get; set; }

        public trackInfo() { }
        public trackInfo(XElement element)
        {
            this.name = element.Element("name").Value.ToString();
            string url_str = element.Element("url").Value.ToString();
            if (url_str.StartsWith("www.")) url_str = @"http://" + url_str;
            if (element.Element("duration") != null)
                this.duration = Int32.Parse(element.Element("duration").Value.ToString());
            else if (element.Attribute("position") != null)
            this.url = new Uri(url_str);
            try
            { this.smallImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "small" select el.Value.ToString()).First()); }
            catch (UriFormatException) { this.smallImage = null; }
            catch (InvalidOperationException) { this.smallImage = null; }
            try
            { this.mediumImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "medium" select el.Value.ToString()).First()); }
            catch (UriFormatException) { this.mediumImage = null; }
            catch (InvalidOperationException) { this.mediumImage = null; }
            try
            { this.largeImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "large" select el.Value.ToString()).First()); }
            catch (UriFormatException) { this.largeImage = null; }
            catch (InvalidOperationException) { this.largeImage = null; }
            try
            { this.extralargeImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "extralarge" select el.Value.ToString()).First()); }
            catch (UriFormatException) { this.extralargeImage = null; }
            catch (InvalidOperationException) { this.extralargeImage = null; }
        }
    }
}
