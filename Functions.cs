/*
 * Hold different functions used by all classes
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace twitch_stream_check
{
    public static class Functions
    {
        /// <summary>
        /// Returns a printable list of an objects content
        /// </summary>
        /// <param name="obj">The object to print</param>
        /// <returns>String to print out</returns>
        public static string var_dump(object obj)
        {
            return var_dump(obj, 0);
        }
        
        /// <summary>
        /// Returns a printable list of an objects content
        /// <para>Taken from http://ruuddottech.blogspot.de/2009/07/php-vardump-method-for-c.html</para>
        /// <para>User: http://www.blogger.com/profile/13374242341618330246</para>
        /// <para>Date: 06.03.2015 13:41</para>
        /// </summary>
        /// <param name="obj">The object to print</param>
        /// <param name="recursion">At which level to start - usually at 0</param>
        /// <returns>String to print out</returns>
        public static string var_dump(object obj, int recursion)
        {
            StringBuilder result = new StringBuilder();
          
            // Protect the method against endless recursion
            if (recursion < 5)
            {
                // Determine object type
                Type t = obj.GetType();
          
                // Get array with properties for this object
                PropertyInfo[] properties = t.GetProperties();
          
                foreach (PropertyInfo property in properties)
                {
                    try
                    {
                        // Get the property value
                        object value = property.GetValue(obj, null);
          
                        // Create indenting string to put in front of properties of a deeper level
                        // We'll need this when we display the property name and value
                        string indent = String.Empty;
                        string spaces = "|  ";
                        string trail = "|..";
                          
                        if (recursion > 0)
                        {
                            indent = new StringBuilder(trail).Insert(0, spaces, recursion - 1).ToString();
                        }
          
                        if (value != null)
                        {
                            // If the value is a string, add quotation marks
                            string displayValue = value.ToString();
                            if (value is string) displayValue = String.Concat('"', displayValue, '"');
          
                            // Add property name and value to return string
                            result.AppendFormat("{0}{1} = {2}\n", indent, property.Name, displayValue);
          
                            try
                            {
                                if (!(value is ICollection))
                                {
                                    // Call var_dump() again to list child properties
                                    // This throws an exception if the current property value
                                    // is of an unsupported type (eg. it has not properties)
                                    result.Append(var_dump(value, recursion + 1));
                                }
                                else
                                {
                                    // 2009-07-29: added support for collections
                                    // The value is a collection (eg. it's an arraylist or generic list)
                                    // so loop through its elements and dump their properties
                                    int elementCount = 0;
                                    foreach (object element in ((ICollection)value))
                                    {
                                        string elementName = String.Format("{0}[{1}]", property.Name, elementCount);
                                        indent = new StringBuilder(trail).Insert(0, spaces, recursion).ToString();
          
                                        // Display the collection element name and type
                                        result.AppendFormat("{0}{1} = {2}\n", indent, elementName, element.ToString());
          
                                        // Display the child properties
                                        result.Append(var_dump(element, recursion + 2));
                                        elementCount++;
                                    }
          
                                    result.Append(var_dump(value, recursion + 1));
                                }
                            }
                            catch { }
                        }
                        else
                        {
                            // Add empty (null) property to return string
                            result.AppendFormat("{0}{1} = {2}\n", indent, property.Name, "null");
                        }
                    }
                    catch
                    {
                        // Some properties will throw an exception on property.GetValue()
                        // I don't know exactly why this happens, so for now i will ignore them...
                    }
                }
            }
          
            return result.ToString();
        }
        
        /// <summary>
        /// Tries to get more detailed info on the exception
        /// </summary>
        /// <param name="e">Supply the Exception that has occured.</param>
        /// <returns>Returns object: 0 Line number, 1 Column number, 2 Filename, 3 Methodname</returns>
        public static object[] ParseException(Exception ex)
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
        
        /// <summary>
        /// Convert the settings dialog data to settings save data.
        /// </summary>
        /// <param name="s">String with linefeeds to be converted</param>
        /// <returns>String Array</returns>
        public static String[] convertLFtoArray(String s)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "CONVERTLFTOARRAY: Convert a string with newlines to an array that splits at newlines");
            return s.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
        }
        
        /// <summary>
        /// Convert the settings savefile data to settings dialog output.
        /// </summary>
        /// <param name="a">String Array to be merged</param>
        /// <returns>String with line feeds</returns>
        public static String convertArraytoLF(String[] a)
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
        public static string convertAlphaNum(string s, bool preserveNewline= false)
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
        public static int convertNum(string s, int iFallback = 0)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "CONVERTNUM: Converting " + s + " with fallback: " + iFallback);
            int i = 0;
            if (!Int32.TryParse(s, out i)) {
                i = iFallback;
            }
            
            return i;
        }
        
        public static int GetNthIndex(string sText, string sSearch, int iOccurred)
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
        public static int GetNthIndex(string sText, string sSearch, int iOccurred, int iPos, int iRunning)
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
        
        /// <summary>
        /// Get data from an URL
        /// </summary>
        /// <param name="sURL">URL to contact</param>
        /// <returns>Returns true if request was successfull</returns>
        public static WebCheckResponse doWebRequest(string sURL)
        {
            WebCheckResponse WebCheck = new WebCheckResponse();
            
            try {
                Debug.WriteLineIf(GlobalVar.DEBUG, "DOWEBREQUEST: Making a new Web Request");
                HttpWebRequest HttpWReq = (HttpWebRequest)WebRequest.Create(sURL);
                Debug.WriteLineIf(GlobalVar.DEBUG, "DOWEBREQUEST: Putting response into var");
                WebCheck.HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();
                Debug.WriteLineIf(GlobalVar.DEBUG, "DOWEBREQUEST: Response stored for future use");
                
                if (WebCheck.HttpWResp.StatusCode == HttpStatusCode.OK) {
                }
                    
            } catch (Exception ex) {
                Logging Log = new Logging();
                Log.Add("doWebRequest>" + ex.Message);
            }
            
            return WebCheck;
        }
    }
    
    /// <summary>
    /// Class to handle settings load and save
    /// </summary>
    public class AppSettings<T> where T : new()
    {
        private const string DEFAULT_FILENAME = "twitch-stream-check-settings.json";
        
        public void Save(object obj)
        {
            File.WriteAllText("bla.json", JsonConvert.SerializeObject(obj));
        }
        /// <summary>
        /// Store settings into file
        /// </summary>
        /// <param name="fileName">Configfile</param>
        public void Save(string fileName = DEFAULT_FILENAME)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "SETTINGS:SAVE: Data save initialized - using self");
            File.WriteAllText(fileName, JsonConvert.SerializeObject(this));
        }
        
        /// <summary>
        /// Store supplied settings into file
        /// </summary>
        /// <param name="pSettings">Settings class</param>
        /// <param name="fileName">Configfile</param>
        public static void Save(T pSettings, string fileName = DEFAULT_FILENAME)
        {
            Debug.WriteLineIf(GlobalVar.DEBUG, "SETTINGS:SAVE: Data save initialized - with given object");
            File.WriteAllText(fileName, JsonConvert.SerializeObject(pSettings));
        }
        
        /// <summary>
        /// Load settings from config file
        /// </summary>
        /// <param name="fileName">Configfile</param>
        /// <returns>Returns the settings class with loaded settings</returns>
        public static T Load(string fileName = DEFAULT_FILENAME)
        {
            // TODO: Check if loaded data matches current settings in datatypes?
            Debug.WriteLineIf(GlobalVar.DEBUG, "SETTINGS:LOAD: Data load initialized");
            T t = new T();
            try {
                if (File.Exists(fileName)) {
                    Debug.WriteLineIf(GlobalVar.DEBUG, "SETTINGS:LOAD: Convert our loaded data into an useable object");
                    t = JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName));
                }
            } catch (Exception ex) {
                Debug.WriteLineIf(GlobalVar.DEBUG, "!EXCEPTION!:SETTINGS:LOAD: Something went wrong during load: " + ex.Message);
                MessageBox.Show("Error loading settings." + Environment.NewLine + "Using default values", "Info", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                Logging Log = new Logging();
                Log.Add("AppSettings:Load>" + ex.Message);
            }
            return t;
        }
    }
    
    /// <summary>
    /// Log to file
    /// </summary>
    public class Logging
    {
        /// <summary>
        /// Name of current active application
        /// </summary>
        private string sAppname;
        
        public Logging()
        {
            // get filename to use for logging
            try {
                sAppname = Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            } catch (Exception) {
                sAppname = "twitch-stream-check";
            }
        }
        
        /// <summary>
        /// Write content to file
        /// </summary>
        /// <param name="sLine">What to write</param>
        public void Add(string sLine)
        {
            sLine = DateTime.Now.ToString("yyMMdd-HHmmss: ") + sLine;
            // add to existing file or create new with the application name
            try {
                StreamWriter file = new StreamWriter(sAppname + ".log", true);
                file.WriteLine(sLine);
                file.Close();
            } catch (Exception ex) {
                // show the user that there was an error and we were also unable to write to the error log
                MessageBox.Show(sLine + Environment.NewLine + Environment.NewLine +
                                "Error while trying to write to error log:" + Environment.NewLine + ex.Message + Environment.NewLine + Environment.NewLine +
                                "Application will now exit!" + Environment.NewLine + Environment.NewLine +
                                "Please report this error at" + Environment.NewLine + GlobalVar.PROJECTURL, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                Environment.Exit(0);
            }
        }
    }
    
    /// <summary>
    /// Class for each single stream entry
    /// </summary>
    public class WebCheckResponse
    {
        public bool bSuccess { get; set; }
        public HttpWebResponse HttpWResp { get; set; }
        
        public WebCheckResponse()
        {
            bSuccess = false;
            HttpWResp = null;
        }
    }
    
    /// <summary>
    /// Holds static global variables
    /// </summary>
    public static class GlobalVar
    {
        public const bool DEBUG = true; // enable or disable debug messages
        public const bool GENERATEDATA = true; // generate random data (add/remove entries randomly)
        public const string PROJECTURL = "https://github.com/Autositz/twitch-stream-check/issues"; // link to the project page to report errors
    }
}