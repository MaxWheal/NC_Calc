using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using Configuration;
using Geometries;

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
                Children.Add(FilledBox(area.Start, area.End, new SolidColorBrush(Colors.Red.ChangeAlpha(150))));
            }
        }

        public void AddStartPosition(T_P_4D value)
        {
            var StartPosition = new BoxVisual3D
            {
                Center = new Point3D(0, 0, 150),
                Length = Config.Params.Values["GRIPPER_DIM[0]"],
                Width = Config.Params.Values["GRIPPER_DIM[1]"],
                Height = 300,
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

        #endregion
    }
}
