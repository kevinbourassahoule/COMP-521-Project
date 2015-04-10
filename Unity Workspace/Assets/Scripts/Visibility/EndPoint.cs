using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class EndPoint
{
	public Vector2 Position { get; set; }
	public bool    Begin { get; set; }
	public Segment Segment { get; set; }
	public float   Angle { get; set; }
	
	public EndPoint()
	{
		Position = Vector2.zero;
		Begin = false;
		Segment = null;
		Angle = 0;
	}
	
	public override bool Equals(object obj)
	{
		if(obj is EndPoint)
		{
			EndPoint other = (EndPoint)obj;
			
			return Position.Equals(other.Position) && Begin.Equals(other.Begin) && Angle.Equals(other.Angle);
		}
		
		return false;
	}
	
	public override int GetHashCode()
	{
		return Position.GetHashCode() + Begin.GetHashCode() + Angle.GetHashCode();
	}
	
	public override string ToString()
	{
		return "{ p:" + Position.ToString() + "a: " + Angle + " in " + Segment.ToString() + "}";
	}
}