/*
 * Created by SharpDevelop.
 * User: Autositz
 * Date: 09.03.2015
 * Time: 14:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
        private int iCurrentCheck;
        /// <summary>
        /// Current number of streamlist entries to check
        /// </summary>
        private int iMaxCheck;
        /// <summary>
        /// Currently active streams to display in icon tooltip
        /// </summary>
        private int _iActiveStreams;
        /// <summary>
        /// Background timer to call events
        /// </summary>
        private System.Timers.Timer tBackgroundTimer;
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
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
            this.Hide();
            
            // load error logging
            this.Log = new Logging();
            
            // set the check value if a check is currently running
            bGettingData = false;
            iCurrentCheck = 0;
            iMaxCheck = 0;
            iActiveStreams = 0;
            
            
            System.Timers.Timer tMainTimer = new System.Timers.Timer();
            tMainTimer.Interval = 1500; // uncomment this for faster cycles on small entries
            // redo associated actions
            tMainTimer.AutoReset = true;
            // set the action we want to do at the given interval
            tMainTimer.Elapsed += new System.Timers.ElapsedEventHandler(MainForm.test);
            // make sure the timer is starting
            tMainTimer.Enabled = true; // enable timer
            
//            Program.objSettingsForm = new SettingsForm(); // really needed at start?
            
//            settings = new MySettings();
            // load stored settings from file and check if default values need to be put in
            settings = MySettings.Load();
            putDefaultSettings();
//            settings = Program.objSettingsForm.Start(); // really needed at start?
            
            
            
            // create a new timer to run stuff at given interval
            tBackgroundTimer = new System.Timers.Timer();
            // FIXME RELEASE: Make sure to set checkinterval timer for release!
            // start timer with 1000ms * 60s * interval-minutes
            tBackgroundTimer.Interval = (settings.checkinterval * 60 * 1000);
//            tBackgroundTimer.Interval = 5000; // uncomment this for faster cycles on small entries
            // redo associated actions
            tBackgroundTimer.AutoReset = true;
            // set the action we want to do at the given interval
            tBackgroundTimer.Elapsed += new ElapsedEventHandler(doTimer);
            // make sure the timer is starting
            tBackgroundTimer.Enabled = true; // enable timer
            
            
            // clear up what we used
//            Program.objSettingsForm.Dispose(); // really needed at start?
        }
        
        /// <summary>
        /// What to do each time our timer calls for us
        /// </summary>
        void doTimer(object sender, EventArgs e)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "DOTIMER: Timer called us");
            Task.Factory.StartNew(checkStreams);
            
        }
        
        private static void test(object sender, EventArgs e)
        {
            
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
            tBackgroundTimer.Interval = iTimer * 60 * 1000;
            Debug.WriteLineIf(GlobalVar.DEBUG, "SETTIMER: Set new interval to: " + tBackgroundTimer.Interval + " ms");
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
                ToolStripItem tsiNewItem = new ToolStripMenuItem(sUser, null, (sender, e) => openStream(sUser), sUser);
                tsiNewItem.ToolTipText = "Playing: " + Functions.convertAlphaNum(sGame);
                Debug.WriteLineIf(GlobalVar.DEBUG, "MENUENTRYCREATE: Created new MenuItem");
                // add the new stream on top of the menu
                try {
                    MyMenu.Invoke((MethodInvoker) delegate {
                                      lock (MyMenu) {
                                          MyMenu.Items.Insert(0, tsiNewItem);
                                      }
                                  });
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
            MyMenu.Invoke((MethodInvoker) delegate {
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
            
            
            Debug.WriteLineIf(GlobalVar.DEBUG, "MENUENTRYREMOVE: END");
            return bRet;
        }
        
        private bool updateToolTip()
        {
            if (this.InvokeRequired)
                return (bool)this.Invoke ((Func<bool>)updateToolTip);
            
            string s = "";
            // display info when we are currently running an update
            if (bGettingData) {
                s = iActiveStreams + " active Streams (Updating " + iCurrentCheck + "/" + iMaxCheck + ")";
            } else {
                s = iActiveStreams + " active Streams";
            }
            notifyIcon1.Text = s;
//            MyMenu.Invoke((MethodInvoker) delegate {notifyIcon1.Text = s;});
            
            return true;
        }
        
        /// <summary>
        /// Check if an account is currently online
        /// </summary>
        /// <param name="sAccount">Streamname</param>
        /// <returns>Returns game name if stream is online</returns>
        public string getOnlineStatus(string sAccount)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "GETONLINESTATUS: Putting response into var");
            string sReturn = null;
            WebCheckResponse WebCheck;
            sAccount = Functions.convertAlphaNum(sAccount);
            const string sBaseURL = "https://api.twitch.tv/kraken/streams/";
            string sParams = "";
            string responseString = "";
            
            string sURL = sBaseURL + sAccount + sParams;
            Debug.WriteLineIf(GlobalVar.DEBUG, "GETONLINESTATUS: Created URL: " + sURL);
            WebCheck = Functions.doWebRequest(sURL);
            if (sAccount != "" && (WebCheck.HttpWResp.StatusCode == HttpStatusCode.OK)) {
                Debug.WriteLineIf(GlobalVar.DEBUG, "GETONLINESTATUS: Parsing WebResponse");
                try {
                    using (Stream stream = WebCheck.HttpWResp.GetResponseStream()) {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        responseString = reader.ReadToEnd();
                    }
                } catch (Exception ex) {
                    Debug.WriteLineIf(GlobalVar.DEBUG, "GETONLINESTATUS: Failed on datastream: " + ex.Message);
                    Log.Add("getOnlineStatus>" + ex.Message);
                }
                
                
                Debug.WriteLineIf(GlobalVar.DEBUG, "GETONLINESTATUS: Creating workable object from json response");
                try {
                    dynamic data = JsonConvert.DeserializeObject(responseString);
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
                    Log.Add("getOnlineStatus>" + ex.Message);
                }
                
                
                
            }
            
            Debug.WriteLineIf(GlobalVar.DEBUG, "GETONLINESTATUS: User is playing: " + sReturn);
            return sReturn;
        }
        
        /// <summary>
        /// Open stream in default browser
        /// </summary>
        /// <param name="sUser">Streamname</param>
        public void openStream(string sUser)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "OPENSTREAM: Launch a browser to watch stream");
            string sURL = getStreamURL(Functions.convertAlphaNum(sUser));
            Debug.WriteLineIf(GlobalVar.DEBUG, "OPENSTREAM: Opening URL: " + sURL);
            try {
                Process.Start(sURL);
            } catch (Exception ex) {
                Log.Add("openStream>" + ex.Message);
            }
        }
        
        /// <summary>
        /// Creates an URL to a supplied Twitch Stream
        /// </summary>
        /// <param name="sUser">Streamname</param>
        /// <returns>Returns URL of a twitch stream</returns>
        public string getStreamURL(string sUser)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "GETSTREAMURL: Create an URL for browser");
            const string sBaseURL   = "http://twitch.tv/";
            const string sAppendURL = "/";
            
            // create new url and make sure there is no sneaky code in the username
            string sURL = sBaseURL + Functions.convertAlphaNum(sUser) + sAppendURL;
            
            return sURL;
        }
        
        /// <summary>
        /// Get information if a user is currently streaming
        /// </summary>
        /// <param name="sUser">Streamname</param>
        public bool checkUser(string sUser)
        {
            bool bActive = false;
            Debug.WriteLineIf(GlobalVar.DEBUG, "CHECKUSER: We lookup a user: " + sUser);
            //notifyIcon1.ShowBalloonTip(1500, "Checking user", sUser, ToolTipIcon.Info);
            string sGame = getOnlineStatus(sUser);
            Debug.WriteLineIf(GlobalVar.DEBUG, "CHECKUSER: We lookuped a user");
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
        public void checkStreams()
        {
            // create a local copy of the current list of streams to avoid getting a mixup when changing settings, settings were used directly earlier
            IList<twStream> aCheckUsers = settings.streams; // store a local copy of streamlist
            
            Debug.WriteLineIf(GlobalVar.DEBUG, "CHECKSTREAMS: We gotta check our list of streams");
            if (!bGettingData) {
                bGettingData = true;
                
                // set maxcheck for progress display
                iMaxCheck = aCheckUsers.Count;
                try {
                    for (int i = 0; i < iMaxCheck; i++) {
                        iCurrentCheck = i + 1;
                        checkUser(aCheckUsers[i].sStreamname);
                    }
                } catch (Exception ex) {
                    // user index could be out of bounds
                    Log.Add("checkStreams>" + ex.Message);
                }
                
                bGettingData = false;
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
                updateToolTip();
            }
        }
        
        /// <summary>
        /// Enter default settings values if non have been loaded
        /// </summary>
        public void putDefaultSettings()
        {
            if (settings.checkinterval == 0 || settings.checkinterval < 1) {
                settings.checkinterval = 5;
            }
            
            if (settings.checkaccount == null) {
                settings.checkaccount = "Account";
            }
            
            // add default entries when list is empty
            if (settings.streams == null) {
                // FIXME RELEASE: set default streams
                string[] streamers = new string[] {"Autositz", "DRUCKWELLETV", "Garrynewman", "Denkii"};
//                string[] streamers = new string[] {"Entry 1", "Entry 2", "Entry 3", "Entry 4"};
                settings.streams = new List<twStream>();
                foreach (string s in streamers) {
                    twStream s2 = new twStream();
                    s2.bImportant= true;
                    s2.sStreamname = s;
                    settings.streams.Add(s2);
                }
            }
            
        }
        
    }
    
    /// <summary>
    /// Settings definition
    /// </summary>
    public class MySettings : AppSettings<MySettings>
    {
        public int checkinterval;
        public string checkaccount;
        public IList<twStream> streams;
        
        public MySettings()
        {
            // population of default data moved to putDefaultSettings() to avoid getting extra List entries on Load
        }
    }
    
    /// <summary>
    /// Class for each single stream entry
    /// </summary>
    public class twStream
    {
        public bool bImportant { get; set; }
        public string sStreamname { get; set; }
        
        public twStream()
        {
            bImportant = false;
            sStreamname = "";
        }
    }
}
