using System;
using System.Xml;
using System.Xml.Serialization;

namespace Geometries
{
    public class T_P_4D : T_P_3D
    {
        #region Contructors
        public T_P_4D()
        {

        }

        public T_P_4D(T_P_4D value)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
            C = value.C;
        }

        public T_P_4D(double x = 0, double y = 0, double z = 0, double c = 0)
        {
            X = x;
            Y = y;
            Z = z;
            C = c;
        }

        #endregion

        #region Properties
        [XmlAttribute]
        public double C { get; set; }
        #endregion

        #region Methods

        #region ToString
        /// <summary>
        /// Returns a string to represent the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"X: {X} Y: {Y} Z: {Z} C: {C}";
        }
        #endregion

        #region Check for lower value

        /// <summary>
        /// Checks for the minimum value.
        /// </summary>
        /// <returns>The lower value.</returns>
        public static T_P_4D Min(T_P_4D point_1, T_P_4D point_2)
        {
            T_P_4D pointMin = new T_P_4D();

            pointMin.X = Math.Min(point_1.X, point_2.X);
            pointMin.Y = Math.Min(point_1.Y, point_2.Y);
            pointMin.Z = Math.Min(point_1.Z, point_2.Z);
            pointMin.C = Math.Min(point_1.C, point_2.C);

            return pointMin;
        }

        #endregion

        #region Check for higher value

        /// <summary>
        /// Checks for the maximum value.
        /// </summary>
        /// <returns>The higher value.</returns>
        public static T_P_4D Max(T_P_4D point_1, T_P_4D point_2)
        {
            T_P_4D pointMax = new T_P_4D();

            pointMax.X = Math.Max(point_1.X, point_2.X);
            pointMax.Y = Math.Max(point_1.Y, point_2.Y);
            pointMax.Z = Math.Max(point_1.Z, point_2.Z);
            pointMax.C = Math.Max(point_1.C, point_2.C);

            return pointMax;
        }

        #endregion
       
        #endregion

        #region Conversions

        #endregion
    }

    public class T_P_3D : T_P_2D
    {
        #region Contructors
        public T_P_3D()
        {

        }

        public T_P_3D(double x = 0, double y = 0, double z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        #endregion

        #region Properties
        [XmlAttribute]
        public double Z { get; set; }
        #endregion

        #region Methods

        #region ToString
        /// <summary>
        /// Returns a string to represent the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"X: {X} Y: {Y} Z: {Z}";
        }
        #endregion

        #endregion

        #region Conversions

        public static implicit operator System.Windows.Media.Media3D.Point3D(T_P_3D p_3D) 
            => new System.Windows.Media.Media3D.Point3D(p_3D.X, p_3D.Y, p_3D.Z);

        #endregion

    }

    public class T_P_2D
    {
        #region Contructors
        public T_P_2D()
        {

        }

        public T_P_2D(double x = 0, double y = 0)
        {
            X = x;
            Y = y;
        }
        #endregion

        #region Properties
        [XmlAttribute]
        public double X { get; set; }

        [XmlAttribute]
        public double Y { get; set; }
        #endregion

        #region Methods

        #region ToString
        /// <summary>
        /// Returns a string to represent the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"X: {X} Y: {Y}";
        }
        #endregion

        #endregion
    }
}