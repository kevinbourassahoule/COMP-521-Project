using UnityEngine;
using System.Collections.Generic;

public class AIPlayer : AbstractPlayer 
{	
	public AbstractPlayer Target { get; set; }

	private Squad squad;
	private bool targetInSight;
	private float AngleBetweenPlayerAndTarget;
	private float shootTimer;
	private SpriteRenderer spriteRenderer;

	private const int OBJECT_INFLUENCE_DISTANCE = 5;

	// Use this for initialization
	void Start () 
	{
		health = Environment.Instance.PlayerMaxHealth;
		squad = transform.parent.FindChild("Squad").GetComponent<Squad>();
		targetInSight = false;
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		ApplyForces();
		//keep track of if we saw the player last frame
		bool previouslyInSight = targetInSight;
		//do we see him now?
		targetInSight = SeesTarget ();
		if(targetInSight)
		{
			if(!previouslyInSight){
				//alert the squad of us now being in sight of the enemy
				squad.OnEnemyDetected(Target);
			}
			if(AngleBetweenPlayerAndTarget < Environment.Instance.PlayerShootAngle && Time.time - shootTimer > Environment.Instance.PlayerShootWaitTime)
			{
				//shoot only if we see the enemy are within shooting angle range and havent shot too recently
				Shoot ();
				shootTimer = Time.time;
			}
		}
		else if (previouslyInSight)
		{
			//if we saw the enemy last frame and now lost him, let the squad know
			squad.OnLostSightOfEnemy(Target);
		}
		//if we arent in the players Visibility Polygon dont render 
		if (WithinBounds(Environment.Instance.MeshVertices))
		{
			spriteRenderer.enabled = true;
		}
		else {
			spriteRenderer.enabled = false;
		}
	}
	
	public override void OnKilledEnemy(AbstractPlayer deadPlayer)
	{
		squad.OnKilledEnemy(deadPlayer);
	}
	
	/*
	 * Returns whether or not this player has his current target in his line of sight.
	 */
	private bool SeesTarget()
	{
		// Check whether the target is within this player's field of view
		if(Target != null)
		{
			// Draw a line towards target for testing purposes
//			Debug.DrawLine(transform.position, transform.position + (Target.transform.position - transform.position).normalized * Environment.Instance.PlayerMaxSight);
			
			AngleBetweenPlayerAndTarget = Vector3.Angle(transform.right, Target.transform.position - transform.position);
			if (AngleBetweenPlayerAndTarget > Environment.Instance.PlayerFOVAngle)
			{

				return false;
			}

			// Send a ray in the direction of the target 
			Vector2 direction = (Target.transform.position - transform.position).normalized;
			RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + 0.2f*direction,
			                                     direction,Environment.Instance.PlayerMaxSight);

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
		// Forces
		Vector2 lookAtForces        = (Vector2)(squad.transform.position - transform.position);
		Vector2 movementForces      = (Vector2)(squad.transform.position - transform.position);
		
		Vector2 nearbyWallForce     = Vector2.zero;
		int     nearbyWallCount     = 0;
		Vector2 nearbyAIForce       = Vector2.zero;
		int     nearbyAICount       = 0;
		Vector2 nearbyCoverForce    = Vector2.zero;
		
		Wall	nearestWall         = null;
		float	nearestWallDistance = Mathf.Infinity;

		float   nearestCoverDistance = Mathf.Infinity;
		Vector2 nearestCover		 = Vector2.zero;
		// Get all nearby objects
		Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, OBJECT_INFLUENCE_DISTANCE);
		
