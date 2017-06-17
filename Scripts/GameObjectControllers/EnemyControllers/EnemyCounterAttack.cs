using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyCounterAttack : MonoBehaviour
{			
	public delegate void EnemyStartCounterAttack (Vector3 playerCoord,Transform player,Transform enemy,Vector3 enemyPos,bool blockedByOrac);
	public static event EnemyStartCounterAttack OnEnemyStartCounterAttack;
		
	public delegate void PlayerGetReadyToDie (Transform player,Transform enemy);
	public static event PlayerGetReadyToDie OnPlayerGetReadyToDie;
	
	public delegate void OracDefendStart ();
	public static event OracDefendStart OnOracDefendStart;
		
	public List<Vector3>
		counterAttackCoords;

	public bool inBlubbaRange = false;
	public bool alreadyInTrance = false;
	
	private List<Vector3>attackRange;

	private Vector3 tileMapCoord;

	private bool counterAttackEnabled = true;
	private bool enemy1 = false;
	private bool enemy2 = false;
	private bool enemy3 = false;

	private bool blockedByOrac = false;

	private tk2dTileMap tileMap;
		
	void OnEnable ()
	{
		InputManager.OnEnemySelected += StartCounterAttackOrTranceOrDeath;

		PlayersEnemySensor.OnEnemyInRange += BeInBlubbaRange;
		PlayersEnemySensor.OnEnemyOutOfRange += BeOutOfBlubbaRange;

		PlayerMoveAnimator.OnPlayerIsMoving += ReleaseTranceOnPlayerIsMoving;
		PlayerDeath.OnPlayerIsDead += ReleaseTranceOnPlayerIsDead;
		TileMapUpdate.OnTileMapUpdated += UpdateCounterAttackCoords;

		InputManager.OnPlayerSelected += ReleaseTranceOnPlayerSelected;
	}
	void OnDisable ()
	{
		InputManager.OnEnemySelected -= StartCounterAttackOrTranceOrDeath;

		PlayersEnemySensor.OnEnemyInRange -= BeInBlubbaRange;
		PlayersEnemySensor.OnEnemyOutOfRange -= BeOutOfBlubbaRange;

		PlayerMoveAnimator.OnPlayerIsMoving -= ReleaseTranceOnPlayerIsMoving;
		PlayerDeath.OnPlayerIsDead -= ReleaseTranceOnPlayerIsDead;
		TileMapUpdate.OnTileMapUpdated -= UpdateCounterAttackCoords;

		InputManager.OnPlayerSelected -= ReleaseTranceOnPlayerSelected;
	}
		
	void Start ()
	{
		counterAttackCoords = new List<Vector3> ();
		tileMap = GameObject.Find ("TileMap").GetComponent <tk2dTileMap> ();
		attackRange = new List<Vector3> ();

		int thisX, thisY;
		tileMap.GetTileAtPosition (transform.position, out thisX, out thisY);
		
		tileMapCoord = new Vector3 (thisX, thisY);

		enemy1 = this.name == "Enemy1";
		enemy2 = this.name == "Enemy2";
		enemy3 = this.name == "Enemy3";

		if (enemy1) {

			attackRange.Add (new Vector3 (transform.position.x, transform.position.y + 128));
			attackRange.Add (new Vector3 (transform.position.x + 128, transform.position.y + 128));
			attackRange.Add (new Vector3 (transform.position.x + 128, transform.position.y));
			attackRange.Add (new Vector3 (transform.position.x + 128, transform.position.y - 128));
			attackRange.Add (new Vector3 (transform.position.x, transform.position.y - 128));
			attackRange.Add (new Vector3 (transform.position.x - 128, transform.position.y - 128));
			attackRange.Add (new Vector3 (transform.position.x - 128, transform.position.y));
			attackRange.Add (new Vector3 (transform.position.x - 128, transform.position.y + 128));

		} else if (enemy2) {

			attackRange.Add (new Vector3 (transform.position.x, transform.position.y + 256));
			attackRange.Add (new Vector3 (transform.position.x + 256, transform.position.y));
			attackRange.Add (new Vector3 (transform.position.x, transform.position.y - 256));
			attackRange.Add (new Vector3 (transform.position.x - 256, transform.position.y));

		} else if (enemy3) {

			attackRange.Add (new Vector3 (transform.position.x + 128, transform.position.y + 128));
			attackRange.Add (new Vector3 (transform.position.x + 128, transform.position.y - 128));
			attackRange.Add (new Vector3 (transform.position.x - 128, transform.position.y - 128));
			attackRange.Add (new Vector3 (transform.position.x - 128, transform.position.y + 128));

			int multiplierLeft = (int)tileMapCoord.x;
			int multiplierRight = tileMap.width - (int)tileMapCoord.x;
			int multiplierDownwards = (int)tileMapCoord.y;
			int multiplierUpwards = tileMap.height - (int)tileMapCoord.y;

			for (int i = 1; i< multiplierLeft; i ++) {

				attackRange.Add (new Vector3 (transform.position.x - (i * 128), transform.position.y));
			}

			for (int j = 1; j< multiplierRight; j ++) {
				
				attackRange.Add (new Vector3 (transform.position.x + (j * 128), transform.position.y));
			}

			for (int k = 1; k< multiplierUpwards; k ++) {
				
				attackRange.Add (new Vector3 (transform.position.x, transform.position.y + (k * 128)));
			}

			for (int l =1; l< multiplierDownwards; l ++) {
				
				attackRange.Add (new Vector3 (transform.position.x, transform.position.y - (l * 128)));
			}
		}

		UpdateCounterAttackCoords (tileMap);
	}
		
	void StartCounterAttackOrTranceOrDeath (Transform enemy, Vector3 playerCoord, Transform player)
	{	
		//if (player.name == "Blubba" && !inBlubbaRange && enemy.GetComponent<EnemyCounterAttack> ().alreadyInTrance)
		//return;
		//TRANCE
		if (player.name == "Blubba" && inBlubbaRange && !alreadyInTrance) {

			if (player.GetComponent<BlubbaController> ().enemiesInTrance.Count < 3) {

				if (enemy == transform) {

					gameObject.SendMessage ("StartTranceAnim");
					player.SendMessage ("StartAttackAnim");
					counterAttackEnabled = false;
					alreadyInTrance = true;
					return;
				}

				int enemyX, enemyY;
				tileMap.GetTileAtPosition (enemy.position, out enemyX, out enemyY);

				Vector3 enemyCoord = new Vector3 (enemyX, enemyY);

				if (Mathf.Abs (enemyCoord.x - tileMapCoord.x) == 1 || 
					Mathf.Abs (enemyCoord.x - tileMapCoord.x) == 2) {

					if (enemyCoord.y == tileMapCoord.y && tileMapCoord.y != playerCoord.y) {

						gameObject.SendMessage ("StartTranceAnim");
						player.SendMessage ("StartAttackAnim");
						counterAttackEnabled = false;
						alreadyInTrance = true;
					} else {

						inBlubbaRange = false;
						counterAttackEnabled = true;
						alreadyInTrance = false;
						this.BroadcastMessage ("HideAllTiles");

					}
			
				} else if (Mathf.Abs (enemyCoord.y - tileMapCoord.y) == 1 || 
					Mathf.Abs (enemyCoord.y - tileMapCoord.y) == 2) {

					if (enemyCoord.x == tileMapCoord.x && tileMapCoord.x != playerCoord.x) {

						Vector3 enemyLeftPos = new Vector3 (enemy.position.x - 128, enemy.position.y);
						Vector3 enemyRightPos = new Vector3 (enemy.position.x + 128, enemy.position.y);

						Vector3 enemySecondLeftPos = new Vector3 (enemy.position.x - 256, enemy.position.y);
						Vector3 enemySecondRightPos = new Vector3 (enemy.position.x + 256, enemy.position.y);

						Collider2D [] enemyRight = new Collider2D[1];
						Collider2D [] enemyLeft = new Collider2D[1];

						Collider2D [] enemySecondRight = new Collider2D[1];
						Collider2D [] enemySecondLeft = new Collider2D[1];

						int layerMaskEnemy = 1 << 11;

						int leftNeighborFound = Physics2D.OverlapPointNonAlloc (enemyLeftPos, enemyLeft, layerMaskEnemy);
						int rightNeighborFound = Physics2D.OverlapPointNonAlloc (enemyRightPos, enemyRight, layerMaskEnemy);

						int secondLeftNeighborFound = Physics2D.OverlapPointNonAlloc (enemySecondLeftPos, enemySecondLeft, layerMaskEnemy);
						int secondRightNeighborFound = Physics2D.OverlapPointNonAlloc (enemySecondRightPos, enemySecondRight, layerMaskEnemy);

						bool otherEnemiesMatchToLeft = false;
						bool otherEnemiesMatchToRight = false;

						if (leftNeighborFound > 0 && enemyLeft [0].GetComponent<EnemyCounterAttack> ().inBlubbaRange) {

							otherEnemiesMatchToLeft = true;
						}

						if (secondLeftNeighborFound > 0 && enemySecondLeft [0].GetComponent<EnemyCounterAttack> ().inBlubbaRange) {
							
							otherEnemiesMatchToLeft = true;
						}

						if (rightNeighborFound > 0 && enemyRight [0].GetComponent<EnemyCounterAttack> ().inBlubbaRange) {
							otherEnemiesMatchToRight = true;
						}

						if (secondRightNeighborFound > 0 && enemySecondRight [0].GetComponent<EnemyCounterAttack> ().inBlubbaRange) {
							otherEnemiesMatchToRight = true;
						}
														
						if (otherEnemiesMatchToLeft || otherEnemiesMatchToRight) {

							inBlubbaRange = false;
							counterAttackEnabled = true;
							alreadyInTrance = false;
							this.BroadcastMessage ("HideAllTiles");

						} else {
							gameObject.SendMessage ("StartTranceAnim");
							player.SendMessage ("StartAttackAnim");
							counterAttackEnabled = false;
							alreadyInTrance = true;		
						}										
		
					} else {

						inBlubbaRange = false;
						counterAttackEnabled = true;
						alreadyInTrance = false;
						this.BroadcastMessage ("HideAllTiles");

					}

				} else {

					inBlubbaRange = false;
					counterAttackEnabled = true;
					alreadyInTrance = false;
					this.BroadcastMessage ("HideAllTiles");

				}

			} else {
				inBlubbaRange = false;
				counterAttackEnabled = true;
				alreadyInTrance = false;
				this.BroadcastMessage ("HideAllTiles");

			}

			//NO COUNTERATTACK WHEN IN TRANCE
			if (! counterAttackEnabled)
				return;
		}

		//DEATH OR IGNORE (if not Blubba attacking)
		
		if (! counterAttackCoords.Contains (playerCoord) || ! counterAttackEnabled || transform == enemy)
			return;

		//ENEMY2 & ENEMY3 DETECT WALLS AND ORAC
		if (enemy2 || enemy3) {

			RaycastHit2D[] wall = new RaycastHit2D [1];
			int layerMaskWalls = 1 << 17;        
			int wallHit = Physics2D.LinecastNonAlloc (transform.position, player.position, wall, layerMaskWalls);
			
			if (wallHit > 0)
				return;

			RaycastHit2D[] playerRayCast = new RaycastHit2D [4];
			int layerMaskPlayer = 1 << 8;        
			int playerHit = Physics2D.LinecastNonAlloc (transform.position, player.position, playerRayCast, layerMaskPlayer);
			
			if (playerHit > 0) {

				foreach (RaycastHit2D playerFound in playerRayCast) {
					if (playerFound.collider != null && playerFound.collider.name == "Orac") {
						if (playerCoord.x != tileMapCoord.x) {
							if (playerCoord.y != tileMapCoord.y) {
								blockedByOrac = false;
								// DIAGONAL COUNTER ATTACK STILL ON FOR ENEMY3 WHEN ORAC IS NEAR
								StartCoroutine (CounterAttack (playerCoord, player));
								if (OnPlayerGetReadyToDie != null)
									OnPlayerGetReadyToDie (player, this.transform);
								return;
							} else {
								blockedByOrac = true;
								if (OnOracDefendStart != null)
									OnOracDefendStart ();
								
							}
						} else {
							
							blockedByOrac = true;
							if (OnOracDefendStart != null)
								OnOracDefendStart ();
						}
						//BLOCKED COUNTER ATTACK
						StartCoroutine (CounterAttack (playerCoord, player));
						return;
					} else {
						blockedByOrac = false;
					}
				}
			} else {
				blockedByOrac = false;
			}
		} 				
								
		//COUNTER ATTACK
	   						
		StartCoroutine (CounterAttack (playerCoord, player));
				
		if (OnPlayerGetReadyToDie != null) {

			OnPlayerGetReadyToDie (player, this.transform);
		}
											
	}
		
	IEnumerator CounterAttack (Vector3 playerCoord, Transform player)
	{
		if (player.name == "Pinky")
			yield return new WaitForSeconds (2);
		else 
			yield return new WaitForSeconds (0.8f);

		//counterAttackEnabled = false;

		if (OnEnemyStartCounterAttack != null) {
			OnEnemyStartCounterAttack (playerCoord, player, this.transform, this.transform.position, blockedByOrac);
		}
	}

	private void UpdateCounterAttackCoords (tk2dTileMap newMap)
	{
		tk2dTileMap map = newMap;
		counterAttackCoords.Clear ();

		foreach (Vector3 cAttackPos in attackRange) {
			
			int tileId = map.GetTileIdAtPosition (cAttackPos, 0);
				
			if (tileId > 0) {
				if (map.GetTileInfoForTileId (tileId).stringVal == "path") {

					int x, y;
					map.GetTileAtPosition (cAttackPos, out x, out y);
					
					Vector3 coord = new Vector3 (x, y);
					
					counterAttackCoords.Add (coord);
				}
			}
		}
	}
		
	private void BeInBlubbaRange (Transform enemy, Transform player, Transform sensor)
	{
		if (transform == enemy) {

			if (player.name == "Blubba" && !alreadyInTrance) {
				
				inBlubbaRange = true;

			} 
		}
	}

	private void BeOutOfBlubbaRange (Transform enemy, Transform player, Transform sensor)
	{
		if (transform == enemy) {
			
			if (player.name == "Blubba") {
				
				gameObject.SendMessage ("Idle");
				inBlubbaRange = false;
				counterAttackEnabled = true;
				alreadyInTrance = false;
			} 
		}
	}
	
	private void ReleaseTranceOnPlayerIsMoving (Transform player)
	{

		if (! alreadyInTrance && ! counterAttackEnabled)
			counterAttackEnabled = true;

		if (player.name == "Blubba") {
				
			if (alreadyInTrance) {
				gameObject.SendMessage ("TranceFinish");
				inBlubbaRange = false;
				alreadyInTrance = false;
			}
		}
	}
		
	private void ReleaseTranceOnPlayerIsDead (Transform player)
	{

		if (! counterAttackEnabled)
			counterAttackEnabled = true;

		if (alreadyInTrance) {
			gameObject.SendMessage ("TranceFinish");
			inBlubbaRange = false;
			alreadyInTrance = false;
		}	
	}

	private void ReleaseTranceOnPlayerSelected (Transform player)
	{

		if (! alreadyInTrance)
			counterAttackEnabled = true;

		if (player.name == "Blubba") {

			if (alreadyInTrance) {
				gameObject.SendMessage ("TranceFinish");
				inBlubbaRange = false;
				alreadyInTrance = false;
				counterAttackEnabled = true;
			} 
		}	
	}
		
	private void Enemy2Blitz () //called by enemy 2 attack anim
	{
		transform.Find ("Blitz").GetComponent<Enemy2Blitz> ().SendMessage ("StartBlitzAnim");
	}

}
