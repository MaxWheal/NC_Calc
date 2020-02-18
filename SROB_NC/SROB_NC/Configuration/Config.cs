using Configuration.Ini;
using Configuration.Parameters;
using Configuration.RestrictiveAreas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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



        #endregion

        #region Methods

        #region Initialize
        /// <summary>
        /// Sets up <see cref="Config"/> 
        /// </summary>
        /// <param name="path"></param>
        public static void Initialize(string iniPath)
        {
            Ini = new IniCollection(iniPath + "NC_Calc.ini");

            ResAreas = new ResAreaCollection(iniPath + Ini.Values["GeoDat"]);

            Params = new ParameterCollecion(iniPath + Ini.Values["Parameter"]);
        }
        #endregion

        #endregion


    }
}
