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
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace lastfm
{
    public static class Scrobbling
    {
        public static void Scrobbles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Scrobble();
            Session.Scrobbles.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Scrobbling.Scrobbles_CollectionChanged);
        }
        public static void Scrobble()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                for (int k = 0; k < Session.Scrobbles.Count; k += 50)
                {
                    List<trackInfo> lst = new List<trackInfo>();
                    for (int i = 0; i < 50 && i + k < Session.Scrobbles.Count; ++i)
                    {
                        lst.Add(Session.Scrobbles[i + k]);
                    }
                    try { track.scrobble(lst); }
                    catch (TaskCanceledException) { }
                }
                Session.Scrobbles = new ObservableCollection<trackInfo>();
            }
        }
    }
}
