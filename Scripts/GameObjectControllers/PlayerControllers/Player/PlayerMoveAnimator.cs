using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]

public class PlayerMoveAnimator : PlayerPathController
{		
	public delegate void PlayerIsMoving (Transform player);

	public static event PlayerIsMoving OnPlayerIsMoving;
	
	public delegate void PlayerFinishedMovement (Transform player,bool oracIsDefending);

	public static event PlayerFinishedMovement OnPlayerFinishedMovement;
		
	public float WalkTime = 0.45f;
	public float FlySpeed = 300;
		
	public Vector3 SpriteOffset = new Vector3 (0, 15f, 0);
	public bool thisIsSelected = false;
			
	public enum PathStepEaseType
	{
		Linear,
		Spring,
		EaseInOutSine
	}

	public PathStepEaseType pathStepEasing;

	[HideInInspector]
	public Vector3
		currentPlayerCoord;

	private int startSortingOrder = 0;
	private int destSortingOrder = 0;
	private Vector3 playerMoveStartPos;
	private iTween.EaseType easeType;
	private Animator animator;
	private Transform enemySensors;
	private List<Vector3> animPath;
	private Vector3[] animPathArray;
	private List<Vector3>animPathCoords;
	private Vector3 moveDestination;
	private Vector3 moveDestinationCoord;
	private Vector3 arrivalFromCoord;
	private Vector3 pathStepCoord;
	private bool isFacingRight = true;
	private bool readyToContinueMove = true;		
	private Hashtable scaleTweenParams;
	private Hashtable rotateTweenParams;
	private Vector3 exitPos;
	private Behaviour halo;
	private bool pinky = false;
	private bool greengo = false;
	private bool blubba = false;
	private bool orac = false;
	private bool selectFxEnabled = false;
	private bool oracIsDefending = false;

	private AudioSource[] sounds;
	private AudioSource selectFX;
	private AudioSource selectVoice;
	private AudioSource moveVoice;
	private AudioSource nonoVoice;
	private bool shockedByFence = false;
	private Transform myFence;
	private List<Transform> playersInMyFence;
		
	private MoveTargetXController moveTargetXController;
			
	void OnEnable ()
	{
		PlayerPathController.OnPlayerPathCalculated += InitMove;
		PlayerPathController.OnActivePlayer += SetActive;
				
		LevelManager.OnPlayOutroAnimations += TeleportToNextLevel;
		LevelManager.OnStartLevel += TurnOffHalo;
		InputManager.OnEnemyUnreachable += Nono;
		LevelManager.OnNoEnemiesLeft += Juhu;
		EnemyCounterAttack.OnOracDefendStart += RegisterOracDefend;
		OracDefend.OnOracFinishedDefend += UnregisterOracDefend;
		FencePlayerSensor.OnPlayerEnteredFence += RegisterPlayerInFence;
		FencePlayerSensor.OnPlayerExitedFence += UnregisterPlayerInFence;
		
	}
		
	void OnDisable ()
	{
		PlayerPathController.OnPlayerPathCalculated -= InitMove;
		PlayerPathController.OnActivePlayer -= SetActive;
				
		LevelManager.OnPlayOutroAnimations -= TeleportToNextLevel;
		LevelManager.OnStartLevel -= TurnOffHalo;
		
		InputManager.OnEnemyUnreachable -= Nono;
		LevelManager.OnNoEnemiesLeft -= Juhu;
		EnemyCounterAttack.OnOracDefendStart -= RegisterOracDefend;
		OracDefend.OnOracFinishedDefend -= UnregisterOracDefend;
		FencePlayerSensor.OnPlayerEnteredFence -= RegisterPlayerInFence;
		FencePlayerSensor.OnPlayerExitedFence -= UnregisterPlayerInFence;
	}
		
