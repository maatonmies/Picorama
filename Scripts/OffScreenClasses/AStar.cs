using UnityEngine;
using tk2dRuntime.TileMap;

using System.Collections;
using System.Collections.Generic;

namespace AStar
{
		public class SearchNode
		{
				public Vector2 position;
				
				public float distanceToGoal;
				public float distanceTraveled;
				
				public bool isInOpenList;
				public bool isInClosedList;
				
				public SearchNode parent;
				public SearchNode[] neighborNodes;
				
				public bool isWalkable;
		
		}

		public class Pathfinder
		{
				private SearchNode[,] searchNodes;
				
				private int levelWidth;
				private int levelHeight;
				
				private List<SearchNode> openList = new List<SearchNode> ();
				private List<SearchNode> closedList = new List<SearchNode> ();
				
				private tk2dTileMap map;
				private float tileCenter;
				
				
				
				public Pathfinder (tk2dTileMap tileMap)
				{
						if (tileMap != null) {
						
								map = tileMap;
				
								levelWidth = map.width;
								levelHeight = map.height;
								
								tileCenter = map.data.tileSize.x / 2;
				
								InitializeSearchNodes ();
								
								return;
						}
						
						Debug.LogError ("Invalid tilemap sent to Pathfinder !");
						
				}
				
				private void InitializeSearchNodes ()
				{
						searchNodes = new SearchNode[levelWidth, levelHeight];
						
						for (int x = 0; x < levelWidth; x++) {
								for (int y = 0; y < levelHeight; y++) {
				
										SearchNode node = new SearchNode ();
										
										node.position = new Vector2 (x, y);
										
										int tileId = map.GetTile ((int)(node.position.x), (int)(node.position.y), 0);
										
										if (tileId < 0) {
										
												Debug.LogError ("Invalid tiles found! Pathfinder cannot calculate paths for empty tiles. Please make sure the whole grid of the tileMap is painted!");
												Debug.Log (node.position);
												return;
										}
					
										node.isWalkable = map.GetTileInfoForTileId (tileId).stringVal == "path";
										
										if (node.isWalkable == true) {
										
												node.neighborNodes = new SearchNode[4];
												searchNodes [x, y] = node;
										}
								}
						}
						
						for (int x = 0; x < levelWidth; x++) {
								for (int y = 0; y < levelHeight; y++) {
					
										SearchNode node = searchNodes [x, y];
					
										if (node == null || node.isWalkable == false) {
												continue;
										}
					
										Vector2[] neighbors = new Vector2[]
										{
										   new Vector2 (x, y - 1),
										   new Vector2 (x, y + 1),
										   new Vector2 (x - 1, y),
										   new Vector2 (x + 1, y),
										};
										
										for (int i = 0; i < neighbors.Length; i ++) {
										
												Vector2 position = neighbors [i];
												
												if (position.x < 0 || position.x > levelWidth - 1 || 
														position.y < 0 || position.y > levelHeight - 1) {
														
														continue;
												}
												
												SearchNode neighborNode = searchNodes [
												(int)position.x, 
												(int)position.y];
												
												if (neighborNode == null || neighborNode.isWalkable == false) {
												
														continue;
												}
												
												node.neighborNodes [i] = neighborNode;
										}
								}
						}
						
				}
				
				private float Heuristic (Vector2 point1, Vector2 point2)
				{
						return Mathf.Abs (point1.x - point2.x) + Mathf.Abs (point1.y - point2.y);
								
						//return Mathf.Sqrt (Mathf.Pow ((point1.x - point2.x), 2) + Mathf.Pow ((point1.y - point2.y), 2));
				}
		
				private void ResetSearchNodes ()
				{
						openList.Clear ();
						closedList.Clear ();
						
						for (int x = 0; x < levelWidth; x++) {
						
								for (int y = 0; y < levelHeight; y++) {
								
										SearchNode node = searchNodes [x, y];
										
										if (node == null) {
												continue;
										}
										
										node.isInOpenList = false;
										node.isInClosedList = false;
										
										node.distanceTraveled = float.MaxValue;
										node.distanceToGoal = float.MaxValue;
								}
						}
				}
				
				private SearchNode FindBestNode ()
				{
				
						SearchNode currentNode = openList [0];
				
						float smallestDistanceToGoal = float.MaxValue;
						
						for (int i = 0; i < openList.Count; i ++) {
						
								if (openList [i].distanceToGoal < smallestDistanceToGoal) {
						
										currentNode = openList [i];
										smallestDistanceToGoal = currentNode.distanceToGoal;
								}
						}
						
						return currentNode;
				}
				
				private List<Vector3> FindFinalPath (SearchNode startNode, SearchNode endNode)
				{
						closedList.Add (endNode);
						
						SearchNode parentNode = endNode.parent;
						
						while (parentNode != startNode) {
						
								closedList.Add (parentNode);
								parentNode = parentNode.parent;
						}
						
						List<Vector3> finalPath = new List<Vector3> ();
						
						for (int i = closedList.Count - 1; i >= 0; i --) {
						
								Vector3 nodePosition = map.GetTilePosition ((int)(closedList [i].position.x),
								(int)(closedList [i].position.y));
								
								finalPath.Add (new Vector3 (nodePosition.x + tileCenter, nodePosition.y + tileCenter, 0));
						}
						
						return finalPath;
				}
				
				public List<Vector3> FindPath (Vector2 startPosition, Vector2 endPosition)
				{
						if (startPosition == endPosition) {
				
								return new List<Vector3> ();	
						} 
						
						ResetSearchNodes ();
						
						if ((int)endPosition.x > searchNodes.GetUpperBound (0)) {
								return new List<Vector3> ();
						}
						SearchNode startNode = searchNodes [(int)startPosition.x, (int)startPosition.y];
						SearchNode endNode = searchNodes [(int)endPosition.x, (int)endPosition.y];

						startNode.isInOpenList = true;
						startNode.distanceToGoal = Heuristic (startPosition, endPosition);
						startNode.distanceTraveled = 0;
				
						openList.Add (startNode);
			
						while (openList.Count > 0) {
						
								SearchNode currentNode = FindBestNode ();
						
								if (currentNode == null) {
								
										break;
								}
								
								if (currentNode == endNode) {
								
										return FindFinalPath (startNode, endNode);
								}
								
								for (int i = 0; i < currentNode.neighborNodes.Length; i++) {
								
										SearchNode neighbor = currentNode.neighborNodes [i];
										
										if (neighbor == null || neighbor.isWalkable == false) {
										
												continue;
										
										}
										
										float currentDistTraveled = currentNode.distanceTraveled + 1;
										float heuristic = Heuristic (neighbor.position, endPosition);
										
										if (neighbor.isInOpenList == false && neighbor.isInClosedList == false) {
										
												neighbor.distanceTraveled = currentDistTraveled;
												neighbor.distanceToGoal = currentDistTraveled + heuristic;
												neighbor.parent = currentNode;
												neighbor.isInOpenList = true;
												
												openList.Add (neighbor);
												
										} else if (neighbor.isInOpenList || neighbor.isInClosedList) {
										
												if (neighbor.distanceTraveled > currentDistTraveled) {
										
														neighbor.distanceTraveled = currentDistTraveled;
														neighbor.distanceToGoal = currentDistTraveled + heuristic;
														neighbor.parent = currentNode;
												}
										}
								}
								
								openList.Remove (currentNode);
								currentNode.isInClosedList = true;
						}
						
						//No path found
						
						return new List<Vector3> ();
				}
		}
}
