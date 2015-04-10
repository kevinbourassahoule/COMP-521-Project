using System.Collections.Generic;

public class EndPointComparer : IComparer<EndPoint>
{
    public EndPointComparer() { }
    
    public int Compare(EndPoint a, EndPoint b)
    {
        if (a.Angle > b.Angle) { return 1; }
        if (a.Angle < b.Angle) { return -1; }
        if (!a.Begin && b.Begin) { return 1; }
        if (a.Begin && !b.Begin) { return -1; }

        return 0;
    }
}