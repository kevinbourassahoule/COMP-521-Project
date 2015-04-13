using UnityEngine;
using System.Collections.Generic;

public class EndPoint
{
	public Vector2 Position { get; set; }
	public bool    Begin    { get; set; }
	public Segment Segment  { get; set; }
	public float   Angle    { get; set; }
	
	public EndPoint()
	{
		Position = Vector2.zero;
		Begin = false;
		Segment = null;
		Angle = 0;
	}
	
	public bool IsLeftOf(EndPoint other, Vector2 point)
	{
		float cross = (other.Position.x - this.Position.x) * (point.y - this.Position.y)
			        - (other.Position.y - this.Position.y) * (point.x - this.Position.x);
		
		return cross < 0;
	}
}

public class EndPointComparer : IComparer<EndPoint>
{
	public EndPointComparer() {}
	
	public int Compare(EndPoint a, EndPoint b)
	{
		if (a.Angle > b.Angle) { return 1; }
		if (a.Angle < b.Angle) { return -1; }
		if (!a.Begin && b.Begin) { return 1; }
		if (a.Begin && !b.Begin) { return -1; }
		
		return 0;
	}
}