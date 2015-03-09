﻿/*
 * Created by SharpDevelop.
 * User: Autositz
 * Date: 01.03.2015
 * Time: 00:19
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
    /// Description of SettingsForm.
    /// </summary>
    public partial class SettingsForm : Form
    {
        /// <summary>
        /// Hold currently loaded settings
        /// </summary>
        public MySettings settings;
        /// <summary>
        /// Holds the last response from a web request (make sure to not make rival requests at the same time)
        /// </summary>
        private HttpWebResponse HttpWResp;
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
        
        
        public SettingsForm()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            // try to hide the form on startup
            this.Hide();
            
            // load error logging
            this.Log = new Logging();
            
            // load stored settings from file and check if default values need to be put in
            this.settings = MySettings.Load();
            putDefaultSettings();
        }
        
        public MySettings Start()
        {
            this.Start(this.settings);
            return this.settings;
        }
        
        /// <summary>
        /// Start putting data - moved in here to avoid invoke complaining about uninitialized component
        /// </summary>
        public MySettings Start(MySettings objSettings)
        {
            // put supplied settings into working space
            this.settings = objSettings;
            
            // set the check value if a check is currently running
            bGettingData = false;
            iCurrentCheck = 0;
            iMaxCheck = 0;
            iActiveStreams = 0;
            // add settings into form
            putSettingsIntoForm();
            
            // take a guess what we are doing here ...
            int[] iTMP = { 116, 104, 97, 45, 109, 111, 98, 64, 103, 109, 120, 46, 100, 101 };
            string sTMP = "";
            foreach (int eTMP in iTMP) {
                sTMP += char.ConvertFromUtf32(eTMP);
            }
            
            
            int i;
            // create clickable feedback links
//            linkLabelFeedback.Text = "Send Feedback via eMail\r\nSend Feedback via Steam";
            // get 2nd occurance of Send Feedback to avoid counting it over and over again
            i = linkLabelFeedback.Text.IndexOf("Send Feedback");
            Debug.WriteLineIf(GlobalVar.DEBUG, "SETTINGSFORM: Send Feedback 1 located at: " + i);
            linkLabelFeedback.Links.Add( i, 13, "mailto:"+sTMP+"?subject=[TwitchStreamCheckerFeedback]");
            sTMP = null; // clear tempvar
            // get 2nd occurance of Send Feedback to avoid counting it over and over again
            i = GetNthIndex(linkLabelFeedback.Text, "Send Feedback", 2);
            Debug.WriteLineIf(GlobalVar.DEBUG, "SETTINGSFORM: Send Feedback 2 located at: " + i);
            linkLabelFeedback.Links.Add( i, 13, "steam://friends/message/76561197960330502"); // autositz: 76561197960330502  kretze: 76561197993179564
            
            
            // for testing purpose only, form needs to be initialized first, timer inits only once
//            System.Timers.Timer tTimerFirst = new System.Timers.Timer();
//            tTimerFirst.AutoReset = false;
//            tTimerFirst.Interval = 10000;
//            tTimerFirst.Elapsed += new ElapsedEventHandler(doTimer);
//            tTimerFirst.Enabled = false;
//            Task.Factory.StartNew(checkStreams);
            
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
            
            
            return this.settings;
        }
        
        /// <summary>
        /// What to do each time our timer calls for us
        /// </summary>
        void doTimer(object sender, EventArgs e)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "DOTIMER: Timer called us");
            Task.Factory.StartNew(checkStreams);
            
        }
        
        /// <summary>
        /// Close the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolStripMenuItemClickExit(object sender, EventArgs e)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "TOOLSTRIPMENUITEMCLICKEXIT: Menu Click - Exit");
            if (HttpWResp != null) {
                HttpWResp.Close();
                Debug.WriteLineIf(GlobalVar.DEBUG, "TOOLSTRIPMENUITEMCLICKEXIT: WebResponse closed");
            }
                
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
            // populate settings form with currently store data
            putSettingsIntoForm();
            
            // make sure we see a settings window which was previously hidden, hopefully...
            this.Show();
            this.Focus();
        }
        
        /// <summary>
        /// Store the settings in the config file upon clicking OK and reset the values in the form with cleaned up values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ButtonSettingsClickSAVE(object sender, EventArgs e)
        {
            // disable the button to avoid double save and reenable it at the end
            Button bSender = sender as Button;
            bSender.Enabled = false;
            
            Debug.WriteLineIf(GlobalVar.DEBUG, "BUTTONSETTINGSCLICKSAVE: Button click - OK");
            // try to hide the settings window
            this.Hide();
            
            Debug.WriteLineIf(GlobalVar.DEBUG, "BUTTONSETTINGSCLICKSAVE: Store the form field data into the settings");
            // get current settings from form and store them in the file
            // check interval
            this.settings.checkinterval = convertNum(comboBoxInterval.Text, 10);
            comboBoxInterval.Text = this.settings.checkinterval.ToString();
            tBackgroundTimer.Interval = this.settings.checkinterval * 60 * 1000;
            Debug.WriteLineIf(GlobalVar.DEBUG, "BUTTONSETTINGSCLICKSAVE: Set new interval to: " + tBackgroundTimer.Interval + " ms");
            
            // check account name
            this.settings.checkaccount = convertAlphaNum(textBoxAccountCheck.Text);
            textBoxAccountCheck.Text = this.settings.checkaccount;
            Debug.WriteLineIf(GlobalVar.DEBUG, "BUTTONSETTINGSCLICKSAVE: Set new Account to: " + textBoxAccountCheck.Text);
            
            // check streams
            
            
            this.settings.Save();
            bSender.Enabled = true;
        }
        /// <summary>
        /// Close the Settings menu without storing data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ButtonSettingsClickCANCEL(object sender, EventArgs e)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "BUTTONSETTINGSCLICKCANCEL: Button click - Cancel");
            this.Visible = false;
        }
        
        /// <summary>
        /// Get all channels a user is following
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ButtonSettingsClickGetUsers(object sender, EventArgs e)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "BUTTONSETTINGSCLICKGETUSERS: Button click - Get streams a user is following - got auth token?");
            // TODO: Implement ButtonSettingsGetUsersClick, requires token, add field for token and gettoken url to twitch, no default token, check if user and token create correct answer
            MessageBox.Show("Not available right now.", "Channels following");
            return;
            //getAccountFollowings(textBoxAccountCheck.Text);
        }
        
        /// <summary>
        /// Convert the settings dialog data to settings save data.
        /// </summary>
        /// <param name="s">String with linefeeds to be converted</param>
        /// <returns>String Array</returns>
        public String[] convertLFtoArray(String s)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "CONVERTLFTOARRAY: Convert a string with newlines to an array that splits at newlines");
            return s.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
        }
        
        /// <summary>
        /// Convert the settings savefile data to settings dialog output.
        /// </summary>
        /// <param name="a">String Array to be merged</param>
        /// <returns>String with line feeds</returns>
        public String convertArraytoLF(String[] a)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "CONVERTARRAYTOLF: Convert an array to string with newlines");
            return string.Join(Environment.NewLine, a);
        }
        
        /// <summary>
        /// Clear any non-alphanumeric from text with the option to clear or preserve newlines.
        /// </summary>
        /// <param name="s">String to be checked</param>
        /// <param name="preserveNewline">Keep new lines or strip them too</param>
        /// <returns>Cleaned string</returns>
        public string convertAlphaNum(string s, bool preserveNewline= false)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "CONVERTALPHANUM: Converting text string to contain only valid chars with by preserving newline: " + preserveNewline);
            char[] arr = s.ToCharArray();
            
            if (preserveNewline) {
                arr = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c) || c == '\n' || c == '\r' || c == '_')));
            } else {
                arr = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c) || c == '_')));
            }
            
            String str = new string(arr);
            
            return str;
        }
        
        /// <summary>
        /// Clear any non-numeric from text with fallback integer value
        /// </summary>
        /// <param name="s">String to be converted to an int</param>
        /// <param name="iFallback">Integer value to be used when conversion fails</param>
        /// <returns>Converted Integer or Fallback value</returns>
        public int convertNum(string s, int iFallback = 0)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "CONVERTNUM: Converting " + s + " with fallback: " + iFallback);
            int i = 0;
            if (!Int32.TryParse(s, out i)) {
                i = iFallback;
            }
            
            return i;
        }
        
        /// <summary>
        /// Put currently stored settings into settings form
        /// </summary>
        private void putSettingsIntoForm()
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "PUTSETTINGSINTOFORM: Get settings into the form fields");
            comboBoxInterval.Text = convertNum(settings.checkinterval.ToString()).ToString();
            textBoxAccountCheck.Text = convertAlphaNum(this.settings.checkaccount);
            
            try {
                
                Debug.WriteLineIf(GlobalVar.DEBUG, "PUTSETTINGSINTOFORM: Fill bsStream and dgvStreams with settings");
                // rebind BindingSource to make sure it is bound.. designer shows it bound but does not when this point in code is reached...
                bsStreams.DataSource = settings.streams;
                dgvStreams.DataSource = bsStreams;
                
                dgvStreams.AutoGenerateColumns = false; // columns are set at designtime
                dgvStreams.Refresh();
            } catch (Exception ex) {
                Log.Add("getOnlineStatus>" + ex.Message);
            }
            
        }
        
        /// <summary>
        /// Resize columns to usefull needs but keep the possibility of user resize afterwards (will get reset after each new input)
        /// Maybe not needed when Streamnames are set to Fill?
        /// </summary>
        private void ResizeColumns()
        {
            int iMax = dgvStreams.Columns.Count;
            Debug.WriteLineIf(GlobalVar.DEBUG, "RESIZECOLUMNS: Columns: " + iMax);
            int iWidth = 0;
            // skip the first column as it should never change
            for (int i = 1; i < iMax; i++) {
                Debug.WriteLineIf(GlobalVar.DEBUG, "RESIZECOLUMNS: ColumnType[" + i + "]: " + dgvStreams.Columns[i].CellType);
                dgvStreams.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                iWidth = dgvStreams.Columns[i].Width;
                dgvStreams.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvStreams.Columns[i].Width = iWidth;
            }
        }
        
        /// <summary>
        /// Get all streams an account is following, requires auth token!
        /// </summary>
        /// <param name="sAccount">Stream account to check</param>
        /// <returns>Returns true if successfull</returns>
        public bool getAccountFollowings(string sAccount)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "GETACCOUNTFOLLOWINGS: Parsing done");
            bool bSuccess = false;
            sAccount = convertAlphaNum(sAccount);
            string sBaseURL = "https://api.twitch.tv/kraken/streams/followed/";
            string sParams = "";
            string responseString = "";
            
            string sURL = sBaseURL + sAccount + sParams;
            
            if (doWebRequest(sURL)) {
                using (Stream stream = HttpWResp.GetResponseStream()) {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    responseString = reader.ReadToEnd();
                }
                
                dynamic data = JsonConvert.DeserializeObject(responseString);
                
                //MessageBox.Show(data.ToString());
            }
            
            
            return bSuccess;
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
            sAccount = convertAlphaNum(sAccount);
            const string sBaseURL = "https://api.twitch.tv/kraken/streams/";
            string sParams = "";
            string responseString = "";
            
            string sURL = sBaseURL + sAccount + sParams;
            Debug.WriteLineIf(GlobalVar.DEBUG, "GETONLINESTATUS: Created URL: " + sURL);
            if (sAccount != "" && doWebRequest(sURL)) {
                Debug.WriteLineIf(GlobalVar.DEBUG, "GETONLINESTATUS: Parsing WebResponse");
                try {
                    using (Stream stream = HttpWResp.GetResponseStream()) {
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
        /// Get data from an URL
        /// </summary>
        /// <param name="sURL">URL to contact</param>
        /// <returns>Returns true if request was successfull</returns>
        public bool doWebRequest(string sURL)
        {
            bool bRet = false;
            
            try {
                Debug.WriteLineIf(GlobalVar.DEBUG, "DOWEBREQUEST: Making a new Web Request");
                HttpWebRequest HttpWReq = (HttpWebRequest)WebRequest.Create(sURL);
                Debug.WriteLineIf(GlobalVar.DEBUG, "DOWEBREQUEST: Putting response into var");
                HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();
                Debug.WriteLineIf(GlobalVar.DEBUG, "DOWEBREQUEST: Response stored for future use");
                
                if (HttpWResp.StatusCode == HttpStatusCode.OK)
                    bRet = true;
            } catch (Exception ex) {
                Log.Add("doWebRequest>" + ex.Message);
            }
            
            return bRet;
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
                tsiNewItem.ToolTipText = "Playing: " + convertAlphaNum(sGame);
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
                    object[] tmp = ParseException(ex);
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
        /// Open stream in default browser
        /// </summary>
        /// <param name="sUser">Streamname</param>
        public void openStream(string sUser)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "OPENSTREAM: Launch a browser to watch stream");
            string sURL = getStreamURL(convertAlphaNum(sUser));
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
            string sURL = sBaseURL + convertAlphaNum(sUser) + sAppendURL;
            
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
        /// Tries to get more detailed info on the exception
        /// </summary>
        /// <param name="e">Supply the Exception that has occured.</param>
        /// <returns>Returns object: 0 Line number, 1 Column number, 2 Filename, 3 Methodname</returns>
        public object[] ParseException(Exception ex)
        {
            object[] data = { 0, 0, "", "" }; // <int>line, <int>column, <string>filename, <string>methodname
            
            //Get a StackTrace object for the exception
            StackTrace st = new StackTrace(ex, true);
        
            //Get the first stack frame
            StackFrame frame = st.GetFrame(0);
        
            //Get the file name
            data[2] = frame.GetFileName();
        
            //Get the method name
            data[3] = frame.GetMethod().Name;
        
            //Get the line number from the stack frame
            data[0] = frame.GetFileLineNumber();
        
            //Get the column number
            data[1] = frame.GetFileColumnNumber();
            
            return data;
        }
        
        private void linkLabelFeedback_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string sLink = e.Link.LinkData.ToString();
            Debug.WriteLineIf(GlobalVar.DEBUG, "LINKLABELFEEDBACK_LINKCLICKED: Opening Link: " + sLink);
            try {
                Process.Start(sLink);
            } catch (Exception ex) {
                Log.Add("linkLabelFeedback_LinkClicked>" + ex.Message);
            }
        }
        
        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "SETTINGSFORM_FORMCLOSING: Someone clicked Close");
            if (e.CloseReason == CloseReason.UserClosing) {
                Debug.WriteLineIf(GlobalVar.DEBUG, "SETTINGSFORM_FORMCLOSING: User closed the form");
                e.Cancel = true;
                
                // TODO: lazy work to hide the form, remove it and recreate maybe? no need to hold the settings form in memory all the time i guess...
                SettingsForm senders = sender as SettingsForm;
                if (senders != null) {
                    senders.Hide();
                }
                
            }
        }
        
//        void dgvStreams_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
//        {
//            // only redo the first column header
//            if (e.RowIndex == -1 && e.ColumnIndex == -1) {
//                Brush brushTMP = new SolidBrush(e.CellStyle.ForeColor);
//                e.PaintBackground(e.ClipBounds, true);
//                e.Graphics.DrawString(e.FormattedValue.ToString(), e.CellStyle.Font, brushTMP, e.CellBounds.X - 3, e.CellBounds.Y, StringFormat.GenericDefault);
//                e.Handled = true;
//            }
//        }
        
        /// <summary>
        /// Check if the data entered matches what we need/want
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvStreams_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string sColumn = dgvStreams.Columns[e.ColumnIndex].Name;
            string sText = e.FormattedValue.ToString();
            Debug.WriteLineIf(GlobalVar.DEBUG, "DGVSTREAMS_CELLCALIDATING: START");
            // Don't continue if if we are not checking the streamname
            if (sColumn.Equals("sStreamname")) {
                Debug.WriteLineIf(GlobalVar.DEBUG, "DGVSTREAMS_CELLCALIDATING: checking streamname");
                // Check if only valid chars have been entered
                foreach (char c in sText)
                {
                    Debug.WriteLineIf(GlobalVar.DEBUG, "DGVSTREAMS_CELLCALIDATING: checking: " + sText + ": " + c);
                    if (!Char.IsLetterOrDigit(c) && c != '_') {
                        Debug.WriteLineIf(GlobalVar.DEBUG, "DGVSTREAMS_CELLCALIDATING: Character is not allowed: " + c);
                        dgvStreams.Rows[e.RowIndex].ErrorText = "Please only enter valid streamnames (a-z, _)";
                        MessageBox.Show(dgvStreams.Rows[e.RowIndex].ErrorText + Environment.NewLine + Environment.NewLine + sText, "Invalid Streamname", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        dgvStreams.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Red;
                        e.Cancel = true;
                    }
                }
            }
        }
        
        /// <summary>
        /// Clear error message to make sure there is non displayed anymore
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgvStreams_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "DGVSTREAMS_CELLENDEDIT: Cell edit ended");
            dgvStreams.Rows[e.RowIndex].ErrorText = String.Empty;
            dgvStreams.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Empty;
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
        /// Handle clicks into the Streamlist cells
        /// </summary>
        /// <param name="sender">DataGridView</param>
        /// <param name="e">DataGridViewCellEventArgs</param>
        void dgvStreams_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // only DataGridView allowed in here because we are racist!
            var senderGrid = (DataGridView)sender;
            
            // check if a button was clicked and if it's not inside the header
            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                try {
                    senderGrid.Rows.RemoveAt(e.RowIndex);
                } catch (Exception ex) {
                    Log.Add("dgvStreams_CellContentClick>" + ex.Message);
                }
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
        
        public int GetNthIndex(string sText, string sSearch, int iOccurred)
        {
            return GetNthIndex(sText, sSearch, iOccurred, 0, 1);
        }
        /// <summary>
        /// Finds the position of the Nth occurrance of Search in Text
        /// </summary>
        /// <param name="sText"></param>
        /// <param name="sSearch"></param>
        /// <param name="iOccurred">int </param>
        /// <param name="iPos"></param>
        /// <param name="iRunning"></param>
        /// <returns>Position of the Nth Occurrance</returns>
        public int GetNthIndex(string sText, string sSearch, int iOccurred, int iPos, int iRunning)
        {
            int iRet = -1;
            // search index, increase starting point and interval if something was found
            Debug.WriteLineIf(GlobalVar.DEBUG, "GETNTHINDEX: Starting search at \""+iPos+"\" for \""+sSearch+"\" in \""+sText+"\"");
            iPos = sText.IndexOf(sSearch, iPos);
            Debug.WriteLineIf(GlobalVar.DEBUG, "GETNTHINDEX: Found entry at: " + iPos);
            // we are in the depth we need
            if (iRunning == iOccurred)
            {
                Debug.WriteLineIf(GlobalVar.DEBUG, "GETNTHINDEX: We found the "+iOccurred+" occurance at: "+iPos);
                iRet = iPos;
            } else
            {
                Debug.WriteLineIf(GlobalVar.DEBUG, "GETNTHINDEX: Doing another round: " + iRunning);
                iRunning++; // increase by one to know how often we have run
                iPos++; // increase by one to find the same entry again
                iRet = GetNthIndex(sText, sSearch, iOccurred, iPos, iRunning);
            }
            
            
            return iRet;
        }
    }
    
    public class twDataGridViewButtonColumn : DataGridViewButtonColumn
    {
        public override DataGridViewCellStyle DefaultCellStyle {
            get {
                DataGridViewCellStyle TMPDefaultCellStyle = new DataGridViewCellStyle();
                TMPDefaultCellStyle.Padding = new Padding(0, TMPDefaultCellStyle.Padding.Top, 0, TMPDefaultCellStyle.Padding.Bottom);
                return TMPDefaultCellStyle;
            }
            set {
                base.DefaultCellStyle = value;
            }
        }
    }
}
