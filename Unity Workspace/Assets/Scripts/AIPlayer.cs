using UnityEngine;
using System.Collections.Generic;

public class AIPlayer : AbstractPlayer 
{	
	private const int OBJECT_INFLUENCE_DISTANCE = 5;

	public AbstractPlayer Target { get; set; }
	
	private Squad squad;
	
	// Use this for initialization
	void Start () 
	{
		health = Environment.Instance.PlayerMaxHealth;
		squad = transform.parent.FindChild("Squad").GetComponent<Squad>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		ApplyForces();
	}
	
	/*
	 * Returns whether or not this player has his current target in his line of sight.
	 */
	private bool SeesTarget()
	{
		// Check whether the target is within this player's field of view
		Debug.DrawRay(transform.forward, Target.transform.position - transform.position);
		if (Vector3.Angle(transform.forward, Target.transform.position - transform.position) > Environment.Instance.PlayerFOVAngle)
		{
			return false;
		}
	
		// Send a ray in the direction of the target 
		RaycastHit2D hit = Physics2D.Raycast(transform.position,
		                                     Target.transform.position - transform.position,
		                                     LayerMask.GetMask("Player"));
		
		// Check if the ray hit the target
		if (hit.collider != null && hit.collider.gameObject.Equals(Target.gameObject))
		{
			return true;
		}
		else {
			return false;
		}
	}
	
	private void ApplyForces()
	{
		Vector2 lookAtForces = Vector2.zero;
		Vector2 movementForces = Vector2.zero;
		Vector2 currForce;
	
		Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, OBJECT_INFLUENCE_DISTANCE);	// TODO layer mask?
		
		// Follow squad
		movementForces += (Vector2) (squad.transform.position - transform.position);
		
		foreach (Collider2D coll in nearbyObjects)
		{
			currForce = (Vector2) (coll.transform.position - transform.position).normalized / 
						Vector3.Distance(coll.transform.position, transform.position);
				
			if (coll.tag == "Player" && !coll.gameObject.Equals(gameObject))
			{
				movementForces -= currForce;
				lookAtForces -= currForce;
			}
		}
		
		// Update position
		transform.position = Vector3.MoveTowards(transform.position, transform.position + (Vector3) movementForces, Environment.Instance.PlayerMaxSpeed);
		
		// Update rotation
		transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(lookAtForces.y, lookAtForces.x) * Mathf.Rad2Deg, 
												  Vector3.forward);
	}
	
	// TODO Could be cool for squad decision-making
//	private List<AbstractPlayer> EnemiesInSight()
//	{
//		
//	}
}
