using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
	private float speed;
	private Vector3 direction;
	// Use this for initialization
	void Start () 
	{
		//transform.parent = GameObject.Find("Environment/Bullets").transform;
		direction = transform.right;

	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		transform.position += direction * Environment.Instance.BulletSpeed;
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
				//turn off friendly fire
				if(transform.parent.parent != coll.transform.parent)
				{
					coll.gameObject.GetComponent<AbstractPlayer>().OnReceivedBullet();
				}
				break;
			case "HumanPlayer":
				if(transform.parent.parent != coll.transform.parent)
				{
					coll.gameObject.GetComponent<AbstractPlayer>().OnReceivedBullet();
				}
				break;
			default:
				break;
		}
	}
}
