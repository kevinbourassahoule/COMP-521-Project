using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Wall : MonoBehaviour {

	public Vector2 botLeft;
	public Vector2 botRight;
	public Vector2 topLeft;
	public Vector2 topRight;
	private Vector2 coverLeftPoint;
	private Vector2 coverRightPoint;
	// Use this for initialization
	void Start () {
		Quaternion angle = Quaternion.Euler (transform.eulerAngles);
		Vector3 playerExtents = this.GetComponent<BoxCollider2D> ().size * 0.5f ;
		//find bounds and covers
		botLeft              = new Vector2 ((-playerExtents.x* transform.localScale.x) + transform.position.x, -(playerExtents.y* transform.localScale.y) + transform.position.y);
		botRight             = new Vector2 ((playerExtents.x* transform.localScale.x) + transform.position.x, -(playerExtents.y* transform.localScale.y)+ transform.position.y);
		topLeft              = new Vector2 ((-playerExtents.x* transform.localScale.x) + transform.position.x, (playerExtents.y* transform.localScale.y) + transform.position.y);
		topRight             = new Vector2 ((playerExtents.x* transform.localScale.x) + transform.position.x, (playerExtents.y* transform.localScale.y) + transform.position.y);
		coverLeftPoint       = new Vector2 ((-playerExtents.x * transform.localScale.x) + transform.position.x, transform.position.y);
		coverRightPoint      = new Vector2 ((playerExtents.x * transform.localScale.x) + transform.position.x, transform.position.y);

		//roate points
		botLeft              = RotatePoint2d (botLeft, angle);
		botRight             = RotatePoint2d (botRight, angle);
		topLeft              = RotatePoint2d (topLeft, angle);
		topRight             = RotatePoint2d (topRight, angle);
		coverLeftPoint       = RotatePoint2d (coverLeftPoint, angle);
		coverRightPoint      = RotatePoint2d (coverRightPoint, angle);

		//instantiate covers
		GameObject coverLeft = new GameObject ();
		coverLeft.transform.position   = coverLeftPoint;
		coverLeft.transform.parent     = transform;
		coverLeft.transform.name       = "CoverPointLeft";
		coverLeft.transform.tag        = "CoverPoint";

		GameObject coverRight = new GameObject ();
		coverRight.transform.position  = coverRightPoint;
		coverRight.transform.parent    = transform;
		coverRight.transform.name      = "CoverPointRight";
		coverRight.transform.tag       = "CoverPoint";
		
	}
	
	private Vector2 RotatePoint2d(Vector2 oldPoint, Quaternion angle)   
	{
		return (Vector2) (angle * ((Vector3)oldPoint - transform.position) + transform.position);    
	}
}
