using UnityEngine;
using System.Collections.Generic;

public class Squad : MonoBehaviour 
{
	private Vector2 				objective;
	private List<AIPlayer> 			members 		= new List<AIPlayer>();
	private HashSet<AbstractPlayer> enemiesInSight 	= new HashSet<AbstractPlayer>();
	
	// A reference to the squad's current state
	private delegate void State();
	private State currentState;

	// Use this for initialization
	void Start () 
	{
		gameObject.name = "Squad";
	
		// Spawn squad members
		for (int i = 0; i < Environment.Instance.PlayersPerTeam; i++)
		{
			AIPlayer member = ((GameObject) GameObject.Instantiate(Environment.Instance.AIPlayerPrefab,
								   					   			   (Vector2) transform.position + Vector2.right * i,	// TODO more graceful spawn position?
								   					   			   Quaternion.identity)).GetComponent<AIPlayer>();
			member.transform.parent = transform.parent;
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
		return ((Vector2)transform.position).Equals(objective);
	}
	
	private void MoveTowardsObjective()
	{
		transform.position = Vector2.MoveTowards(transform.position, objective, Environment.Instance.PlayerMaxSpeed);
	}
	
	// TODO Could be cool if multiple enemies
	private AIPlayer GetMemberClosestToTarget(AbstractPlayer target)
	{
		float closestDistance = Mathf.Infinity;
		float currDistance;
		AIPlayer closestMember = null;
		
		foreach (AIPlayer member in members)
		{
			currDistance = Vector2.Distance(member.transform.position, target.transform.position);
			
			if (currDistance < closestDistance)
			{
				closestDistance = currDistance;
				closestMember = member;
			}
		}
		
		return closestMember;
	}
	
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
		
	}
	public bool IsAttacking() { return currentState == Attacking; }
	
	
	private void GoingToLastSeenPosition()
	{
		
	}
	public bool IsGoingToLastSeenPosition() { return currentState == GoingToLastSeenPosition; }
	
	/************
	 * CALLBACKS
	 ************/
	 
	public void OnEnemyDetected(AbstractPlayer enemy)
	{
		enemiesInSight.Add(enemy);
	}
	
	public void OnLostSightOfEnemy(AbstractPlayer enemy)
	{
		enemiesInSight.Remove(enemy);
	}
}
