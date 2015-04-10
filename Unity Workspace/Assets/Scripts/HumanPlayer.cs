using UnityEngine;
using System.Collections;

public class HumanPlayer : AbstractPlayer
{
	private VisibilityComputer vision;
	private const float ROT_SPEED = 180f;

	// Use this for initialization
	void Start () {		
		health = Environment.Instance.PlayerMaxHealth;
		vision = new VisibilityComputer(transform.position, 10);
		//tell the environment about the mesh.
		//Environment.Instance.MeshVertices = vision.Triangles;
	}
	
	// Update is called once per frame
	void Update () 
	{		
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePos.z = 0;
		transform.right = mousePos - transform.position;
		
		// Handle shooting
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			Shoot();
		}
		
		// Handle movement
		Vector3 direction = Vector3.zero;
		if (Input.GetKey(KeyCode.W))
			direction += Vector3.up;
		if (Input.GetKey(KeyCode.S))
			direction += -Vector3.up;
		if (Input.GetKey(KeyCode.D))
			direction += Vector3.right;
		if (Input.GetKey(KeyCode.A))
			direction += -Vector3.right;
			
		transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, Environment.Instance.PlayerMaxSpeed);
		
		// Update vision
		vision.Origin = transform.position;
		vision.Compute();
	}
	public override void Die()
	{
		GameObject.Destroy(gameObject);
	}
}
