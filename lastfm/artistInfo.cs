using System;
using System.Net;
using System.Linq;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Text;

namespace lastfm
{
    public class artistInfo
    {
        public string name { get; set; }
        public string mbid { get; set; }
        public Uri url { get; set; }
        public string bio { get; set; }
        public List<artistInfo> similar { get; set; }
        public List<tagInfo> tags { get; set; }
        public Uri smallImage { get; set; }
        public Uri mediumImage { get; set; }
        public Uri largeImage { get; set; }
        public Uri extralargeImage { get; set; }
        public Uri megaImage { get; set; }
        public statistics stats { get; set; }

        public artistInfo() { }
        public artistInfo(XElement element)
        {
            if (element.Element("name") != null)
                this.name = element.Element("name").Value.ToString();
            else
            {
                this.name = element.Value;
                return;
            }

            string url_str = element.Element("url").Value.ToString();
            if (url_str.StartsWith("www.")) url_str = @"http://" + url_str;
            this.url = new Uri(url_str);

            if (element.Element("bio") != null && element.Element("bio").Element("content") != null && element.Element("bio").Element("content").DescendantNodes().OfType<XCData>().Count() != 0)
            {
                XCData cdata = element.Element("bio").Element("content").DescendantNodes().OfType<XCData>().First();
                this.bio = cdata.Value;
            }
            if (element.Element("mbid") != null)
                this.mbid = element.Element("mbid").Value.ToString();
            if (element.Element("image") != null)
            {
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
                try
                { this.megaImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "mega" select el.Value.ToString()).First()); }
                catch (UriFormatException) { this.megaImage = null; }
                catch (InvalidOperationException) { this.megaImage = null; }
            }
            if (element.Element("tags") != null)
                this.tags = new List<tagInfo>((from item in element.Element("tags").Elements() select new tagInfo(item)));
            if (element.Element("similar") != null)
                this.similar = new List<artistInfo>((from item in element.Element("similar").Elements() select new artistInfo(item)));
        }
    }
}
