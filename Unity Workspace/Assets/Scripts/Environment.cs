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
	public int 			EnemyTeamCount;
	public int 			PlayersPerTeam;
	public GameType 	Type;
	public float 		BulletSpeed;
	public float		PlayerMaxHealth;
	public float		PlayerMaxSpeed;
	public float        PlayerMaxSight;
	public float        PlayerShootWaitTime;

	[Range(0,360)] public float	PlayerFOVAngle;
	[Range(0,360)] public float	PlayerSHOOTAngle;
	// Prefabs
	public GameObject	HumanPlayerPrefab;
	public GameObject	SquadPrefab;
	public GameObject	AIPlayerPrefab;
	public GameObject 	BulletPrefab;
	
	// Static map properties
	public static float  Height { get; private set; }
	public static float  Width  { get; private set; }
	public static Wall[] Walls  { get; private set; }

	// Mesh vertices
	private MeshFilter VisibilityPolygonMeshFilter;
	public Vector3[] MeshVertices { get; set; }
	
	// Singleton instance of environment
	public static Environment Instance { get; private set; }
	
	// Use this for initialization
	void Start () 
	{
		Instance = GetComponent<Environment>();
		
		/*Height = transform.FindChild("Map/Floor").GetComponent<Renderer>().bounds.size.y;
		Width  = transform.FindChild("Map/Floor").GetComponent<Renderer>().bounds.size.x;
		*/
		Height = transform.FindChild("Map/TopLeft").position.y - transform.FindChild("Map/BotLeft").position.y; 
		Width  = transform.FindChild("Map/BotRight").position.x - transform.FindChild("Map/BotLeft").position.x;
		Walls  = transform.FindChild("Map/Walls").GetComponentsInChildren<Wall>();
		VisibilityPolygonMeshFilter = transform.FindChild ("VisibilityPolygon").GetComponent<MeshFilter> ();
		MeshVertices = VisibilityPolygonMeshFilter.mesh.vertices;
		PlayerFOVAngle      = 120.0f;
		PlayerSHOOTAngle    = 30.0f;
		PlayerMaxSight      = 3.5f;
		PlayerShootWaitTime = .25f;
		BulletSpeed         = .05f;
		// Spawn players 
		// FIXME 1v1 for now...
		switch (Type)
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
		MeshVertices = VisibilityPolygonMeshFilter.mesh.vertices;
	}
}
