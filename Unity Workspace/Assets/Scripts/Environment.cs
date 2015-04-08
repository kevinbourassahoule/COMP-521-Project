using UnityEngine;
using System.Collections.Generic;

public enum GameType
{
	PLAYER_VS_PLAYER,
	PLAYER_VS_ENVIRONMENT,
	ENVIRONMENT_VS_ENVIRONMENT
}

public class Environment : MonoBehaviour 
{
	// Environment properties to be defined in editor
	public int 			TeamCount;
	public int 			PlayersPerTeam;
	public GameType 	Type;
	public float 		BulletSpeed;
	public float		PlayerMaxHealth;
	public float		PlayerMaxSpeed;
	
	[Range(0,360)] public float	PlayerFOVAngle;
	
	// Prefabs
	public GameObject	HumanPlayerPrefab;
	public GameObject	SquadPrefab;
	public GameObject	AIPlayerPrefab;
	public GameObject 	BulletPrefab;
	
	// Static map properties
	public static float  Height { get; private set; }
	public static float  Width  { get; private set; }
	public static Wall[] Walls  { get; private set; }
	
	// Singleton instance of environment
	public static Environment Instance { get; private set; }
	
	// Use this for initialization
	void Start () 
	{
		Instance = GetComponent<Environment>();
		
		Height = transform.FindChild("Map/Floor").GetComponent<Renderer>().bounds.size.y;
		Width  = transform.FindChild("Map/Floor").GetComponent<Renderer>().bounds.size.x;
		Walls  = transform.FindChild("Map/Walls").GetComponentsInChildren<Wall>();
		
		// Spawn players 
		// FIXME 1v1 for now...
		switch (Type)
		{
		case GameType.PLAYER_VS_ENVIRONMENT:
			// Spawn human player
			GameObject human = (GameObject) GameObject.Instantiate(HumanPlayerPrefab, Vector2.zero, Quaternion.identity);
			human.transform.parent = transform.FindChild("Teams");
			
			// Spawn enemy squads
			GameObject squad = (GameObject) GameObject.Instantiate(SquadPrefab, Vector2.zero, Quaternion.identity);
			squad.transform.parent = transform.FindChild("Teams");
			
			break;
		}
	}
}
