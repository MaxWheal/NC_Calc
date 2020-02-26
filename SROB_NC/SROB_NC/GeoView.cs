using Configuration;
using HelixToolkit.Wpf;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Geometries;
using System.Collections.Generic;
using System.Linq;

namespace SROB_NC
{
    class GeoView : HelixViewport3D
    {
        #region Constructors

        public GeoView()
        {
            ZoomExtentsWhenLoaded = true;
            ShowViewCube = false;
            ShowCoordinateSystem = true;
            Camera.LookDirection = new Vector3D(460, 23330, -7000);

            Initialize();
        }

        #endregion

        #region Properties

        public BoxVisual3D Gripper { get; set; }
        //public DragableBox Gripper { get; set; }
        #endregion

        #region Methods

        #region Initialize
        public void Initialize()
        {
            Children.Clear();
            Children.Add(new DefaultLights());

            //ZeroPoint
            Children.Add(new SphereVisual3D
            {
                Center = new Point3D(0, 0, 0),
                Radius = 70,
                Fill = Brushes.Green
            });

            //Border
            Children.Add(WireframeBox(
                new Point3D(Config.Params.Values["MIN_RAUM[0]"], Config.Params.Values["MIN_RAUM[1]"], Config.Params.Values["MIN_H"]),
                new Point3D(Config.Params.Values["MAX_RAUM[0]"], Config.Params.Values["MAX_RAUM[1]"], Config.Params.Values["MAX_H"]),
                Brushes.Red));

            //Palett
            Children.Add(FilledBox(new Point3D(0, 0, -100),
                new Point3D(Config.Params.Values["PAL_L"], Config.Params.Values["PAL_B"], 0),
                Brushes.LightGray));

            //Gripper
            Gripper = new BoxVisual3D
            {
                Center = new Point3D(0, 0, 150),
                Length = Config.Params.Values["GRIPPER_DIM[0]"],
                Width = Config.Params.Values["GRIPPER_DIM[1]"],
                Height = 300,
                Fill = Brushes.DarkGray,
            };

            Children.Add(Gripper);

            //Restricted areas (render at last for transparency to work)
            foreach (var area in Config.ResAreas.Areas)
            {
                //Children.Add(FilledBox(area.Start, area.End, new SolidColorBrush(Colors.Red.ChangeAlpha(150))));
                Children.Add(WireframeBox(area.Start, area.End, Brushes.Red));
            }
        }
        #endregion

        #region AddStartPosition
        /// <summary>
        /// Adds transparent box to viewport to show StartPosition
        /// </summary>
        /// <param name="value">4D position of StartPosition</param>
        public void AddMidPosition(Point_4D value, Size size)
        {
            var StartPosition = new BoxVisual3D
            {
                Center = new Point3D(0, 0, size.Height/2),
                Length = size.Length,
                Width = size.Width,
                Height = size.Height,
                Fill = new SolidColorBrush(Colors.Green.ChangeAlpha(150))
            };

            Matrix3D matrix = new Matrix3D();
            matrix.Translate(new Vector3D(value.X, value.Y, value.Z));
            matrix.RotateAt(new Quaternion(new Vector3D(0, 0, 1), value.C), new Point3D(value.X, value.Y, value.Z));

            StartPosition.Transform = new MatrixTransform3D(matrix);

            Children.Add(StartPosition);
        }
        #endregion

        #region FilledBox
        /// <summary>
        /// Adds a box to the viewport by defininition of two oposing corner.
        /// </summary>
        /// <param name="start">Start point of box</param>
        /// <param name="end">EndPoint of box</param>
        /// <param name="brush">Brush to be applied to box</param>
        private BoxVisual3D FilledBox(Point3D start, Point3D end, SolidColorBrush brush)
        {
            try
            {
                var box = new BoxVisual3D()
                {
                    Center = new Point3D((start.X + end.X) / 2, (start.Y + end.Y) / 2, (start.Z + end.Z) / 2),
                    Length = end.X - start.X,
                    Width = end.Y - start.Y,
                    Height = end.Z - start.Z,
                    Fill = brush
                };

                return box;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString()); ;
            }

