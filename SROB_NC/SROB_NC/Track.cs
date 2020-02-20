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
        private Point_4D CurrentPosiotion { get; set; }

        private Size MovingSize;

        public Polygon_2D MovingPolygon { get => new Polygon_2D(CurrentPosiotion, MovingSize); }

        public List<RestrictiveArea> RelevantAreas = new List<RestrictiveArea>();

        public List<Point_4D> Waypoints = new List<Point_4D>();
        #endregion

        #region Methods

        #region Solve

        public bool Solve(Size movingSize, out List<Point_4D> motionTable, List<Point_4D> wayPoints = null, List<RestrictiveArea> relevantAreas = null)
        {
            motionTable = null;

            try
            {
                #region Initialize calculation

                //Override properties by parameter
                if (wayPoints != null)
                    Waypoints = wayPoints;

                if (relevantAreas != null)
                    RelevantAreas = relevantAreas;

                if (RelevantAreas == null)
                    //calculation with no Restricive Areas possible (meaningful)?
                    RelevantAreas = new List<RestrictiveArea>();

                #endregion

                #region Check if calculation is 

                //Check inputs
                if (movingSize.Length <= 0 || movingSize.Width <= 0 || movingSize.Height <= 0)
                    return false;

                if (wayPoints.Count < 2)
                    return false;

                #endregion

                #region Check if calculation is necassary
                //Check if motion is necassary
                bool allEqual = true;
                foreach (var points in wayPoints)
                {
                    if (wayPoints[0] != points)
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

                motionTable = new List<Point_4D>();

                CurrentPosiotion = Waypoints[0];


                #endregion

                #region Startpoint inside of Restrictive Area

                if (InResArea(CurrentPosiotion))
                { }

                #endregion

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
        private bool InResArea(Point_4D position = null)
        {
                //use current position if none is given by parameter
            if (position == null)
                position = CurrentPosiotion;

            foreach (var area in RelevantAreas)
            {
                //Position is over or under given height
                if (position.Z > area.Zmax || position.Z < area.Zmin - MovingSize.Height)
                    continue;

                if (Polygon_2D.AreOverlapping(MovingPolygon, area.To2DPolygon)
                    return true;
            }

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

}
