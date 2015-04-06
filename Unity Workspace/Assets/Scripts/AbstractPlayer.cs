using UnityEngine;
using System.Collections;

public abstract class AbstractPlayer : MonoBehaviour 
{
	public GameObject Bullet;
	public float MAX_HEALTH;
	public float Speed;
	public float rotSpeed;
	public Transform walls;

	protected int magazine;
	protected float health;
	
	protected void Shoot()
	{
		GameObject.Instantiate(Bullet,
							   transform.position,
							   transform.rotation);
	}
	
	protected abstract void Move(Vector3 direction, Quaternion rotation);
}