	protected override void Start ()
	{
		base.Start ();
		animator = gameObject.GetComponent<Animator> ();
		enemySensors = transform.GetChild (0);
		enemySensors.gameObject.SetActive (false);
								
		animPath = new List<Vector3> ();

		moveDestination = Vector3.zero;
		moveDestinationCoord = Vector3.zero;
		arrivalFromCoord = Vector3.zero;
		pathStepCoord = Vector3.zero;
		playerMoveStartPos = Vector3.zero;
		
		animPathCoords = new List<Vector3> ();
		animPathArray = null;
		myFence = null;
		playersInMyFence = new List<Transform> ();

		if (pathStepEasing == PathStepEaseType.Linear)
			easeType = iTween.EaseType.linear;
		else if (pathStepEasing == PathStepEaseType.Spring)
			easeType = iTween.EaseType.spring;
		else if (pathStepEasing == PathStepEaseType.EaseInOutSine)
			easeType = iTween.EaseType.easeInOutSine;
										
		rotateTweenParams = new Hashtable ();
		rotateTweenParams.Add ("amount", new Vector3 (0, 0, 8));
		rotateTweenParams.Add ("speed", 700);
		rotateTweenParams.Add ("easetype", iTween.EaseType.linear);
		rotateTweenParams.Add ("looptype", iTween.LoopType.loop);
				
		scaleTweenParams = new Hashtable ();
		scaleTweenParams.Add ("scale", new Vector3 (0, 0, 0));
		scaleTweenParams.Add ("time", 4.5f);
		scaleTweenParams.Add ("easetype", iTween.EaseType.easeInOutQuint);	
		scaleTweenParams.Add ("oncomplete", "ReportPlayerReachedExit");
		
		if (GameObject.Find ("Exit") != null)
			exitPos = GameObject.Find ("Exit").transform.position;	
						
		halo = (Behaviour)GetComponent ("Halo");	
				
		pinky = this.gameObject.name == "Pinky";
		greengo = this.gameObject.name == "Greengo";
		blubba = this.gameObject.name == "Blubba";
		orac = this.gameObject.name == "Orac";		

		moveTargetXController = GameObject.Find ("MoveTargetX").GetComponent<MoveTargetXController> ();

		sounds = gameObject.GetComponents<AudioSource> ();
		selectFX = sounds [0];

		if (pinky) {
			selectVoice = sounds [3];
			moveVoice = sounds [5];
			nonoVoice = sounds [11];

			int currentPlayerCoordX, currentPlayerCoordY;
			map.GetTileAtPosition (GameObject.Find ("PinkyStartPoint").transform.position, out currentPlayerCoordX, out currentPlayerCoordY);
			currentPlayerCoord = new Vector3 (currentPlayerCoordX, currentPlayerCoordY);
			
			GetComponent<Renderer> ().sortingOrder = (int)(((map.height - 2) - currentPlayerCoord.y) * 2);

		} else if (blubba) {
			selectVoice = sounds [4];
			moveVoice = sounds [5];
			nonoVoice = sounds [9];

			int currentPlayerCoordX, currentPlayerCoordY;
			map.GetTileAtPosition (GameObject.Find ("BlubbaStartPoint").transform.position, out currentPlayerCoordX, out currentPlayerCoordY);
			currentPlayerCoord = new Vector3 (currentPlayerCoordX, currentPlayerCoordY);
			
			GetComponent<Renderer> ().sortingOrder = (int)(((map.height - 2) - currentPlayerCoord.y) * 2);
			
		} else if (greengo) {
			selectVoice = sounds [1];
			moveVoice = sounds [2];
			nonoVoice = sounds [10];

			int currentPlayerCoordX, currentPlayerCoordY;
			map.GetTileAtPosition (GameObject.Find ("GreengoStartPoint").transform.position, out currentPlayerCoordX, out currentPlayerCoordY);
			currentPlayerCoord = new Vector3 (currentPlayerCoordX, currentPlayerCoordY);
			
			GetComponent<Renderer> ().sortingOrder = (int)(((map.height - 2) - currentPlayerCoord.y) * 2);

			
		} else if (orac) {
			selectVoice = sounds [1];
			moveVoice = sounds [2];
			nonoVoice = sounds [8];

			int currentPlayerCoordX, currentPlayerCoordY;
			map.GetTileAtPosition (GameObject.Find ("OracStartPoint").transform.position, out currentPlayerCoordX, out currentPlayerCoordY);
			currentPlayerCoord = new Vector3 (currentPlayerCoordX, currentPlayerCoordY);
			
			GetComponent<Renderer> ().sortingOrder = (int)(((map.height - 2) - currentPlayerCoord.y) * 2);
		}		
	}
		
