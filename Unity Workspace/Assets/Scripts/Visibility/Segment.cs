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
	
	public override bool Equals(object obj)
	{
		if (obj is Segment)
		{
			Segment other = obj as Segment;
			
			return  P1.Equals(other.P1) && P2.Equals(other.P2);
		}
		
		return false;
	}
	
	public override int GetHashCode()
	{
		return  P1.GetHashCode() + P2.GetHashCode();
	}
	
	public override string ToString()
	{
		return "{" + P1.Position.ToString() + ", " + P2.Position.ToString() + "}";
	}
}