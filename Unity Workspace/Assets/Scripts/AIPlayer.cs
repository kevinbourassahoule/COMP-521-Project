using UnityEngine;
using System.Collections.Generic;

public class AIPlayer : AbstractPlayer 
{	
	public AbstractPlayer Target { get; set; }
	
	private Squad squad;
	
	// Use this for initialization
	void Start () 
	{
		this.squad = GameObject.Find("Environment/Squad").GetComponent<Squad>(); // TODO change this if multiple squads
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	/*
	 * Returns whether or not this player has his current target in his line of sight.
	 */
	private bool SeesTarget()
	{
		// Check whether the target is within this player's field of view
		if (Vector3.Angle(transform.forward, Target.transform.position - transform.position) > FOVangle)
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
	
	// TODO Could be cool for squad decision-making
//	private List<AbstractPlayer> EnemiesInSight()
//	{
//		
//	}
}