	private void TurnOffHalo (Transform startingPlayer)
	{
		if (startingPlayer != transform)
			halo.enabled = false;
	}
		
	private void SetActive (Transform activePlayer)
	{
		thisIsSelected = this.gameObject.transform == activePlayer;
		enemySensors.gameObject.SetActive (thisIsSelected);
				
		if (thisIsSelected) {
			halo.enabled = true;

			if (! selectFX.isPlaying && selectFxEnabled)
				selectFX.Play ();

			if (! selectVoice.isPlaying)
				selectVoice.Play ();
		} else {
			halo.enabled = false;
		}
		if (!selectFxEnabled)
			selectFxEnabled = true;
	}
			
	private void Flip ()
	{
		if (blubba)
			return;

		isFacingRight = !isFacingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;

		enemySensors.parent = null;
		transform.localScale = theScale;
		enemySensors.parent = transform;

	}
		
	private void CheckFlipRight ()
	{		
		if (blubba)
			return;

		if (!isFacingRight)
			Flip ();
	}

	private void CheckFlipLeft ()
	{
		if (blubba)
			return;

		if (isFacingRight)
			Flip ();
	}
		
	private void InitMove (List<Vector3> animationPath, List<Vector3> pathCoords)
	{
		if (thisIsSelected) {
			
			if (playersInMyFence.Count > 0) {
			
				animator.Play ("Nono");
				nonoVoice.Play ();
				moveTargetXController.SendMessage ("UnreachableMoveTarget", transform);
				return;
			}
			
			if (animationPath == null || animationPath.Count < 1) {
						
				animPath.Clear ();
				animPathCoords.Clear ();
								
				if (this.name == "Blubba") {
						
					if (GetComponent<BlubbaController> ().isAlreadyAttacking) {
						moveTargetXController.SendMessage ("UnreachableMoveTarget", transform);

						return;
					}
				}
				animator.Play ("Nono");
				nonoVoice.Play ();
				moveTargetXController.SendMessage ("UnreachableMoveTarget", transform);
				return;
			}
						
			animPath = animationPath;
			animPathCoords = pathCoords;
						
			moveDestination = animPath [animPath.Count - 1];
			
			for (int i = 0; i < animPath.Count; i++) {
				animPath [i] += SpriteOffset;
			}
						
			animPathArray = animPath.ToArray ();
												
			int moveDestCoordX, moveDestCoordY;
			map.GetTileAtPosition (moveDestination, out moveDestCoordX, out moveDestCoordY);
						
			moveDestinationCoord = new Vector3 (moveDestCoordX, moveDestCoordY);
			
			int currentPlayerCoordX, currentPlayerCoordY;
			map.GetTileAtPosition (transform.position, out currentPlayerCoordX, out currentPlayerCoordY);
			
			currentPlayerCoord = new Vector3 (currentPlayerCoordX, currentPlayerCoordY);
			playerMoveStartPos = transform.position;
						
			if (animPath.Count > 1) {
				int arrivalFromCoordX, arrivalFromCoordY;
				map.GetTileAtPosition (animPath [animPath.Count - 2], out arrivalFromCoordX, out arrivalFromCoordY);
				arrivalFromCoord = new Vector3 (arrivalFromCoordX, arrivalFromCoordY);
			} else
				arrivalFromCoord = currentPlayerCoord;
						
			startSortingOrder = GetComponent<Renderer> ().sortingOrder;		
			ReportPlayerIsMoving ();
						
			if (greengo)
				InitGreengoWalkAnimation ();
			else
				InitWalkAnimation ();
		}
	}
		
	private void InitWalkAnimation ()
	{
		pathStepCoord = animPathCoords [0];

		Vector2 destCoord = animPathCoords [animPathCoords.Count - 1];
		int YDiff = (int)currentPlayerCoord.y - (int)destCoord.y;
		destSortingOrder = GetComponent<Renderer> ().sortingOrder + YDiff * 2;

		GetComponent<Renderer> ().sortingOrder = 30;

		moveVoice.Play ();

		if (pathStepCoord.x > currentPlayerCoord.x) {
			animator.Play ("TurnRight");
				
		} else if (pathStepCoord.x < currentPlayerCoord.x) {
				
			if (! orac && isFacingRight)
				Flip ();
			animator.Play ("TurnLeft");
		} else if (pathStepCoord.y < currentPlayerCoord.y)
			animator.Play ("TurnDown");
		else
			animator.Play ("TurnUp");
						
		readyToContinueMove = true;
				
	}
		
