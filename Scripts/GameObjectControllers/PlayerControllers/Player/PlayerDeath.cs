using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerPathController))]
[RequireComponent(typeof(PlayerMoveAnimator))]
[RequireComponent(typeof(Animator))]

public class PlayerDeath : MonoBehaviour
{
	public delegate void PlayerIsDead (Transform player);
	public static event PlayerIsDead OnPlayerIsDead;
		
	public delegate void BlubbaFailedAttack ();
	public static event BlubbaFailedAttack OnBlubbaFailedAttack;
		
	public delegate void PlayerSuccessfulAttack ();
	public static event PlayerSuccessfulAttack OnPlayerSuccessfulAttack;
		
	private Animator animator;
	private bool playerIsGoingToDie = false;
	private Vector3 smokeFXPos;
	private Transform enemySensors;
	private List<Transform> enemiesAttacking;
	private Behaviour halo;
	private bool pinky = false;
	private bool blubba = false;
	private bool rebornFromAshes = false;
	private bool counterAttackInfoReceived = false;
	private tk2dTileMap tileMap;

	private Vector3 playerPositionAtCounterAttack;
	
	public GameObject hitFX;
	public float hitFXdelay = 0.2f;
	public GameObject smokeFX;

	private int prevSortingOrder = 0;

	void OnEnable ()
	{
		EnemyCounterAttack.OnPlayerGetReadyToDie += PrepareForDeath;
		InputManager.OnRetrySelected += PlayRebirthAnim;
	}
	void OnDisable ()
	{		
		EnemyCounterAttack.OnPlayerGetReadyToDie -= PrepareForDeath;
		InputManager.OnRetrySelected -= PlayRebirthAnim;
	}
	void Start ()
	{
		enemySensors = transform.GetChild (0);
		
		animator = GetComponent<Animator> ();
				
		enemiesAttacking = new List<Transform> ();
		playerPositionAtCounterAttack = Vector3.zero;

		halo = (Behaviour)GetComponent ("Halo");

		pinky = this.name == "Pinky";
		blubba = this.name == "Blubba";

		if (pinky)
			smokeFXPos = new Vector3 (0, -60, 0);
		else 
			smokeFXPos = new Vector3 (0, -20, 0);

		tileMap = GameObject.Find ("TileMap").GetComponent<tk2dTileMap> ();
	}
		
	private void PrepareForDeath (Transform player, Transform enemy)
	{		
		if (player != transform)
			return;
				
		if (! counterAttackInfoReceived) {

			if (blubba) {
								
				if (OnBlubbaFailedAttack != null)
					OnBlubbaFailedAttack ();
			}
						
			playerIsGoingToDie = true;
			enemySensors.gameObject.SetActive (false);

			playerPositionAtCounterAttack = player.position;
			counterAttackInfoReceived = true;
		}

		if (! enemiesAttacking.Contains (enemy))
			enemiesAttacking.Add (enemy);
	}
	
	private void ReEnableControlsOrDie () //called by Attack Animation
	{
		prevSortingOrder = GetComponent<Renderer> ().sortingOrder;

		if (playerIsGoingToDie) {
				
			halo.enabled = false;

			Transform killerEnemy;

			enemiesAttacking.Sort (CompareEnemies);

			killerEnemy = enemiesAttacking [0];

			if (killerEnemy.name == "Enemy1") {

				if (killerEnemy.position.x > playerPositionAtCounterAttack.x) 
					gameObject.SendMessageUpwards ("CheckFlipRight");
				else if (killerEnemy.position.x < playerPositionAtCounterAttack.x)
					gameObject.SendMessageUpwards ("CheckFlipLeft");

				if (! blubba)
					GetComponent<Renderer> ().sortingOrder += 1;

				animator.Play ("Die");
				Invoke ("PlayHitFX", hitFXdelay);

			} else if (killerEnemy.name == "Enemy2") {

				animator.Play ("IdleAndShock");
				rebornFromAshes = true;

			} else if (killerEnemy.name == "Enemy3") {

				int enemyX, enemyY;
				int playerX, playerY;

				tileMap.GetTileAtPosition (killerEnemy.position, out enemyX, out enemyY);
				tileMap.GetTileAtPosition (playerPositionAtCounterAttack, out playerX, out playerY);

				if (enemyX != playerX) {

					//DIAGONAL
					if (enemyY != playerY) {

						if (! blubba)
							GetComponent<Renderer> ().sortingOrder += 1;
						
						animator.Play ("Die");
						Invoke ("PlayHitFX", hitFXdelay);

						//HORIZONTAL
					} else {
						animator.Play ("Burn");
						rebornFromAshes = true;
					}

					//VERTICAL
				} else {

					animator.Play ("Burn");
					rebornFromAshes = true;
				}
			}

		} else {

			if (! blubba) {
				
				animator.Play ("Idle");	
			
				if (OnPlayerSuccessfulAttack != null)
					OnPlayerSuccessfulAttack ();
			}
		}
	}
		
	private void PlayHitFX ()
	{
		GameObject hitFXClone = Instantiate (hitFX, transform.position, Quaternion.identity) as GameObject;
		Destroy (hitFXClone.gameObject, 3);
	}
		
	private void ReportDeath ()
	{
		if (OnPlayerIsDead != null)
			OnPlayerIsDead (transform);
	}
		
	private void PlayRebirthAnim (Transform deadPlayer)
	{
		if (deadPlayer == transform) {
				
			if (rebornFromAshes) {
				animator.Play ("RebornFromAshes");
			} else
				animator.Play ("Reborn");
		} 
	}
		
	private void Rebirth ()
	{
		enemySensors.gameObject.SetActive (true);
		
		gameObject.SendMessageUpwards ("CheckFlipRight");
		
		animator.Play ("Idle");
				
		playerIsGoingToDie = false;
		rebornFromAshes = false;

		enemiesAttacking.Clear ();
		halo.enabled = true;
		counterAttackInfoReceived = false;

		GetComponent<Renderer> ().sortingOrder = prevSortingOrder;
	}
		
	private void SmokeFx ()
	{
		GameObject smokeFXClone = Instantiate (smokeFX, transform.position + smokeFXPos, Quaternion.identity) as GameObject;
		Destroy (smokeFXClone.gameObject, 7);
	}

	private int CompareEnemies (Transform enemyX, Transform enemyY)
	{
		if (enemyX.name == "Enemy1") {			
			if (enemyY.name == "Enemy1") {
				return 0;
			} else { 
				return -1;
			}
		} else if (enemyX.name == "Enemy2") {
			
			if (enemyY.name == "Enemy1") {
				return 1;
			} else if (enemyY.name == "Enemy2") {
				return 0;
			} else { 
				return -1;
			}
		} else {
			if (enemyY.name == "Enemy1") {
				return 1;
			} else if (enemyY.name == "Enemy2") {
				return 1;
			} else {
				if (Mathf.Abs (enemyX.position.x - this.transform.position.x) > Mathf.Abs (enemyY.position.x - this.transform.position.x)) {
					return 1;
				} else {
					return 0;
				}
			}
		}
	}
}
