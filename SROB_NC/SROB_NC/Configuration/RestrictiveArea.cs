﻿using System;
using System.Collections.Generic;
using Geometries;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using SROB_NC;

namespace Configuration.RestrictiveAreas
{
    /// <summary>
    /// Collection of restricitve areas
    /// </summary>
    [XmlRoot("ConfigCollection")]
    public class ResAreaCollection
    {
        #region Constructors
        public ResAreaCollection()
        {

        }

        public ResAreaCollection(string path)
        {
            if (path.Substring(path.Length - 3) == ".xml")
                ReadFromXML(path);
            else
                ReadFromCFG(path);
        }

        #endregion

        #region Properties

        [XmlArray("Restricted_Areas")]
        [XmlArrayItem("Area")]
        public List<RestrictiveArea> Areas = new List<RestrictiveArea>();

        #endregion

        #region Methods

        #region ReadFromXML
        /// <summary>
        /// Reads XML to instance
        /// </summary>
        /// <param name="path">Absolute path of file to be read</param>
        private bool ReadFromXML(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ResAreaCollection));
            try
            {
                using (Stream reader = new FileStream(path, FileMode.Open))
                {
                    ResAreaCollection readConfig = (ResAreaCollection)serializer.Deserialize(reader);
                    Areas = readConfig.Areas;

                    return true;
                }
            }

            catch (Exception)
            {
                Console.WriteLine($"XML Room configuration at{path} could not be read.");
                return false;
            }

        }
        #endregion

        #region ReadFromCFG
        /// <summary>
        /// Reads .cfg to instance
        /// </summary>
        /// <param name="path">Absolute path of file to be read</param>
        private bool ReadFromCFG(string path)
        {
            try
            {
                var lines = File.ReadAllText(path).Replace("\r\n", "").Split(new string[] { "OBST_DEF" }, StringSplitOptions.None);

                for (int i = 1; i < lines.Length; i++)
                {
                    var data = lines[i].Split('\t');

                    if (data[14] == "1") continue;
                    if (data[28] == "1") continue;
                    if (double.Parse(data[23]) > 0) continue;

                    var allowedMotion = Axis.None;

                    if (ushort.TryParse(data[12].Substring(0,1), out ushort motion))
                    {
                        switch (motion)
                        {
                            case 1:
                                allowedMotion = Axis.X;
                                break;

                            case 2:
                                allowedMotion = Axis.Y;
                                break;

                            case 3:
                                allowedMotion = Axis.Z;
                                break;
                        }
                    }

                    Areas.Add(new RestrictiveArea
                    {
                        Name = data[2],
                        Start = new Point_3D(float.Parse(data[4]), float.Parse(data[5]), float.Parse(data[6])),
                        End = new Point_3D(float.Parse(data[7]), float.Parse(data[8]), float.Parse(data[9])),
                        AllowedMotion = allowedMotion,
                        CoOpArea = (CoOpModes)short.Parse(data[25])
                    });
                }

                return true;
            }

            catch (Exception)
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
            XmlSerializer serializer = new XmlSerializer(typeof(ResAreaCollection));

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
            return $"{Areas.Count} Restriced Areas";
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Definition of restricitve area to inhibit motion
    /// </summary>
    public class RestrictiveArea
    {
        #region Constructor
        public RestrictiveArea()
        {
        }

        public RestrictiveArea(string name, Point_3D start, Point_3D end)
        {
            Name = name;
            Start = start;
            End = end;
        }

        #endregion

        #region Properties

        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public Point_3D Start { get; set; }

        [XmlElement]
        public Point_3D End { get; set; }

        [XmlElement]
        public Axis AllowedMotion { get; set; }

        [XmlIgnore]
        public double Zmin { get => Math.Min(Start.Z, End.Z); }

        [XmlIgnore]
        public double Zmax { get => Math.Max(Start.Z, End.Z); }

        [XmlIgnore]
        public  CoOpModes CoOpArea { get; set; }

        #endregion

        #region Methods

        #region To2DPolygon
        /// <summary>
        /// Creates a 2D Polygon from the Restrictive Area
        /// </summary>
        /// <returns></returns>
        public Polygon_2D ToPolygon_2D()
        {
            try
            {
                var poly = new Polygon_2D();

                poly.Points.Add(Start);
                poly.Points.Add(new Point_2D(End.X, Start.Y));
                poly.Points.Add(End);
                poly.Points.Add(new Point_2D(Start.X, End.Y));

                return poly;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
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
            return $"{Name}: {Start} | {End}";
        }

        #endregion

        #endregion
    }
}
