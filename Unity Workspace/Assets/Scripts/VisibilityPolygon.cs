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
	/*
	 * 
	 * 
	 private List<KeyValuePair<Vector2,anglePlusWallUp>> sortWalls(Vector3 player,Wall[] walls){
		List<KeyValuePair<Vector2,anglePlusWallUp>> wallPoints = new List<KeyValuePair<Vector2,anglePlusWallUp>> ();
		foreach (Wall wb in walls) {
			anglePlusWallUp botL = new anglePlusWallUp(Mathf.Atan2(wb.botLeft.y - player.y,wb.botLeft.x - player.x),wb.transform.up);
			anglePlusWallUp botR = new anglePlusWallUp(Mathf.Atan2(wb.botRight.y - player.y,wb.botRight.x - player.x),wb.transform.up);
			anglePlusWallUp topL = new anglePlusWallUp(Mathf.Atan2(wb.topLeft.y - player.y,wb.topLeft.x - player.x),wb.transform.up);
			anglePlusWallUp topR = new anglePlusWallUp(Mathf.Atan2(wb.topRight.y - player.y,wb.topRight.x - player.x),wb.transform.up);

			//add to the list the key value pair, key:bound position; value; angular value
			wallPoints.Add(new KeyValuePair<Vector2, anglePlusWallUp>(wb.botLeft,botL));
			wallPoints.Add(new KeyValuePair<Vector2, anglePlusWallUp>(wb.botRight,botR));
			wallPoints.Add(new KeyValuePair<Vector2, anglePlusWallUp>(wb.topLeft,topL));
			wallPoints.Add(new KeyValuePair<Vector2, anglePlusWallUp>(wb.topRight,topR));
		}
		//sorts the list by value clockwise
		wallPoints.Sort ((firstPair,nextPair) =>
		{
			return firstPair.Value.angle.CompareTo (nextPair.Value.angle);
		}
		);
		return wallPoints;

	}*/
	
	// Constructor
	public VisibilityPolygon(AbstractPlayer player, Wall[] walls)
	{
		this.viewPosition = player.transform.position;
		this.walls = walls;
	}
	
	//takes in the position of the player and the parent game object that has all the walls as its children
	public void RecomputePolygon(Vector3 curViewPosition)
	{
		viewPosition = curViewPosition;
		List<Vector3> polygonVertices = new List<Vector3> ();
		polygonVertices.Add(viewPosition);
		//sort the wall points
		sortedWallPoints = sortWalls (viewPosition, walls);
		foreach (KeyValuePair<Vector2,float> wallPoint in sortedWallPoints) {
			//Debug.Log (wallPoint.Key);
			//RaycastHit2D hit =(Physics2D.Raycast(viewPosition,(wallPoint.Key - (Vector2)viewPosition).normalized,100.0f, LayerMask.GetMask("Wall")));
			//if(hit.collider != null){
			//maybe loosen up equality, if it hit the target wallpoint shoot another ray until failure
			List<Vector3> curHits = getAllHits((Vector2) curViewPosition,wallPoint.Key,(wallPoint.Key - (Vector2)curViewPosition).normalized);
			if(curHits.Count > 0)
				polygonVertices.AddRange(curHits);
			//}
		}
		createPolygonMesh(polygonVertices);
	}
	
	private List<Vector3> getAllHits(Vector2 start,Vector2 target,Vector2 direction){
		RaycastHit2D Hit;
		List<Vector3> hitPoints = new List<Vector3> ();
		Hit = (Physics2D.CircleCast(start,0.001f,direction,100.0f,LayerMask.GetMask("Wall")));
		//if theres a hit, update new starting point and last hit point
		if(Hit.collider != null){
			//hit = nextHit;
			//Debug.DrawLine(start,Hit.point);
			//Debug.DrawLine(viewPosition,Hit.point);
			if(Vector2.Distance(Hit.point,target) <= .1f){//hit corner
				/*bool hitAnotherCorner = false;
				foreach (KeyValuePair<Vector2,float> wallPoint in sortedWallPoints) {
					//if youre in front of the hit, and youre in the same direction
					if(wallPoint.Key != start && Vector2.Dot((wallPoint.Key - (Vector2)viewPosition).normalized, direction) == 1){
						List<Vector3> curHits = getAllHits(Hit.point,wallPoint.Key,direction);
						if(curHits.Count > 0)
						hitPoints.AddRange(curHits);
						//hitPoints.Add(Hit.point);
						hitAnotherCorner = true;
					}
				}
				if(!hitAnotherCorner){
				*/

				RaycastHit2D nextHit;
				nextHit = (Physics2D.Raycast(Hit.point,/*.001f,*/direction,100.0f,LayerMask.GetMask("Wall")));
				if(nextHit.collider != null && nextHit.distance > Vector2.kEpsilon){
					//Vector2 projHit = Hit.point + (Vector2)((Vector2.Dot(Hit.collider.transform.up,direction)/*/Hit.collider.transform.up.magnitude*/) * Hit.collider.transform.up); 
					//Vector2 viewToProjHit = (projHit - (Vector2)viewPosition).normalized;
					Vector2 viewToWall = ((Vector2)Hit.collider.transform.position - (Vector2)viewPosition).normalized;
					//float projAngle = Mathf.Atan2(projHit.y - viewPosition.y,projHit.x - viewPosition.x);
					float transformAngle = Mathf.Atan2(Hit.collider.transform.position.y - viewPosition.y,Hit.collider.transform.position.x - viewPosition.x);
					float hitAngle  = Mathf.Atan2(target.y - viewPosition.y,target.x - viewPosition.x);


					Debug.DrawLine(Hit.collider.transform.position,viewPosition);
					//Debug.DrawLine(nextHit.point,Hit.point);
					if(transformAngle > hitAngle){
						hitPoints.Add (nextHit.point);
						hitPoints.Add (Hit.point);

						//Debug.Log ("greater than");

					}
					else{
						hitPoints.Add (Hit.point);
						hitPoints.Add (nextHit.point);

						//Debug.Log ("less than or equal");
					}
					//Debug.DrawLine(Hit.point,nextHit.point);
					//hitPoints.Add(Hit.point);

					//Debug.Log ("hit corner");
					//hitPoints.Add (Hit.point);
				}

			}
			else{//didnt hit a corner just add the point
				hitPoints.Add(Hit.point);
			}


				/*
					hitPoints.Insert(0,wallPoint.Key);
					hitPoints.Insert(0,nextHit.point);
				}*/
			

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
			if(Vector2.Dot((mesh.vertices[i]-mesh.vertices[0]).normalized,(mesh.vertices[i+1]-mesh.vertices[0]).normalized) != 1){
				triangleVertices.Add(0);

				triangleVertices.Add(i+1);
				triangleVertices.Add(i);
			}
		}
		triangleVertices.Add (0);

		triangleVertices.Add (1);
		triangleVertices.Add (mesh.vertices.Length-1);
		//
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
