using Configuration.Ini;
using Configuration.Parameters;
using Configuration.RestrictiveAreas;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;


namespace Configuration
{
    public static class Config
    {
        #region Properties
        public static IniCollection Ini { get; set; }

        public static ResAreaCollection ResAreas { get; set; }

        public static ParameterCollecion Params { get; set; }

        public static ShutterCollecion Shutters { get; set; }

        #endregion

        #region Methods

        #region Initialize
        /// <summary>
        /// Sets up <see cref="Config"/> 
        /// </summary>
        /// <param name="path"></param>
        internal static void Initialize()
        {
            try
            {
                Ini = new IniCollection(CheckPathValid(Environment.CurrentDirectory + "/../NC_Calc.ini"));

                ResAreas = new ResAreaCollection(Ini.Values["GeoDat"] = CheckPathValid(Ini.Values["GeoDat"]));
                Params = new ParameterCollecion(Ini.Values["Parameter"] = CheckPathValid(Ini.Values["Parameter"]));
                Shutters = new ShutterCollecion(Ini.Values["Shutters"] = CheckPathValid(Ini.Values["Shutters"]));

                Ini.WriteToXML(Environment.CurrentDirectory + "/../NC_Calc.ini");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        #endregion

        #region CheckPathValid
        /// <summary>
        /// Checks a given string if file exists at location, if not opens FileDialog, returns new path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string CheckPathValid(string path, string initDir = "")
        {
            if (File.Exists(path))
                return path;

            var fDialog = new OpenFileDialog()
            {
                Title = $"File {path} could not be found.",
                RestoreDirectory = true
            };

            if (initDir.Length > 0)
                fDialog.InitialDirectory = Path.GetFullPath(initDir);

            if (fDialog.ShowDialog() == false)
                Application.Current.Shutdown();

            //return CheckPathValid(fDialog.FileName);
            return fDialog.FileName;
        }
        #endregion

        #region Reset

        public static void Reset()
        {
            Ini.Values["GeoDat"] = "GeoDat.cfg";
            Ini.Values["Parameter"] = "Parameter.xml";
            Ini.Values["Shutters"] = "Shutters.xml";

            Ini.WriteToXML(Environment.CurrentDirectory + "/../NC_Calc.ini");
        }

        #endregion

        #endregion


    }
}
