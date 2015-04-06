using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisibilityPolygon {
	
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
	//takes in the position of the player and the parent game object that has all the walls as its children
	 public VisibilityPolygon(Vector3 player, Transform walls){
		List<Vector2> polygonVertices = new List<Vector2> ();
		//sort the wall points
		List<KeyValuePair<Vector2,float>> sortedWallPoints = sortWalls (player, walls);
		foreach (KeyValuePair<Vector2,float> wallPoint in sortedWallPoints) {
			Debug.DrawRay(player,(wallPoint.Key - (Vector2) player).normalized);
		}

	}
}
