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

namespace lastfm
{
    public class artistInfo
    {
        public string name { get; set; }
        public string mbid { get; set; }
        public Uri url { get; set; }
        public Uri smallImage { get; set; }
        public Uri mediumImage { get; set; }
        public Uri largeImage { get; set; }
        public Uri extralargeImage { get; set; }
        public Uri megaImage { get; set; }
        public statistics stats { get; set; }

        public artistInfo() { }
        public artistInfo(XElement element)
        {
            this.name = element.Element("name").Value.ToString();
            this.mbid = element.Element("mbid").Value.ToString();
            this.url = new Uri(element.Element("url").Value.ToString());
            try
            { this.smallImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "small" select el.Value.ToString()).First()); }
            catch (NullReferenceException) { this.smallImage = null; }
            try
            { this.smallImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "medium" select el.Value.ToString()).First()); }
            catch (NullReferenceException) { this.mediumImage = null; }
            try
            { this.smallImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "large" select el.Value.ToString()).First()); }
            catch (NullReferenceException) { this.largeImage = null; }
            try
            { this.smallImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "extralarge" select el.Value.ToString()).First()); }
            catch (NullReferenceException) { this.extralargeImage = null; }
            try
            { this.smallImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "mega" select el.Value.ToString()).First()); }
            catch (NullReferenceException) { this.megaImage = null; }
            /*
            //this.mediumImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "medium" select el.Value.ToString()).First());
            //this.largeImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "large" select el.Value.ToString()).First());
            //this.extralargeImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "extralarge" select el.Value.ToString()).First());
            //this.megaImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "mega" select el.Value.ToString()).First());
            */
        }
    }
}
