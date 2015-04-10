using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class VisibilityPolygon2
{
	private GameObject viewer;
	private PolygonCollider2D[] obstacles;
	
	// The mesh representation of the polygon
	private Mesh mesh;
	private MeshFilter meshFilter;
	
	public VisibilityPolygon2(GameObject viewer, PolygonCollider2D[] obstacles)
	{
		this.viewer = viewer;
		this.obstacles = obstacles;
		
		meshFilter = GameObject.FindGameObjectWithTag("Visibility").GetComponent<MeshFilter>();
		meshFilter.mesh = new Mesh();
	}
	
	/*
	 * Recomputes the visibility polygon and displays it.
	 */
	public void RecomputeVision()
	{
		Vector3 currPlayerPosition = viewer.transform.position;
		AngleComparer angleComparer = new AngleComparer(currPlayerPosition);
		
		// Get raycast hits for each obstacle
		List<VisibilityHit[]> obstacleHits = new List<VisibilityHit[]>();
		foreach (PolygonCollider2D obstacle in obstacles)
		{
			RaycastHit2D raycastHit;
			Vector3 raycastDirection;
			VisibilityHit[] visibilityHits = new VisibilityHit[obstacle.points.Length + 2];
			
			// Shoot ray towards each vertex of obstacle
			for (int i = 0; i < obstacle.points.Length; i++)
			{
				raycastDirection = obstacle.transform.TransformPoint(obstacle.points[i]) - currPlayerPosition;
				
				raycastHit = Physics2D.Raycast(currPlayerPosition,
				                        	   raycastDirection,
				                        	   Mathf.Infinity,
											   LayerMask.GetMask("Wall"));
											   
				visibilityHits[i + 1] = new VisibilityHit(raycastHit.point);
				
				Debug.DrawRay(currPlayerPosition, visibilityHits[i + 1].point - currPlayerPosition);
			}
			
			// Sort hits 
			Array.Sort(visibilityHits, 1, visibilityHits.Length - 2, angleComparer);
			visibilityHits[1].isLeftEndpoint = true;
			visibilityHits[visibilityHits.Length - 2].isRightEndpoint = true;
			
			// Handle endpoints
			raycastDirection = obstacle.transform.TransformPoint(obstacle.points[0]) - currPlayerPosition;
			raycastHit = Physics2D.Raycast(visibilityHits[1].point + raycastDirection.normalized * .01f,
				                    raycastDirection,
			                        Mathf.Infinity,
				                    LayerMask.GetMask("Wall"));
			visibilityHits[0] = new VisibilityHit(raycastHit.point);
//			Debug.DrawRay(visibilityHits[1].point + raycastDirection.normalized * .1f, raycastDirection);
			
			raycastDirection = obstacle.transform.TransformPoint(obstacle.points[obstacle.pathCount - 1]) - currPlayerPosition;
			raycastHit = Physics2D.Raycast(visibilityHits[visibilityHits.Length - 1].point + raycastDirection * .01f,
                                    raycastDirection,
                    				Mathf.Infinity,
                                    LayerMask.GetMask("Wall"));
			visibilityHits[visibilityHits.Length - 1]= new VisibilityHit(raycastHit.point);
//			Debug.DrawRay(visibilityHits[visibilityHits.Length - 1].point + raycastDirection.normalized * .1f, raycastDirection);
			
			// Add sorted hits to the list of hits by obstacle
			obstacleHits.Add(visibilityHits);
		}
		
		// Flatten list to get mesh points
		List<VisibilityHit> meshVertices = new List<VisibilityHit>();
		foreach (VisibilityHit[] hits in obstacleHits)
		{
			foreach(VisibilityHit hit in hits)
			{
				meshVertices.Add(hit);
			}
		}
		
		// Sort entire list
		meshVertices.Sort(angleComparer);
		
		// Add player current position as point
		meshVertices.Insert(0, new VisibilityHit(currPlayerPosition));
		
		// Create array of triangle indeces
		int[] meshTriangles = new int[(meshVertices.Count - 2) * 3];
		for (int i = 0, j = 0; i < meshVertices.Count - 2; i++, j += 3)
		{
			meshTriangles[j]     = 0;
			meshTriangles[j + 1] = i + 1;
			meshTriangles[j + 2] = i + 2;
		}
		
		// Generate mesh
		meshFilter.mesh.Clear();
		meshFilter.mesh.vertices = meshVertices.ConvertAll(new Converter<VisibilityHit, Vector3>(VisibilityHitToVector3)).ToArray();
		meshFilter.mesh.triangles = meshTriangles;
		meshFilter.mesh.RecalculateNormals();	// TODO optimize?
	}
	
	struct VisibilityHit
	{
		public VisibilityHit(Vector3 point, bool isLeftEndpoint = false, bool isRightEndpoint = false)
		{
			this.point = point;
			this.isLeftEndpoint = isLeftEndpoint;
			this.isRightEndpoint = isRightEndpoint;
		}
	
		public Vector3 point;
		public bool isLeftEndpoint;
		public bool isRightEndpoint;
	}
	
	private static Vector3 VisibilityHitToVector3(VisibilityHit hit)
	{
		return hit.point;
	}
	
	class AngleComparer : IComparer<VisibilityHit>
	{
		private Vector3 viewerPosition;
		
		public AngleComparer(Vector3 viewerPosition)
		{
			this.viewerPosition = viewerPosition;
		}
	
		int IComparer<VisibilityHit>.Compare(VisibilityHit v1, VisibilityHit v2)
		{
			float v1Angle = Vector3.Angle(-Vector3.right, v1.point - viewerPosition);
			if (v1.point.y < 0) v1Angle = 360 - v1Angle;
			
			float v2Angle = Vector3.Angle(-Vector3.right, v2.point - viewerPosition);
			if (v2.point.y < 0) v2Angle = 360 - v1Angle;
			
			if (v1Angle == v2Angle)
			{
				if (v1.isLeftEndpoint || v2.isLeftEndpoint)
				{
					if ((v1.point - viewerPosition).sqrMagnitude > (v2.point - viewerPosition).sqrMagnitude)
					{
						return -1;
					}
					else {
						return 1;
					}
				}
				else {	// isRightPoint
					if ((v1.point - viewerPosition).sqrMagnitude > (v2.point - viewerPosition).sqrMagnitude)
					{
						return 1;
					}
					else {
						return -1;
					}
				}
			}
			else {
				return v1Angle.CompareTo(v2Angle);
			}
		}
	}
}