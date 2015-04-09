using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisibilityPolygon 
{
	private Vector3 viewPosition;
	private Wall[] walls;
	private List<KeyValuePair<Vector2,float>> sortedWallPoints;
	//takes in the position of the player and the parent game object that has all the walls as its children

	private List<KeyValuePair<Vector2,float>> sortWalls(Vector3 player,Wall[] walls){
		List<KeyValuePair<Vector2,float>> wallPoints = new List<KeyValuePair<Vector2,float>> ();
		foreach (Wall wb in walls) {
			//add to the list the key value pair, key:bound position; value; angular value
			wallPoints.Add(new KeyValuePair<Vector2, float>(wb.botLeft,Mathf.Atan2(wb.botLeft.y - player.y,wb.botLeft.x - player.x)));
			wallPoints.Add(new KeyValuePair<Vector2, float>(wb.botRight,Mathf.Atan2(wb.botRight.y - player.y,wb.botRight.x - player.x)));
			wallPoints.Add(new KeyValuePair<Vector2, float>(wb.topLeft,Mathf.Atan2(wb.topLeft.y - player.y,wb.topLeft.x - player.x)));
			wallPoints.Add(new KeyValuePair<Vector2, float>(wb.topRight,Mathf.Atan2(wb.topRight.y - player.y,wb.topRight.x - player.x)));
		}
		//sorts the list by value counterclockwise
		wallPoints.Sort ((firstPair,nextPair) =>
		{
			return firstPair.Value.CompareTo (nextPair.Value);
		}
		);
		return wallPoints;

	}

	public VisibilityPolygon(AbstractPlayer player, Wall[] walls)
	{
		this.viewPosition = player.transform.position;
		this.walls = walls;
	}
	
	//takes in the position of the player, computes the visibility polygon vertices, and displays it
	public void RecomputePolygon(Vector3 curViewPosition)
	{
		viewPosition = curViewPosition;
		List<Vector3> polygonVertices = new List<Vector3> ();
		polygonVertices.Add(viewPosition);
		//sort the wall points
		sortedWallPoints = sortWalls (viewPosition, walls);
		foreach (KeyValuePair<Vector2,float> wallPoint in sortedWallPoints) {
			List<Vector3> curHits = getAllHits((Vector2) curViewPosition,(wallPoint.Key - (Vector2)curViewPosition).normalized);
			if(curHits.Count > 0)
				polygonVertices.AddRange(curHits);
		}
		createPolygonMesh(polygonVertices);
	}

	/*takes in a start position and a direction, returns a list of all the hits along the way */
	private List<Vector3> getAllHits(Vector2 start,Vector2 direction){
		RaycastHit2D Hit;
		List<Vector3> hitPoints = new List<Vector3> ();
		Hit = (Physics2D.CircleCast(start,0.001f,direction,100.0f,LayerMask.GetMask("Wall")));
		if(Hit.collider != null && Hit.distance > Vector2.kEpsilon){
			RaycastHit2D nextHit;
			nextHit = (Physics2D.Raycast(Hit.point + .05f*direction,direction,100.0f,LayerMask.GetMask("Wall")));
			if(nextHit.collider != null && nextHit.distance > Vector2.kEpsilon){
				//calculate the angle from the player to the wall position
				float transformAngle = Mathf.Atan2(Hit.collider.transform.position.y - viewPosition.y,Hit.collider.transform.position.x - viewPosition.x);
				//calculate angle from player to our raycast hit
				float hitAngle  = Mathf.Atan2(Hit.point.y - viewPosition.y,Hit.point.x - viewPosition.x);
				//decide which hit to store first.
				if(transformAngle > hitAngle){
					hitPoints.Add (nextHit.point);
					List<Vector3> tempList = getAllHits(nextHit.point,direction);
					if(tempList.Count > 0)
						hitPoints.AddRange(tempList);
					hitPoints.Add (Hit.point);
				}
				else{
					hitPoints.Add (Hit.point);
					List<Vector3> tempList = getAllHits(nextHit.point,direction);
					if(tempList.Count > 0)
						hitPoints.AddRange(tempList);
					hitPoints.Add (nextHit.point);
				}
			}

			else{//didnt hit a corner just add the point
				hitPoints.Add(Hit.point);
			}

		}
		return hitPoints;
	}
	private void createPolygonMesh(List<Vector3> polygonVertices){
		Transform polygon = GameObject.FindGameObjectWithTag("Visibility").transform;
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
			//if(Vector2. ((mesh.vertices[i]-mesh.vertices[0]).normalized-(mesh.vertices[i+1]-mesh.vertices[0]).normalized) >.1f){
				triangleVertices.Add(0);

				triangleVertices.Add(i+1);
				triangleVertices.Add(i);
			//}

		}
		triangleVertices.Add (0);

		triangleVertices.Add (1);
		triangleVertices.Add (mesh.vertices.Length-1);
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
