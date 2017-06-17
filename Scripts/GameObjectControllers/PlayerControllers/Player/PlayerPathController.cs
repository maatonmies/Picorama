using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AStar;

public class PlayerPathController: MonoBehaviour
{		
		public delegate void PlayerPathCalculated (List<Vector3> path,List<Vector3> pathCoords);
		public static event PlayerPathCalculated OnPlayerPathCalculated;
		
		public delegate void ActivePlayer (Transform activePlayer);
		public static event ActivePlayer OnActivePlayer;
		
		private Vector3 moveTarget;
		private Pathfinder pathfinder;
		protected tk2dTileMap map;
		private bool thisPlayerIsSelected = false;
		
		void OnEnable ()
		{
				InputManager.OnPlayerSelected += SetActivePlayerToGameObject;
				InputManager.OnMoveTargetSelected += RegisterMoveTarget;
				TileMapUpdate.OnTileMapUpdated += UpdateMap;
		}
		
		void OnDisable ()
		{
		
				InputManager.OnMoveTargetSelected -= RegisterMoveTarget;
				InputManager.OnPlayerSelected -= SetActivePlayerToGameObject;
				TileMapUpdate.OnTileMapUpdated -= UpdateMap;
		}
		
		protected virtual void Start ()
		{
				map = GameObject.Find ("TileMap").GetComponent <tk2dTileMap> ();
				
				if (map != null) 
						pathfinder = new Pathfinder (map);
				
				moveTarget = Vector3.zero;
				
		}
		
		private void SetActivePlayerToGameObject (Transform player)
		{				
				if (this.gameObject.transform != player) {
			
						this.thisPlayerIsSelected = false;
						return;
				}

				this.thisPlayerIsSelected = true;
				OnActivePlayer (player);
		}
		
		private void RegisterMoveTarget (Vector3 targetTileCoord, Vector3 targetTilePos, Transform player)
		{		
				if (thisPlayerIsSelected) {
								
						this.moveTarget = targetTileCoord;
						
						List<Vector3> path = MovePath ();
						
						List<Vector3> pathCoords = new List<Vector3> ();
			
						foreach (Vector3 pathStep in path) {
								int pathStepCoordX, pathStepCoordY;
								map.GetTileAtPosition (pathStep, out pathStepCoordX, out pathStepCoordY);
								pathCoords.Add (new Vector3 (pathStepCoordX, pathStepCoordY));
						}
						
						if (OnPlayerPathCalculated != null) {
				
								OnPlayerPathCalculated (path, pathCoords);
						}
				}		
		}
		
		protected List<Vector3> MovePath ()
		{
				int moveStartX, moveStartY;
				map.GetTileAtPosition (this.gameObject.transform.position, out moveStartX, out moveStartY);
				Vector3 moveStart = new Vector3 (moveStartX, moveStartY, 0);
				return pathfinder.FindPath (moveStart, moveTarget);
		}
		
		private void UpdateMap (tk2dTileMap newMap)
		{
				pathfinder = new Pathfinder (newMap);
		}
}
