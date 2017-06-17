using UnityEngine;
using System.Collections;

[RequireComponent(typeof(tk2dTileMap))]

public class TileMapUpdate : MonoBehaviour
{
	public delegate void TileMapUpdated (tk2dTileMap newMap);
	public static event TileMapUpdated OnTileMapUpdated;
		
	private tk2dTileMap tileMap;
		
	void OnEnable ()
	{
		EnemyAnimator.OnEnemyIsDead += MakeEnemyTileWalkable;
		ExitController.OnExitIsReady += MakeExitWalkable;
		LevelManager.OnStartLevel += ResetMap;
	}
		
	void OnDisable ()
	{		
		EnemyAnimator.OnEnemyIsDead -= MakeEnemyTileWalkable;
		ExitController.OnExitIsReady -= MakeExitWalkable;
		LevelManager.OnStartLevel -= ResetMap;

	}
		
	void Start ()
	{
		tileMap = GetComponent<tk2dTileMap> ();
	}

	private void ResetMap (Transform startingPlayer)
	{
		tileMap.GetTileInfoForTileId (24).stringVal = null;
		tileMap.Build ();
		
		if (OnTileMapUpdated != null) {
			OnTileMapUpdated (tileMap);
		}

	}
		
	private void MakeEnemyTileWalkable (Transform enemy, Vector3 playerCoordinates, Transform player)
	{
		int x, y;
		tileMap.GetTileAtPosition (enemy.position, out x, out y);
		tileMap.ClearTile (x, y, 1);
		tileMap.SetTile (x, y, 0, 4); // Set "path" tile for BottomLayer
		tileMap.Build ();
				
		if (OnTileMapUpdated != null) {
			OnTileMapUpdated (tileMap);
		}

	}

	private void MakeExitWalkable (Vector3 exitPos)
	{
		tileMap.GetTileInfoForTileId (tileMap.GetTileIdAtPosition (exitPos, 0)).stringVal = "path";
		tileMap.Build ();
		
		if (OnTileMapUpdated != null) {
			OnTileMapUpdated (tileMap);
		}
	}
}
