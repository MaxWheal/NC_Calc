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
    [XmlRoot("ConfigCollection")]
    public class IniCollection
    {
        #region Contructors
        public IniCollection()
        {

        }

        public IniCollection(string path)
        {
            ReadFromXML(path);

            Values?.Clear();
            Values = ValuesMem.ToDictionary(x => x.Key, x => x.Value);
        }

        #endregion

        #region Properties
        [XmlElement("Config_String")]
        public List<Ini> ValuesMem = new List<Ini>();

        [XmlIgnore]
        public Dictionary<string, string> Values;
        #endregion

        #region Methods

        #region ReadFromXML
        /// <summary>
        /// Reads XML to instance
        /// </summary>
        /// <param name="path">Absolute path of file to be read</param>
        private bool ReadFromXML(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(IniCollection));
            try
            {
                using (Stream reader = new FileStream(path, FileMode.Open))
                {
                    IniCollection readConfig = (IniCollection)serializer.Deserialize(reader);
                    ValuesMem = readConfig.ValuesMem;

                    return true;
                }
            }

            catch (Exception)
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
            XmlSerializer serializer = new XmlSerializer(typeof(IniCollection));

            try
            {
                using (TextWriter writer = new StreamWriter(path))
                {
                    serializer.Serialize(writer, this);
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
            return $"{ValuesMem.Count} KeyValue Pairs";
        }

        #endregion

        #endregion

    }

    public class Ini
    {
        #region Contructors
        public Ini()
        {

        }

        public Ini(string key, string value)
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
