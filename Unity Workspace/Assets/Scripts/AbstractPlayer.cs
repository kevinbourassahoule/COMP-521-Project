using UnityEngine;
using System.Collections;

public abstract class AbstractPlayer : MonoBehaviour 
{
	public GameObject Bullet;
	public float MAX_HEALTH;
	public float Speed;
	public float rotSpeed;
	public float FOVangle;
	public Transform walls;
	
	protected int magazine;
	protected float health;
	
	protected void Shoot()
	{
		GameObject.Instantiate(Bullet,
							   transform.position,
							   transform.rotation);
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
