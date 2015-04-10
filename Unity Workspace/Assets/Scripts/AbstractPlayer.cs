using UnityEngine;
using System.Collections;

public abstract class AbstractPlayer : MonoBehaviour 
{	
	protected int magazine;
	protected float health;
	
	protected void Shoot()
	{
		GameObject bullet = (GameObject) GameObject.Instantiate(Environment.Instance.BulletPrefab,
							  									transform.position + (transform.right)*.2f,
							   									transform.rotation);
							   									
		bullet.transform.parent = Environment.Instance.transform.FindChild("Bullets");
		Bullet b = bullet.GetComponent<Bullet> ();
		b.firer = transform;
	}

	// Called by a bullet when it hits this player
	public void OnReceivedBullet ()
	{
		health--;
		
		if (health <= 0)
		{
			Die();
		}
	}
	
	public abstract void Die ();
	protected void killedPlayer (AbstractPlayer deadPlayer){}
}
