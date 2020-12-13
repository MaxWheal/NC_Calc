using System.Collections.Generic;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System.Windows;
using SROB_3DViewer;

namespace Geometries
{
    public class Pnt4D : Pnt3D
    {
        #region Contructors
        public Pnt4D()
        {

        }

        public Pnt4D(Pnt4D point_4D)
        {
            X = point_4D.X;
            Y = point_4D.Y;
            Z = point_4D.Z;
            C = point_4D.C;
        }

        public Pnt4D(double x = 0, double y = 0, double z = 0, double c = 0)
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
            return $"{base.ToString()}|{C:0.0}";
        }
        #endregion

        #region Equals
        /// <summary>
        /// Determines whether the specified <see cref="Pnt4D"/> is equal to the current instance.
        /// </summary>
        /// <param name="obj">he object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            Pnt4D point = (Pnt4D)obj;
            return base.Equals(obj) && this.C == point.C;
        }
        #endregion

        #region Check for lower value

        /// <summary>
        /// Checks for the minimum value.
        /// </summary>
        /// <returns>The lower value.</returns>
        public static Pnt4D Min(Pnt4D point_1, Pnt4D point_2)
        {
            Pnt4D pointMin = new Pnt4D
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
        public static Pnt4D Max(Pnt4D point_1, Pnt4D point_2)
        {
            Pnt4D pointMax = new Pnt4D
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

    public class Pnt3D : Pnt2D
    {
        #region Contructors
        public Pnt3D()
        {

        }

        public Pnt3D(double x = 0, double y = 0, double z = 0)
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
            return $"{base.ToString()}|{Z:0.0}";
        }
        #endregion

        #region Equals
        /// <summary>
        /// Determines whether the specified <see cref="Pnt3D"/> is equal to the current instance.
        /// </summary>
        /// <param name="obj">he object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            Pnt3D point = (Pnt3D)obj;
            return base.Equals(obj) && this.Z == point.Z;
        }
        #endregion

        #endregion

        #region Conversions

        public static implicit operator System.Windows.Media.Media3D.Point3D(Pnt3D p_3D)
            => new System.Windows.Media.Media3D.Point3D(p_3D.X, p_3D.Y, p_3D.Z);

        #endregion

    }

    public class Pnt2D
    {
        #region Contructors
        public Pnt2D()
        {

        }

        public Pnt2D(double x = 0, double y = 0)
        {
            X = x;
            Y = y;
        }

        public Pnt2D(Pnt2D point_2D)
        {
            X = point_2D.X;
            Y = point_2D.Y;
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
            return $"{X:0.0}|{Y:0.0}";
        }
        #endregion

        #region GetPointMin
        #endregion

        #region Equals
        /// <summary>
        /// Determines whether the specified <see cref="Pnt2D"/> is equal to the current instance.
        /// </summary>
        /// <param name="obj">he object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            Pnt2D point = (Pnt2D)obj;
            return this.X == point.X && this.Y == point.Y;
        }
        #endregion

        #endregion
    }

    public class Segment_4D
    {
        #region Constructors
        public Segment_4D()
        {

        }

        public Segment_4D(Pnt4D start, Pnt4D end)
        {
            Start = start;
            End = end;
        }
        #endregion

        #region Properties

        public Pnt4D Start { get; set; }
        public Pnt4D End { get; set; }
        public double Length { get => Math.Abs(Math.Sqrt(Math.Pow((End.X - Start.X), 2) + Math.Pow((End.Y - Start.Y), 2) + Math.Pow((End.Z - Start.Z), 2))); }

        #endregion

        #region Methods

        #region GetPositionAt
        /// <summary>
        /// Gets Point on 4D Segment by position
        /// </summary>
        /// <param name="axis"> segment following position will be given if <see cref="Axis.None"/> lenght of segment must be given</param>
        /// <param name="position">position on segment</param>
        /// <returns>Point at given position</returns>
        public Pnt4D GetPositionAt(Axis axis, double position)
        {
            double relDist = 0;
            switch (axis)
            {
                case Axis.X:
                    if (Start.X == End.X)
                        return Start;

                    relDist = (position - Start.X) / (End.X - Start.X);
                    break;

                case Axis.Y:
                    if (Start.Y == End.Y)
                        return Start;

                    relDist = (position - Start.Y) / (End.Y - Start.Y);
                    break;

                case Axis.None:
                    relDist = position / Length;
                    break;

            }

            return new Pnt4D
            {
                X = Start.X + (End.X - Start.X) * relDist,
                Y = Start.Y + (End.Y - Start.Y) * relDist,
                Z = Start.Z + (End.Z - Start.Z) * relDist,
                C = Start.C + (End.C - Start.C) * relDist,
            };
        }
        #endregion

        #region GetDirectionOf
        /// <summary>
        /// Gets Direction of given axis as value
        /// </summary>
        /// <param name="axis">axis of motion to be checked</param>
        /// <returns>Returns 0 if not motion, -1 if direction is negativ, 1 if direction is positive</returns>
        public int GetDirectionOf(Axis axis)
        {
            switch (axis)
            {
                case Axis.X:
                    if (Start.X == End.X) return 0;

                    return Start.X < End.X ? 1 : -1;

                case Axis.Y:
                    if (Start.Y == End.Y) return 0;

                    return Start.Y < End.Y ? 1 : -1;

                case Axis.Z:
                    if (Start.Z == End.Z) return 0;

                    return Start.Z < End.Z ? 1 : -1;

                case Axis.C:
                    if (Start.C == End.C) return 0;

                    return Start.C < End.C ? 1 : -1;

                case Axis.None:
                default:
                    return 0;
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
            return $"From {Start} to {End}";
        }
        #endregion

        #endregion

        #region Conversions

        public static implicit operator Segment_2D(Segment_4D seg_4D)
            => new Segment_2D(new Pnt2D(seg_4D.Start), new Pnt2D(seg_4D.End));

        #endregion
    }

    public partial class Segment_2D
    {
        #region Constructors
        public Segment_2D()
        {

        }

        public Segment_2D(Pnt2D start, Pnt2D end)
        {
            Start = start;
            End = end;
        }
        #endregion

        #region Properties

        public Pnt2D Start { get; set; }
        public Pnt2D End { get; set; }

        public double Slope { get => (Start.Y - End.Y) / (Start.X - End.X); }
        public double AxSection { get => Start.Y - Start.X * Slope; }

        public double Length { get => Math.Abs(Math.Sqrt(Math.Pow((End.X - Start.X), 2) + Math.Pow((End.Y - Start.Y), 2))); }

        #endregion

        #region Methods

        #region IsIntersecting Segment
        /// <summary>
        /// Checks if given <see cref="Segment_2D"/> intersects with this.
        /// </summary>
        /// <param name="segment"></param>
        /// <returns>Intersection detected</returns>
        public bool IsIntersecting(Segment_2D segment)
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
        private static Orientation GetOrientation(Pnt2D point1, Pnt2D point2, Pnt2D point3)
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
        public bool IsIntersecting(Pnt2D center, double diameter)
        {

            //calculating line's perpendicular distance to ball
            Vector circle = new Vector(center.X - this.Start.X, center.Y - this.Start.Y);
            double circle_onNormal = circle.ProjectOn(this.ToVector().Rotate(Math.PI / 2));

            //Collision if distance is less than diameter
            if (Math.Abs(circle_onNormal) <= diameter / 2)
                return true;

            return false;
        }
        #endregion

        #region ToVector
        /// <summary>
        /// Creates <see cref="Vector"/> from <see cref="Segment_2D"/>
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

        public Size(double length, double width, double height)
        {
            Length = length;
            Width = width;
            Height = height;
        }
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
        public Polygon_2D(Pnt4D position, Size size)
        {
            var polygonRaw = new Polygon_2D();

            //Set Size at origin
            polygonRaw.Points.Add(new Pnt2D(size.Length / 2, size.Width / 2));
            polygonRaw.Points.Add(new Pnt2D(-size.Length / 2, size.Width / 2));
            polygonRaw.Points.Add(new Pnt2D(-size.Length / 2, -size.Width / 2));
            polygonRaw.Points.Add(new Pnt2D(size.Length / 2, -size.Width / 2));

            foreach (var corner in polygonRaw.Points)
            {
                var point = new Pnt2D
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
        public List<Pnt2D> Points = new List<Pnt2D>();

        public Pnt2D PointMin { get => new Pnt2D(Points.Min(pt => pt.X), Points.Min(pt => pt.Y)); }
        public Pnt2D PointMax { get => new Pnt2D(Points.Max(pt => pt.X), Points.Max(pt => pt.Y)); }

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
        public static bool IsPointInPoly(Pnt2D point, Polygon_2D polygon)
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

                List<Segment_2D> segments = polygon.ToSegments_2D();

                Pnt2D point1 = new Pnt2D();
                Pnt2D point2 = new Pnt2D();

                foreach (var segment in segments)
                {
                    point1.X = segment.Start.X - point.X;
                    point1.Y = segment.Start.Y - point.Y;
                    point2.X = segment.End.X - point.X;
                    point2.Y = segment.End.Y - point.Y;

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
        private static double GetTheta(Pnt2D point1, Pnt2D point2)
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
            foreach (var segment1 in this.ToSegments_2D())
            {
                foreach (var segment2 in polygon.ToSegments_2D())
                {
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
        public bool IsOverlapping(Pnt2D center, double diameter)
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
                var segment = new Segment_2D(this.Points[i], this.Points[i < this.Points.Count - 1 ? i + 1 : 0]);

                if (segment.IsIntersecting(center, diameter))
                    return true;
            }

            return false;
        }
        #endregion

        #region ToSegments
        public List<Segment_2D> ToSegments_2D()
        {
            var segments = new List<Segment_2D>();

            for (int i = 0; i < this.Points.Count - 1; i++)
            {
                segments.Add(new Segment_2D(this.Points[i], this.Points[i + 1]));
            }

            //add remaining last
            segments.Add(new Segment_2D(this.Points[Points.Count - 1], this.Points[0]));

            return segments;
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