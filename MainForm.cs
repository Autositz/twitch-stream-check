/*
 * Created by SharpDevelop.
 * User: Autositz
 * Date: 09.03.2015
 * Time: 14:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

//#define _HACK

using System;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Data;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace twitch_stream_check
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Hold currently loaded settings
        /// </summary>
        public MySettings settings;
        /// <summary>
        /// Hold the settings form
        /// </summary>
        public SettingsForm objSettingsForm;
        /// <summary>
        /// Is true if there is currently a Streamlist check running
        /// </summary>
        private bool bGettingData;
        /// <summary>
        /// The running number of the current check
        /// </summary>
        private int _iCurrentCheck;
        /// <summary>
        /// Current number of streamlist entries to check
        /// </summary>
        private int iMaxCheck;
        /// <summary>
        /// Currently active streams to display in icon tooltip
        /// </summary>
        private int _iActiveStreams;
        /// <summary>
        /// Time in seconds until the next check will start
        /// </summary>
        private int _iNextCheck;
        /// <summary>
        /// Background timer to check streams
        /// </summary>
        private System.Timers.Timer tStreamsTimer;
        private DateTime tStreamsTimerLastStart;
        /// <summary>
        /// Background timer to check streams
        /// </summary>
        private System.Timers.Timer tToolTipTimer;
        /// <summary>
        /// Check own account
        /// </summary>
        private System.Timers.Timer tAccountTimer;
        /// <summary>
        /// True if own account is currently online
        /// </summary>
        private bool bAccountOnline;
        /// <summary>
        /// Holds data to add at end of tooltip if account is online
        /// </summary>
        private string sAccountOnlineData;
        /// <summary>
        /// Use logging for exceptions
        /// </summary>
        private Logging Log;
        
        public MainForm()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            // Create controls so Invoke won't complain later
            this.CreateControl();
            MyMenu.CreateControl();
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
            this.Hide();
            
            // load error logging
            this.Log = new Logging();
            
            tAccountTimer = new System.Timers.Timer();
            tAccountTimer.Interval = 23000;
            tAccountTimer.AutoReset = true;
            tAccountTimer.Elapsed += new System.Timers.ElapsedEventHandler(CheckAccount);
            tAccountTimer.Enabled = true; // enable timer
            
//            Program.objSettingsForm = new SettingsForm(); // really needed at start?
            
//            settings = new MySettings();
            // load stored settings from file and check if default values need to be put in
            settings = MySettings.Load();
            SetDefaultSettings();
//            settings = Program.objSettingsForm.Start(); // really needed at start?
            
            // set the check value if a check is currently running
            bGettingData = false;
            iCurrentCheck = 0;
            iMaxCheck = 0;
            iActiveStreams = 0;
            bAccountOnline = false;
            sAccountOnlineData = "";
            
            
            // create a new timer to run stuff at given interval
            tStreamsTimer = new System.Timers.Timer();
            // FIXME RELEASE: Make sure to set checkinterval timer for release!
            // start timer with 1000ms * 60s * interval-minutes
            tStreamsTimer.Interval = (settings.checkinterval * 60 * 1000);
//            tStreamsTimer.Interval = 15000; // uncomment this for faster cycles on small entries
            // redo associated actions
            tStreamsTimer.AutoReset = true;
            // set the action we want to do at the given interval
            tStreamsTimer.Elapsed += new ElapsedEventHandler(DoTimer);
            // make sure the timer is starting
            tStreamsTimer.Enabled = true; // enable timer
            tStreamsTimerLastStart = DateTime.Now;
            
            
            tToolTipTimer = new System.Timers.Timer();
            tToolTipTimer.Interval = 1000;
            tToolTipTimer.AutoReset = true;
            tToolTipTimer.Elapsed += new ElapsedEventHandler(CheckNextTime);
            tToolTipTimer.Enabled = true;
            
//            tStartDelayTimer = new System.Timers.Timer();
//            tStartDelayTimer.Interval = 5000;
//            tStartDelayTimer.AutoReset = false;
//            tStartDelayTimer.Elapsed += new ElapsedEventHandler(StartDelay);
//            tStartDelayTimer.Enabled = true;
            
            // clear up what we used
//            Program.objSettingsForm.Dispose(); // really needed at start?
        }
        
        /// <summary>
        /// What to do each time our timer calls for us
        /// </summary>
        void DoTimer(object sender, EventArgs e)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "DOTIMER: Timer called us");
            tStreamsTimerLastStart = DateTime.Now;
            Task.Factory.StartNew(CheckStreams);
        }
        
        void CheckNextTime (object sender, EventArgs e)
        {
            UpdateToolTip();
        }
        
        /// <summary>
        /// Start ToolTip updates only if other things have finished initializing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
//        void StartDelay (object sender, EventArgs e)
//        {
//            
//            tStartDelayTimer.Enabled = false;
//            tStartDelayTimer.Dispose();
//        }
        
        /// <summary>
        /// Check own account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CheckAccount(object sender, EventArgs e)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "CHECKACCOUNT: Timer called us");
            const string sBaseURL = "https://api.twitch.tv/kraken/streams/";
            string sParams = "";
            if (settings.checkaccount == "")
                return;
            Debug.WriteLineIf(GlobalVar.DEBUG, "CHECKACCOUNT: checking account: " + settings.checkaccount);
            WebCheckResponse WebCheck = Functions.DoWebRequest(sBaseURL + settings.checkaccount + sParams);
            
            if (WebCheck.bSuccess) {
                sAccountOnlineData = "";
                Debug.WriteLineIf(GlobalVar.DEBUG, "CHECKACCOUNT: Got something to work with");
                dynamic data = Functions.ParseWebResponseToObject(WebCheck.HttpWResp);
                Debug.WriteLineIf(GlobalVar.DEBUG, "CHECKACCOUNT: Parse done");
                
                if (data.stream.viewers != null) {
                    sAccountOnlineData += Environment.NewLine + "Viewers: " + data.stream.viewers;
                    Debug.WriteLineIf(GlobalVar.DEBUG, "CHECKACCOUNT: SET: viewers");
                }
                if (data.stream.average_fps != null) {
                    sAccountOnlineData += Environment.NewLine + "FPS: " + data.stream.average_fps;
                    Debug.WriteLineIf(GlobalVar.DEBUG, "CHECKACCOUNT: SET: average_fps");
                }
                if (data.stream.video_height != null) {
                    sAccountOnlineData += Environment.NewLine + "Resolution: " + data.stream.video_height + "p";
                    Debug.WriteLineIf(GlobalVar.DEBUG, "CHECKACCOUNT: SET: video_height");
                }
                if (data.stream.channel.delay != null) {
                    sAccountOnlineData += Environment.NewLine + "Delay: " + data.stream.channel.delay;
                    Debug.WriteLineIf(GlobalVar.DEBUG, "CHECKACCOUNT: SET: delay");
                }
                if (data.stream.channel.followers != null) {
                    sAccountOnlineData += Environment.NewLine + "Followers: " + data.stream.channel.followers;
                    Debug.WriteLineIf(GlobalVar.DEBUG, "CHECKACCOUNT: SET: followers");
                }
                if (data.stream.channel.views != null) {
                    sAccountOnlineData += Environment.NewLine + "Views: " + data.stream.channel.views;
                    Debug.WriteLineIf(GlobalVar.DEBUG, "CHECKACCOUNT: SET: views");
                }
                bAccountOnline = true;
                Debug.WriteLineIf(GlobalVar.DEBUG, "CHECKACCOUNT: Account online " + sAccountOnlineData);
            } else {
                Debug.WriteLineIf(GlobalVar.DEBUG, "CHECKACCOUNT: No success");
                bAccountOnline = false;
                sAccountOnlineData = "";
            }
        }
        
        /// <summary>
        /// Close the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolStripMenuItemClickExit(object sender, EventArgs e)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "TOOLSTRIPMENUITEMCLICKEXIT: Menu Click - Exit");
                
            Application.Exit();
        }
        
        /// <summary>
        /// About info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolStripMenuItemClickAbout(object sender, EventArgs e)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "TOOLSTRIPMENUITEMCLICKABOUT: Menu Click - About");
            MessageBox.Show("This tool will check if any of the configured streams is currently online.", "Twitch Stream Checker");
        }
        
        /// <summary>
        /// Open the Settings menu and populate it with current settings data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolStripMenuItemClickSettings(object sender, EventArgs e)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "TOOLSTRIPMENUITEMCLICKSETTINGS: Menu Click - Settings");
            
            if (objSettingsForm == null || objSettingsForm.IsDisposed) {
                objSettingsForm = new SettingsForm(this);
                objSettingsForm.Start(this.settings);
            }
            
            // make sure we see a settings window which was previously hidden, hopefully...
            objSettingsForm.Show();
            objSettingsForm.Focus();
        }
        
        public void SetTimer(int iTimer)
        {
            tStreamsTimer.Interval = iTimer * 60 * 1000;
            tStreamsTimerLastStart = DateTime.Now;
            Debug.WriteLineIf(GlobalVar.DEBUG, "SETTIMER: Set new interval to: " + tStreamsTimer.Interval + " ms");
        }
        
        /// <summary>
        /// Create a new menu entry to follow the stream, but check if it already exists
        /// </summary>
        /// <param name="sUser">Streamname</param>
        private bool MenuEntryCreate(string sUser, string sGame)
        {
            // make sure we can access the settingsform
            if (this.InvokeRequired)
            {
                Debug.WriteLineIf(GlobalVar.DEBUG, "MENUENTRYCREATE: NEEDS INVOKE");
                return (bool)this.Invoke ((Func<string,string,bool>)MenuEntryCreate, sUser, sGame);
            }
            
            Debug.WriteLineIf(GlobalVar.DEBUG, "MENUENTRYCREATE: START");
            Debug.WriteLineIf(GlobalVar.DEBUG, "MENUENTRYCREATE: Creating new Menu entry for " + sUser + " playing game " + sGame);
            bool bRet = false;
            int iMenuIDX = MyMenu.Items.IndexOfKey(sUser);
            Debug.WriteLineIf(GlobalVar.DEBUG, "MENUENTRYCREATE: Result of looking up menu entry: " + iMenuIDX);
            
            // -1 means we have no matching menu entry and will add a new one, otherwise do nothing
            if (iMenuIDX == -1) {
                notifyIcon1.ShowBalloonTip(900, "Stream is LIVE", sUser + " playing " + sGame, ToolTipIcon.None);

                // add the new stream on top of the menu and append the other entries back to the menu
                // make sure to add name to be able to search by key!
                ToolStripItem tsiNewItem = new ToolStripMenuItem(sUser, null, (sender, e) => OpenStream(sUser), sUser);
                tsiNewItem.ToolTipText = "Playing: " + Functions.ConvertAlphaNum(sGame);
                Debug.WriteLineIf(GlobalVar.DEBUG, "MENUENTRYCREATE: Created new MenuItem");
                // add the new stream on top of the menu
                try {
                    if (InvokeRequired) {
                        MyMenu.Invoke((MethodInvoker)delegate {
                            lock (MyMenu) {
                                MyMenu.Items.Insert(0, tsiNewItem);
                            }
                        });
                    } else {
                        lock (MyMenu) {
                            MyMenu.Items.Insert(0, tsiNewItem);
                        }
                    }
                    
//                    MyMenu.Items.Insert(0, tsiNewItem);
                    bRet = true;
                    // increase active streams number only when we are really adding a new one
                    iActiveStreams++;
                } catch (Exception ex) {
                    object[] tmp = Functions.ParseException(ex);
                    Debug.WriteLineIf(GlobalVar.DEBUG, "!EXCEPTION!:MENUENTRYCREATE: Error inserting menu item (" + tmp[2] + ":" + tmp[3] + ":"  + tmp[0] + "): " + ex.Message);
                    Log.Add("MenuEntryCreate>" + ex.Message);
                }
                
                Debug.WriteLineIf(GlobalVar.DEBUG, "MENUENTRYCREATE: Item added to Menu");
            }
            
            Debug.WriteLineIf(GlobalVar.DEBUG, "MENUENTRYCREATE: END");
            return bRet;
        }
        
        /// <summary>
        /// Check if a menu entry exists and remove it
        /// </summary>
        /// <param name="sUser">Streamname</param>
        private bool MenuEntryRemove(string sUser)
        {
            // make sure we can access the settingsform
            if (this.InvokeRequired)
            {
                Debug.WriteLineIf(GlobalVar.DEBUG, "MENUENTRYREMOVE: NEEDS INVOKE");
                return (bool)this.Invoke ((Func<string,bool>)MenuEntryRemove, sUser);
            }
            
            Debug.WriteLineIf(GlobalVar.DEBUG, "MENUENTRYREMOVE: Remove menu entry for stream: " + sUser);
            bool bRet = false;
            if (InvokeRequired) {
                MyMenu.Invoke((MethodInvoker)delegate {
                    Debug.WriteLineIf(GlobalVar.DEBUG, "MENUENTRYREMOVE: Locking MyMenu");
                    lock (MyMenu) {
                        int iMenuIDX = MyMenu.Items.IndexOfKey(sUser);
                        Debug.WriteLineIf(GlobalVar.DEBUG, "MENUENTRYREMOVE: Menu index: " + iMenuIDX);
                                  
                                  
                        if (iMenuIDX != -1) {
                            Debug.WriteLineIf(GlobalVar.DEBUG, "MENUENTRYREMOVE: Entry found at: " + iMenuIDX);
                            notifyIcon1.ShowBalloonTip(750, "Stream is Offline", sUser, ToolTipIcon.Info);
                            MyMenu.Items.RemoveAt(iMenuIDX);
//                                      MyMenu.Items.RemoveAt(iMenuIDX);
                            bRet = true;
                            // decrease active streams number only when we are really removing an entry
                            iActiveStreams--;
                        }
                    }
                });
            } else {
                Debug.WriteLineIf(GlobalVar.DEBUG, "MENUENTRYREMOVE: Locking MyMenu");
                lock (MyMenu) {
                    int iMenuIDX = MyMenu.Items.IndexOfKey(sUser);
                    Debug.WriteLineIf(GlobalVar.DEBUG, "MENUENTRYREMOVE: Menu index: " + iMenuIDX);
                              
                              
                    if (iMenuIDX != -1) {
                        Debug.WriteLineIf(GlobalVar.DEBUG, "MENUENTRYREMOVE: Entry found at: " + iMenuIDX);
                        notifyIcon1.ShowBalloonTip(750, "Stream is Offline", sUser, ToolTipIcon.Info);
                        MyMenu.Items.RemoveAt(iMenuIDX);
//                                      MyMenu.Items.RemoveAt(iMenuIDX);
                        bRet = true;
                        // decrease active streams number only when we are really removing an entry
                        iActiveStreams--;
                    }
                }
            }
            
            
            Debug.WriteLineIf(GlobalVar.DEBUG, "MENUENTRYREMOVE: END");
            return bRet;
        }
        
        private bool UpdateToolTip()
        {
            if (this.InvokeRequired)
                return (bool)this.Invoke ((Func<bool>)UpdateToolTip);
            
            string sText    = iActiveStreams + " active Streams";
            string sAdd     = "";
            string sAccount = "";
            string sDay     = "";
            string sHour    = "";
            string sMin     = "";
            string sSec     = "";
            int iDay        = 0;
            int iHour       = 0;
            int iMin        = 0;
            int iSec        = 0;
            // display info when we are currently running an update
            if (bGettingData) {
                sAdd = " (Updating " + iCurrentCheck + "/" + iMaxCheck + ")";
            } else {
                try {
                    TimeSpan t = TimeSpan.FromSeconds(settings.checkinterval * 60) - (DateTime.Now - tStreamsTimerLastStart);
                    iDay    = t.Days;       // how many days
                    iHour   = t.Hours;      // how many hours
                    iMin    = t.Minutes;    // how many minutes
                    iSec    = t.Seconds;    // how many seconds
                } catch (Exception ex) {
                    // possible division by zero
                    Log.Add("UpdateToolTip>" + ex.Message);
                }
                
                if (iDay > 0) {
                    sDay    = " " + iDay + "d";
                }
                if (iHour > 0) {
                    sHour   = " " + iHour + "h";
                }
                if (iMin > 0) {
                    sMin    = " " + iMin + "m";
                }
                if (iSec > 0) {
                    sSec    = " " + iSec + "s";
                }
                
                if (sDay != "" || sHour != "" || sMin != "" || sSec != "") {
                    sAdd = " - next check in" + sDay + sHour + sMin + sSec;
                }
            }
            
            if (bAccountOnline) {
                sAccount = sAccountOnlineData;
//                Debug.WriteLineIf(GlobalVar.DEBUG, "UPDATETOOLTIP: Account Online " + sAccountOnlineData);
            }
//            notifyIcon1.Text = sText + sAdd + sAccount; // tooltip max 63 chars
            notifyIcon1.Text = sText + sAdd;
//            MyMenu.Invoke((MethodInvoker) delegate {notifyIcon1.Text = s;});
            Debug.WriteLineIf(GlobalVar.DEBUG, "UPDATETOOLTIP: " + notifyIcon1.Text);
            return true;
        }
        
        /// <summary>
        /// Check if an account is currently online
        /// </summary>
        /// <param name="sAccount">Streamname</param>
        /// <returns>Returns game name if stream is online</returns>
        public string GetOnlineStatus(string sAccount)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "GETONLINESTATUS: Putting response into var");
            string sReturn = null;
            WebCheckResponse WebCheck;
            sAccount = Functions.ConvertAlphaNum(sAccount);
            const string sBaseURL = "https://api.twitch.tv/kraken/streams/";
            string sParams = "";
            string responseString = "";
            
            string sURL = sBaseURL + sAccount + sParams;
            Debug.WriteLineIf(GlobalVar.DEBUG, "GETONLINESTATUS: Created URL: " + sURL);
            WebCheck = Functions.DoWebRequest(sURL);
            if (sAccount != "" && (WebCheck.HttpWResp.StatusCode == HttpStatusCode.OK)) {
                Debug.WriteLineIf(GlobalVar.DEBUG, "GETONLINESTATUS: Parsing WebResponse");
//                try {
//                    using (Stream stream = WebCheck.HttpWResp.GetResponseStream()) {
//                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
//                        responseString = reader.ReadToEnd();
//                    }
//                } catch (Exception ex) {
//                    Debug.WriteLineIf(GlobalVar.DEBUG, "GETONLINESTATUS: Failed on datastream: " + ex.Message);
//                    Log.Add("GetOnlineStatus>" + ex.Message);
//                }
                
                dynamic data = Functions.ParseWebResponseToObject(WebCheck.HttpWResp);
                Debug.WriteLineIf(GlobalVar.DEBUG, "GETONLINESTATUS: Creating workable object from json response");
                try {
//                    dynamic data = JsonConvert.DeserializeObject(responseString);
                    if (data.stream == null) {
                        Debug.WriteLineIf(GlobalVar.DEBUG, "GETONLINESTATUS: data.stream is null - user offline or error");
                        // offline handling
                        
                    } else {
                        Debug.WriteLineIf(GlobalVar.DEBUG, "GETONLINESTATUS: data.stream contains something - user should be online");
                        // online handling
                        
                        sReturn = data.stream.game;
                    }
                } catch (Exception ex) {
                    Debug.WriteLineIf(GlobalVar.DEBUG, "!EXCEPTION!:GETONLINESTATUS: Failed on Deserialize: " + ex.Message);
                    Log.Add("GetOnlineStatus>" + ex.Message);
                }
                
                
                
            }
            
            Debug.WriteLineIf(GlobalVar.DEBUG, "GETONLINESTATUS: User is playing: " + sReturn);
            return sReturn;
        }
        
        /// <summary>
        /// Open stream in default browser
        /// </summary>
        /// <param name="sUser">Streamname</param>
        public void OpenStream(string sUser)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "OPENSTREAM: Launch a browser to watch stream");
            string sURL = GetStreamURL(Functions.ConvertAlphaNum(sUser));
            Debug.WriteLineIf(GlobalVar.DEBUG, "OPENSTREAM: Opening URL: " + sURL);
            try {
                Process.Start(sURL);
            } catch (Exception ex) {
                Log.Add("OpenStream>" + ex.Message);
            }
        }
        
        /// <summary>
        /// Creates an URL to a supplied Twitch Stream
        /// </summary>
        /// <param name="sUser">Streamname</param>
        /// <returns>Returns URL of a twitch stream</returns>
        public string GetStreamURL(string sUser)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "GETSTREAMURL: Create an URL for browser");
            const string sBaseURL   = "http://twitch.tv/";
            const string sAppendURL = "/";
            
            // create new url and make sure there is no sneaky code in the username
            string sURL = sBaseURL + Functions.ConvertAlphaNum(sUser) + sAppendURL;
            
            return sURL;
        }
        
        /// <summary>
        /// Get information if a user is currently streaming
        /// </summary>
        /// <param name="sUser">Streamname</param>
        public bool CheckUser(string sUser)
        {
            bool bActive = false;
            Debug.WriteLineIf(GlobalVar.DEBUG, "CHECKUSER: We lookup a user: " + sUser);
            //notifyIcon1.ShowBalloonTip(1500, "Checking user", sUser, ToolTipIcon.Info);
            string sGame = GetOnlineStatus(sUser);
            Debug.WriteLineIf(GlobalVar.DEBUG, "CHECKUSER: We looked up a user");
#if _HACK
            // HACK: creating random entries
            if (GlobalVar.GENERATEDATA) {
                Random rnd = new Random();
                char[] let = new char[10];
                for (int i = 0; i < 10; i++) {
                    let[i] = (char)('a' + rnd.Next(0, 26));
                }
                sGame = new string(let);
                if (rnd.Next(0, 1000) < 500)
                    sGame = null;
                Debug.WriteLineIf(GlobalVar.DEBUG, "HACK:CHECKUSER: Random user: " + sGame);
            }
#endif
            
            if (sGame != null) {
                //MessageBox.Show(sUser + Environment.NewLine + sGame, "Creating Menu");
                MenuEntryCreate(sUser, sGame);
                bActive = true;
            } else {
                MenuEntryRemove(sUser);
            }
            
            return bActive;
        }
        
        /// <summary>
        /// Check all configured streams
        /// </summary>
        public void CheckStreams()
        {
            // create a local copy of the current list of streams to avoid getting a mixup when changing settings, settings were used directly earlier
            IList<TwStream> aCheckUsers = settings.streams; // store a local copy of streamlist
            
            Debug.WriteLineIf(GlobalVar.DEBUG, "CHECKSTREAMS: We gotta check our list of streams");
            if (!bGettingData) {
                bGettingData = true;
                
                // set maxcheck for progress display
                iMaxCheck = aCheckUsers.Count;
                try {
                    for (int i = 0; i < iMaxCheck; i++) {
                        iCurrentCheck = i + 1;
                        CheckUser(aCheckUsers[i].sStreamname);
                    }
                } catch (Exception ex) {
                    // user index could be out of bounds
                    Log.Add("CheckStreams>" + ex.Message);
                }
                
                // bGettingData needs to be set to false first so iCurrentCheck update will work correctly
                bGettingData = false;
                iCurrentCheck = 0;
            } else {
                Debug.WriteLineIf(GlobalVar.DEBUG, "CHECKSTREAMS: Another check is still running..." + iCurrentCheck + "/" + iMaxCheck);
                // FIXME RELEASE: Make sure to have this enabled for release! So users get a warning when their list can't be processed in between intervals.
                MessageBox.Show("Consider raising the interval a bit." + Environment.NewLine + "Processing " + iCurrentCheck + "/" + iMaxCheck + Environment.NewLine + Environment.NewLine +
                                "If this message keeps coming up over and over again at the same step then consider restarting the program.",
                                "Check already running", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }
        
        /// <summary>
        /// Wrapper to update ToolTip everytime the streamer count changes
        /// </summary>
        public int iActiveStreams {
            get{ return _iActiveStreams; }
            set {
                _iActiveStreams = value;
                UpdateToolTip();
            }
        }
        
        /// <summary>
        /// Wrapper to update ToolTip everytime the streamer count changes
        /// </summary>
        public int iCurrentCheck {
            get{ return _iCurrentCheck; }
            set {
                _iCurrentCheck = value;
                UpdateToolTip();
            }
        }
        
        /// <summary>
        /// Wrapper to update ToolTip everytime the streamer count changes
        /// </summary>
        public int iNextCheck {
            get{ return _iNextCheck; }
            set {
                _iNextCheck = value;
                UpdateToolTip();
            }
        }
        
        /// <summary>
        /// Enter default settings values if non have been loaded
        /// </summary>
        public void SetDefaultSettings()
        {
            if (settings.checkinterval == 0 || settings.checkinterval < 1) {
                settings.checkinterval = 5;
            }
            
            if (settings.checkaccount == null) {
                settings.checkaccount = "";
            }
            
            // add default entries when list is empty
            if (settings.streams == null) {
                // FIXME RELEASE: set default streams
                string[] streamers = new string[] {"Autositz", "DRUCKWELLETV", "Garrynewman", "Denkii"};
//                string[] streamers = new string[] {"Entry 1", "Entry 2", "Entry 3", "Entry 4"};
                settings.streams = new List<TwStream>();
                foreach (string s in streamers) {
                    TwStream s2 = new TwStream();
                    s2.bImportant= true;
                    s2.sStreamname = s;
                    settings.streams.Add(s2);
                }
            }
            
        }
        
        /// <summary>
        /// Display info on Account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void NotifyIcon1MouseClick(object sender, EventArgs e)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "NOTIFYICON1MOUSECLICK: Info clicked");
            notifyIcon1.ShowBalloonTip(30, settings.checkaccount, sAccountOnlineData, ToolTipIcon.Info);
        }
        
    }
    
    /// <summary>
    /// Settings definition
    /// </summary>
    public class MySettings : AppSettings<MySettings>
    {
        public int checkinterval;
        public string checkaccount;
        public IList<TwStream> streams;
        
        public MySettings()
        {
            // population of default data moved to putDefaultSettings() to avoid getting extra List entries on Load
        }
    }
    
    /// <summary>
    /// Class for each single stream entry
    /// </summary>
    public class TwStream
    {
        public bool bImportant { get; set; }
        public string sStreamname { get; set; }
        
        public TwStream()
        {
            bImportant = false;
            sStreamname = "";
        }
    }
}
