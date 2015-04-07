using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisibilityPolygon 
{
	private Vector3 viewPosition;
	private Transform walls;
	private Transform player;
	//takes in the position of the player and the parent game object that has all the walls as its children
	private List<KeyValuePair<Vector2,float>> sortWalls(Vector3 player,Transform walls){
		List<KeyValuePair<Vector2,float>> wallPoints = new List<KeyValuePair<Vector2,float>> ();
		foreach (Transform wall in walls) {
			//add to the list the key value pair, key:bound position; value; angular value
			WallBounds wb = wall.GetComponent<WallBounds>();
			wallPoints.Add(new KeyValuePair<Vector2, float>(wb.botLeft,Mathf.Atan2(wb.botLeft.y - player.y,wb.botLeft.x - player.x)));
			wallPoints.Add(new KeyValuePair<Vector2, float>(wb.botRight,Mathf.Atan2(wb.botRight.y - player.y,wb.botRight.x - player.x)));
			wallPoints.Add(new KeyValuePair<Vector2, float>(wb.topLeft,Mathf.Atan2(wb.topLeft.y - player.y,wb.topLeft.x - player.x)));
			wallPoints.Add(new KeyValuePair<Vector2, float>(wb.topRight,Mathf.Atan2(wb.topRight.y - player.y,wb.topRight.x - player.x)));
		}
		//sorts the list by value clockwise
		wallPoints.Sort ((firstPair,nextPair) =>
		{
			return firstPair.Value.CompareTo (nextPair.Value);
		}
		);
		return wallPoints;

	}
	
	// Constructor
	public VisibilityPolygon(AbstractPlayer playerClass, Transform walls)
	{
		this.player = playerClass.transform;
		this.viewPosition = playerClass.transform.position;
		this.walls = walls;
	}
	
	//takes in the position of the player and the parent game object that has all the walls as its children
	public void RecomputePolygon(Vector3 curViewPosition)
	{
		viewPosition = curViewPosition;
		List<Vector3> polygonVertices = new List<Vector3> ();
		polygonVertices.Add(viewPosition);
		//sort the wall points
		List<KeyValuePair<Vector2,float>> sortedWallPoints = sortWalls (viewPosition, walls);
		foreach (KeyValuePair<Vector2,float> wallPoint in sortedWallPoints) {
			RaycastHit2D hit =(Physics2D.Raycast(viewPosition,(wallPoint.Key - (Vector2)viewPosition).normalized,100.0f));
			if(hit.collider != null){
			//maybe loosen up equality, if it hit the target wallpoint shoot another ray until failure
				RaycastHit2D nextHit;
				polygonVertices.Add(hit.point);
				if(Vector2.Distance(hit.point, wallPoint.Key) <= Vector2.kEpsilon){
					nextHit = (Physics2D.Raycast(wallPoint.Key,(wallPoint.Key - (Vector2)viewPosition).normalized,100.0f));
						//if theres a hit, update new starting point and last hit point
						if(nextHit.collider != null){
							hit = nextHit;
							polygonVertices.Add(hit.point);
							
						}
				}
				//hit now contains last collider on original path

				Debug.DrawLine(curViewPosition,hit.point);
			}
		}
		createPolygonMesh(polygonVertices);
	}
	private void createPolygonMesh(List<Vector3> polygonVertices){
		Transform polygon = player.GetChild (0);
		MeshFilter mf = polygon.GetComponent<MeshFilter> ();
		Mesh mesh = mf.mesh;
		mesh.Clear();
		mesh = new Mesh ();
		mf.mesh = mesh;

		//vertices
		mesh.vertices = polygonVertices.ToArray ();

		//triangles
		List <int> triangleVertices = new List<int> ();
		for (int i = 0; i < mesh.vertices.Length -1; i++) {
			triangleVertices.Add(0);
			triangleVertices.Add(i);
			triangleVertices.Add(i+1);
		}
		mesh.triangles = triangleVertices.ToArray ();

		//normals, all transform.forward
		Vector3[] normals = new Vector3[mesh.vertices.Length];
		for (int i = 0; i < normals.Length; i++) {
			normals[i] = Vector3.forward;
		}
		mesh.normals = normals;
		//uv
		Vector2[] uv = new Vector2[mesh.vertices.Length];
		for (int i = 0; i < uv.Length; i++) {
			uv[i] = Vector2.one;
		}
		mesh.uv = uv;
	}
}