            return null;
        }
        #endregion

        #region FilledCylinder
        private TubeVisual3D FilledCylinder(Point3D start, Point3D end, double diameter, SolidColorBrush brush)
        {
            try
            {

                var path = new Point3DCollection
                {
                    start,
                    end
                };

                var cylinder = new TubeVisual3D
                {
                    Path = path,
                    Diameter = diameter,
                    AddCaps = true,
                    Fill = brush,
                };

                return cylinder;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString()); ;
            }

            return null;
        }
        #endregion

        #region WireframeBox
        /// <summary>
        /// Adds a box to the viewport by defininition of two oposing corner.
        /// </summary>
        /// <param name="start">Start point of box</param>
        /// <param name="end">EndPoint of box</param>
        /// <param name="brush">Brush to be applied to box</param>
        private BoundingBoxWireFrameVisual3D WireframeBox(Point3D start, Point3D end, SolidColorBrush brush)
        {
            try
            {
                var box = new BoundingBoxWireFrameVisual3D
                {
                    BoundingBox = new Rect3D(
                        start.X,
                        start.Y,
                        start.Z,

                        end.X - start.X,
                        end.Y - start.Y,
                        end.Z - start.Z
                    ),

                    Thickness = 1,
                    Color = brush.Color
                };

                return box;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString()); ;
            }

            return null;
        }
        #endregion

        #region AddFlatProjection
        /// <summary>
        /// Adds a flat Projection to the viewport
        /// </summary>
        /// <param name="center">Midpoint of projection</param>
        /// <param name="size">Length or Diameter of projection</param>
        /// <param name="width">Optional: If assigned drawn as rectangle</param>
        /// <param name="projectionHeight">Optional height where projeciton is shown, if not assigned shown at max Height</param>
        public void AddFlatProjection(Point_2D center, double size, double width = 0, double projectionHeight = 0)

        {
            Visual3D projection;

            if (width > 0)
            {
                projection = new BoxVisual3D
                {
                    Center = new Point3D(0, 0, -1),
                    Length = size,
                    Width = width,
                    Height = 1,
                    Fill = new SolidColorBrush(Colors.MediumVioletRed.ChangeAlpha(150)),
                };
            }
            else
            {
                projection = FilledCylinder(new Point3D(0, 0, -1), new Point3D(0, 0, 0), size,
                    new SolidColorBrush(Colors.MediumVioletRed.ChangeAlpha(150)));
            }

            Matrix3D matrix = new Matrix3D();
            matrix.Translate(new Vector3D(center.X, center.Y, projectionHeight > 0 ? projectionHeight : Config.Params.Values["MAX_H"]));

            projection.Transform = new MatrixTransform3D(matrix);

            Children.Add(projection);
        }

        #endregion

        #region AddTrack
        /// <summary>
        /// Adds list of points and plots as track
        /// </summary>
        /// <param name="points"></param>
        internal void AddTrack(List<Point_4D> points)
        {
            try
            {
                if (points.Count < 2)
                    return;

                for (int i = 1; i < points.Count; i++)
                {
                    var trace = new LinesVisual3D
                    {
                        Color = Brushes.Orange.Color,
                        Thickness = 1
                    };

                    Children.Add(trace);

                    trace.Points.Add(points[i - 1]);
                    trace.Points.Add(points[i]);
                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        #endregion

        #region AddPolyon
        internal void AddPolygon(List<Point_2D> corners, double height)
        {
            if (corners.Count < 3)
                return;

            var points = new List<Point_4D>();

            foreach (var corner in corners)
            {
                points.Add(new Point_4D(corner.X, corner.Y, height, 0));
            }

            //To close Polygon
            points.Add(new Point_4D(points[0]));

            AddTrack(points);
        }
        #endregion

        #endregion
    }
}
