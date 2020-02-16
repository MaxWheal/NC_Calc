using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit;
using HelixToolkit.Wpf;

namespace SROB_NC
{
    class DragableBox : Manipulator
    {

        #region Properties

        /// <summary>
        /// Identifies the <see cref="Length"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
            "Length", typeof(double), typeof(DragableBox), new UIPropertyMetadata(2.0, UpdateGeometry));


        /// <summary>
        /// Gets or sets the length of the manipulator arrow.
        /// </summary>
        /// <value> The length. </value>
        public double Length
        {
            get => (double)GetValue(LengthProperty);
            set => SetValue(LengthProperty, value);
        }
        public Vector3D Direction { get; set; }
        private Point3D lastPointX;
        private Point3D lastPointY;

        public Vector3D HitPlaneNormalY { get; set; }

        #endregion


        #region Methods

        #region UpdateGeometry
        /// <summary>
        /// Updates the geometry.
        /// </summary>
        protected override void UpdateGeometry()

        {
            var mb = new MeshBuilder(false, false);
            var p0 = new Point3D(0, 0, 0);

            mb.AddBox(p0, 2000, 200, 200);
            Model.Geometry = mb.ToMesh();
        }
        #endregion

        #region OnMouseDown
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            var direction = this.ToWorld(this.Direction);

            var up = Vector3D.CrossProduct(this.Camera.LookDirection, direction);
            var hitPlaneOrigin = this.ToWorld(this.Position);
            this.HitPlaneNormal = Vector3D.CrossProduct(up, direction);
            var p = e.GetPosition(this.ParentViewport);

            var np = this.GetNearestPoint(p, hitPlaneOrigin, this.HitPlaneNormal, new Vector3D(0, 0, 1));

            if (np == null) return;
        }
        #endregion


        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.IsMouseCaptured)
            {
                var hitPlaneOrigin = this.ToWorld(this.Position);
                var p = e.GetPosition(this.ParentViewport);


                var nearestPoint = this.GetNearestPoint(p, hitPlaneOrigin, this.HitPlaneNormal, new Vector3D(0, 0, 1));
                if (nearestPoint == null) return;
                    
                var delta = this.ToLocal(nearestPoint.Value) - this.lastPointX;

                if (this.TargetTransform != null)
                {
                    var translatetransform = new TranslateTransform3D(delta);
                    this.TargetTransform = Transform3DHelper.CombineTransform(translatetransform, this.TargetTransform);
                }
                else
                    this.Position += delta;

                nearestPoint = this.GetNearestPoint(p, hitPlaneOrigin, this.HitPlaneNormal, new Vector3D(0, 0, 1));

                if (nearestPoint != null)
                    this.lastPointX = this.ToLocal(nearestPoint.Value);

            }
        }

        #region GetNearestPoint
        /// <summary>
        /// Gets the nearest point on the translation axis.
        /// </summary>
        /// <param name="position">
        /// The position (in screen coordinates).
        /// </param>
        /// <param name="hitPlaneOrigin">
        /// The hit plane origin (world coordinate system).
        /// </param>
        /// <param name="hitPlaneNormal">
        /// The hit plane normal (world coordinate system).
        /// </param>
        /// <returns>
        /// The nearest point (world coordinates) or null if no point could be found.
        /// </returns>
        private Point3D? GetNearestPoint(Point position, Point3D hitPlaneOrigin, Vector3D hitPlaneNormal, Vector3D direction)
        {
            var hpp = this.GetHitPlanePoint(position, hitPlaneOrigin, hitPlaneNormal);
            if (hpp == null)
                return null;

            var ray = new Ray3D(this.ToWorld(this.Position), this.ToWorld(direction));

            return ray.GetNearest(hpp.Value);
        }
        #endregion

        #endregion

    }

}
