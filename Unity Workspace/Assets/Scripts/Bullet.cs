using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
	private float speed;

	// Use this for initialization
	void Start () 
	{
		transform.parent = GameObject.Find("Environment/Bullets").transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		transform.position += transform.right * Environment.Instance.BulletSpeed;
	}
	
	void OnCollisionEnter2D(Collision2D coll) 
	{
		// Perform action on collided object
		switch (coll.gameObject.tag)
		{
			case "Wall":
				GameObject.Destroy(gameObject);
				break;
			case "Player":
				coll.gameObject.GetComponent<AbstractPlayer>().OnReceivedBullet();
				break;
			default:
				break;
		}
	}
}
