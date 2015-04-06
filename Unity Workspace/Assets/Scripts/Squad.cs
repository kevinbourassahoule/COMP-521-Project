using UnityEngine;
using System.Collections.Generic;

public class Squad : MonoBehaviour 
{
	public int NumberOfSquadMembers;
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
		for (int i = 0; i < NumberOfSquadMembers; i++)
		{
			GameObject member = (GameObject) GameObject.Instantiate(AIPlayerPrefab,
								   					   				(Vector2)transform.position + Vector2.right * i,	// TODO more graceful spawn position?
								   					   				Quaternion.identity);
			members.Add(member.GetComponent<AIPlayer>());
		}
		
		currentState = Patrolling;
	}
	
	// Update is called once per frame
	void Update () 
	{
		currentState();
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
	
	private void Patrolling()
	{
		
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
