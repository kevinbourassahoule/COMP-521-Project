using UnityEngine;
using System.Collections.Generic;

public class Environment : MonoBehaviour 
{
	// Map properties
	public float Height { get; private set; }
	public float Width  { get; private set; }
	public WallBounds[] Walls { get; private set; }
	
	// Bullet properties
	public float BulletSpeed;

	// Use this for initialization
	void Start () 
	{
		Height = gameObject.GetComponent<Collider2D>().bounds.size.y;
		Width  = gameObject.GetComponent<Collider2D>().bounds.size.x;
		Walls  = transform.FindChild("Walls").GetComponentsInChildren<WallBounds>();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
