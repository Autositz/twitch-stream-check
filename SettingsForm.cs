/*
 * Created by SharpDevelop.
 * User: Autositz
 * Date: 01.03.2015
 * Time: 00:19
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
    /// Description of SettingsForm.
    /// </summary>
    public partial class SettingsForm : Form
    {
        /// <summary>
        /// Hold currently loaded settings
        /// </summary>
        public MySettings settings;
        /// <summary>
        /// Use logging for exceptions
        /// </summary>
        private Logging Log;
        /// <summary>
        /// Hold object of MainForm
        /// </summary>
        public MainForm objMainForm;
        
        
        public SettingsForm(MainForm mainform)
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            objMainForm = mainform; // make mainform accessible from settings form
            this.CreateControl();
            
            // try to hide the form on startup
            this.Hide();
            
            // load error logging
            this.Log = new Logging();
            
            // load stored settings from file and check if default values need to be put in
//            this.settings = MySettings.Load(); // do this in MainForm!
            
            
        }
        
        /// <summary>
        /// Wrapper for starting to put default data
        /// </summary>
        /// <returns></returns>
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
            
            // add settings into form
            PutSettingsIntoForm();
            
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
            i = Functions.GetNthIndex(linkLabelFeedback.Text, "Send Feedback", 2);
            Debug.WriteLineIf(GlobalVar.DEBUG, "SETTINGSFORM: Send Feedback 2 located at: " + i);
            linkLabelFeedback.Links.Add( i, 13, "steam://friends/message/76561197960330502"); // autositz: 76561197960330502  kretze: 76561197993179564
            
            
            return this.settings;
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
            int iTMP = Functions.ConvertNum(comboBoxInterval.Text, 10);
            int iTMP2 = Functions.ConvertNum(comboBoxIntervalImportant.Text, 5);
            // set timer to the new value entered in settings
            if (iTMP != this.settings.checkinterval) {
                this.settings.checkinterval = iTMP;
                objMainForm.SetTimer(this.settings.checkinterval);
            }
            if (iTMP2 != this.settings.checkintervalimportant) {
                this.settings.checkintervalimportant = iTMP2;
                objMainForm.SetTimer(this.settings.checkintervalimportant, "important");
            }
            
            // display balloon info for online/offline status
            this.settings.ballooninfo = checkBoxBalloonInfo.Checked;
            
            // old settings compatibility mode, create accountinfos if there is non present
            if (this.settings.accountinfo == null) {
                this.settings.accountinfo = new AccountInfoDetails();
            }
            // account information display
            this.settings.accountinfo.created_at    = checkBoxAccountCreatedAt.Checked;
            this.settings.accountinfo.delay         = checkBoxAccountDelay.Checked;
            this.settings.accountinfo.followers     = checkBoxAccountFollowers.Checked;
            this.settings.accountinfo.average_fps   = checkBoxAccountFPS.Checked;
            this.settings.accountinfo.game          = checkBoxAccountGame.Checked;
            this.settings.accountinfo.video_height  = checkBoxAccountVideoHeight.Checked;
            this.settings.accountinfo.viewers       = checkBoxAccountViewers.Checked;
            this.settings.accountinfo.views         = checkBoxAccountViews.Checked;
            
            // check account name
            this.settings.checkaccount = Functions.ConvertAlphaNum(textBoxAccountCheck.Text);
            Debug.WriteLineIf(GlobalVar.DEBUG, "BUTTONSETTINGSCLICKSAVE: Set new Account to: " + textBoxAccountCheck.Text);
            
            // check streams
            // they are stored directly in settings.streams
            
            this.settings.Save();
            bSender.Enabled = true;
            // TODO MAIN: make sure all data is stored in mainform!!!
            objMainForm.settings = this.settings;
            this.Dispose();
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
            // TODO MAIN: make sure all data is stored in mainform!!!
            this.Dispose();
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
        /// Put currently stored settings into settings form
        /// </summary>
        private void PutSettingsIntoForm()
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "PUTSETTINGSINTOFORM: Get settings into the form fields");
            comboBoxInterval.Text               = Functions.ConvertNum(settings.checkinterval.ToString()).ToString();
            comboBoxIntervalImportant.Text      = Functions.ConvertNum(settings.checkintervalimportant.ToString()).ToString();
            checkBoxBalloonInfo.Checked         = settings.ballooninfo;
            // check if accountinfo is there, compatibility with old settings save
            if (settings.accountinfo == null) {
                checkBoxAccountFPS.Checked = true;
                checkBoxAccountCreatedAt.Checked = true;
                checkBoxAccountDelay.Checked = true;
                checkBoxAccountFollowers.Checked = true;
                checkBoxAccountGame.Checked = true;
                checkBoxAccountVideoHeight.Checked = true;
                checkBoxAccountViewers.Checked = true;
                checkBoxAccountViews.Checked = true;
            } else {
                checkBoxAccountFPS.Checked = settings.accountinfo.average_fps;
                checkBoxAccountCreatedAt.Checked = settings.accountinfo.created_at;
                checkBoxAccountDelay.Checked = settings.accountinfo.delay;
                checkBoxAccountFollowers.Checked = settings.accountinfo.followers;
                checkBoxAccountGame.Checked = settings.accountinfo.game;
                checkBoxAccountVideoHeight.Checked = settings.accountinfo.video_height;
                checkBoxAccountViewers.Checked = settings.accountinfo.viewers;
                checkBoxAccountViews.Checked = settings.accountinfo.views;
            }
            textBoxAccountCheck.Text            = Functions.ConvertAlphaNum(this.settings.checkaccount);
            
            try {
                
                Debug.WriteLineIf(GlobalVar.DEBUG, "PUTSETTINGSINTOFORM: Fill bsStream and dgvStreams with settings");
                // rebind BindingSource to make sure it is bound.. designer shows it bound but does not when this point in code is reached...
                bsStreams.DataSource = settings.streams;
                dgvStreams.DataSource = bsStreams;
                
                dgvStreams.AutoGenerateColumns = false; // columns are set at designtime
                dgvStreams.Refresh();
            } catch (Exception ex) {
                Log.Add("PutSettingsIntoForm>" + ex.Message);
            }
            
        }
        
        /// <summary>
        /// Get all streams an account is following, requires auth token!
        /// </summary>
        /// <param name="sAccount">Stream account to check</param>
        /// <returns>Returns true if successfull</returns>
        public bool GetAccountFollowings(string sAccount)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "GETACCOUNTFOLLOWINGS: Parsing done");
            bool bSuccess = false;
            WebCheckResponse WebCheck;
            sAccount = Functions.ConvertAlphaNum(sAccount);
            string sBaseURL = "https://api.twitch.tv/kraken/streams/followed/";
            string sParams = "";
            string responseString = "";
            
            string sURL = sBaseURL + sAccount + sParams;
            
            WebCheck = Functions.DoWebRequest(sURL);
            if (WebCheck.HttpWResp.StatusCode == HttpStatusCode.OK) {
                using (Stream stream = WebCheck.HttpWResp.GetResponseStream()) {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    responseString = reader.ReadToEnd();
                }
                
                dynamic data = JsonConvert.DeserializeObject(responseString);
                
                //MessageBox.Show(data.ToString());
            }
            
            
            return bSuccess;
        }
        
        /// <summary>
        /// Open Feedback links
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
    }
    
    /// <summary>
    /// Class to change left-right Padding of DataGridViewButton
    /// </summary>
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
