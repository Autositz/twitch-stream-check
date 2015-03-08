﻿/*
 * Created by SharpDevelop.
 * User: Autositz
 * Date: 01.03.2015
 * Time: 00:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace twitch_stream_check
{
    partial class SettingsForm
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip MyMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonSettingsGetUsers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxInterval;
        private System.Windows.Forms.Button buttonSettingsCANCEL;
        private System.Windows.Forms.Button buttonSettingsOK;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.TextBox textBoxAccountCheck;
        private System.Windows.Forms.LinkLabel linkLabelFeedback;
        private System.Windows.Forms.DataGridView dgvStreams;
        private twDataGridViewButtonColumn Delete;
        private System.Windows.Forms.DataGridViewCheckBoxColumn bImportant;
        private System.Windows.Forms.DataGridViewTextBoxColumn sStreamname;
        private System.Windows.Forms.BindingSource bsStreams;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxAccountToken;
        private System.Windows.Forms.GroupBox groupBoxStreamSettings;
        private System.Windows.Forms.GroupBox groupBoxChannelsFollowing;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBoxIntervalSettings;
        private System.Windows.Forms.ComboBox comboBoxIntervalImportant;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        
        /// <summary>
        /// Disposes resources used by the form.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.MyMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonSettingsGetUsers = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxAccountCheck = new System.Windows.Forms.TextBox();
            this.comboBoxInterval = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonSettingsOK = new System.Windows.Forms.Button();
            this.buttonSettingsCANCEL = new System.Windows.Forms.Button();
            this.linkLabelFeedback = new System.Windows.Forms.LinkLabel();
            this.dgvStreams = new System.Windows.Forms.DataGridView();
            this.Delete = new twitch_stream_check.twDataGridViewButtonColumn();
            this.bImportant = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.sStreamname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bsStreams = new System.Windows.Forms.BindingSource(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxAccountToken = new System.Windows.Forms.TextBox();
            this.groupBoxStreamSettings = new System.Windows.Forms.GroupBox();
            this.groupBoxChannelsFollowing = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBoxIntervalSettings = new System.Windows.Forms.GroupBox();
            this.comboBoxIntervalImportant = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.MyMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStreams)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsStreams)).BeginInit();
            this.groupBoxStreamSettings.SuspendLayout();
            this.groupBoxChannelsFollowing.SuspendLayout();
            this.groupBoxIntervalSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.MyMenu;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Twitch Stream Checker";
            this.notifyIcon1.Visible = true;
            // 
            // MyMenu
            // 
            this.MyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.toolStripMenuItem1,
            this.aboutToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.MyMenu.Name = "notifyIconMenu";
            this.MyMenu.Size = new System.Drawing.Size(117, 76);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(113, 6);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(116, 22);
            this.toolStripMenuItem1.Text = "Settings";
            this.toolStripMenuItem1.ToolTipText = "Configure program behavior";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.ToolStripMenuItemClickSettings);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.ToolTipText = "Tell me more";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItemClickAbout);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItemClickExit);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(364, 42);
            this.label1.TabIndex = 1;
            this.label1.Text = "Enter twitch usernames you wish to check\r\n! = Important stream will be checked mo" +
    "re often\r\n(not implemented yet)";
            // 
            // buttonSettingsGetUsers
            // 
            this.buttonSettingsGetUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSettingsGetUsers.Location = new System.Drawing.Point(295, 11);
            this.buttonSettingsGetUsers.Name = "buttonSettingsGetUsers";
            this.buttonSettingsGetUsers.Size = new System.Drawing.Size(75, 23);
            this.buttonSettingsGetUsers.TabIndex = 3;
            this.buttonSettingsGetUsers.Text = "Get users";
            this.buttonSettingsGetUsers.UseVisualStyleBackColor = true;
            this.buttonSettingsGetUsers.Click += new System.EventHandler(this.ButtonSettingsClickGetUsers);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(283, 41);
            this.label2.TabIndex = 4;
            this.label2.Text = "Click the \"Get users\" button to fill the list with usernames which \"Account\" is c" +
    "urrently following.\r\nExisting list will be overwritten!";
            // 
            // textBoxAccountCheck
            // 
            this.textBoxAccountCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxAccountCheck.Location = new System.Drawing.Point(61, 60);
            this.textBoxAccountCheck.Name = "textBoxAccountCheck";
            this.textBoxAccountCheck.Size = new System.Drawing.Size(109, 20);
            this.textBoxAccountCheck.TabIndex = 5;
            this.textBoxAccountCheck.Text = "Account";
            // 
            // comboBoxInterval
            // 
            this.comboBoxInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBoxInterval.FormatString = "N0";
            this.comboBoxInterval.FormattingEnabled = true;
            this.comboBoxInterval.Items.AddRange(new object[] {
            "5",
            "10",
            "15",
            "20",
            "25",
            "30",
            "45",
            "60"});
            this.comboBoxInterval.Location = new System.Drawing.Point(55, 81);
            this.comboBoxInterval.Name = "comboBoxInterval";
            this.comboBoxInterval.Size = new System.Drawing.Size(41, 21);
            this.comboBoxInterval.TabIndex = 6;
            this.comboBoxInterval.Text = "3";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(258, 42);
            this.label3.TabIndex = 7;
            this.label3.Text = "Interval at which streams are checked (m)\r\nKeep in mind that the timer is RESET e" +
    "verytime the settings are saved!";
            // 
            // buttonSettingsOK
            // 
            this.buttonSettingsOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSettingsOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonSettingsOK.Location = new System.Drawing.Point(482, 380);
            this.buttonSettingsOK.Name = "buttonSettingsOK";
            this.buttonSettingsOK.Size = new System.Drawing.Size(75, 23);
            this.buttonSettingsOK.TabIndex = 8;
            this.buttonSettingsOK.Text = "SAVE";
            this.buttonSettingsOK.UseVisualStyleBackColor = true;
            this.buttonSettingsOK.Click += new System.EventHandler(this.ButtonSettingsClickSAVE);
            // 
            // buttonSettingsCANCEL
            // 
            this.buttonSettingsCANCEL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSettingsCANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonSettingsCANCEL.Location = new System.Drawing.Point(563, 380);
            this.buttonSettingsCANCEL.Name = "buttonSettingsCANCEL";
            this.buttonSettingsCANCEL.Size = new System.Drawing.Size(75, 23);
            this.buttonSettingsCANCEL.TabIndex = 9;
            this.buttonSettingsCANCEL.Text = "CANCEL";
            this.buttonSettingsCANCEL.UseVisualStyleBackColor = true;
            this.buttonSettingsCANCEL.Click += new System.EventHandler(this.ButtonSettingsClickCANCEL);
            // 
            // linkLabelFeedback
            // 
            this.linkLabelFeedback.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabelFeedback.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
            this.linkLabelFeedback.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.linkLabelFeedback.Location = new System.Drawing.Point(0, 381);
            this.linkLabelFeedback.Name = "linkLabelFeedback";
            this.linkLabelFeedback.Size = new System.Drawing.Size(376, 34);
            this.linkLabelFeedback.TabIndex = 10;
            this.linkLabelFeedback.Text = "Send Feedback via eMail\r\nSend Feedback via Steam";
            this.linkLabelFeedback.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkLabelFeedback.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelFeedback_LinkClicked);
            // 
            // dgvStreams
            // 
            this.dgvStreams.AllowUserToResizeColumns = false;
            this.dgvStreams.AllowUserToResizeRows = false;
            this.dgvStreams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvStreams.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvStreams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStreams.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Delete,
            this.bImportant,
            this.sStreamname});
            this.dgvStreams.Location = new System.Drawing.Point(6, 89);
            this.dgvStreams.MultiSelect = false;
            this.dgvStreams.Name = "dgvStreams";
            this.dgvStreams.RowHeadersVisible = false;
            this.dgvStreams.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvStreams.Size = new System.Drawing.Size(364, 183);
            this.dgvStreams.TabIndex = 11;
            this.dgvStreams.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvStreams_CellContentClick);
            this.dgvStreams.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvStreams_CellEndEdit);
            this.dgvStreams.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgvStreams_CellValidating);
            // 
            // Delete
            // 
            this.Delete.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Delete.DefaultCellStyle = dataGridViewCellStyle1;
            this.Delete.Frozen = true;
            this.Delete.HeaderText = "";
            this.Delete.MinimumWidth = 10;
            this.Delete.Name = "Delete";
            this.Delete.ReadOnly = true;
            this.Delete.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Delete.Text = "X";
            this.Delete.ToolTipText = "Delete Stream from list";
            this.Delete.UseColumnTextForButtonValue = true;
            this.Delete.Width = 21;
            // 
            // bImportant
            // 
            this.bImportant.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.bImportant.DataPropertyName = "bImportant";
            this.bImportant.HeaderText = "!";
            this.bImportant.MinimumWidth = 10;
            this.bImportant.Name = "bImportant";
            this.bImportant.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.bImportant.ToolTipText = "Is this an important stream and should be monitored seperatly?";
            this.bImportant.Width = 21;
            // 
            // sStreamname
            // 
            this.sStreamname.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.sStreamname.DataPropertyName = "sStreamname";
            this.sStreamname.HeaderText = "Stream names";
            this.sStreamname.MaxInputLength = 250;
            this.sStreamname.MinimumWidth = 50;
            this.sStreamname.Name = "sStreamname";
            this.sStreamname.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.sStreamname.ToolTipText = "Name of the stream to follow";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label4.Location = new System.Drawing.Point(6, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(364, 27);
            this.label4.TabIndex = 12;
            this.label4.Text = "Changed Streams will be used as long as the program is running but will only be s" +
    "tored when Save is used!";
            // 
            // textBoxAccountToken
            // 
            this.textBoxAccountToken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAccountToken.Location = new System.Drawing.Point(270, 60);
            this.textBoxAccountToken.Name = "textBoxAccountToken";
            this.textBoxAccountToken.Size = new System.Drawing.Size(100, 20);
            this.textBoxAccountToken.TabIndex = 13;
            this.textBoxAccountToken.Text = "Token";
            // 
            // groupBoxStreamSettings
            // 
            this.groupBoxStreamSettings.Controls.Add(this.dgvStreams);
            this.groupBoxStreamSettings.Controls.Add(this.label1);
            this.groupBoxStreamSettings.Controls.Add(this.label4);
            this.groupBoxStreamSettings.Location = new System.Drawing.Point(0, 0);
            this.groupBoxStreamSettings.Name = "groupBoxStreamSettings";
            this.groupBoxStreamSettings.Size = new System.Drawing.Size(376, 278);
            this.groupBoxStreamSettings.TabIndex = 14;
            this.groupBoxStreamSettings.TabStop = false;
            this.groupBoxStreamSettings.Text = "Stream Settings";
            // 
            // groupBoxChannelsFollowing
            // 
            this.groupBoxChannelsFollowing.Controls.Add(this.label6);
            this.groupBoxChannelsFollowing.Controls.Add(this.label5);
            this.groupBoxChannelsFollowing.Controls.Add(this.label2);
            this.groupBoxChannelsFollowing.Controls.Add(this.textBoxAccountCheck);
            this.groupBoxChannelsFollowing.Controls.Add(this.textBoxAccountToken);
            this.groupBoxChannelsFollowing.Controls.Add(this.buttonSettingsGetUsers);
            this.groupBoxChannelsFollowing.Location = new System.Drawing.Point(0, 285);
            this.groupBoxChannelsFollowing.Name = "groupBoxChannelsFollowing";
            this.groupBoxChannelsFollowing.Size = new System.Drawing.Size(376, 86);
            this.groupBoxChannelsFollowing.TabIndex = 15;
            this.groupBoxChannelsFollowing.TabStop = false;
            this.groupBoxChannelsFollowing.Text = "Channels Following";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.Location = new System.Drawing.Point(6, 63);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 17);
            this.label6.TabIndex = 15;
            this.label6.Text = "Account";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.Location = new System.Drawing.Point(203, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 17);
            this.label5.TabIndex = 14;
            this.label5.Text = "AuthToken";
            // 
            // groupBoxIntervalSettings
            // 
            this.groupBoxIntervalSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxIntervalSettings.Controls.Add(this.comboBoxIntervalImportant);
            this.groupBoxIntervalSettings.Controls.Add(this.label8);
            this.groupBoxIntervalSettings.Controls.Add(this.label7);
            this.groupBoxIntervalSettings.Controls.Add(this.label3);
            this.groupBoxIntervalSettings.Controls.Add(this.comboBoxInterval);
            this.groupBoxIntervalSettings.Location = new System.Drawing.Point(380, 0);
            this.groupBoxIntervalSettings.Name = "groupBoxIntervalSettings";
            this.groupBoxIntervalSettings.Size = new System.Drawing.Size(270, 108);
            this.groupBoxIntervalSettings.TabIndex = 16;
            this.groupBoxIntervalSettings.TabStop = false;
            this.groupBoxIntervalSettings.Text = "Interval Settings";
            // 
            // comboBoxIntervalImportant
            // 
            this.comboBoxIntervalImportant.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxIntervalImportant.FormatString = "N0";
            this.comboBoxIntervalImportant.FormattingEnabled = true;
            this.comboBoxIntervalImportant.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
            this.comboBoxIntervalImportant.Location = new System.Drawing.Point(223, 81);
            this.comboBoxIntervalImportant.Name = "comboBoxIntervalImportant";
            this.comboBoxIntervalImportant.Size = new System.Drawing.Size(41, 21);
            this.comboBoxIntervalImportant.TabIndex = 18;
            this.comboBoxIntervalImportant.Text = "3";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.Location = new System.Drawing.Point(163, 84);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(54, 17);
            this.label8.TabIndex = 17;
            this.label8.Text = "Important";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.Location = new System.Drawing.Point(6, 84);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 17);
            this.label7.TabIndex = 8;
            this.label7.Text = "Normal";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonSettingsCANCEL;
            this.ClientSize = new System.Drawing.Size(650, 415);
            this.Controls.Add(this.groupBoxIntervalSettings);
            this.Controls.Add(this.groupBoxChannelsFollowing);
            this.Controls.Add(this.groupBoxStreamSettings);
            this.Controls.Add(this.linkLabelFeedback);
            this.Controls.Add(this.buttonSettingsCANCEL);
            this.Controls.Add(this.buttonSettingsOK);
            this.Icon = this.notifyIcon1.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.MyMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStreams)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsStreams)).EndInit();
            this.groupBoxStreamSettings.ResumeLayout(false);
            this.groupBoxChannelsFollowing.ResumeLayout(false);
            this.groupBoxChannelsFollowing.PerformLayout();
            this.groupBoxIntervalSettings.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
