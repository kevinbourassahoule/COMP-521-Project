using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
	public AbstractPlayer Firer;
	
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
		
		if (Mathf.Abs(transform.position.x) < 0 || 
		    Mathf.Abs(transform.position.x) > Environment.Width ||
		    Mathf.Abs(transform.position.y) < 0 || 
		    Mathf.Abs(transform.position.y) > Environment.Width)
		{
			GameObject.Destroy(gameObject);
		}
	}
	
	void OnCollisionEnter2D(Collision2D coll) 
	{
		// Perform action on collided object
		if (coll.transform != null) 
		{
			switch (coll.gameObject.tag) 
			{
				case "Wall":
					GameObject.Destroy(gameObject);
					break;
				case "AIPlayer":
				case "HumanPlayer":
					AbstractPlayer playerHit = coll.gameObject.GetComponent<AbstractPlayer>();
					
					// No friendly fire
					if (!Firer.transform.parent.Equals(playerHit.transform.parent))
					{
						playerHit.OnReceivedBullet ();
						
						if (playerHit.IsDead())
						{
							Firer.OnKilledEnemy(playerHit);
						}
						
						// Destroy bullet
						GameObject.Destroy(gameObject);
					}
					
					break;
				default:
					break;
			}
		}
		
		
	}
}