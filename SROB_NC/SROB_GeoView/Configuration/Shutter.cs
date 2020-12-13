using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Geometries;

namespace Configuration
{
    /// <summary>
    /// Collection of restricitve areas
    /// </summary>
    [XmlRoot("ArrayOfSchaler")]
    public class ShutterCollecion
    {
        #region Constructors
        public ShutterCollecion()
        {

        }

        public ShutterCollecion(string path)
        {
            ReadFromXML(path);

            Shutters?.Clear();

            Shutters = ShutterList.ToDictionary(x => x.Key, x => x);
        }

        #endregion

        #region Properties

        [XmlElement("Schaler")]
        public List<Shutter> ShutterList = new List<Shutter>();

        [XmlIgnore]
        public Dictionary<int, Shutter> Shutters = new Dictionary<int, Shutter>();

        #endregion

        #region Methods

        #region ReadFromXML
        /// <summary>
        /// Reads XML and to instance object
        /// </summary>
        /// <param name="path">Absolute path of file to be read</param>
        private bool ReadFromXML(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ShutterCollecion));
            try
            {
                using (Stream reader = new FileStream(path, FileMode.Open))
                {
                    ShutterCollecion readConfig = (ShutterCollecion)serializer.Deserialize(reader);
                    ShutterList = readConfig.ShutterList;

                    return true;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"XML Shutter configuration at{path} could not be read.");
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
            XmlSerializer serializer = new XmlSerializer(typeof(ShutterCollecion));

            try
            {
                using (TextWriter writer = new StreamWriter(path))
                {
                    serializer.Serialize(writer, this);
                }
                return true;
            }

            catch (Exception)
            {
                Console.WriteLine($"XML Shutter configuration at{path} could not be written.");
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
            return $"{Shutters.Count} Shutters";
        }

        #endregion

        #endregion
    }

    public class Shutter
    {
        #region Constructors
        public Shutter()
        {

        }

        #endregion

        #region Properties

        [XmlAttribute("Kennung")]
        public int Key { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("Laenge")]
        public double Length { get; set; }

        [XmlElement("Breite")]
        public double Width { get; set; }

        [XmlElement("Hoehe")]
        public double Height { get; set; }

        [XmlIgnore]
        public Size Size
        {
            get => new Size(Length, Width, Height);
        }
        
        #endregion

        #region Methods

        #region ToString
        /// <summary>
        /// Returns a string to represent the current object.
        /// </summary>
        /// <returns></returns>

        public override string ToString()
        {
            return $"{Key} | {Name}";
        }
        #endregion

        #endregion
    }
}