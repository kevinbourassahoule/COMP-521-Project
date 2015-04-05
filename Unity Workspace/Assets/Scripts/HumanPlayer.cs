using UnityEngine;
using System.Collections;

public class HumanPlayer : AbstractPlayer
{
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Handle orientation
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if (Physics.Raycast(ray, out hit))
		{
			transform.LookAt(hit.point);
		}
			
		// Handle shooting
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			Shoot();
		}
		
		// Handle movement
		Vector3 direction = Vector3.zero;
		
		if (Input.GetKeyDown(KeyCode.W))
		{
			direction += Vector3.up;
		}
		if (Input.GetKeyDown(KeyCode.A))
		{
			direction += Vector3.left;
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			direction += Vector3.down;
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			direction += Vector3.right;
		}
		
		Move(direction);
	}
	
	override protected void Move(Vector3 direction)
	{
		transform.position += direction.normalized * Speed;
	}
}
