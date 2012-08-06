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
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;

namespace lastfm
{
    public class trackInfo
    {
        public string name { get; set; }
        public artistInfo artist { get; set; }
        public albumInfo album { get; set; }
        public List<tagInfo> tags { get; set; }
        public string description { get; set; }
        public string id { get; set; }
        public int duration { get; set; } //in milliseconds
        public int position { get; set; } //track number in album
        public int playcount { get; set; }
        public bool userloved { get; set; }
        public Uri url { get; set; }
        public Uri smallImage { get; set; }
        public Uri mediumImage { get; set; }
        public Uri largeImage { get; set; }
        public Uri extralargeImage { get; set; }
        public DateTime date { get; set; }

        public trackInfo() { }
        public trackInfo(XElement element)
        {
            this.name = element.Element("name").Value.ToString();
            string url_str = element.Element("url").Value.ToString();
            if (url_str.StartsWith("www.")) url_str = "http://" + url_str;
            this.url = new Uri(url_str);
            if (element.Element("duration") != null && !string.IsNullOrEmpty(element.Element("duration").Value))
                this.duration = Int32.Parse(element.Element("duration").Value.ToString());
            if (element.Attribute("rank") != null)
                this.position = Int32.Parse(element.Attribute("rank").Value);
            if (element.Element("album") != null)
            {
                this.album = new albumInfo(element.Element("album"));
                if (element.Element("album").Attribute("position") != null && !String.IsNullOrEmpty(element.Element("album").Attribute("position").Value))
                    this.position = Int32.Parse(element.Element("album").Attribute("position").Value);
            }
            if (element.Element("artist") != null)
                this.artist = new artistInfo(element.Element("artist"));
            if (element.Element("toptags") != null)
                this.tags = new List<tagInfo>(from el in element.Element("toptags").Elements() select new tagInfo(el));
            if (element.Element("playcount") != null)
                playcount = Int32.Parse(element.Element("playcount").Value);
            if (element.Element("userloved") != null)
            {
                if (element.Element("userloved").Value == "1")
                    userloved = true;
                else
                    userloved = false;
            }
            if (element.Element("wiki") != null && element.Element("wiki").Element("content") != null)
            {
                XCData cdata = element.Element("wiki").Element("content").DescendantNodes().OfType<XCData>().First();
                this.description = cdata.Value;
            }
            if (element.Element("date") != null)
            {
                TimeSpan sinceNull = new DateTime(1970, 1, 1) + new TimeSpan(0, 0, Int32.Parse(element.Element("date").Attribute("uts").Value)) - new DateTime(0);
                this.date = (new DateTime(sinceNull.Ticks, DateTimeKind.Utc)).ToLocalTime();
            }
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

        public trackInfo(Song song)
        {
            this.name = song.Name;
            this.artist = new artistInfo(song.Artist);
            this.album = new albumInfo(song.Album);
        }
        public trackInfo(Song song, DateTime timeStamp)
            : this(song)
        {
            this.date = timeStamp;
        }
    }
}
