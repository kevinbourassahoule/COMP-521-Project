using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class VisibilityComputer
{
	// The viewer position
	public Vector2 Origin { get; set; }
	
	// Max distance of visibility polygon
	public float Radius { get; private set; }
	
	// The visibility polygon
	private MeshFilter meshFilter;
	
	// The map elements       
	private List<EndPoint> endpoints;
	private List<Segment> segments;
	
	// A radial comparer for sorting endpoints 
	private EndPointComparer radialComparer;
	
	
	public VisibilityComputer(Vector2 origin, float radius)
	{
		segments = new List<Segment>();
		endpoints = new List<EndPoint>();
		radialComparer = new EndPointComparer();
		
		// Instantiate mesh
		meshFilter = GameObject.FindGameObjectWithTag("Visibility").GetComponent<MeshFilter>();
		meshFilter.mesh = new Mesh();
		
		// Instantiate segments
		foreach (Wall wall in Environment.Walls)
		{
			AddSegment(wall.botLeft + (wall.botRight - wall.botLeft) * .5f,
			           wall.topLeft + (wall.topRight - wall.topLeft) * .5f);
		}
		
		this.Origin = origin;
		this.Radius = radius;            
		LoadBoundaries();
	}
	
	/*
	 * Adds a segment to the visibility polygon.
	 */
	private void AddSegment(Vector2 p1, Vector2 p2)
	{
		Segment segment    = new Segment();
		EndPoint endPoint1 = new EndPoint();
		EndPoint endPoint2 = new EndPoint();
		
		// Initialize endpoints
		endPoint1.Position = p1;
		endPoint1.Segment = segment;
		endPoint2.Position = p2;
		endPoint2.Segment = segment;
		
		// Initialize segment
		segment.P1 = endPoint1;
		segment.P2 = endPoint2;
		
		// Add segment and endpoints to data structure
		segments.Add(segment);
		endpoints.Add(endPoint1);
		endpoints.Add(endPoint2);
	}
	
	/*
	 * Loads the boundaries of the visibility polygon (maximum visibility)
	 */  
	private void LoadBoundaries()
	{
		// Top
		AddSegment(new Vector2(Origin.x - Radius, Origin.y - Radius),
		           new Vector2(Origin.x + Radius, Origin.y - Radius));
		
		// Bottom
		AddSegment(new Vector2(Origin.x - Radius, Origin.y + Radius),
		           new Vector2(Origin.x + Radius, Origin.y + Radius));
		
		// Left
		AddSegment(new Vector2(Origin.x - Radius, Origin.y - Radius),
		           new Vector2(Origin.x - Radius, Origin.y + Radius));
		
		// Right
		AddSegment(new Vector2(Origin.x + Radius, Origin.y - Radius),
		           new Vector2(Origin.x + Radius, Origin.y + Radius));
	}        
	
	/*
	 * Updates segments' values for radial sorting.
	 */
	private void UpdateSegments()
	{            
		foreach(Segment segment in segments)
		{
			// Update angles
			segment.P1.Angle = (float)Math.Atan2(segment.P1.Position.y - Origin.y,
			                                     segment.P1.Position.x - Origin.x);
			segment.P2.Angle = (float)Math.Atan2(segment.P2.Position.y - Origin.y,
			                                     segment.P2.Position.x - Origin.x);
			
			// Map angle between -Pi and Pi
			float dAngle = segment.P2.Angle - segment.P1.Angle;
			if (dAngle <= -Mathf.PI) { dAngle += 2 * Mathf.PI; }
			if (dAngle > Mathf.PI) { dAngle -= 2 * Mathf.PI; }
			
			// Update which endpoint is the segment's start
			segment.P1.Begin = (dAngle > 0.0f);
			segment.P2.Begin = !segment.P1.Begin;                
		}
	}               
	
	/*
	 * Computes the visibility polygon and updates the visibility mesh.
	 */
	public void Compute()
	{
		List<Vector2> meshVertices = new List<Vector2>();
		LinkedList<Segment> open = new LinkedList<Segment>();
		
		UpdateSegments();
		endpoints.Sort(radialComparer);
		
		float currentAngle = 0;
		
		for (int pass = 0; pass < 2; pass++)
		{
			foreach(EndPoint p in endpoints)
			{
				Segment currentOld = (open.Count == 0 ? null : open.First.Value);
				
				if (p.Begin)                    
				{
					// Insert into the right place in the list
					var node = open.First;
					while (node != null && p.Segment.InFrontOf(node.Value, Origin))
					{
						node = node.Next;
					}
					
					if (node == null)
					{
						open.AddLast(p.Segment);
					}
					else
					{
						open.AddBefore(node, p.Segment);
					}
				}
				else
				{
					open.Remove(p.Segment);
				}
				
				Segment currentNew = null;
				if(open.Count != 0)
				{                
					currentNew = open.First.Value;
				}
				
				if(currentOld != currentNew)
				{
					if(pass == 1)
					{
						AddTriangle(meshVertices, currentAngle, p.Angle, currentOld);
					}
					currentAngle = p.Angle;
				}
			}
		}
		
		// Clear mesh
		meshFilter.mesh.Clear();
		
		// Add origin to mesh vertices
		meshVertices.Insert(0, Origin);
		
		// Set mesh's vertices
		meshFilter.mesh.vertices = meshVertices.ConvertAll<Vector3>(delegate(Vector2 input) 
		{
			return (Vector3) input;
		}).ToArray();
		
		// Set mesh's triangle indeces
		List<int> meshTriangles = new List<int>();
		
		for (int i = 0; i < meshVertices.Count - 2; i++)
		{
			meshTriangles.Add(0);
			meshTriangles.Add(i + 2);
			meshTriangles.Add(i + 1);

			Debug.DrawLine(meshVertices[0], meshVertices[i+1]);
			Debug.DrawLine(meshVertices[0], meshVertices[i+2]);
			Debug.DrawLine(meshVertices[i+1], meshVertices[i+2]);
		}
		meshFilter.mesh.triangles = meshTriangles.ToArray();
		
		// Set mesh's normals
		List<Vector3> normals = new List<Vector3> ();
		for (int i = 0; i < meshVertices.Count; i++)
		{
			normals.Add (Vector3.forward);
		}
		meshFilter.mesh.normals = normals.ToArray ();
	}       
	
	/*
	 * Adds outermost endpoints of a triangle to a triangle fan vertex list.
	 */
	private void AddTriangle(List<Vector2> triangles, float angle1, float angle2, Segment segment)
	{
		Vector2 p1 = Origin;
		Vector2 p2 = new Vector2(Origin.x + (float)Math.Cos(angle1), Origin.y + (float)Math.Sin(angle1));
		Vector2 p3 = Vector2.zero;
		Vector2 p4 = Vector2.zero;
		
		if(segment != null)
		{
			// Stop the triangle at the intersecting segment
			p3.x = segment.P1.Position.x;
			p3.y = segment.P1.Position.y;
			p4.x = segment.P2.Position.x;
			p4.y = segment.P2.Position.y;
		}
		else {
			p3.x = Origin.x + (float)Math.Cos(angle1) * Radius * 2;
			p3.y = Origin.y + (float)Math.Sin(angle1) * Radius * 2;
			p4.x = Origin.x + (float)Math.Cos(angle2) * Radius * 2;
			p4.y = Origin.y + (float)Math.Sin(angle2) * Radius * 2;
		}
		
		Vector2 pBegin = Segment.IntersectSegments(p3, p4, p1, p2);
		
		p2.x = Origin.x + (float)Math.Cos(angle2);
		p2.y = Origin.y + (float)Math.Sin(angle2);
		
		Vector2 pEnd = Segment.IntersectSegments(p3, p4, p1, p2);
		
		triangles.Add(pBegin);
		triangles.Add(pEnd);
	}
}