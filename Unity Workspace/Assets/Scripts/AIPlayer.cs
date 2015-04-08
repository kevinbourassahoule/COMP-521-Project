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
		squad = transform.parent.GetComponent<Squad>();
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
	
	override protected void Move(Vector3 direction, Quaternion rotation)
	{
		
	}
	
	private void ApplyForces()
	{
		Vector2 accumulatedForces = Vector2.zero;
		Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, OBJECT_INFLUENCE_DISTANCE);	// TODO layer mask?
		
		// Follow squad
		accumulatedForces += (Vector2) (squad.transform.position - transform.position);
		
		if (squad.IsPatrolling())
		{
			
		}
		
		accumulatedForces = accumulatedForces.normalized * Environment.Instance.PlayerMaxSpeed;
		
		transform.position += (Vector3) accumulatedForces;
	}
	
	// TODO Could be cool for squad decision-making
//	private List<AbstractPlayer> EnemiesInSight()
//	{
//		
//	}
}
