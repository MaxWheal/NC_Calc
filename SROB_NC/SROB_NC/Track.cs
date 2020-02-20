using Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<T_P_4D> Waypoints = new List<T_P_4D>();

        public Polygon_2D MovingPolygon { get; set; }

        public Size MovingSize;
        #endregion

        #region Methods

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
