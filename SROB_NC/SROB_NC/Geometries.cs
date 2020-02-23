using System.Collections.Generic;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System.Windows;

namespace Geometries
{
    public class Point_4D : Point_3D
    {
        #region Contructors
        public Point_4D()
        {

        }

        public Point_4D(Point_4D value)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
            C = value.C;
        }

        public Point_4D(double x = 0, double y = 0, double z = 0, double c = 0)
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

        /// <summary>
        /// C in radiants
        /// </summary>
        [XmlIgnore]
        public double C_rad
        {
            get => Math.PI * C / 180;
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
            return $"{base.ToString()} | {C:0.0}";
        }
        #endregion

        #region Check for lower value

        /// <summary>
        /// Checks for the minimum value.
        /// </summary>
        /// <returns>The lower value.</returns>
        public static Point_4D Min(Point_4D point_1, Point_4D point_2)
        {
            Point_4D pointMin = new Point_4D
            {
                X = Math.Min(point_1.X, point_2.X),
                Y = Math.Min(point_1.Y, point_2.Y),
                Z = Math.Min(point_1.Z, point_2.Z),
                C = Math.Min(point_1.C, point_2.C)
            };

            return pointMin;
        }

        #endregion

        #region Check for higher value

        /// <summary>
        /// Checks for the maximum value.
        /// </summary>
        /// <returns>The higher value.</returns>
        public static Point_4D Max(Point_4D point_1, Point_4D point_2)
        {
            Point_4D pointMax = new Point_4D
            {
                X = Math.Max(point_1.X, point_2.X),
                Y = Math.Max(point_1.Y, point_2.Y),
                Z = Math.Max(point_1.Z, point_2.Z),
                C = Math.Max(point_1.C, point_2.C)
            };

            return pointMax;
        }

        #endregion

        #endregion

        #region Conversions

