using UnityEngine;
using System.Collections;

public abstract class AbstractPlayer : MonoBehaviour 
{	
	protected int magazine;
	protected float health;
	
	protected void Shoot()
	{
		GameObject bullet = (GameObject) GameObject.Instantiate(Environment.Instance.BulletPrefab,
							  									transform.position,
							   									transform.rotation);
							   									
		bullet.transform.parent = Environment.Instance.transform.FindChild("Bullets");
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
	
	private void Die()
	{
		GameObject.Destroy(gameObject);
	}
	protected void killedPlayer(AbstractPlayer deadPlayer){} 
}
