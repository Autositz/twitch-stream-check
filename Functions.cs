/*
 * Hold different functions used by all classes
 */


using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Collections.Generic;

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