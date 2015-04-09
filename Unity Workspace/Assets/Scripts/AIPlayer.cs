using UnityEngine;
using System.Collections.Generic;

public class AIPlayer : AbstractPlayer 
{	
	private const int OBJECT_INFLUENCE_DISTANCE = 5;

	public AbstractPlayer Target { get; set; }
	
	private Squad squad;

	private bool targetInSight;

	public Vector2 lastSeenPositionFromSquad; 
	// Use this for initialization
	void Start () 
	{
		health = Environment.Instance.PlayerMaxHealth;
		squad = transform.parent.FindChild("Squad").GetComponent<Squad>();
		targetInSight = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		lastSeenPositionFromSquad = squad.lastSeenPosition;
		ApplyForces();
		bool previouslyInSight = targetInSight;
		targetInSight = SeesTarget ();
		if(targetInSight)
		{
			squad.OnEnemyDetected(Target);
			Shoot ();
		}
		else if(previouslyInSight)
		{
			squad.OnLostSightOfEnemy(Target);
		}
	}
	
	/*
	 * Returns whether or not this player has his current target in his line of sight.
	 */
	private bool SeesTarget()
	{
		// Check whether the target is within this player's field of view
		Debug.DrawRay(transform.position, Target.transform.position - transform.position);
		if (Vector3.Angle(transform.right, Target.transform.position - transform.position) > Environment.Instance.PlayerFOVAngle)
		{
			return false;
		}

		// Send a ray in the direction of the target 
		Vector2 direction = (Target.transform.position - transform.position).normalized;
		RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + 0.2f*direction,
		                                     direction/*,
		                                     LayerMask.GetMask("Player")*/);

		// Check if the ray hit the target
		Debug.Log (hit.transform.name);
		if (hit.collider != null && hit.transform == Target.transform)
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

		//check if we are attaking or seeking enemy
		//if (!squad.IsAttacking () && !squad.IsGoingToLastSeenPosition ()) {
			// Follow squad
		movementForces += (Vector2)(squad.transform.position - transform.position);
		
		foreach (Collider2D coll in nearbyObjects) {
			currForce = (Vector2)(coll.transform.position - transform.position).normalized / 
				Vector3.Distance (coll.transform.position, transform.position);
				
			if (coll.tag == "Player" && !coll.gameObject.Equals (gameObject)) {
				movementForces -= currForce;
				lookAtForces -= currForce;
			}
		}
		if(squad.IsAttacking())
		{
			currForce = (lastSeenPositionFromSquad - (Vector2)transform.position).normalized / 
				Vector2.Distance (lastSeenPositionFromSquad,(Vector2) transform.position);
			movementForces += 5.0f*currForce;
			lookAtForces += 5.0f*currForce;
		}
		//}
		/*//if we are seeking the player
		if(squad.IsGoingToLastSeenPosition ())
		{
			movementForces += (Vector2)(squad.transform.position - transform.position);
			
			foreach (Collider2D coll in nearbyObjects) {
				currForce = (Vector2)(coll.transform.position - transform.position).normalized / 
					Vector3.Distance (coll.transform.position, transform.position);
				
				if (coll.tag == "Player" && !coll.gameObject.Equals (gameObject)) {
					movementForces -= currForce;
					lookAtForces -= currForce;
				}
			}

		}
		//if we are attacking
		if(squad.IsAttacking())
		{

		}
		*/
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
