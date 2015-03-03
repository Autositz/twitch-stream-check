/*
 * Created by SharpDevelop.
 * User: Autositz
 * Date: 01.03.2015
 * Time: 00:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

namespace twitch_stream_check
{
    /// <summary>
    /// Class with program entry point.
    /// </summary>
    internal sealed class Program
    {
        /// <summary>
        /// Program entry point.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // prevent form from being displayed at startup
            MainForm objForm = new MainForm();
            
            // if we don not have our JSON.NET .dll we gotta use the embedded one
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            
            Application.Run(objForm);
            
        }
        
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
        
        static string[] GetListOfEmbeddedResources()
        {
            return  Assembly.GetExecutingAssembly().GetManifestResourceNames();
        }
        
    }
}
