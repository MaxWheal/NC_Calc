﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Geometries;

namespace Configuration.Parameters
{
    /// <summary>
    /// Collection of restricitve areas
    /// </summary>
    [XmlRoot("ParameterValues")]
    public class ParameterCollecion
    {
        #region Constructors
        public ParameterCollecion()
        {

        }

        public ParameterCollecion(string path)
        {
            ReadFromXML(path);

            Values?.Clear();

            foreach (var parameter in ParameterList)
            {
                if (parameter.Dimensions.Count() > 1)
                {
                    foreach (var dimension in parameter.Dimensions)
                    {
                        Values[$"{parameter.Key}[{dimension.Index}]"] = double.Parse(dimension.Values.Value.Replace(".", ","));
                    }
                }

                else
                    Values[$"{parameter.Key}"] = double.Parse(parameter.Dimensions[0].Values.Value.Replace(".", ","));

            }
       }

        #endregion

        #region Properties

        [XmlElement("Parameter")]
        public List<ParameterValues> ParameterList = new List<ParameterValues>();

        [XmlIgnore]
        public Dictionary<string, double> Values = new Dictionary<string, double>();

        #endregion

        #region Methods

        #region ReadFromXML
        /// <summary>
        /// Reads XML and to instance object
        /// </summary>
        /// <param name="path">Absolute path of file to be read</param>
        private bool ReadFromXML(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ParameterCollecion));
            try
            {
                using (Stream reader = new FileStream(path, FileMode.Open))
                {
                    ParameterCollecion readConfig = (ParameterCollecion)serializer.Deserialize(reader);
                    ParameterList = readConfig.ParameterList;

                    return true;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"XML Room configuration at{path} could not be read.");
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
            XmlSerializer serializer = new XmlSerializer(typeof(ParameterCollecion));

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
                Console.WriteLine($"XML Room configuration at{path} could not be written.");
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
            return $"{Values.Count} Parameters";
        }

        #endregion

        #region Get a 4D Point from the Parameters

        /// <summary>
        /// Gets the 4D from the Parameter Values.
        /// </summary>
        /// <param name="key">The key of the Parameter.</param>
        /// <returns>The 4D Point.</returns>
        public static Point_4D GetPoint4D(string key)
        {
            Point_4D point4D = new Point_4D();
            
            point4D.X = Config.Params.Values[$"{key}[0]"];
            point4D.Y = Config.Params.Values[$"{key}[1]"];
            point4D.Z = Config.Params.Values[$"{key}[2]"];
            point4D.C = Config.Params.Values[$"{key}[3]"];

            return point4D;
        }

        #endregion

        #endregion

    }

    [Serializable()]
    [XmlRoot("Parameter")]
    public class ParameterValues
    {
        [XmlAttribute("Name")]
        public string Key { get; set; }

        //[XmlElement("Values")]
        //public Values Values { get; set; }

        [XmlElement("Dimension")]
        public List<Dimension> Dimensions { get; set; }

        public ParameterValues()
        {
        }

        public override string ToString()
        {
            if (Dimensions.Count > 1)
                return $"{Key}: {Dimensions.Count} Dimensions";

            else
                return $"{Key}: {Dimensions[0].Values.Value} {Dimensions[0].Values.Unit}";
        }
    }

    [Serializable()]
    public class Dimension
    {
        [XmlAttribute("Index")]
        public string Index { get; set; }

        [XmlElement("Values")]
        public Values Values { get; set; }

        public Dimension()
        {
        }

        public override string ToString()
        {
                return $"{Values.Value} {Values.Unit}";
        }

    }

    [Serializable()]
    public class Values
    {
        [XmlAttribute("Value")]
        public string Value { get; set; }

        [XmlAttribute("Unit")]
        public string Unit { get; set; }

        [XmlAttribute("Min")]
        public string Min { get; set; }

        [XmlAttribute("Max")]
        public string Max { get; set; }


        public Values()
        {
        }
    }
}
