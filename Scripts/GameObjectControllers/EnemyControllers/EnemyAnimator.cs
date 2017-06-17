using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class EnemyAnimator : MonoBehaviour
{
	public delegate void EnemyIsDead (Transform enemy,Vector3 playerCoordinates,Transform player);

	public static event EnemyIsDead OnEnemyIsDead;
		
	public delegate void EnemyIsInTrance (Transform enemy);
	
	public static event EnemyIsInTrance OnEnemyIsInTrance;

	private Animator animator;
	private tk2dTileMap tileMap;

	public GameObject burstFX;
	public GameObject enemyFlare;
	private ParticleSystem enemyWaves;

	private Vector3 burstFXPosition;
	private Vector3 thisTileCoord;
	private bool spriteIsFacingLeft = true;
	private bool thisIsAttacked = false;
	private Vector3 currentPlayerCoord;
	private Transform currentKillingPlayer;
	private bool inverseScale = false;
	private EnemyDangerTileController enemyDangerTileController;
	private Vector3 originalScale;

	private bool introAnimComplete = false;

	private bool enemy1 = false;
	private bool enemy2 = false;
	private bool enemy3 = false;

	private EnemyCounterAttack enemyCounterAttack;

	private int originalSortingOrder = 0;

	public AudioSource enemyPop;

	public AudioSource enemyStab;

	public AudioSource enemyLaugh;

	private AudioSource spawnFx;

	
	void OnEnable ()
	{
		InputManager.OnEnemySelected += StartDeath;		
		InputManager.OnRetrySelected += PlayRebirthAnim;
		EnemyCounterAttack.OnEnemyStartCounterAttack += AssignCounterAttackAnimation;
		
		PlayerDeath.OnPlayerSuccessfulAttack += Die;
				
		PathTileAnim.OnIntroTileAnimComplete += InitIntroAnim;
				
	}

	void OnDisable ()
	{		
		InputManager.OnEnemySelected -= StartDeath;
		InputManager.OnRetrySelected -= PlayRebirthAnim;
		
		EnemyCounterAttack.OnEnemyStartCounterAttack -= AssignCounterAttackAnimation;	
		PlayerDeath.OnPlayerSuccessfulAttack -= Die;
				
		PathTileAnim.OnIntroTileAnimComplete -= InitIntroAnim;

	}
		
	void Start ()
	{

		originalScale = transform.localScale;
		animator = gameObject.GetComponent<Animator> ();
		tileMap = GameObject.Find ("TileMap").GetComponent<tk2dTileMap> ();
		enemyCounterAttack = gameObject.GetComponent<EnemyCounterAttack> ();

		enemyWaves = transform.FindChild ("EnemyWaves").GetComponent<ParticleSystem> ();

		int x, y;
		tileMap.GetTileAtPosition (transform.position, out x, out y);
		thisTileCoord = new Vector3 (x, y);

		GetComponent<Renderer> ().sortingOrder = (int)(((tileMap.height - 2) - thisTileCoord.y) * 2);

		enemy1 = this.name == "Enemy1";
		enemy2 = this.name == "Enemy2";
		enemy3 = this.name == "Enemy3";

		enemyPop = GetComponents<AudioSource> () [0];

		if (enemy1) {
				
			burstFXPosition = new Vector3 (this.transform.position.x, this.transform.position.y, 0);
			enemyStab = GetComponents<AudioSource> () [1];
			enemyLaugh = GetComponents<AudioSource> () [2];
			spawnFx = GetComponents<AudioSource> () [3];
								
		} else if (enemy2) {
						
			burstFXPosition = new Vector3 (this.transform.position.x, this.transform.position.y - 30, 0);
			enemyLaugh = GetComponents<AudioSource> () [1];
			spawnFx = GetComponents<AudioSource> () [3];


		} else if (enemy3) {

			burstFXPosition = new Vector3 (this.transform.position.x, this.transform.position.y - 20, 0);
			enemyLaugh = GetComponents<AudioSource> () [2];
			enemyStab = GetComponents<AudioSource> () [4];
			spawnFx = GetComponents<AudioSource> () [5];

			
		}
				
		currentPlayerCoord = Vector3.zero;
		currentKillingPlayer = null;
		enemyDangerTileController = transform.Find ("DangerTiles").GetComponent<EnemyDangerTileController> ();
				
		if (transform.localScale.x < 0) {
			inverseScale = true;
			spriteIsFacingLeft = false;
		}
								
		transform.localScale = new Vector3 (0, 0, 0);

		originalSortingOrder = GetComponent<Renderer> ().sortingOrder;

	}
		
	private void InitIntroAnim ()
	{		
		if (introAnimComplete)
			return;

		float multiplier = (thisTileCoord.x + thisTileCoord.y) + thisTileCoord.x;
		float delay = 0.15f * multiplier;
				
		Invoke ("IntroAnim", delay);	
		introAnimComplete = true;
	}
		
	private void IntroAnim ()
	{				
		transform.localScale = originalScale;
		animator.SetInteger ("AnimState", 8);
				
		StartCoroutine (AnimOffset ());		
	}
		
	private void StartDeath (Transform enemy, Vector3 playerCoord, Transform player)
	{				
		if (this.transform != enemy || player.name == "Blubba") 
			return;

		if (playerCoord.y > thisTileCoord.y)
			GetComponent<Renderer> ().sortingOrder -= 2;

		if (enemyWaves.enableEmission)
			enemyWaves.enableEmission = false;
				
		currentPlayerCoord = playerCoord;
		currentKillingPlayer = player;
		thisIsAttacked = true;
				
		enemyDangerTileController.SendMessage ("HideAllTiles");

		if (player.position.x > transform.position.x && !inverseScale)
			Flip ();
		else if (player.position.x < transform.position.x && inverseScale)
			Flip ();
				
		if (player.name == "Pinky") {					
			animator.SetInteger ("AnimState", 1);
			burstFX.GetComponent<ParticleSystem> ().startDelay = 1.8f;

		} else {
			animator.SetInteger ("AnimState", -1);
			burstFX.GetComponent<ParticleSystem> ().startDelay = 0.7f;

		}
				
		GameObject burstFXClone = Instantiate (burstFX, burstFXPosition, Quaternion.identity) as GameObject;
				
		Destroy (burstFXClone, 5);
	}
		
	private void AssignCounterAttackAnimation (Vector3 playerCoord, Transform player, Transform enemy, Vector3 enemyPos, bool blockedByOrac)
	{
		if (this.transform != enemy)
			return;

		GetComponent<Renderer> ().sortingOrder += 1;

		thisIsAttacked = false;
										
		int playerX = (int)playerCoord.x;
		int playerY = (int)playerCoord.y;
				
		//RIGHT
		if (playerX > thisTileCoord.x) {
				
			if (playerY > thisTileCoord.y) {
				animator.SetInteger ("AnimState", 3);

				if (spriteIsFacingLeft)
					Flip ();
			} else if (playerY < thisTileCoord.y) {

				animator.SetInteger ("AnimState", 5);
				if (spriteIsFacingLeft)
					Flip ();
			} else {
				animator.SetInteger ("AnimState", 4);
				if (spriteIsFacingLeft && !inverseScale)
					Flip ();
					
			}

			//LEFT

		} else if (playerX < thisTileCoord.x) {
				
			if (playerY > thisTileCoord.y) {

				animator.SetInteger ("AnimState", 3);
								
			} else if (playerY < thisTileCoord.y) {

				animator.SetInteger ("AnimState", 5);
								
			} else {
				animator.SetInteger ("AnimState", 4);

				if (inverseScale)
					Flip ();
			}
		} else {
			if (playerY > thisTileCoord.y) {
				animator.SetInteger ("AnimState", 2);
			} else {
				animator.SetInteger ("AnimState", 6);
			}
		}
	}
		
	private void Flip ()
	{
		spriteIsFacingLeft = !spriteIsFacingLeft;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
		
	private void StartTranceAnim ()
	{
		animator.SetInteger ("AnimState", 9);
		enemyWaves.enableEmission = true;
				
		if (OnEnemyIsInTrance != null)
			OnEnemyIsInTrance (this.transform);
	}
		
	private void Idle ()
	{
		if (enemy2 || enemy3) {
			if (inverseScale && spriteIsFacingLeft) {
				Flip ();
			} else if (! inverseScale && !spriteIsFacingLeft) {
				Flip ();
			}
		} else {
			if (!spriteIsFacingLeft)
				Flip ();
		}
						
		animator.SetInteger ("AnimState", 0);				
		thisIsAttacked = false;
		GetComponent<Renderer> ().sortingOrder = originalSortingOrder;
	}
		
	private void PlayRebirthAnim (Transform deadPlayer)
	{		
		if (! thisIsAttacked)
			return;
		if (deadPlayer.name != "Blubba") {
			animator.SetInteger ("AnimState", 8);
		}
	}
		
	private void Die ()
	{
		if (thisIsAttacked) {

			Destroy (enemyCounterAttack);
			GetComponent<Collider2D> ().enabled = false;
			Instantiate (enemyFlare, burstFXPosition, Quaternion.identity);

			if (OnEnemyIsDead != null) 
				OnEnemyIsDead (this.transform, currentPlayerCoord, currentKillingPlayer);
			
			Destroy (gameObject, 2);
			thisIsAttacked = false;
		}
	}
		
	private void TranceFinish ()
	{
		animator.SetInteger ("AnimState", 10);
		enemyWaves.enableEmission = false;
	}
		
	private IEnumerator AnimOffset ()
	{
		yield return new WaitForSeconds (1);
		
		animator.speed = Random.Range (0, 2000);
				
		yield return new WaitForSeconds (0.001f);
				
		animator.speed = 1;
		
	}

	private void PlayEnemyLaugh ()
	{
		enemyLaugh.Play ();
	}


	private void PlayEnemyPop ()
	{
		enemyPop.Play ();
		
		if (enemy2)
			GetComponents<AudioSource> () [2].Play ();
			
		if (enemy3)
			GetComponents<AudioSource> () [3].Play ();
	}

	private void PlayEnemy1Stab ()
	{
		enemyStab.pitch = Random.Range (1, 1.15f);
		enemyStab.Play ();
	}

	private void PlaySpawnFx ()
	{
		spawnFx.pitch = Random.Range (0.5f, 0.8f);
		spawnFx.Play ();
	}
		
}