using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class WallBounds : MonoBehaviour {

	public Vector2 botLeft;
	public Vector2 botRight;
	public Vector2 topLeft;
	public Vector2 topRight;
	// Use this for initialization
	void Start () {
		Vector3 playerExtents = this.GetComponent<BoxCollider2D> ().size * 0.5f;
		botLeft = new Vector2 (-playerExtents.x + transform.position.x, -playerExtents.y + transform.position.y);
		botRight = new Vector2 (playerExtents.x + transform.position.x, -playerExtents.y + transform.position.y);
		topLeft = new Vector2 (-playerExtents.x + transform.position.x, playerExtents.y + transform.position.y);
		topRight = new Vector2 (playerExtents.x + transform.position.x, playerExtents.y + transform.position.y);
	}
	

}
