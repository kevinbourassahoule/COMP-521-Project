using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Wall : MonoBehaviour {

	public Vector2 botLeft;
	public Vector2 botRight;
	public Vector2 topLeft;
	public Vector2 topRight;
	// Use this for initialization
	void Start () {
		Vector3 playerExtents = this.GetComponent<BoxCollider2D> ().size * 0.5f ;
		botLeft = new Vector2 ((-playerExtents.x* transform.localScale.x) + transform.position.x, -(playerExtents.y* transform.localScale.y) + transform.position.y);
		botRight = new Vector2 ((playerExtents.x* transform.localScale.x) + transform.position.x, -(playerExtents.y* transform.localScale.y)+ transform.position.y);
		topLeft = new Vector2 ((-playerExtents.x* transform.localScale.x) + transform.position.x, (playerExtents.y* transform.localScale.y) + transform.position.y);
		topRight = new Vector2 ((playerExtents.x* transform.localScale.x) + transform.position.x, (playerExtents.y* transform.localScale.y) + transform.position.y);

		Quaternion angle = Quaternion.Euler (transform.eulerAngles);
		botLeft = RotatePoint2d (botLeft, angle);
		botRight = RotatePoint2d (botRight, angle);
		topLeft = RotatePoint2d (topLeft, angle);
		topRight = RotatePoint2d (topRight, angle);

	}
	
	private Vector2 RotatePoint2d(Vector2 oldPoint, Quaternion angle)   
	{
		return (Vector2) (angle * ((Vector3)oldPoint - transform.position) + transform.position);    
	}
}
