using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

		wallPoints = wallPoints.OrderBy (DictionaryEntry => DictionaryEntry.Value).ToList();
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
		if(Hit.collider != null && Hit.distance > .1f){
			RaycastHit2D nextHit;
			//check to see if raycasting will result in us being inside our current wall
			if(!Physics2D.OverlapCircle(Hit.point + .1f*direction,0.00001f)){
				Debug.DrawLine(Hit.point + .1f*direction,Hit.point);
				//get the next hit, start position slightly offset in the hit direction
				nextHit = (Physics2D.CircleCast(Hit.point + .1f*direction,0.001f,direction,100.0f,LayerMask.GetMask("Wall")));
				if(nextHit.collider != null && nextHit.distance > Vector2.kEpsilon){
					//decide which hit to store first.
					//calculate the angle from the player to the wall position
					float transformAngle = 0.0f;
					//calculate angle from player to our raycast hit
					float hitAngle       = 0.0f;
					//if the wall is to the left of our player, compare the angles relative to up
					/*if(Hit.collider.transform.position.x - viewPosition.x <=0){
						transformAngle = Vector2.Angle(Vector2.up,(Vector2)Hit.collider.transform.position - (Vector2)viewPosition);
						hitAngle       = Vector2.Angle(Vector2.up,Hit.point - (Vector2)viewPosition);
					//otherwise the wall is to the right so compare to down
					}else{
						transformAngle = Vector2.Angle(-Vector2.up,(Vector2)Hit.collider.transform.position - (Vector2)viewPosition);
						hitAngle       = Vector2.Angle(-Vector2.up,Hit.point - (Vector2)viewPosition);
					}*/
					transformAngle = Mathf.Atan2(Hit.collider.transform.position.y - viewPosition.y,Hit.collider.transform.position.x - viewPosition.x);
					hitAngle       = Mathf.Atan2(Hit.point.y - viewPosition.y,Hit.point.x - viewPosition.x);
					if(transformAngle >= hitAngle){
						hitPoints.Add (nextHit.point);
						hitPoints.Add (Hit.point);
					}
					else{
						hitPoints.Add (Hit.point);
						hitPoints.Add (nextHit.point);
					}
				}
			}
			//didnt hit a corner just add the point
			else{
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
