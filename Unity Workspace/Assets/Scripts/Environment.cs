using UnityEngine;
using System.Collections.Generic;

public class Environment : MonoBehaviour 
{
	// Map properties
	public float Height { get; private set; }
	public float Width  { get; private set; }
	public WallBounds[] Walls { get; private set; }
	
	// Bullet properties
	public static float BulletSpeed;

	// Use this for initialization
	void Start () 
	{
//		Height = transform.FindChild("Map").GetComponent<Renderer>().bounds.size.y;
//		Width  = transform.FindChild("Map").GetComponent<Renderer>().bounds.size.x;
		Walls  = transform.FindChild("Walls").GetComponentsInChildren<WallBounds>();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
