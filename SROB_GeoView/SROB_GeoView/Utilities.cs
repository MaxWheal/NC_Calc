using System;

namespace SROB_3DViewer
{
    [Flags]
    public enum Axis
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4,
        C = 8
    }

    public enum CoOpModes : short
    {
        NOTDEFINED = 0,
        Seperated = 1,
        Cooperativ = 2
    }

}