using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlubbaController : MonoBehaviour
{
	public bool
		isAlreadyAttacking = false;
		
	private Animator animator;

	[HideInInspector]
	public List<Transform>
		enemiesInTrance;

	private ParticleSystem blubbaWaves;
	private AudioSource wavesSound;
	private AudioSource jump;
	private AudioSource move;
	private AudioSource victoryVoice;
	private AudioSource huu;
	private AudioSource dieVoice;

	void OnEnable ()
	{
		EnemyAnimator.OnEnemyIsInTrance += RegisterEnemyInTrance;
		PlayersEnemySensor.OnEnemyOutOfRange += CheckEnemiesOnEnemyOutOfRange;
		EnemyAnimator.OnEnemyIsDead += CheckEnemiesOnEnemyIsDead;
		PlayerMoveAnimator.OnPlayerIsMoving += CheckEnemiesOnPlayerIsMoving;
		PlayerDeath.OnPlayerIsDead += CheckEnemiesOnPlayerIsDead;
		InputManager.OnEnemySelected += StartWaves;
		InputManager.OnPlayerSelected += CheckEnemiesOnPlayerSelected;
		
	}
	void OnDisable ()
	{
		EnemyAnimator.OnEnemyIsInTrance -= RegisterEnemyInTrance;
		PlayersEnemySensor.OnEnemyOutOfRange -= CheckEnemiesOnEnemyOutOfRange;
		EnemyAnimator.OnEnemyIsDead -= CheckEnemiesOnEnemyIsDead;
		PlayerMoveAnimator.OnPlayerIsMoving -= CheckEnemiesOnPlayerIsMoving;
		PlayerDeath.OnPlayerIsDead -= CheckEnemiesOnPlayerIsDead;
		InputManager.OnEnemySelected -= StartWaves;	
		InputManager.OnPlayerSelected -= CheckEnemiesOnPlayerSelected;

	}
		
	void Start ()
	{
		animator = gameObject.GetComponent<Animator> ();
		enemiesInTrance = new List<Transform> ();
		blubbaWaves = GetComponentInChildren<ParticleSystem> ();
		wavesSound = GetComponents<AudioSource> () [1];
		jump = GetComponents<AudioSource> () [2];
		move = GetComponents<AudioSource> () [3];
		victoryVoice = GetComponents<AudioSource> () [6];
		huu = GetComponents<AudioSource> () [7];
		dieVoice = GetComponents<AudioSource> () [8];
	}
		
	private void StartWaves (Transform enemy, Vector3 playerCoord, Transform player)
	{
		if (player == transform) {

			blubbaWaves.enableEmission = true;
		}
	}

	private void StopWaves ()
	{
		blubbaWaves.enableEmission = false;
	}
	
	private void StartAttackAnim ()
	{
		animator.Play ("AttackStart");
		isAlreadyAttacking = true;

	}
	private void SingMelody ()
	{
		wavesSound.Play ();
	}
		
	private void RegisterEnemyInTrance (Transform enemy)
	{
		if (!enemiesInTrance.Contains (enemy))
			enemiesInTrance.Add (enemy);
	}

	private void CheckEnemiesOnEnemyOutOfRange (Transform enemy, Transform player, Transform sensor)
	{				
		if (! enemiesInTrance.Contains (enemy) || player != this.transform)
			return;
						
		enemiesInTrance.Clear ();
				
		animator.Play ("AttackFinish");
		isAlreadyAttacking = false;

		wavesSound.Stop ();

	}
		
	private void CheckEnemiesOnEnemyIsDead (Transform enemy, Vector3 playerCoord, Transform player)
	{				
		if (! enemiesInTrance.Contains (enemy))
			return;
		
		enemiesInTrance.Remove (enemy);
		
		if (enemiesInTrance.Count < 1) {
			animator.Play ("AttackFinish");
			isAlreadyAttacking = false;
			blubbaWaves.enableEmission = false;
			
			wavesSound.Stop ();
		}
	}
		
	private void CheckEnemiesOnPlayerIsMoving (Transform player)
	{				
		if (player == this.transform) {
						
			enemiesInTrance.Clear ();
			isAlreadyAttacking = false;
			blubbaWaves.enableEmission = false;

			wavesSound.Stop ();
		}					
	}

	private void CheckEnemiesOnPlayerSelected (Transform player)
	{				
		if (player == this.transform && isAlreadyAttacking) {
			
			enemiesInTrance.Clear ();
			isAlreadyAttacking = false;
			blubbaWaves.enableEmission = false;
			animator.Play ("AttackFinish");

			wavesSound.Stop ();


		}					
	}
		
	private void CheckEnemiesOnPlayerIsDead (Transform player)
	{		
		StopWaves ();

		wavesSound.Stop ();

		if (player == this.transform) {
			enemiesInTrance.Clear ();
			isAlreadyAttacking = false;
			
		} else if (enemiesInTrance.Count != 0) {
			enemiesInTrance.Clear ();
			animator.Play ("AttackFinish");
			isAlreadyAttacking = false;
		}
	}

	private void PlayJumpSound ()
	{
		jump.Play ();
	}

	private void PlayMoveSound ()
	{
		if (!move.isPlaying)
			move.Play ();
	}

	private void PlayVictoryVoice ()
	{
		victoryVoice.Play ();
	}

	private void PlayHuu ()
	{
		huu.Play ();
	}

	private void PlayDieVoice ()
	{
		dieVoice.Play ();
	}
}