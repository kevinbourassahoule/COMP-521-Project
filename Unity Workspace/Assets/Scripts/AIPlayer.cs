using UnityEngine;
using System.Collections.Generic;

public class AIPlayer : AbstractPlayer 
{	
	private const int OBJECT_INFLUENCE_DISTANCE = 5;

	public AbstractPlayer Target { get; set; }
	
	private Squad squad;

	private bool targetInSight;

	public Vector2 lastSeenPositionFromSquad; 

	private float AngleBetweenPlayerAndTarget;

	private float shootTimer;

	private SpriteRenderer spriteRenderer;

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
		//get position from squad
		lastSeenPositionFromSquad = squad.lastSeenPosition;
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
			if(AngleBetweenPlayerAndTarget < Environment.Instance.PlayerSHOOTAngle && Time.time - shootTimer > Environment.Instance.PlayerShootWaitTime)
			{
				//shoot only if we see the enemy are within shooting angle range and havent shot too recently
				Shoot ();
				shootTimer = Time.time;
			}
		}
		else if(previouslyInSight)
		{
			//if we saw the enemy last frame and now lost him, let the squad know
			squad.OnLostSightOfEnemy(Target);
		}
		//if we arent in the players Visibility Polygon dont render 
		if(WithinBounds(Environment.Instance.MeshVertices))
		{
			spriteRenderer.enabled = true;
		}
		else {
			spriteRenderer.enabled = false;
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
			Debug.DrawLine(transform.position, transform.position + (Target.transform.position - transform.position).normalized * Environment.Instance.PlayerMaxSight);
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
		Vector2 lookAtForces   = Vector2.zero;
		Vector2 movementForces = Vector2.zero;
		Vector2 currForce      = Vector2.zero;

		Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, OBJECT_INFLUENCE_DISTANCE);	// TODO layer mask?
		
		// Follow squad
		currForce += (Vector2)(squad.transform.position - transform.position);
		float closestCoverDistance    = Mathf.Infinity;
		Vector2 closestCover          = Vector2.zero;
		foreach (Collider2D coll in nearbyObjects) {
			if (coll.tag == "Player" && !coll.gameObject.Equals (gameObject)) {
				currForce -= (Vector2)(coll.transform.position - transform.position).normalized / 
					Vector3.Distance (coll.transform.position, transform.position);
			}
			//if theres a wall near us and were attacking find the closest cover point
			if(coll.tag == "Wall" && (squad.IsAttacking() || squad.IsGoingToLastSeenPosition()))
			{
				//check if wall has a good cover point
				Wall curWall        = coll.transform.GetComponent<Wall>();
				//we need it to be the closer of the two wall covers. Otherwise well get stuck
				Vector2 coverChoice = (Vector2.Distance(curWall.coverLeftPoint,transform.position) < 
				                       Vector2.Distance(curWall.coverRightPoint,transform.position)) 
											? curWall.coverLeftPoint : curWall.coverRightPoint;
				//we also need that the distance from the enemy to the wall transform is less than the distance from the enemy to our cover
				coverChoice         = (Vector2.Distance(coverChoice,lastSeenPositionFromSquad) > 
				                       Vector2.Distance(coll.transform.position,lastSeenPositionFromSquad)) 
											? coverChoice : Vector2.zero;
				//if its closer then then previous min update
				if(coverChoice != Vector2.zero)
				{
					float curCoverDistance = Vector3.Distance(coverChoice,transform.position);
					if(curCoverDistance < closestCoverDistance)
					{
						closestCoverDistance = curCoverDistance;
						closestCover         = coverChoice;
					}
				}
			}
		}
		//need to consider cover points if attacking
		if(squad.IsAttacking())
		{
			Vector2 attackPlayerForce = (lastSeenPositionFromSquad - (Vector2)transform.position).normalized / 
											Vector2.Distance (lastSeenPositionFromSquad,(Vector2) transform.position);
			//if there was no legitimate cover spot
			if(closestCover == Vector2.zero)
			{
				movementForces += attackPlayerForce;
				lookAtForces   += attackPlayerForce;
				
			}
			else
			{
				Vector2 coverForce  = (closestCover - (Vector2)transform.position).normalized / 
											Vector2.Distance (closestCover,(Vector2) transform.position);
				movementForces += coverForce + attackPlayerForce;
				lookAtForces   = lookAtForces - coverForce +attackPlayerForce;
			}
		}

		//add forces
		movementForces += currForce;
		lookAtForces   += currForce;

		// Update position
		transform.position = Vector3.MoveTowards(transform.position, transform.position + (Vector3) movementForces, Environment.Instance.PlayerMaxSpeed);
		
		// Update rotation
		Vector3 moveDirection = (transform.position + (Vector3) lookAtForces) - transform.position;
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
