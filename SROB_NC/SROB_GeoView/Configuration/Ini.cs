using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

namespace Configuration.Ini
{
    /// <summary>
    /// Collection of configuration Strings
    /// </summary>
    public class IniCollection
    {
        #region Contructors
        public IniCollection()
        {

        }

        public IniCollection(string path)
        {
            _iniPath = path;
            ReadFromXML(path);
        }
        #endregion

        #region Members

        public Dictionary<string, string> Values;
        private string _iniPath = "";

        #endregion

        #region Methods

        #region ReadFromXML
        /// <summary>
        /// Reads XML to instance
        /// </summary>
        /// <param name="path">Absolute path of file to be read</param>
        public bool ReadFromXML(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<ConfigStrings>));
            try
            {
                using (Stream reader = new FileStream(path, FileMode.Open))
                {
                    Values?.Clear();
                    Values = ((List<ConfigStrings>)serializer.Deserialize(reader)).ToDictionary(x => x.Key, x => x.Value);

                    return true;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"XML Configuraion Strings at{path} could not be read.");
                return false;
            }

        }
        #endregion

        #region WriteToXML
        /// <summary>
        /// Write object to XML File spezified by path
        /// </summary>
        /// <param name="path">Absolute path of file to be written</param>
        /// <returns></returns>
        public bool WriteToXML(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<ConfigStrings>));

            try
            {
                using (TextWriter writer = new StreamWriter(path))
                {
                    List<ConfigStrings> toWriteConfig = new List<ConfigStrings>();

                    foreach (var keyValuePair in Values)
                        toWriteConfig.Add(new ConfigStrings(keyValuePair.Key, keyValuePair.Value));

                    serializer.Serialize(writer, toWriteConfig);
                }
                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"XML Configuraion Strings at{path} could not be written.");
                return false;
            }
        }

        #endregion

        #region ToString
        /// <summary>
        /// Returns a string to represent the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Values.Count} KeyValue Pairs";
        }

        #endregion

        #endregion

    }

    public class ConfigStrings
    {
        #region Contructors
        public ConfigStrings()
        {

        }

        public ConfigStrings(string key, string value)
        {
            Key = key;
            Value = value;
        }

        #endregion

        #region Properties
        [XmlAttribute]
        public string Key { get; set; }

        [XmlAttribute]
        public string Value { get; set; }
        #endregion

        #region Methods

        #region ToString

        /// <summary>
        /// Returns a string to represent the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Key}: {Value}";
        }

        #endregion

        #endregion

    }

}
