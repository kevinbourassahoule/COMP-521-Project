using UnityEngine;
using System.Collections;

public abstract class AbstractPlayer : MonoBehaviour 
{
	public float MAX_HEALTH;
	public float Speed;
	public float rotSpeed;
	public float FOVangle;
	
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
		GameObject.Destroy(this);
	}
	
	protected abstract void Move(Vector3 direction, Quaternion rotation);
}