	private void InitGreengoWalkAnimation ()
	{
		pathStepCoord = animPathCoords [animPathCoords.Count - 1];
		int YDiff = (int)currentPlayerCoord.y - (int)pathStepCoord.y;
		destSortingOrder = GetComponent<Renderer> ().sortingOrder + YDiff * 2;
		
		GetComponent<Renderer> ().sortingOrder = 30;
						
		if (pathStepCoord.x > currentPlayerCoord.x) {
			animator.Play ("TurnRight");
			
		} else if (pathStepCoord.x < currentPlayerCoord.x) {
			
			if (isFacingRight)
				Flip ();
			animator.Play ("TurnLeft");
		} else if (pathStepCoord.y < currentPlayerCoord.y)
			animator.Play ("TurnDown");
		else
			animator.Play ("TurnUp");		
	}

	private void Move ()
	{
		if (animPath.Count > 0 && readyToContinueMove) {
				
			pathStepCoord = animPathCoords [0];

			iTween.MoveTo (gameObject, iTween.Hash (
						"time", WalkTime,
						"position", animPath [0],
						"oncomplete", "UpdateWalkAnimOnPath",
						"easetype", easeType));	
						
			currentPlayerCoord = animPathCoords [0];			
			animPath.RemoveAt (0);
			animPathCoords.RemoveAt (0);
								
			readyToContinueMove = false;					
		}
	}
		
	private void GreengoMove ()
	{
		if (animPath.Count > 0) {
						
			if (animPath.Count > 1) {
				iTween.MoveTo (gameObject, iTween.Hash (
					    "speed", FlySpeed,
				        "position", animPath [animPath.Count - 1],
				        "oncomplete", "GreengoIdle",
				        "onupdate", "GreengoUpdateWalkAnimOnPath",
				        "easetype", easeType,
				        "path", animPathArray));
				        
			} else {
						
				iTween.MoveTo (gameObject, iTween.Hash (
					"speed", FlySpeed / 2,
					"position", animPath [animPath.Count - 1],
					"oncomplete", "GreengoIdle",
					"onupdate", "GreengoUpdateWalkAnimOnPath",
					"easetype", iTween.EaseType.linear));				
			}
		}
	}
		
	private void UpdateWalkAnimOnPath ()
	{		
		if (shockedByFence)
			return;
			
		if (animPath.Count > 0) {
				
			pathStepCoord = animPathCoords [0];

			if (pathStepCoord.x > currentPlayerCoord.x) {
				if (!isFacingRight)
					Flip ();
				animator.Play ("WalkRight");
				
			
			} else if (pathStepCoord.x < currentPlayerCoord.x) {
			
				if (isFacingRight)
					Flip ();
				animator.Play ("WalkLeft");
			} else if (pathStepCoord.y < currentPlayerCoord.y) {
				if (!isFacingRight)
					Flip ();
				animator.Play ("WalkDown");
			} else {
						
				animator.Play ("WalkUp");
			}
								
			readyToContinueMove = true;

			if (orac)
				Move ();
		} else {
			Idle ();
		}
				
	}
		
	private void GreengoUpdateWalkAnimOnPath ()
	{
		int currentPlayerCoordX, currentPlayerCoordY;
		map.GetTileAtPosition (transform.position, out currentPlayerCoordX, out currentPlayerCoordY);
		currentPlayerCoord = new Vector3 (currentPlayerCoordX, currentPlayerCoordY);	
		
		if (shockedByFence)
			return;
		if (animPath.Count > 0) {
			
			Vector3 pathStepCoord = animPathCoords [0];
			
			if (pathStepCoord.x > currentPlayerCoord.x) {
				if (!isFacingRight)
					Flip ();
				
			} else if (pathStepCoord.x < currentPlayerCoord.x) {
				
				if (isFacingRight)
					Flip ();			
			}
			
			if (pathStepCoord == currentPlayerCoord) {
				currentPlayerCoord = animPathCoords [0];			
				animPath.RemoveAt (0);
				animPathCoords.RemoveAt (0);
			}		
		} 
	}

