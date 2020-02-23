using Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuration;

namespace SROB_NC
{
    class Track
    {
        #region Constructors
        public Track()
        {

        }

        #endregion

        #region Properties
        public Point_4D CurrentPosition { get; set; }

        private Point_4D CurrentEndPosition { get; set; }

        public Size MovingSize;

        public Polygon_2D MovingPolygon { get => new Polygon_2D(CurrentPosition, MovingSize); }

        public List<RestrictiveArea> RelevantAreas = new List<RestrictiveArea>();

        public List<Point_4D> Waypoints = new List<Point_4D>();

        private int _curMotionNr = -1;
        public int CurMotionNr
        {
            get
            {
                _curMotionNr++;
                return _curMotionNr;
            }
            set { _curMotionNr = value; }
        }

        private int _endMotionNr = 101;
        public int EndMotionNr
        {
            get
            {
                _endMotionNr--;
                return _endMotionNr;
            }
            set { _endMotionNr = value; }
        }

        #endregion

        #region Structs and Enums
        #endregion

        #region Methods

        #region Solve

        public bool Solve(Size movingSize, out List<Point_4D> motionTable, List<Point_4D> wayPoints = null, List<RestrictiveArea> relevantAreas = null)
        {
            motionTable = null;

            try
            {
                #region Initialize calculation
                CurMotionNr = -1;
                EndMotionNr = 101;

                //Override properties by parameter
                if (wayPoints != null)
                    Waypoints = wayPoints;

                if (relevantAreas != null)
                    RelevantAreas = relevantAreas;

                if (RelevantAreas == null)
                    //calculation with no Restricive Areas possible (meaningful)?
                    RelevantAreas = new List<RestrictiveArea>();

                #endregion

                #region Check if calculation is possible

                //Check inputs
                if (movingSize.Length <= 0 || movingSize.Width <= 0 || movingSize.Height <= 0)
                    return false;

                if (Waypoints.Count < 2)
                    return false;

                #endregion

                #region Check if calculation is necassary
                //Check if motion is necassary
                bool allEqual = true;
                foreach (var points in Waypoints)
                {
                    if (Waypoints[0] != points)
                    {
                        allEqual = false;
                        break;
                    }
                }

                if (allEqual)
                {
                    //not null but no points necassary
                    motionTable = new List<Point_4D>();
                    return true;
                }

                #endregion

                #region Start calculation

                var motionPointList = new List<Motionpoint>();

                CurrentPosition = Waypoints[0];
                CurrentEndPosition = Waypoints[Waypoints.Count - 1];


                motionPointList.Add(new Motionpoint(CurrentPosition, CurMotionNr));
                motionPointList.Add(new Motionpoint(CurrentEndPosition, EndMotionNr));

                #endregion

                #region Check start in collision area

                List<RestrictiveArea> collidingAreas;

                if (InResArea(out collidingAreas, CurrentPosition))
                {
                    if (collidingAreas.All(x => x.AllowedMotion == RestrictiveArea.Motion.Z))
                    {
                        //NextPosition above colliding Areas
                        CurrentPosition.Z = collidingAreas.OrderBy(x => x.Zmax).Last().Zmax;

                        motionPointList.Add(new Motionpoint(CurrentPosition, CurMotionNr));
                    }
                    else
                        return false;
                }

                #endregion

                #region Check end in collision area

                if (InResArea(out collidingAreas, CurrentEndPosition))
                {
                    if (collidingAreas.All(x => x.AllowedMotion == RestrictiveArea.Motion.Z))
                    {
                        //NextPosition above colliding Areas
                        CurrentEndPosition.Z = collidingAreas.OrderBy(x => x.Zmax).Last().Zmax;

                        motionPointList.Add(new Motionpoint(CurrentEndPosition, EndMotionNr));
                    }
                    else
                        return false;
                }

                #endregion

                #region Solve XYC in 2D
                //To be done, now assume that direct line is possible
                #endregion

                #region Solve Z

                #endregion

                motionTable = new List<Point_4D>();

                foreach (var item in motionPointList.OrderBy(x => x.MotionNr))
                {
                    motionTable.Add(item.Position);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        #endregion

        #region InResArea
        /// <summary>
        /// Checks if position is in restrictive area
        /// </summary>
        /// <param name="position">Optional: if null Current Position is used</param>
        /// <returns></returns>
        private bool InResArea(out List<RestrictiveArea> collidingAreas, Point_4D position = null)
        {
            //use current position if none is given by parameter
            if (position == null)
                position = CurrentPosition;

            collidingAreas = new List<RestrictiveArea>();

            var movingPolygon = new Polygon_2D(position, MovingSize);

            foreach (var area in RelevantAreas)
            {
                //Position is over or under given height
                if (position.Z > area.Zmax || position.Z < area.Zmin - MovingSize.Height)
                    continue;

                if (movingPolygon.IsOverlapping(area.To2DPolygon()))
                    collidingAreas.Add(area);
            }

            if (collidingAreas.Count > 0)
                return true;

            return false;
        }
        #endregion

        #region ToString
        /// <summary>
        /// Returns a string to represent the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Waypoints.Count < 2) { return "null"; }

            return $"{Waypoints.Count} Points from {Waypoints[0]} to {Waypoints[Waypoints.Count - 1]}";
        }
        #endregion

        #endregion
    }

    class Motionpoint
    {
        #region Constructors
        public Motionpoint(Point_4D position, int motionNr)
        {
            Position = new Point_4D(position);
            MotionNr = motionNr;
        }
        #endregion

        #region Properties
        public Point_4D Position { get; set; }
        public int MotionNr { get; set; }

        #endregion

    }

}