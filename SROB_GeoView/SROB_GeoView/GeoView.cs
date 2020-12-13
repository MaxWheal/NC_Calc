using HelixToolkit.Wpf;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using System.Windows;
using Geometries;
using Configuration.RestrictiveAreas;
using Configuration;
using System.ComponentModel;

namespace SROB_3DViewer
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

        #region Headposition
        private ModelVisual3D _head = new ModelVisual3D();

        public static new readonly DependencyProperty CurrentPositionProperty =
            DependencyProperty.Register("Position", typeof(Pnt4D), typeof(GeoView),
                new FrameworkPropertyMetadata(
                                        new PropertyChangedCallback(CurrentPositionChanged),
                                        new CoerceValueCallback(CurrentPositionChanged)));
        public Pnt4D Position
        {
            get => (Pnt4D)GetValue(CurrentPositionProperty);
            set => SetValue(CurrentPositionProperty, value);

        }
        #endregion

        #region PartInGripper
        private BoxVisual3D _part = new BoxVisual3D();

        public static readonly DependencyProperty PartsizeProperty =
            DependencyProperty.Register("Partsize", typeof(Geometries.Size), typeof(GeoView),
                new FrameworkPropertyMetadata(
                                        new PropertyChangedCallback(PartChanged),
                                        new CoerceValueCallback(PartChanged)));
        public Geometries.Size Partsize
        {
            get => (Geometries.Size)GetValue(PartsizeProperty);
            set => SetValue(PartsizeProperty, value);

        }
        #endregion

        #region Restrictive Areas
        private ModelVisual3D _resAreaModels = new ModelVisual3D();

        public static readonly DependencyProperty ResAreasProperty =
            DependencyProperty.Register("ResAreas", typeof(List<RestrictiveArea>), typeof(GeoView),
                new FrameworkPropertyMetadata(
                                        new PropertyChangedCallback(ResAreasChanged),
                                        new CoerceValueCallback(ResAreasChanged)));
        public List<RestrictiveArea> ResAreas
        {
            get => (List<RestrictiveArea>)GetValue(ResAreasProperty);
            set => SetValue(ResAreasProperty, value);
        }
        #endregion

        #endregion

        #region Methods

        #region Initialize
        public void Initialize()
        {
            try
            {
                Children.Clear();
                _head.Children.Clear();
                _part.Children.Clear();
                _resAreaModels.Children.Clear();

                Children.Add(new DefaultLights());
                Children.Add(_head);

                //ZeroPoint
                Children.Add(new SphereVisual3D
                {
                    Center = new Point3D(0, 0, 0),
                    Radius = 70,
                    Fill = Brushes.Green
                });

                Children.Add(WireframeBox(
                    new Point3D(Config.Params.Values["MIN_RAUM[0]"], Config.Params.Values["MIN_RAUM[1]"], Config.Params.Values["MIN_H"]),
                    new Point3D(Config.Params.Values["MAX_RAUM[0]"], Config.Params.Values["MAX_RAUM[1]"], Config.Params.Values["MAX_H"]),
                    Brushes.Red));

                //Palett
                Children.Add(FilledBox(new Point3D(0, 0, -100),
                    new Point3D(Config.Params.Values["PAL_L"], Config.Params.Values["PAL_B"], 0),
                    Brushes.LightGray));

                //MovingBody
                _head.Children.Add( new BoxVisual3D
                {
                    Length = Config.Params.Values["GREIFER_KOPF_L_GES"],
                    Width = Config.Params.Values["GREIFER_KOPF_B_GES"],
                    Height = 300,
                    Fill = Brushes.White,
                    Center = new Point3D(0, 0, 150),
                });

                _head.Children.Add(_part);

                ResAreas = Config.ResAreas.Areas;
            }

            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region AddStartPosition
        ///// <summary>
        ///// Adds transparent box to viewport to show StartPosition
        ///// </summary>
        ///// <param name="value">4D position of StartPosition</param>
        //public void AddMidPosition(Pnt4D value, Geometries.Size size)
        //{
        //    var StartPosition = new BoxVisual3D
        //    {
        //        Center = new Point3D(0, 0, size.Height / 2),
        //        Length = size.Length,
        //        Width = size.Width,
        //        Height = size.Height,
        //        Fill = new SolidColorBrush(Colors.Green.ChangeAlpha(150))
        //    };

        //    Matrix3D matrix = new Matrix3D();
        //    matrix.Translate(new Vector3D(value.X, value.Y, value.Z));
        //    matrix.RotateAt(new Quaternion(new Vector3D(0, 0, 1), value.C), new Point3D(value.X, value.Y, value.Z));

        //    StartPosition.Transform = new MatrixTransform3D(matrix);

        //    MidPositions.Children.Add(StartPosition);
        //}
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

        #region Track
        /// <summary>
        /// Adds list of points and plots as track
        /// </summary>
        /// <param name="points"></param>
        private ModelVisual3D Track(List<Point3D> points, SolidColorBrush brush)
        {
            var track = new ModelVisual3D();

            try
            {

                if (points.Count < 2) return track;

                for (int i = 1; i < points.Count; i++)
                {
                    var trace = new LinesVisual3D
                    {
                        Color = brush.Color,
                        Thickness = 1
                    };

                    trace.Points.Add(new Point3D(points[i - 1].X, points[i - 1].Y, points[i - 1].Z));
                    trace.Points.Add(points[i]);

                    track.Children.Add(trace);

                    track.Children.Add(new SphereVisual3D
                    {
                        Center = points[i - 1],
                        Radius = 70,
                        Fill = new SolidColorBrush(brush.Color.ChangeAlpha(10))
                    });
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return track;
        }

        #endregion

        #region RedrawTranspObjects
        /// <summary>
        /// For tranparancy to work transparant objects must be drawn at last.
        /// </summary>
        public void RedrawTransparants()
        {
            Children.Remove(_resAreaModels);

            Children.Add(_resAreaModels);
        }

        #endregion

        #region CurrentpositionChanged
        private static void CurrentPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((GeoView)d).CurrentGripperposition();
        private static object CurrentPositionChanged(DependencyObject d, Object e)
        {
            ((GeoView)d).CurrentGripperposition();
            return e;
        }

        /// <summary>
        /// Updates the viewed current position
        /// </summary>
        private void CurrentGripperposition()
        {
            if (Position == null) return;

            Matrix3D matrix = new Matrix3D();
            matrix.Translate(new Vector3D(Position.X, Position.Y, Position.Z));
            matrix.RotateAt(new Quaternion(new Vector3D(0, 0, 1), Position.C), new Point3D(Position.X, Position.Y, Position.Z));

           _head.Transform = new MatrixTransform3D(matrix);
        }
        #endregion

        #region PartChanged
        private static void PartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((GeoView)d).UpdatePartInGripper();
        private static object PartChanged(DependencyObject d, Object e)
        {
            ((GeoView)d).UpdatePartInGripper();
            return e;
        }

        /// <summary>
        /// Updates the viewed current position
        /// </summary>
        private void UpdatePartInGripper()
        {
            //MovingBody
            _part.Length = Partsize.Length;
            _part.Width = Partsize.Width;
            _part.Height = Partsize.Height;
            _part.Fill = Brushes.DarkGray;
            _part.Center = new Point3D(0, 0, -Partsize.Height/2);
        }
        #endregion

        #region ResAreasChanged
        private static void ResAreasChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((GeoView)d).UpdateResAreas();
        private static object ResAreasChanged(DependencyObject d, Object e)
        {
            ((GeoView)d).UpdateResAreas();
            return e;
        }

        /// <summary>
        /// Updates the ResAreas Object 
        /// </summary>
        private void UpdateResAreas()
        {
            if (ResAreas == null) return;

            ResAreas.ForEach(area => _resAreaModels.Children.Add(FilledBox(area.Start, area.End, new SolidColorBrush(Colors.Red.ChangeAlpha(120)))));

            RedrawTransparants();
        }

        #endregion

        #endregion
    }
}