	private void ReportPlayerIsMoving ()
	{		
		if (OnPlayerIsMoving != null) {
			OnPlayerIsMoving (this.transform);
		}

	}
		
	private void Idle ()
	{															
		if (moveDestinationCoord.x > arrivalFromCoord.x) {

			if (orac) {
				animator.Play ("TurnFrontFromRight");
			} else
				animator.Play ("TurnFrontFromSide");
						
		} else if (moveDestinationCoord.x < arrivalFromCoord.x) {

			if (orac) {
				animator.Play ("TurnFrontFromLeft");
			} else
				animator.Play ("TurnFrontFromSide");

		} else if (moveDestinationCoord.y < arrivalFromCoord.y) {
						
			animator.Play ("TurnFrontFromFront");
		} else {
			CheckFlipRight ();
			animator.Play ("TurnFrontFromBack");
		}	
		GetComponent<Renderer> ().sortingOrder = destSortingOrder;
	}
		
	private void GreengoIdle ()
	{															
		animator.Play ("TurnFront");
		GetComponent<Renderer> ().sortingOrder = destSortingOrder;
	}
		
	private void TeleportToNextLevel ()
	{
		animator.enabled = false;
		enemySensors.gameObject.SetActive (false);
				
		iTween.RotateBy (gameObject, rotateTweenParams);
		iTween.ScaleTo (gameObject, scaleTweenParams);
				
		iTween.MoveTo (gameObject, iTween.Hash (
				"position", exitPos,
				"time", 2,
				"easetype", iTween.EaseType.easeOutQuint));
	}
		
	private void ReportPlayerFinishedMovement () //Called on the first frame of Idle Anim
	{
		if (shockedByFence) {
			shockedByFence = false;
		}
		if (thisIsSelected && shockedByFence)
			shockedByFence = false;
			
		CheckFlipRight ();

		animPath.Clear ();
		animPathCoords.Clear ();
		
		if (OnPlayerFinishedMovement != null) {
			OnPlayerFinishedMovement (transform, oracIsDefending);
		}

	}
		
	private void Nono (Transform player)
	{		
		if (blubba && gameObject.GetComponent<BlubbaController> ().isAlreadyAttacking)
			return;

		if (player == transform) {
			animator.Play ("Nono");

			if (! nonoVoice.isPlaying)
				nonoVoice.Play ();
		}
	}

	private void Juhu ()
	{
		StartCoroutine (PlayJuhuAnim ());
	}

	private IEnumerator PlayJuhuAnim ()
	{
		yield return new WaitForFixedUpdate ();
		CheckFlipRight ();
		
		animator.Play ("Juhu");
	}
	
	private void ShockedByFence ()
	{
		if (orac) {
			GetComponent<ShockHalo> ().SendMessage ("InitFlash");
			return;
		}
		animPath.Clear ();
		animPathCoords.Clear ();
		shockedByFence = true;
		animator.Play ("ShockedByFence");
		moveTargetXController.SendMessage ("UnreachableMoveTarget", transform);	
		GetComponent<Renderer> ().sortingOrder = startSortingOrder;
		iTween.MoveTo (gameObject, iTween.Hash (
			"delay", 0.1f,
			"time", 0.5f,
			"position", playerMoveStartPos,
			"easetype", iTween.EaseType.easeInBack));
	}
	
	private void RegisterOracDefend ()
	{
		oracIsDefending = true;
	}
	
	private void UnregisterOracDefend ()
	{
		oracIsDefending = false;
	}
	
	private void SteppingOnSwitch (Transform fence)
	{
		myFence = fence;
	}
	
	private void SteppingOffSwitch (Transform fence)
	{
		myFence = null;
	}
	
	private void RegisterPlayerInFence (Transform player, Transform fence)
	{	
		if (player == transform)
			return;

		if (fence == myFence)
			playersInMyFence.Add (player);
	}
	
	private void UnregisterPlayerInFence (Transform player, Transform fence)
	{	
		if (player == transform)
			return;

		if (fence == myFence)
			playersInMyFence.Remove (player);
	}
}