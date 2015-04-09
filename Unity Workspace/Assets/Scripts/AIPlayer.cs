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
			if(!previouslyInSight){
				squad.OnEnemyDetected(Target);
			}
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
		if(Target != null)
		{
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
			if (hit.collider != null && hit.transform == Target.transform)
			{
				return true;
			}
			else {
				return false;
			}
		}
		//players dead
		return false;
	}
	
	private void ApplyForces()
	{
		Vector2 lookAtForces   = Vector2.zero;
		Vector2 movementForces = Vector2.zero;
		Vector2 currForce      = Vector2.zero;
		Vector2 currMovementForce = Vector2.zero;
		Vector2 currLookAtForce = Vector2.zero;
		Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, OBJECT_INFLUENCE_DISTANCE);	// TODO layer mask?
		
		// Follow squad
		movementForces += (Vector2)(squad.transform.position - transform.position);
		float closestCoverDistance = float.MaxValue;
		Wall closestCover          = null;
		foreach (Collider2D coll in nearbyObjects) {
			if (coll.tag == "Player" && !coll.gameObject.Equals (gameObject)) {
				currForce -= (Vector2)(coll.transform.position - transform.position).normalized / 
					Vector3.Distance (coll.transform.position, transform.position);
			}
			//if theres a wall near us and were attacking find the closest cover point
			if(coll.tag == "Wall" && (squad.IsAttacking() || squad.IsGoingToLastSeenPosition()))
			{
				float curCoverDistance = Vector3.Distance(coll.transform.position,transform.position);
				if(curCoverDistance < closestCoverDistance)
				{
					closestCoverDistance = curCoverDistance;
					closestCover         = coll.transform.GetComponent<Wall>();
				}
			}
		}
		//need to consider cover points if attacking
		if(squad.IsAttacking() || squad.IsGoingToLastSeenPosition())
		{
			Vector2 attackPlayerForce = (lastSeenPositionFromSquad - (Vector2)transform.position).normalized / 
											Vector2.Distance (lastSeenPositionFromSquad,(Vector2) transform.position);
			if(closestCover == null)
			{
				currForce += attackPlayerForce;
				
			}
			else
			{
				//TODO this could be wrong, may have to optimize, pick further cover between the two
				Vector2 coverChoice = (Vector2.Distance(closestCover.coverLeftPoint,lastSeenPositionFromSquad) > 
				                          Vector2.Distance(closestCover.coverRightPoint,lastSeenPositionFromSquad)) 
												? closestCover.coverLeftPoint : closestCover.coverRightPoint;
				Vector2 coverForce  = (coverChoice - (Vector2)transform.position).normalized / 
											Vector2.Distance (coverChoice,(Vector2) transform.position);
				currForce += coverForce + attackPlayerForce;
			}
		}

		//add forces
		movementForces += currForce;
		//lookAtForces   += currForce;

		// Update position
		transform.position = Vector3.MoveTowards(transform.position, transform.position + (Vector3) movementForces, Environment.Instance.PlayerMaxSpeed);
		
		// Update rotation
		Vector3 moveDirection = (transform.position + (Vector3) movementForces) - transform.position;
		if (moveDirection != Vector3.zero)
		{ 
			transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg,Vector3.forward); 
		}

	}

	public override void Die()
	{
		if(targetInSight)
		{
			squad.OnLostSightOfEnemy(Target);
		}
		GameObject.Destroy(gameObject);
	}
	
	// TODO Could be cool for squad decision-making
//	private List<AbstractPlayer> EnemiesInSight()
//	{
//		
//	}
}
