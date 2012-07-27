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
    public class UserInfo
    {
        public int id { get; set; }
        public int PlayCount { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
        public string RealName { get; set; }
        public string Type { get; set; }
        public char Gender { get; set; }
        public Uri url { get; set; }
        public Uri SmallImage { get; set; }
        public Uri MediumImage { get; set; }
        public Uri LargeImage { get; set; }
        public Uri ExtraLargeImage { get; set; }
        public DateTime Registered { get; set; }

        public UserInfo(XElement element)
        {
            this.Name = element.Element("name").Value;
            this.id = Int32.Parse(element.Element("id").Value);
            this.url = new Uri(element.Element("url").Value);

            if (element.Element("realname") != null && element.Element("realname").Value != null)
                this.RealName = element.Element("realname").Value;
            if (element.Element("age") != null && !String.IsNullOrEmpty(element.Element("age").Value))
                this.Age = Int32.Parse(element.Element("age").Value);
            if (element.Element("playcount") != null)
                this.PlayCount = Int32.Parse(element.Element("playcount").Value);
            try
            { this.SmallImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "small" select el.Value.ToString()).First()); }
            catch (UriFormatException) { this.SmallImage = null; }
            try
            { this.MediumImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "medium" select el.Value.ToString()).First()); }
            catch (UriFormatException) { this.MediumImage = null; }
            try
            { this.LargeImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "large" select el.Value.ToString()).First()); }
            catch (UriFormatException) { this.LargeImage = null; }
            try
            { this.ExtraLargeImage = new Uri((from el in element.Elements("image") where el.Attribute("size").Value.ToString() == "extralarge" select el.Value.ToString()).First()); }
            catch (UriFormatException) { this.ExtraLargeImage = null; }
        }
    }
}
