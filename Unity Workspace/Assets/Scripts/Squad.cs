using UnityEngine;
using System.Collections.Generic;

public class Squad : MonoBehaviour 
{
	public int Count;
	public GameObject AIPlayerPrefab;

	private Vector2 				objective;
	private List<AIPlayer> 			members 		= new List<AIPlayer>();
	private HashSet<AbstractPlayer> enemiesInSight 	= new HashSet<AbstractPlayer>();
	
	// A reference to the squad's current state
	private delegate void State();
	private State currentState;

	// Use this for initialization
	void Start () 
	{
		// Spawn squad members
		for (int i = 0; i < this.Count; i++)
		{
			GameObject member = (GameObject) GameObject.Instantiate(AIPlayerPrefab,
								   					   				(Vector2)transform.position + Vector2.right * i,	// TODO more graceful spawn position?
								   					   				Quaternion.identity);
			members.Add(member.GetComponent<AIPlayer>());
		}
		
		objective = transform.position;
		
		currentState = Patrolling;
	}
	
	// Update is called once per frame
	void Update () 
	{
		currentState();
		
		MoveTowardsObjective();
	}	
	
	private bool IsAtObjective()
	{
		return transform.position.Equals(objective);
	}
	
	private void MoveTowardsObjective()
	{
		
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
			// TODO optimize
			objective.x = Random.Range(0, GameObject.Find("Environment").GetComponent<Environment>().Width);
			objective.y = Random.Range(0, GameObject.Find("Environment").GetComponent<Environment>().Height);
		}
	}
	
	private void Attacking()
	{
		
	}
	
	private void GoingToLastSeenPosition()
	{
		
	}
	
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
