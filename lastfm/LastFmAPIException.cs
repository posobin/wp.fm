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

    /// <summary>
    /// Exception that is being thrown when Last.fm server returns non-ok response
    /// </summary>
    public class LastFmAPIException : Exception
    {
        public int ErrorNumber { get; set; }
        public LastFmAPIException() : base() { }
        public LastFmAPIException(XDocument xml) : base(Request.GetErrorMessage(xml)) { ErrorNumber = Request.CheckStatus(xml); }
        public LastFmAPIException(string message) : base(message) { }
        public LastFmAPIException(string message, Exception inner) : base(message, inner) { }
        public LastFmAPIException(string message, int errorNumber) : base(message) { ErrorNumber = errorNumber; }
        public LastFmAPIException(string message, int errorNumber, Exception inner) : base(message, inner) { ErrorNumber = errorNumber; }

        public override string ToString()
        {
            return base.ToString() + " Server error number: " + this.ErrorNumber.ToString();
        }

        public string ToShortString()
        {
            return this.Message + " Server error number: " + this.ErrorNumber.ToString();
        }
    }
}
