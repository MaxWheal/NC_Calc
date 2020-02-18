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

        public Track(T_P_4D start, T_P_4D end)
        {
            Start = start;
            End = end;
        }
        #endregion

        #region Properties

        public T_P_4D Start { get; set; }
        public T_P_4D End { get; set; }

        public List<T_P_4D> Points = new List<T_P_4D>();

        #endregion

        #region Methods

        #region ToString
        /// <summary>
        /// Returns a string to represent the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Points.Count < 2) { return "null"; }

            return $"{Points.Count} Points from {Points[0]} to {Points[Points.Count - 1]}";
        }
        #endregion

        #endregion
    }
}
