using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{				
	public delegate void PlayerSelected (Transform player);
	public static event PlayerSelected OnPlayerSelected;
		
	public delegate void MoveTargetSelected (Vector3 targetTileCoord,Vector3 targetTilePos,Transform player);
	public static event MoveTargetSelected OnMoveTargetSelected;
		
	public delegate void EnemySelected (Transform enemy,Vector3 playerCoord,Transform player);
	public static event EnemySelected OnEnemySelected;
		
	public delegate void EnemyUnreachable (Transform player);
	public static event EnemyUnreachable OnEnemyUnreachable;
	
	public delegate void RetrySelected (Transform deadPlayer);
	public static event RetrySelected OnRetrySelected;

	public delegate void ExitSelected (Transform player);
	public static event ExitSelected OnExitSelected;
	
	public bool zoomOutEnabled = false;
	public bool touchControls;
	public bool mouseControls;
				
	private Camera mainCamera;
	private CameraDrag cameraDrag;
	private CameraScriptedAnimations cameraScriptedAnim;

	private tk2dTileMap tileMap;
		
	private bool inputEnabled = false;		
	private bool playerMoveEnabled = false;

	private bool cameraDragAllowed = false;
	private bool playerIsDead = false;
	
	private List<Transform> clickableEnemies;
	private Transform attackingPlayer;
	private Transform activePlayer;
	private Transform deadPlayer;

	private Transform Pinky;
	private Transform Blubba;
	private Transform Greengo;
	private Transform Orac;
			
	private bool ignoreTouchEnded = false;
	private bool pinchRegistered = false;
			
	void OnEnable ()
	{
		LevelManager.OnStartLevel += AssignStartingPlayer;
		
		PlayerDeath.OnBlubbaFailedAttack += DisablePlayerMovement;
		PlayerDeath.OnPlayerIsDead += DisablePlayerMovementOnPlayerIsDead;
		PlayerMoveAnimator.OnPlayerIsMoving += DisablePlayerMovementOnPlayerIsMoving;
		PlayerMoveAnimator.OnPlayerFinishedMovement += EnablePlayerMovement;
				
		PlayersEnemySensor.OnEnemyInRange += RegisterEnemyForInput;
		PlayersEnemySensor.OnEnemyOutOfRange += UnregisterEnemyFromInput;
		
		LevelManager.OnPlayOutroAnimations += DisableInput;
		EnemyCounterAttack.OnOracDefendStart += DisablePlayerMovement;
		OracDefend.OnOracFinishedDefend += EnablePlayerMovementOnOracFinishedDefend;
	}
		
	void OnDisable ()
	{
		LevelManager.OnStartLevel -= AssignStartingPlayer;
		
		PlayerDeath.OnBlubbaFailedAttack -= DisablePlayerMovement;
		PlayerDeath.OnPlayerIsDead -= DisablePlayerMovementOnPlayerIsDead;
		PlayerMoveAnimator.OnPlayerIsMoving -= DisablePlayerMovementOnPlayerIsMoving;

		PlayerMoveAnimator.OnPlayerFinishedMovement -= EnablePlayerMovement;

									
		PlayersEnemySensor.OnEnemyInRange -= RegisterEnemyForInput;
		PlayersEnemySensor.OnEnemyOutOfRange -= UnregisterEnemyFromInput;
		
		LevelManager.OnPlayOutroAnimations -= DisableInput;	
		
		EnemyCounterAttack.OnOracDefendStart -= DisablePlayerMovement;
		OracDefend.OnOracFinishedDefend -= EnablePlayerMovementOnOracFinishedDefend;
	}
		
	void Start ()
	{
		mainCamera = GameObject.Find ("Camera").GetComponent<Camera> ();
		cameraDrag = mainCamera.gameObject.GetComponent<CameraDrag> ();
		cameraScriptedAnim = mainCamera.gameObject.GetComponent<CameraScriptedAnimations> ();

		tileMap = GameObject.Find ("TileMap").GetComponent <tk2dTileMap> ();
		
		clickableEnemies = new List<Transform> ();
		attackingPlayer = null;
		activePlayer = null;
		deadPlayer = null;

		if (GameObject.Find ("Pinky") != null)
			Pinky = GameObject.Find ("Pinky").transform;

		if (GameObject.Find ("Blubba") != null)
			Blubba = GameObject.Find ("Blubba").transform;

		if (GameObject.Find ("Greengo") != null)
			Greengo = GameObject.Find ("Greengo").transform;

		if (GameObject.Find ("Orac") != null)
			Orac = GameObject.Find ("Orac").transform;					

	}

	public void PinkySelectorButton ()
	{
		if (Pinky != null && playerMoveEnabled) {

			activePlayer = Pinky;			
			
			if (OnPlayerSelected != null) 
				OnPlayerSelected (Pinky);

			mainCamera.gameObject.SendMessage ("FocusCameraOnPlayer", Pinky);
		}

	}

	public void BlubbaSelectorButton ()
	{
		if (Blubba != null && playerMoveEnabled) {
			
			activePlayer = Blubba;			
			
			if (OnPlayerSelected != null) 
				OnPlayerSelected (Blubba);

			mainCamera.gameObject.SendMessage ("FocusCameraOnPlayer", Blubba);

		}
	}

	public void GreengoSelectorButton ()
	{
		if (Greengo != null && playerMoveEnabled) {
			
			activePlayer = Greengo;			
			
			if (OnPlayerSelected != null) 
				OnPlayerSelected (Greengo);

			mainCamera.gameObject.SendMessage ("FocusCameraOnPlayer", Greengo);

		}
	}

	public void OracSelectorButton ()
	{
		if (Orac != null && playerMoveEnabled) {
			
			activePlayer = Orac;			
			
			if (OnPlayerSelected != null) 
				OnPlayerSelected (Orac);

			mainCamera.gameObject.SendMessage ("FocusCameraOnPlayer", Orac);

		}
	}
		
	private void AssignStartingPlayer (Transform startingPlayer)
	{
		if (OnPlayerSelected != null)
			OnPlayerSelected (startingPlayer);
						
		activePlayer = startingPlayer;
				
		inputEnabled = true;
		playerMoveEnabled = true;
		cameraDragAllowed = true;
	}
		
	private void TouchControls ()
	{
	
		if (Input.touchCount > 0) {
			
			int touchCount = Input.touchCount;	
			
			if (touchCount == 1) {
				
				Touch touchZero = Input.GetTouch (0);
				Vector2 touchPos = touchZero.position;
				Vector2 touchDelta = touchZero.deltaPosition;
				
				switch (touchZero.phase) {
					
				case TouchPhase.Began:
					
					ignoreTouchEnded = false;
					
					break;
					
				case TouchPhase.Moved:
					
					if (cameraDragAllowed) {
						
						Vector2 dragAmount = new Vector2 (touchDelta.x, touchDelta.y);
						cameraDrag.SendMessage ("Drag", -dragAmount);
						ignoreTouchEnded = true;
					}
					
					break;
					
				case TouchPhase.Ended:
					
					if (! ignoreTouchEnded) {

						// UI Touch
						if (IsPointerOverUIObject (touchPos)) 
							return;
						
						Vector3 touchWorldPos = mainCamera.GetComponent<Camera> ().ScreenToWorldPoint (touchPos);
						
						Collider2D [] player = new Collider2D[1];
						Collider2D [] enemy = new Collider2D[1];
						Collider2D [] retryButton = new Collider2D[1]; 
						Collider2D [] exit = new Collider2D[1];    
						
						int layerMaskPlayer = 1 << 8;
						int layerMaskEnemy = 1 << 11;
						int layerMaskRetryButton = 1 << 10;   
						int layerMaskExit = 1 << 13;            
						
						
						int playerClicked = Physics2D.OverlapPointNonAlloc (touchWorldPos, player, layerMaskPlayer);
						int enemyClicked = Physics2D.OverlapPointNonAlloc (touchWorldPos, enemy, layerMaskEnemy);
						int retryButtonClicked = Physics2D.OverlapPointNonAlloc (touchWorldPos, retryButton, layerMaskRetryButton);    
						int exitClicked = Physics2D.OverlapPointNonAlloc (touchWorldPos, exit, layerMaskExit);    
						
						
						//RETRY BUTTON
						if (retryButtonClicked > 0) {
							
							if (OnRetrySelected != null)
								OnRetrySelected (deadPlayer);
							playerIsDead = false;   
						}
						
						if (playerMoveEnabled) {
							
							//PLAYER
							
							if (playerClicked > 0) {
								
								Transform selectedPlayer = player [0].transform;
								activePlayer = selectedPlayer;            
								
								if (OnPlayerSelected != null) 
									OnPlayerSelected (selectedPlayer);
								
								clickableEnemies.Clear ();
								return;
								
							}
							
							//ENEMY
							
							if (enemyClicked > 0) {
								
								if (clickableEnemies.Count > 0 && clickableEnemies.Contains (enemy [0].transform)) {
									
									int x, y;
									tileMap.GetTileAtPosition (attackingPlayer.position, out x, out y);
									
									Vector3 playerCoord = new Vector3 (x, y);
									
									if (OnEnemySelected != null) {
										OnEnemySelected (enemy [0].transform, playerCoord, attackingPlayer);
										
										if (attackingPlayer == Pinky || attackingPlayer == Greengo)
											playerMoveEnabled = false;
										
										return;
										
									}
								} else {
									if (OnEnemyUnreachable != null) {
										OnEnemyUnreachable (activePlayer);
										
										return;
									}
									
								}
							}
							
							//PATH
							
							int targetTileCoordX, targetTileCoordY;
							
							if (tileMap.GetTileAtPosition (touchWorldPos, out targetTileCoordX, out targetTileCoordY)) {
								
								Vector3 targetTileCoord = new Vector3 (targetTileCoordX, targetTileCoordY);
								Vector3 targetTilePos = tileMap.GetTilePosition (targetTileCoordX, targetTileCoordY);
								
								if (tileMap.GetTileInfoForTileId (tileMap.GetTile (targetTileCoordX, targetTileCoordY, 0)).stringVal == "path") {
									
									if (OnMoveTargetSelected != null)
										OnMoveTargetSelected (targetTileCoord, targetTilePos, activePlayer);
									return;
								}
							}
							//EXIT
							if (exitClicked > 0) {
								
								if (OnExitSelected != null)
									OnExitSelected (activePlayer);
							}
						}                          
					} else {
						
						//CAMERA SCROLL
						cameraDrag.SendMessage ("Scroll");
					}
					
					break;
				}
				
			} else if (touchCount == 2 && zoomOutEnabled) {
				
				Touch touchZero = Input.GetTouch (0);
				Touch touchOne = Input.GetTouch (1);
				
				if (touchZero.phase == TouchPhase.Moved && touchOne.phase == TouchPhase.Moved && !pinchRegistered) {
					
					Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
					Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
					
					float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
					float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
					
					float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
					
					if (deltaMagnitudeDiff > 0) {
						cameraScriptedAnim.SendMessage ("ZoomOut");
						pinchRegistered = true;
						ignoreTouchEnded = true;
						
						
					} else if (deltaMagnitudeDiff < 0) {
						
						cameraScriptedAnim.SendMessage ("ZoomIn");
						pinchRegistered = true;
						ignoreTouchEnded = true;
					}
				} else if (touchZero.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Ended) 
					pinchRegistered = false;
			}
			
		}
	}
	
	private void MouseControls ()
	{	
		if (Input.GetAxis ("Horizontal") != 0) {
			Vector2 dragAmountX = new Vector2 (Input.GetAxisRaw ("Horizontal") * 10, 0);
			cameraDrag.SendMessage ("Drag", dragAmountX);
		} else if (Input.GetAxis ("Vertical") != 0) {
			Vector2 dragAmountY = new Vector2 (0, Input.GetAxisRaw ("Vertical") * 10);
			cameraDrag.SendMessage ("Drag", dragAmountY);
		}
		
		if (Input.GetButtonDown ("Fire1"))
			cameraScriptedAnim.SendMessage ("ZoomOut");
			
		if (Input.GetButtonDown ("Fire2"))
			cameraScriptedAnim.SendMessage ("ZoomIn");
		
		         
		if (Input.GetMouseButtonDown (0)) {
				
			Vector2 mousePos = Input.mousePosition;
						
			// GUI
			if (IsPointerOverUIObject (mousePos)) 
				return;
						
			Vector3 touchWorldPos = mainCamera.GetComponent<Camera> ().ScreenToWorldPoint (mousePos);
						
			Collider2D [] player = new Collider2D[1];
			Collider2D [] enemy = new Collider2D[1];
			Collider2D [] retryButton = new Collider2D[1]; 
			Collider2D [] exit = new Collider2D[1];    
						
			int layerMaskPlayer = 1 << 8;
			int layerMaskEnemy = 1 << 11;
			int layerMaskRetryButton = 1 << 10;   
			int layerMaskExit = 1 << 13;            
						
						
			int playerClicked = Physics2D.OverlapPointNonAlloc (touchWorldPos, player, layerMaskPlayer);
			int enemyClicked = Physics2D.OverlapPointNonAlloc (touchWorldPos, enemy, layerMaskEnemy);
			int retryButtonClicked = Physics2D.OverlapPointNonAlloc (touchWorldPos, retryButton, layerMaskRetryButton);    
			int exitClicked = Physics2D.OverlapPointNonAlloc (touchWorldPos, exit, layerMaskExit);    
						
						
			//RETRY BUTTON
			if (retryButtonClicked > 0) {
							
				if (OnRetrySelected != null)
					OnRetrySelected (deadPlayer);   
				playerIsDead = false;
			}
						
			if (playerMoveEnabled) {
							
				//PLAYER
							
				if (playerClicked > 0) {
								
					Transform selectedPlayer = player [0].transform;
					activePlayer = selectedPlayer;            
								
					if (OnPlayerSelected != null) 
						OnPlayerSelected (selectedPlayer);
								
					clickableEnemies.Clear ();
					return;
								
				}
							
				//ENEMY
							
				if (enemyClicked > 0) {
								
					if (clickableEnemies.Count > 0 && clickableEnemies.Contains (enemy [0].transform)) {
									
						int x, y;
						tileMap.GetTileAtPosition (attackingPlayer.position, out x, out y);
									
						Vector3 playerCoord = new Vector3 (x, y);
									
						if (OnEnemySelected != null) {
							OnEnemySelected (enemy [0].transform, playerCoord, attackingPlayer);
										
							if (attackingPlayer == Pinky || attackingPlayer == Greengo)
								playerMoveEnabled = false;
										
							return;
										
						}
					} else {
						if (OnEnemyUnreachable != null) {
							OnEnemyUnreachable (activePlayer);
										
							return;
						}
									
					}
				}
							
				//PATH
							
				int targetTileCoordX, targetTileCoordY;
							
				if (tileMap.GetTileAtPosition (touchWorldPos, out targetTileCoordX, out targetTileCoordY)) {
								
					Vector3 targetTileCoord = new Vector3 (targetTileCoordX, targetTileCoordY);
					Vector3 targetTilePos = tileMap.GetTilePosition (targetTileCoordX, targetTileCoordY);
								
					if (tileMap.GetTileInfoForTileId (tileMap.GetTile (targetTileCoordX, targetTileCoordY, 0)).stringVal == "path") {
									
						if (OnMoveTargetSelected != null)
							OnMoveTargetSelected (targetTileCoord, targetTilePos, activePlayer);
						return;
					}
				}
				//EXIT
				if (exitClicked > 0) {
								
					if (OnExitSelected != null)
						OnExitSelected (activePlayer);
				}
			}                        
		}
	}

	void Update ()
	{		

		if (inputEnabled) {
				
			if (touchControls)
				TouchControls ();
			else if (mouseControls)
				MouseControls ();
		}
	}
		
	private void EnablePlayerMovement (Transform player, bool oracIsDefending)
	{
		if (player != activePlayer || oracIsDefending)
			return;						
		playerMoveEnabled = true;
		if (!zoomOutEnabled)
			zoomOutEnabled = true;
	}
	
	private void EnablePlayerMovementOnOracFinishedDefend ()
	{					
		if (playerIsDead)
			return;
		playerMoveEnabled = true;
	}
		
	private void DisablePlayerMovementOnPlayerIsDead (Transform player)
	{
		playerMoveEnabled = false;
		deadPlayer = player;
		zoomOutEnabled = false;
		playerIsDead = true;
	}

	private void DisablePlayerMovementOnPlayerIsMoving (Transform player)
	{
		playerMoveEnabled = false;
	}
		
	private void DisablePlayerMovement ()
	{
		playerMoveEnabled = false;
	}
	
	private void RegisterEnemyForInput (Transform enemy, Transform player, Transform sensor)
	{
		if (player.name == "Orac")
			return;
		if (! clickableEnemies.Contains (enemy))
			clickableEnemies.Add (enemy);
						
		attackingPlayer = player;
	}

	private void UnregisterEnemyFromInput (Transform enemy, Transform player, Transform sensor)
	{
		if (player.name == "Orac")
			return;
		if (clickableEnemies.Contains (enemy))
			clickableEnemies.Remove (enemy);
	}
		
	private void DisableInput ()
	{
		if (inputEnabled)
			inputEnabled = false;
						
	}

	private bool IsPointerOverUIObject (Vector2 touchPosition)
	{

		PointerEventData eventDataCurrentPosition = new PointerEventData (EventSystem.current);
		eventDataCurrentPosition.position = new Vector2 (touchPosition.x, touchPosition.y);
		
		List<RaycastResult> results = new List<RaycastResult> ();
		EventSystem.current.RaycastAll (eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}
