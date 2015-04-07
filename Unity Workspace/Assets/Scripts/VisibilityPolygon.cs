using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisibilityPolygon 
{
	private Vector3 viewPosition;
	private Transform walls;
		
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
		//sorts the list by value
		wallPoints.Sort ((firstPair,nextPair) =>
		{
			return firstPair.Value.CompareTo (nextPair.Value);
		}
		);
		return wallPoints;

	}
	
	// Constructor
	public VisibilityPolygon(AbstractPlayer player, Transform walls)
	{
		this.viewPosition = player.transform.position;
		this.walls = walls;
	}
	
	//takes in the position of the player and the parent game object that has all the walls as its children
	public void RecomputePolygon(Vector3 curViewPosition)
	{
		viewPosition = curViewPosition;
		List<Vector2> polygonVertices = new List<Vector2> ();
		//sort the wall points
		List<KeyValuePair<Vector2,float>> sortedWallPoints = sortWalls (viewPosition, walls);
		foreach (KeyValuePair<Vector2,float> wallPoint in sortedWallPoints) {
			RaycastHit2D hit =(Physics2D.Raycast(viewPosition,(wallPoint.Key - (Vector2)viewPosition).normalized,100.0f));
			if(hit.collider != null){
			//maybe loosen up equality, if it hit the target wallpoint shoot another ray until failure
				RaycastHit2D nextHit;

				if(Vector2.Distance(hit.point, wallPoint.Key) <= Vector2.kEpsilon){
					nextHit = (Physics2D.Raycast(wallPoint.Key,(wallPoint.Key - (Vector2)viewPosition).normalized,100.0f));
						//if theres a hit, update new starting point and last hit point
						if(nextHit.collider != null)
							hit = nextHit;
							
				}
				//hit now contains last collider on original path
				polygonVertices.Add((Vector2)hit.point);
				Debug.DrawLine(curViewPosition,hit.point);
			}
		}
	}
}
