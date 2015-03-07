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
        private System.Data.DataSet dsStreams;
        private System.Windows.Forms.DataGridViewCheckBoxColumn bImportant;
        private System.Windows.Forms.DataGridViewTextBoxColumn sStream;
        private System.Windows.Forms.Button buttonSettingsAddStream;
        
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
            this.bImportant = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.sStream = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dsStreams = new System.Data.DataSet();
            this.buttonSettingsAddStream = new System.Windows.Forms.Button();
            this.MyMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStreams)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsStreams)).BeginInit();
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
            this.MyMenu.Size = new System.Drawing.Size(153, 98);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem1.Text = "Settings";
            this.toolStripMenuItem1.ToolTipText = "Configure program behavior";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.SettingsToolStripMenuItem1Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.ToolTipText = "Tell me more";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItemClick);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(13, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 71);
            this.label1.TabIndex = 1;
            this.label1.Text = "Enter twitch usernames you wish to check\r\n! = Important stream will be checked mo" +
    "re often\r\n(not implemented yet)";
            // 
            // buttonSettingsGetUsers
            // 
            this.buttonSettingsGetUsers.Location = new System.Drawing.Point(13, 320);
            this.buttonSettingsGetUsers.Name = "buttonSettingsGetUsers";
            this.buttonSettingsGetUsers.Size = new System.Drawing.Size(75, 23);
            this.buttonSettingsGetUsers.TabIndex = 3;
            this.buttonSettingsGetUsers.Text = "Get users";
            this.buttonSettingsGetUsers.UseVisualStyleBackColor = true;
            this.buttonSettingsGetUsers.Click += new System.EventHandler(this.ButtonSettingsGetUsersClick);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(13, 346);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(232, 42);
            this.label2.TabIndex = 4;
            this.label2.Text = "Click the \"Get users\" button to fill the list with usernames which \"Account\" is c" +
    "urrently following";
            // 
            // textBoxAccountCheck
            // 
            this.textBoxAccountCheck.Location = new System.Drawing.Point(94, 322);
            this.textBoxAccountCheck.Name = "textBoxAccountCheck";
            this.textBoxAccountCheck.Size = new System.Drawing.Size(150, 20);
            this.textBoxAccountCheck.TabIndex = 5;
            this.textBoxAccountCheck.Text = "Account";
            // 
            // comboBoxInterval
            // 
            this.comboBoxInterval.FormattingEnabled = true;
            this.comboBoxInterval.Items.AddRange(new object[] {
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
            "15",
            "20",
            "25",
            "30",
            "45",
            "60"});
            this.comboBoxInterval.Location = new System.Drawing.Point(324, 130);
            this.comboBoxInterval.Name = "comboBoxInterval";
            this.comboBoxInterval.Size = new System.Drawing.Size(41, 21);
            this.comboBoxInterval.TabIndex = 6;
            this.comboBoxInterval.Text = "3";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(324, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(156, 71);
            this.label3.TabIndex = 7;
            this.label3.Text = "Interval at which streams are checked (m)\r\nKeep in mind that the timer is RESET e" +
    "verytime the settings are saved!";
            // 
            // buttonSettingsOK
            // 
            this.buttonSettingsOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonSettingsOK.Location = new System.Drawing.Point(324, 399);
            this.buttonSettingsOK.Name = "buttonSettingsOK";
            this.buttonSettingsOK.Size = new System.Drawing.Size(75, 23);
            this.buttonSettingsOK.TabIndex = 8;
            this.buttonSettingsOK.Text = "OK";
            this.buttonSettingsOK.UseVisualStyleBackColor = true;
            this.buttonSettingsOK.Click += new System.EventHandler(this.ButtonSettingsOKClick);
            // 
            // buttonSettingsCANCEL
            // 
            this.buttonSettingsCANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonSettingsCANCEL.Location = new System.Drawing.Point(405, 399);
            this.buttonSettingsCANCEL.Name = "buttonSettingsCANCEL";
            this.buttonSettingsCANCEL.Size = new System.Drawing.Size(75, 23);
            this.buttonSettingsCANCEL.TabIndex = 9;
            this.buttonSettingsCANCEL.Text = "CANCEL";
            this.buttonSettingsCANCEL.UseVisualStyleBackColor = true;
            this.buttonSettingsCANCEL.Click += new System.EventHandler(this.ButtonSettingsCANCELClick);
            // 
            // linkLabelFeedback
            // 
            this.linkLabelFeedback.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.linkLabelFeedback.Location = new System.Drawing.Point(13, 388);
            this.linkLabelFeedback.Name = "linkLabelFeedback";
            this.linkLabelFeedback.Size = new System.Drawing.Size(232, 34);
            this.linkLabelFeedback.TabIndex = 10;
            this.linkLabelFeedback.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkLabelFeedback.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelFeedback_LinkClicked);
            // 
            // dgvStreams
            // 
            this.dgvStreams.AllowUserToResizeColumns = false;
            this.dgvStreams.AllowUserToResizeRows = false;
            this.dgvStreams.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvStreams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStreams.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.bImportant,
            this.sStream});
            this.dgvStreams.Location = new System.Drawing.Point(13, 130);
            this.dgvStreams.MultiSelect = false;
            this.dgvStreams.Name = "dgvStreams";
            this.dgvStreams.RowHeadersVisible = false;
            this.dgvStreams.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvStreams.Size = new System.Drawing.Size(305, 183);
            this.dgvStreams.TabIndex = 11;
            // 
            // bImportant
            // 
            this.bImportant.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.bImportant.DataPropertyName = "bImportant";
            this.bImportant.HeaderText = "!";
            this.bImportant.MinimumWidth = 15;
            this.bImportant.Name = "bImportant";
            this.bImportant.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.bImportant.ToolTipText = "Is this an important stream and should be monitored seperatly?";
            this.bImportant.Width = 21;
            // 
            // sStream
            // 
            this.sStream.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.sStream.DataPropertyName = "sStreamname";
            this.sStream.HeaderText = "Stream names";
            this.sStream.MaxInputLength = 250;
            this.sStream.MinimumWidth = 50;
            this.sStream.Name = "sStream";
            this.sStream.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.sStream.ToolTipText = "Name of the stream to follow";
            // 
            // dsStreams
            // 
            this.dsStreams.DataSetName = "NewDataSet";
            // 
            // buttonSettingsAddStream
            // 
            this.buttonSettingsAddStream.AutoSize = true;
            this.buttonSettingsAddStream.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonSettingsAddStream.Location = new System.Drawing.Point(13, 103);
            this.buttonSettingsAddStream.Name = "buttonSettingsAddStream";
            this.buttonSettingsAddStream.Size = new System.Drawing.Size(72, 23);
            this.buttonSettingsAddStream.TabIndex = 12;
            this.buttonSettingsAddStream.Text = "Add Stream";
            this.buttonSettingsAddStream.UseVisualStyleBackColor = true;
            this.buttonSettingsAddStream.Click += new System.EventHandler(this.ButtonSettingsAddStreamClick);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonSettingsCANCEL;
            this.ClientSize = new System.Drawing.Size(684, 434);
            this.Controls.Add(this.buttonSettingsAddStream);
            this.Controls.Add(this.dgvStreams);
            this.Controls.Add(this.linkLabelFeedback);
            this.Controls.Add(this.buttonSettingsCANCEL);
            this.Controls.Add(this.buttonSettingsOK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxInterval);
            this.Controls.Add(this.textBoxAccountCheck);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonSettingsGetUsers);
            this.Controls.Add(this.label1);
            this.Icon = this.notifyIcon1.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.MyMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStreams)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsStreams)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
