﻿/*
 * Created by SharpDevelop.
 * User: Autositz
 * Date: 01.03.2015
 * Time: 00:19
 */
using System;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Threading;

namespace twitch_stream_check
{
    /// <summary>
    /// Class with program entry point.
    /// </summary>
    internal sealed class Program
    {
        static Mutex mRunning;
        
        /// <summary>
        /// Program entry point.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            bool bFirstRun = false; // we assume this is our first instance
            string sMutex = "twitchstreamcheck_354tfgw345rhfd54z"; // set a unique name
            try
            {
                // open a new mutex and get an exception if there is already one
                Mutex.OpenExisting(sMutex);
            }
            catch
            {
                Program.mRunning = new Mutex(true, sMutex);
                bFirstRun = true;
            }
            
            if (!bFirstRun) {
                Application.Exit();
                MessageBox.Show("Program is already running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            } else {
                
                // if we do not have our JSON.NET .dll we gotta use the embedded one
                // this has to be one of the earliest things...
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
                
                MainForm objMainForm = new MainForm();
                objMainForm.Hide();
                
                Application.Run();
                
                objMainForm.Dispose();
            }
            
        }
        
        /// <summary>
        /// Load embedded resources if no .dll is present
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // get a list of included resources for debugging
//            MessageBox.Show(GetListOfEmbeddedResources().Length.ToString());
//            MessageBox.Show(GetListOfEmbeddedResources()[0].ToString(), "0");
//            MessageBox.Show(GetListOfEmbeddedResources()[1].ToString(), "1");
//            MessageBox.Show(GetListOfEmbeddedResources()[2].ToString(), "2");
//            MessageBox.Show(GetListOfEmbeddedResources()[3].ToString(), "3");
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("twitch_stream_check.Newtonsoft.Json.dll"))
            {
                try {
                    Debug.WriteLineIf(GlobalVar.DEBUG, "!MISSINGDATA!: We have to use embedded Json.NET");
                    byte[] assemblyData = new byte[stream.Length];
                    Debug.WriteLineIf(GlobalVar.DEBUG, "!MISSINGDATA!: Reading data");
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    Debug.WriteLineIf(GlobalVar.DEBUG, "!MISSINGDATA!: Returning data");
                    return Assembly.Load(assemblyData);
                } catch (Exception e) {
                    Debug.WriteLineIf(GlobalVar.DEBUG, "!EXCEPTION!:!MISSINGDATA!: Something went wrong: " + e.Message);
                    throw;
                }
            }
        }
        
        /// <summary>
        /// Dummy to display embedded resources
        /// </summary>
        /// <returns></returns>
        static string[] GetListOfEmbeddedResources()
        {
            return  Assembly.GetExecutingAssembly().GetManifestResourceNames();
        }
        
    }
}
