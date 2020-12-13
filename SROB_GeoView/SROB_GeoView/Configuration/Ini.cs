using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Configuration.Ini
{
    /// <summary>
    /// Collection of configuration Strings
    /// </summary>
    public class IniFile
    {
        #region Contructors
        public IniFile(string IniPath = null)
        {
            _path = new FileInfo(IniPath ?? EXE + ".ini").FullName;
        }
        #endregion

        #region Members

        private string _path;
        private string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        #endregion

        #region Methods

        #region Read
        /// <summary>
        /// Reads a string form the .ini File
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Section"></param>
        /// <returns></returns>
        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, _path);
            return RetVal.ToString();
        }
        #endregion

        #region Write
        /// <summary>
        /// Write to .ini File
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <param name="Section"></param>
        public void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(Section ?? EXE, Key, Value, _path);
        }
        #endregion

        #region Delete key
        /// <summary>
        /// Delets Key from .ini File
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Section"></param>
        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? EXE);
        }
        #endregion

        #region Delete section
        /// <summary>
        /// Delets Section from .ini File
        /// </summary>
        /// <param name="Section"></param>
        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? EXE);
        }
        #endregion

        #region KeyExists
        /// <summary>
        /// Checks if keyExists
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Section"></param>
        /// <returns></returns>
        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }
        #endregion

        #endregion

    }
}