		// Handle force influence for each nearby object
		foreach (Collider2D coll in nearbyObjects) 
		{
			// Handle nearby AIPlayer
			if (coll.CompareTag("AIPlayer")
			 && !coll.gameObject.Equals(gameObject)) 
			{
				nearbyAIForce += (Vector2)(coll.transform.position - transform.position).normalized
					            / Vector3.Distance(coll.transform.position, transform.position);
				nearbyAICount++;
			}
			
			// Handle nearby wall
			if (coll.CompareTag("Wall"))
			{
				Wall currWall = coll.GetComponent<Wall>();
				Vector3 closestWallPoint = coll.GetComponent<PolygonCollider2D>().bounds.ClosestPoint(transform.position);
				
				nearbyWallForce += (Vector2)(closestWallPoint - transform.position).normalized 
					              / Vector3.Distance(transform.position, closestWallPoint);
				nearbyWallCount++;
				
				// Check if this is the nearest wall
				float distanceToWall = Vector2.Distance(currWall.transform.position, transform.position);
				if (distanceToWall < nearestWallDistance)
				{
					nearestWall = currWall;
					nearestWallDistance = distanceToWall;
				}
				// If theres a wall near us and were attacking find the closest cover point
				if(squad.IsAttacking())
				{
					//check if wall has a good cover point
					Wall curWall        = coll.transform.GetComponent<Wall>();
					//we need it to be the closer of the two wall covers. Otherwise well get stuck
					Vector2 coverChoice = (Vector2.Distance(curWall.coverLeftPoint,transform.position) < 
					                       Vector2.Distance(curWall.coverRightPoint,transform.position)) 
												? curWall.coverLeftPoint : curWall.coverRightPoint;
					//we also need that the distance from the enemy to the wall transform is less than the distance from the enemy to our cover
					coverChoice         = (Vector2.Distance(coverChoice,squad.lastSeenPosition) > 
					                       Vector2.Distance(coll.transform.position,squad.lastSeenPosition)) 
												? coverChoice : Vector2.zero;
					//if its closer then then previous min update
					if(coverChoice != Vector2.zero)
					{
						float curCoverDistance = Vector3.Distance(coverChoice,transform.position);
						if(curCoverDistance < nearestCoverDistance)
						{
							nearestCoverDistance = curCoverDistance;
							nearestCover         = coverChoice;
						}
					}
				}
			}
		}
		//need to consider cover points if attacking
		if(squad.IsAttacking())
		{
			if(nearestCover != Vector2.zero)
			{
				Vector2 coverForce  = (nearestCover - (Vector2)transform.position).normalized / 
											Vector2.Distance (nearestCover,(Vector2) transform.position);
				movementForces += coverForce;
				lookAtForces   -=  coverForce;
			}
		}

		// Get nearest cover point from nearest wall and generate a force from its position
		if (Vector2.Distance(nearestWall.coverLeftPoint, transform.position) < 
		    Vector2.Distance(nearestWall.coverRightPoint, transform.position)) 
		{
			nearbyCoverForce = nearestWall.coverLeftPoint - (Vector2) transform.position;
		}
		else {
			nearbyCoverForce = nearestWall.coverRightPoint - (Vector2) transform.position;
		}
		if (squad.IsAttacking())
		{
			movementForces += nearbyCoverForce;
			lookAtForces   += nearbyCoverForce;
		}
		
		// Accumulate forces from walls
		if (nearbyWallCount > 0) 
		{
			movementForces -= nearbyWallForce / nearbyWallCount;
			lookAtForces   -= nearbyWallForce / nearbyWallCount;
		}
		
		// Accumulate forces from nearby AI
		if (nearbyAICount > 0)
		{
			movementForces -= nearbyAIForce / nearbyAICount;
			
			if (squad.IsPatrolling())
			{
				lookAtForces -= nearbyAIForce / nearbyAICount;
			}
		}
		
		if (!Target.IsDead())
		{
			if (squad.IsAttacking() && targetInSight)
			{
				lookAtForces = Target.transform.position - transform.position;
			}
			else if (squad.IsGoingToLastSeenPosition())
			{
				lookAtForces = squad.lastSeenPosition - (Vector2) transform.position;
			}
		}

		// Update position
		transform.position = Vector3.MoveTowards(transform.position, transform.position + (Vector3) movementForces, Environment.Instance.PlayerMaxSpeed);
		
		// Update rotation
		Vector3 lookAtVector = (transform.position + (Vector3) lookAtForces) - transform.position;
		if (lookAtVector != Vector3.zero)
		{ 
			transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(lookAtVector.y, lookAtVector.x) * Mathf.Rad2Deg, Vector3.forward); 
		}
	}

	public override void Die()
	{
		if (targetInSight)
		{
			squad.OnLostSightOfEnemy(Target);
		}
		
		GameObject.Destroy(gameObject);
	}
}
