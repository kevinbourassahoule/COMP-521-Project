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
	public int 			EnemyTeamCount           = 1;
	public int 			PlayersPerTeam           = 5;
	public GameType 	GameType                 = GameType.PLAYER_VS_ENVIRONMENT;
	public float 		BulletSpeed              = .05f;
	public int		    PlayerMaxHealth          = 10;
	public float		PlayerMaxSpeed           = .02f;
	public float        PlayerMaxSight           = 3.5f;
	public float        PlayerShootWaitTime      = .25f;

	[Range(0,360)] public float	PlayerFOVAngle   = 120f;
	[Range(0,360)] public float	PlayerShootAngle = 30f;
	
	// Prefabs
	public GameObject	HumanPlayerPrefab;
	public GameObject	SquadPrefab;
	public GameObject	AIPlayerPrefab;
	public GameObject 	BulletPrefab;
	public GameObject   Map1Prefab;
	
	// Static map properties
	public static float  Height { get; private set; }
	public static float  Width  { get; private set; }
	public static Wall[] Walls  { get; private set; }

	// Mesh vertices
	private MeshFilter visibilityPolygonMeshFilter;
	public Vector3[] MeshVertices { get; set; }
	
	// Singleton instance of environment
	public static Environment Instance { get; private set; }
	
	// Use this for initialization
	void Start () 
	{
		// Set singleton instance
		Instance = GetComponent<Environment>();
		
		// Initialize map properties
		/*Height = transform.FindChild("Map/Floor").GetComponent<Renderer>().bounds.size.y;
		Width  = transform.FindChild("Map/Floor").GetComponent<Renderer>().bounds.size.x;
		*/
		GameObject Map = (GameObject)GameObject.Instantiate (Map1Prefab, Vector2.zero, Quaternion.identity);
		Map.transform.parent = this.transform;
		Map.name = "Map";
		Height = transform.FindChild("Map/TopLeft").position.y - transform.FindChild("Map/BotLeft").position.y; 
		Width  = transform.FindChild("Map/BotRight").position.x - transform.FindChild("Map/BotLeft").position.x;
		Walls  = transform.FindChild("Map/Walls").GetComponentsInChildren<Wall>();
		
		// Initialize mesh properties
		visibilityPolygonMeshFilter = transform.FindChild ("VisibilityPolygon").GetComponent<MeshFilter> ();
		MeshVertices = visibilityPolygonMeshFilter.mesh.vertices;
		
		// Spawn players 
		// FIXME 1v1 for now...
		switch (GameType)
		{
			case GameType.PLAYER_VS_ENVIRONMENT:
				GameObject team;
			
				// Spawn human player
				team = new GameObject("Team0");
				team.transform.parent = transform.FindChild("Teams");
				GameObject human = (GameObject) GameObject.Instantiate(HumanPlayerPrefab, new Vector2(Width*.1f, Height*.1f), Quaternion.identity);
				human.transform.parent = team.transform;
				
				// Spawn enemy squads
				for (int i = 0; i < EnemyTeamCount; i++)
				{
					team = new GameObject("Team" + (i + 1).ToString());
					team.transform.parent = transform.FindChild("Teams");
					GameObject squad = (GameObject) GameObject.Instantiate(SquadPrefab, new Vector2(Width*.75f, Height*.75f), Quaternion.identity);
					squad.transform.parent = team.transform;
				
				}
				
				break;
		}
	}
	
	void Update()
	{
		MeshVertices = visibilityPolygonMeshFilter.mesh.vertices;
	}
}
