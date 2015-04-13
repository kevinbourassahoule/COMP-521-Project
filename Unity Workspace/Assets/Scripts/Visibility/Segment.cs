using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Segment
{
	public EndPoint P1 { get; set; }
	public EndPoint P2 { get; set; }

	public Segment()
	{
		P1 = null;
		P2 = null;
	}
	
	public static Vector2 IntersectSegments(Vector2 s1p1, Vector2 s1p2, Vector2 s2p1, Vector2 s2p2)
	{
		var intersectionPoint = ((s2p2.x - s2p1.x) * (s1p1.y - s2p1.y) - (s2p2.y - s2p1.y) * (s1p1.x - s2p1.x))
			                  / ((s2p2.y - s2p1.y) * (s1p2.x - s1p1.x) - (s2p2.x - s2p1.x) * (s1p2.y - s1p1.y));
								
		return new Vector2(s1p1.x + intersectionPoint * (s1p2.x - s1p1.x), s1p1.y + intersectionPoint * (s1p2.y - s1p1.y));
	}
	
	public bool InFrontOf(Segment other, Vector2 viewPoint)
	{
		// NOTE: we slightly shorten the segments so that
		// intersections of the endpoints (common) don't count as
		// intersections in this algorithm
		
		bool a1 = this.P2.IsLeftOf(this.P1, Vector2.Lerp(other.P1.Position, other.P2.Position, 0.01f));
		bool a2 = this.P2.IsLeftOf(this.P1, Vector2.Lerp(other.P2.Position, other.P1.Position, 0.01f));
		bool a3 = this.P2.IsLeftOf(this.P1, viewPoint);
		
		bool b1 = other.P2.IsLeftOf(other.P1, Vector2.Lerp(this.P1.Position, this.P2.Position, 0.01f));
		bool b2 = other.P2.IsLeftOf(other.P1, Vector2.Lerp(this.P2.Position, this.P1.Position, 0.01f));
		bool b3 = other.P2.IsLeftOf(other.P1, viewPoint);                        
		
		// B is in between the viewer and A.
		if (b1 == b2 && b2 != b3) return true;
		if (a1 == a2 && a2 == a3) return true;
		if (a1 == a2 && a2 != a3) return false;
		if (b1 == b2 && b2 == b3) return false;
		return false;
	}
}