        #endregion
    }

    public class Point_3D : Point_2D
    {
        #region Contructors
        public Point_3D()
        {

        }

        public Point_3D(double x = 0, double y = 0, double z = 0)
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
            return $"{base.ToString()} | {Z:0.0}";
        }
        #endregion

        #endregion

        #region Conversions

        public static implicit operator System.Windows.Media.Media3D.Point3D(Point_3D p_3D)
            => new System.Windows.Media.Media3D.Point3D(p_3D.X, p_3D.Y, p_3D.Z);

        #endregion

    }

    public class Point_2D
    {
        #region Contructors
        public Point_2D()
        {

        }

        public Point_2D(double x = 0, double y = 0)
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
            return $"{X:0.0} | {Y:0.0}";
        }
        #endregion

        #endregion
    }

    public class Segement_4D
    {
        #region Constructors
        public Segement_4D()
        {

        }

        public Segement_4D(Point_4D start, Point_4D end)
        {
            Start = start;
            End = end;
        }
        #endregion

        #region Properties

        public Point_4D Start { get; set; }
        public Point_4D End { get; set; }

        #endregion

        #region Methods

        #region ToString
        /// <summary>
        /// Returns a string to represent the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"From {Start} to {End}";
        }
        #endregion

        #endregion
    }

    public partial class Segement_2D
    {
        #region Constructors
        public Segement_2D()
        {

        }

        public Segement_2D(Point_2D start, Point_2D end)
        {
            Start = start;
            End = end;
        }
        #endregion

        #region Properties

        public Point_2D Start { get; set; }
        public Point_2D End { get; set; }

        #endregion

        #region Methods

        #region IsIntersecting Segment
        /// <summary>
        /// Checks if given <see cref="Segement_2D"/> intersects with this.
        /// </summary>
        /// <param name="segment"></param>
        /// <returns>Intersection detected</returns>
        public bool IsIntersecting(Segement_2D segment)
        {
            try
            {
                Orientation orien1 = GetOrientation(this.Start, this.End, segment.Start);
                Orientation orien2 = GetOrientation(this.Start, this.End, segment.End);
                Orientation orien3 = GetOrientation(segment.Start, segment.End, this.Start);
                Orientation orien4 = GetOrientation(segment.Start, segment.End, this.End);


                //(p1, q1, p2) and (p1, q1, q2) have different orientations and
                //(p2, q2, p1) and (p2, q2, q1) have different orientations.
                if (orien1 != orien2 && orien3 != orien4)
                    return true;

                return false;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }


        #endregion

        #region GetOrientation
        private enum Orientation
        {
            Colinear,
            Clockwise,
            Counterclockwise
        }

        /// <summary>
        /// Checks Orientation of 3 given points
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <returns></returns>
        private static Orientation GetOrientation(Point_2D point1, Point_2D point2, Point_2D point3)
        {
            var test = (point2.Y - point1.Y) * (point3.X - point2.X) -
      (point2.X - point1.X) * (point3.Y - point2.Y);

            if (test == 0)
                return Orientation.Colinear;

            if (test > 0)
                return Orientation.Clockwise;

            return Orientation.Counterclockwise;

        }


        #endregion

        #region IsIntersecting Circle
        /// <summary>
        /// Checks if circle intersects with this.
        /// </summary>
        /// <param name="center">center of circle</param>
        /// <param name="diameter">diameter of circle</param>
        /// <returns>Intersection detected</returns>
        public bool IsIntersecting(Point_2D center, double diameter)
        {

            //calculating line's perpendicular distance to ball
            Vector circle = new Vector(center.X - this.Start.X, center.Y - this.Start.Y);
            double circle_onNormal = circle.ProjectOn(this.ToVector().Rotate(Math.PI /2));

            //Collision if distance is less than diameter
            if (Math.Abs(circle_onNormal) <= diameter/2)
                return true;

            return false;
        }
        #endregion

        #region ToVector
        /// <summary>
        /// Creates <see cref="Vector"/> from <see cref="Segement_2D"/>
        /// </summary>
        /// <returns>Vector from Segment</returns>
        public Vector ToVector()
        {
            return new Vector(End.X - Start.X, End.Y - Start.Y);
        }
        #endregion

        #region ToString
        /// <summary>
        /// Returns a string to represent the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"From {Start} to {End}";
        }
        #endregion

        #endregion
    }

    public static class Extensions
    {
        #region Vector.Rotate
        public static Vector Rotate(this Vector source, double rad)
        {
            var rotated = new Vector
            {
                X = source.X * Math.Cos(rad) - source.Y * Math.Sin(rad),
                Y = source.X * Math.Sin(rad) + source.Y * Math.Cos(rad)
            };

            return rotated;
        }
        #endregion

        #region Vector.ProjectOn
        public static double ProjectOn(this Vector source, Vector vector)
        {
            vector.Normalize();
            return Vector.Multiply(source, vector);
        }
        #endregion
    }

    public struct Size
    {
        public double Length;
        public double Width;
        public double Height;
    }

    public class Polygon_2D
    {
        #region Constructors
        public Polygon_2D()
        {

        }

        #region Polygon_2D from positon and size
        /// <summary>
        /// Creates a 2D polygon from a 4D position
        /// </summary>
        /// <param name=""></param>
        public Polygon_2D(Point_4D position, Size size)
        {
            var polygonRaw = new Polygon_2D();

            //Set Size at origin
            polygonRaw.Points.Add(new Point_2D(size.Length / 2, size.Width / 2));
            polygonRaw.Points.Add(new Point_2D(-size.Length / 2, size.Width / 2));
            polygonRaw.Points.Add(new Point_2D(-size.Length / 2, -size.Width / 2));
            polygonRaw.Points.Add(new Point_2D(size.Length / 2, -size.Width / 2));

            foreach (var corner in polygonRaw.Points)
            {
                var point = new Point_2D
                {

                    //Rotation
                    X = corner.X * Math.Cos(position.C_rad) - corner.Y * Math.Sin(position.C_rad),
                    Y = corner.X * Math.Sin(position.C_rad) + corner.Y * Math.Cos(position.C_rad)
                };

                //Translation
                point.X += position.X;
                point.Y += position.Y;

                this.Points.Add(point);
            }
        }
        #endregion

        #endregion

        #region Properties
        public List<Point_2D> Points = new List<Point_2D>();
        #endregion

        #region Methods

        #region PointInPoly
        public enum PointInPoly
        {
            Error = -1, //Calculation Error
            Out = 0,    //Point is not inside of polygon
            In = 1, //Point is inside of polygon
            OnLine = 1 //Point is on line of polygon
        }

        /// <summary>
        /// Checks if given Point ist inside polygon
        /// </summary>
        /// <param name="point">Point to be checked</param>
        /// <param name="polygon">Polygon to be checked</param>
        /// <returns></returns>
        public static bool IsPointInPoly(Point_2D point, Polygon_2D polygon /*, out PointInPoly result*/)
        {
            try
            {
                if (point == null || polygon == null)
                {
                    //result = PointInPoly.Error;
                    return false;
                }


                if (polygon.Points.Count < 3)
                {
                    //result = PointInPoly.Error;
                    return false;
                }

                double sumOfAngles = 0;

                Point_2D point1 = new Point_2D(0, 0);
                Point_2D point2 = new Point_2D(0, 0);

                for (int i = 0; i < polygon.Points.Count; i++)
                {
                    point1.X = polygon.Points[i].X - point.X;
                    point1.Y = polygon.Points[i].Y - point.Y;
                    point2.X = polygon.Points[i < polygon.Points.Count - 1 ? i + 1 : 0].X - point.X;
                    point2.Y = polygon.Points[i < polygon.Points.Count - 1 ? i + 1 : 0].Y - point.Y;

                    double theta = GetTheta(point1, point2);

                    //Point is on line of polygon
                    if (Math.Round(Math.Abs(theta), 4) == Math.Round(Math.PI, 4))
                    {
                        //result = PointInPoly.OnLine;
                        return true;
                    }

                    sumOfAngles += theta;
                }

                //Point is within polygon
                if (Math.Abs(sumOfAngles) > 2 * Math.PI - 0.0001)
                {
                    //result = PointInPoly.In;
                    return true;
                }

                //Point is not inside of polygon
                //result = PointInPoly.Out;
                return false;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //result = PointInPoly.Error;
                return false;
            }
        }
        #endregion

        #region GetTheta
        /// <summary>
        /// Calculates theta between two given points
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        private static double GetTheta(Point_2D point1, Point_2D point2)
        {
            double theta = Math.Atan2(point2.Y, point2.X) - Math.Atan2(point1.Y, point1.X);

            if (theta > Math.PI)
                theta -= Math.PI * 2;

            if (theta < -Math.PI)
                theta += Math.PI * 2;

            return theta;
        }
        #endregion

        #region IsOverlapping Polygon
        /// <summary>
        /// Checks if given <see cref="Polygon_2D"/> is overlapping.
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="diameter">diameter of circle</param>
        public bool IsOverlapping(Polygon_2D polygon)
        {
            //Check for Intersecting Lines
            for (int i = 0; i < this.Points.Count; i++)
            {
                var segment1 = new Segement_2D(this.Points[i], this.Points[i < this.Points.Count - 1 ? i + 1 : 0]);

                for (int j = 0; j < polygon.Points.Count; j++)
                {
                    var segment2 = new Segement_2D(polygon.Points[j], polygon.Points[j < polygon.Points.Count - 1 ? j + 1 : 0]);

                    if (segment1.IsIntersecting(segment2))
                        return true;
                }
            }

            //Check if Point is within
            if (IsPointInPoly(this.Points[0], polygon))
                return true;

            if (IsPointInPoly(polygon.Points[0], this))
                return true;

            return false;
        }
        #endregion

        #region IsOverlapping Circle
        /// <summary>
        /// Checks if given <see cref="Polygon_2D"/> is overlapping.
        /// </summary>
        /// <param name="center">center of circle</param>
        /// <param name="diameter">diameter of circle</param>
        /// <returns>Collision detected</returns>
        public bool IsOverlapping(Point_2D center, double diameter)
        {
            //Check if center is within
            if (IsPointInPoly(center, this))
                return true;

            //Check if polygon is within circle
            if (Math.Abs(new Vector(this.Points[0].X - center.X, this.Points[0].Y - center.Y).Length) < diameter / 2)
                return true;

            //Check for Intersecting Lines
            for (int i = 0; i < this.Points.Count; i++)
            {
                var segment = new Segement_2D(this.Points[i], this.Points[i < this.Points.Count - 1 ? i + 1 : 0]);

                if (segment.IsIntersecting(center, diameter))
                    return true;
            }

            return false;
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            return Points.ToString();
        }

        #endregion

        #endregion
    }
}