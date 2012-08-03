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
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace lastfm
{
    [DataContract]
    public class Session
    {
        /// <summary>
        /// Last session
        /// </summary>
        public static Session CurrentSession
        { 
            get
            {
                if (System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Contains("Session"))
                    return (Session)System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["Session"];
                else
                    return null;
            }
            set
            {
                System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["Session"] = value;
                System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        /// <summary>
        /// Whether to remember last session or not
        /// </summary>
        public static bool RememberSession 
        {
            get 
            {
                if (!System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Contains("RememberSession"))
                    System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["RememberSession"] = true;
                return (bool)System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["RememberSession"];
            }
            set 
            {
                System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["RememberSession"] = value;
                System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        /// <summary>
        /// Whether to scrobble new song after the song change or not
        /// </summary>
        public static bool AutoScrobbling 
        { 
            get
            {
                if (!System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Contains("AutoScrobbling"))
                    System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["AutoScrobbling"] = true;
                return (bool)System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["AutoScrobbling"];
            }
            set
            {
                System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["AutoScrobbling"] = value;
                System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        /// <summary>
        /// List of all scrobbles that were not completed.
        /// </summary>
        public static ObservableCollection<trackInfo> Scrobbles
        {
            get
            {
                if (!System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Contains("Scrobbles"))
                    System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["Scrobbles"] = new ObservableCollection<trackInfo>();
                return (ObservableCollection<trackInfo>)System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["Scrobbles"];
            }
            set
            {
                System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["Scrobbles"] = value;
                System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        /// <summary>
        /// Last song played
        /// </summary>
        public static Song LastSong 
        {
            get
            {
                if (!System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Contains("LastSong"))
                    System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["LastSong"] = null;
                return (Song)System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["LastSong"];
            }
            set
            {
                System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["LastSong"] = value;
                System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }
        
        /// <summary>
        /// Username used for authorization
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// Session key returned after authorization
        /// </summary>
        [DataMember]
        public string SessionKey { get; set; }

        public Session(string skey, string userName)
        {
            SessionKey = skey;
            UserName = userName;
        }
    }
}
