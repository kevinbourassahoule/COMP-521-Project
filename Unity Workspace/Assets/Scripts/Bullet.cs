using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
	private float speed;

	// Use this for initialization
	void Start () 
	{
		transform.parent = GameObject.Find("Environment/Bullets").transform;
		speed = Environment.BulletSpeed;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		transform.position += transform.forward * speed;
	}
	
	void OnCollisionEnter2D(Collision2D coll) 
	{
		// Perform action on collided object
		switch (coll.gameObject.tag)
		{
			case "Wall":
				break;
			case "Player":
				coll.gameObject.GetComponent<AbstractPlayer>().OnReceivedBullet();
				break;
			default:
				break;
		}
		
		GameObject.Destroy(this);
	}
}
