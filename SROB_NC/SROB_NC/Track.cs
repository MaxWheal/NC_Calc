using Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuration.RestrictiveAreas;
using Configuration.Parameters;

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

        public List<RestrictiveArea> RelevantAreas = new List<RestrictiveArea>();

        public List<Point_4D> Waypoints = new List<Point_4D>();

        public List<Segment_4D> ResultSegments = new List<Segment_4D>();

        public List<Point_4D> ResultPoints = new List<Point_4D>();

        public double Length => ResultSegments.Sum(x => x.Length);

        #endregion

        #region Structs and Enums
        #endregion

        #region Methods

        #region Solve

        public bool Solve(Size movingSize, out List<Point_4D> motionTable, List<Point_4D> wayPoints = null, List<RestrictiveArea> relevantAreas = null)
        {
            motionTable = null;
            ResultSegments.Clear();
            ResultPoints.Clear();

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

                #region Check if calculation is possible

                //Check inputs
                if (movingSize.Length <= 0 || movingSize.Width <= 0 || movingSize.Height <= 0)
                    return false;

                MovingSize = movingSize;

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

                var motionPointList = new List<Point_4D>();

                CurrentPosition = new Point_4D(Waypoints[0]);
                CurrentEndPosition = new Point_4D(Waypoints[Waypoints.Count - 1]);


                motionPointList.Add(new Point_4D(CurrentPosition));
                motionPointList.Add(new Point_4D(CurrentEndPosition));

                #endregion

                #region Check start in collision area

                if (InResArea(out List<RestrictiveArea> collidingAreas, CurrentPosition))
                {
                    if (collidingAreas.All(x => x.AllowedMotion == Axis.Z))
                    {
                        //NextPosition above colliding Areas
                        CurrentPosition.Z = collidingAreas.Max(x => x.Zmax);

                        motionPointList.Insert(1,new Point_4D(CurrentPosition));
                    }
                    else
                        return false;
                }

                #endregion

                #region Check end in collision area
                int endManipulated = 0; // so new points are inserted in the middle

                if (InResArea(out collidingAreas, CurrentEndPosition))
                {
                    if (collidingAreas.All(x => x.AllowedMotion == Axis.Z))
                    {
                        //NextPosition above colliding Areas
                        CurrentEndPosition.Z = collidingAreas.Max(x => x.Zmax);

                        motionPointList.Insert(motionPointList.Count - 1, new Point_4D(CurrentEndPosition));
                        endManipulated = 1;
                    }
                    else
                        return false;
                }

                #endregion

                #region Solve XYC in 2D
                //To be done, now assume that direct line is possible
                #endregion

                #region Solve Z
                var motionSegment = new Segment_4D(CurrentPosition, CurrentEndPosition);

                foreach (var area in RelevantAreas)
                {
                    //get left and entered areas areas by segment intersection

                    foreach (var segment in area.ToPolygon_2D().ToSegments_2D())
                    {
                        if (segment.IsIntersecting(motionSegment))
                        {
                            var midPoint = FindFreeMovementPoint(segment, motionSegment);

                            midPoint.Z = Math.Max(midPoint.Z, area.Zmax);

                            if (!motionPointList.Contains(midPoint))
                                motionPointList.Insert(motionPointList.Count - 1 - endManipulated, midPoint);
                        }
                    }

                }

                #endregion

                #region Copy Points to result

                motionTable = new List<Point_4D>();

                foreach (var point in motionPointList)
                {
                    motionTable.Add(point);

                    ResultPoints.Add(point);

                    //add Segments
                    if (motionTable.Count < 2)
                        continue;

                    ResultSegments.Add(new Segment_4D(motionTable[motionTable.Count - 2], motionTable[motionTable.Count - 1]));
                }

                return true;

                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
        #endregion

        #region findFreeMovementPoint

        private Point_4D FindFreeMovementPoint(Segment_2D collidingSegment, Segment_4D testSegment)
        {
            Point_4D testPoint = null;
            try
            {
                Axis testAxis = collidingSegment.Slope == 0 ? Axis.Y : Axis.X;
                int direction = testSegment.GetDirectionOf(testAxis);
                double startPoint = collidingSegment.Slope == 0 ? collidingSegment.Start.Y : collidingSegment.Start.X;

                testPoint = testSegment.GetPositionAt(testAxis, startPoint);
                Polygon_2D testPolygon = new Polygon_2D(testPoint, MovingSize);

                if (testAxis == Axis.Y)
                {
                    //move on in direction of motionsegment and collision
                    while ((testPolygon.PointMax.Y > startPoint && direction < 0) ||
                        (testPolygon.PointMin.Y < startPoint && direction > 0))
                    {
                        testPoint = testSegment.GetPositionAt(testAxis, testPoint.Y + 10 * direction);
                        testPolygon = new Polygon_2D(testPoint, MovingSize);

                        //motion already left restricted area ToDo?

                        //Avoid endless run
                        if (testPoint.Y > ParameterCollecion.GetPoint4D("PAR_SW_ES_MAX").Y)
                            return null;

                        if (testPoint.Y < ParameterCollecion.GetPoint4D("PAR_SW_ES_MIN").Y)
                            return null;
                    }
                }
                else if (testAxis == Axis.X)
                {

                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return testPoint;
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

                if (movingPolygon.IsOverlapping(area.ToPolygon_2D()))
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

}