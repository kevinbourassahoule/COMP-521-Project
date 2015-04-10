using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	protected bool WithinBounds(Vector3[] triangularVertices)
	{
		Vector2 p  = (Vector2) transform.position;
		for (int i = 0; i < triangularVertices.Length - 2; i++)
		{
			Vector2 p0 = (Vector2)triangularVertices[0];
			Vector2 p1 = (Vector2)triangularVertices[i+1];
			Vector2 p2 = (Vector2)triangularVertices[i+2];

			float area = Mathf.Abs(Vector3.Cross (p0-p1,p0-p2).z) *.5f;

			//compute barrycentric coordinates
			float s    = (p0.y*p2.x - p0.x*p2.y + (p2.y - p0.y)*p.x + (p0.x - p2.x)*p.y);
			float t    = (p0.x*p1.y - p0.y*p1.x + (p0.y - p1.y)*p.x + (p1.x - p0.x)*p.y);
			//point p is contained in p1,p2,p3 if s>0 && t>0 && s+t<2*Area(p1,p2,p3)
			if(s>0 && t>0 && s+t < 2*area)
			{
				return true;
			}
		}
		return false;
	}
}
