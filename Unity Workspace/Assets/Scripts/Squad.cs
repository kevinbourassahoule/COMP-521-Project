using UnityEngine;
using System.Collections.Generic;

public class Squad : MonoBehaviour 
{
	private Vector2 				       objective;
	private List<AIPlayer> 				   members 		    = new List<AIPlayer>();
	private Dictionary<AbstractPlayer,int> enemiesInSight 	= new Dictionary<AbstractPlayer,int>();
	public  Vector2                        lastSeenPosition { get; set; }
	
	// A reference to the squad's current state
	private delegate void State();
	private State currentState;
	
	private float WaitInCoverTimer;
	private const float WAIT_TIME = 5.0f;
	
	// Use this for initialization
	void Start () 
	{
		gameObject.name = "Squad";
	
		// Spawn squad members
		for (int i = 0; i < Environment.Instance.PlayersPerTeam; i++)
		{
			AIPlayer member = ((GameObject) GameObject.Instantiate(Environment.Instance.AIPlayerPrefab,
								   					   			   (Vector2) transform.position + Vector2.right * i * .1f,	// TODO more graceful spawn position?
								   					   			   Quaternion.identity)).GetComponent<AIPlayer>();
			member.transform.parent = transform.parent;
			member.Target = GameObject.FindGameObjectWithTag ("HumanPlayer").GetComponent<AbstractPlayer>();
			members.Add(member);
		}
		
		objective = transform.position;
		
		currentState = Patrolling;
	}
	
	// Update is called once per frame
	void Update () 
	{
		currentState();
	}	
	
	void FixedUpdate()
	{
		MoveTowardsObjective();
	}
	
	private bool IsAtObjective()
	{
		return Vector2.Distance(transform.position, objective) < Vector2.kEpsilon;
	}
	
	private void MoveTowardsObjective()
	{
		transform.position = Vector2.MoveTowards(transform.position, objective, Environment.Instance.PlayerMaxSpeed);
	}
	
	// TODO Could be cool if multiple enemies
//	private AIPlayer GetMemberClosestToTarget(AbstractPlayer target)
//	{
//		float closestDistance = Mathf.Infinity;
//		float currDistance;
//		AIPlayer closestMember = null;
//		
//		foreach (AIPlayer member in members)
//		{
//			currDistance = Vector2.Distance(member.transform.position, target.transform.position);
//			
//			if (currDistance < closestDistance)
//			{
//				closestDistance = currDistance;
//				closestMember = member;
//			}
//		}
//		
//		return closestMember;
//	}
	
	/*********
	 * STATES
	 *********/
	
	// Squad has no idea where the enemy is
	private void Patrolling()
	{
		// Start attacking if an enemy is in sight
		if (enemiesInSight.Count != 0)
		{
			currentState = Attacking;
			return;
		}
		
		// Get random objective 
		if (this.IsAtObjective())
		{
			objective.x = Random.Range(0, Environment.Width);
			objective.y = Random.Range(0, Environment.Height);
		}
	}
	public bool IsPatrolling() { return currentState == Patrolling; }
	
	
	private void Attacking()
	{
		//if we do not see the player go to last scene position
		if(enemiesInSight.Count == 0)
		{
			objective = lastSeenPosition;
			WaitInCoverTimer = Time.time;
			currentState = GoingToLastSeenPosition;
		}
	}
	public bool IsAttacking() { return currentState == Attacking; }
	
	
	private void GoingToLastSeenPosition()
	{
		// Start attacking if an enemy is in sight
		if (enemiesInSight.Count != 0)
		{
			currentState = Attacking;
			return;
		}
		
		// Check to see if player is there
		if (this.IsAtObjective() && Time.time - WaitInCoverTimer >= WAIT_TIME)
		{
			currentState = Patrolling;
			return;
		}
	}
	public bool IsGoingToLastSeenPosition() { return currentState == GoingToLastSeenPosition; }
	
	/************
	 * CALLBACKS
	 ************/
	 
	public void OnEnemyDetected(AbstractPlayer enemy)
	{
		lastSeenPosition = (Vector2) enemy.transform.position;
		
		if (!enemiesInSight.ContainsKey(enemy))
		{
			enemiesInSight.Add(enemy,1);
		}
		else {
			enemiesInSight[enemy]++;
		}
	}
	
	public void OnLostSightOfEnemy(AbstractPlayer enemy)
	{
		enemiesInSight[enemy]--;
		if(enemiesInSight[enemy] == 0)
		{
			enemiesInSight.Remove(enemy);
		}
	}
}
