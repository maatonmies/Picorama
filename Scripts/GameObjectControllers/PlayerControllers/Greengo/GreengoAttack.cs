using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerPathController))]
[RequireComponent(typeof(PlayerMoveAnimator))]

public class GreengoAttack : MonoBehaviour
{
	public GameObject ballBurst;
	private Vector3 originalPos;

	private Vector3 shift;
	private GameObject ball;
	private Animator animator;
		
	private Transform targetEnemy;
		
	private Vector3 upPosition;
	private Vector3 downPosition;
	private Vector3 sidePosition;

	private ParticleSystem ballParticleSystem;

	private AudioSource flySound;
	private AudioSource landSound;
	private AudioSource hopSound;
	private AudioSource attackSound;
	private AudioSource victoryVoice;
	private AudioSource huuu;
	private AudioSource dieVoice;
	private AudioSource shockVoice;


	void OnEnable ()
	{
		InputManager.OnEnemySelected += StartAttack;

	}
	void OnDisable ()
	{		
		InputManager.OnEnemySelected -= StartAttack;			
	}

	void Start ()
	{		
		animator = GetComponent<Animator> ();
		ballParticleSystem = transform.Find ("Ball/BallFX").GetComponent<ParticleSystem> ();
		
		originalPos = Vector3.zero;

		shift = Vector3.zero;
				
		ball = transform.GetChild (2).gameObject;
				
		targetEnemy = null;
				
		upPosition = new Vector3 (-24, 208, 0);
		downPosition = new Vector3 (162, -176, 0);
		sidePosition = new Vector3 (250, 50, 0);

		attackSound = GetComponents<AudioSource> () [2];

		flySound = GetComponents<AudioSource> () [3];
		landSound = GetComponents<AudioSource> () [4];
		hopSound = GetComponents<AudioSource> () [5];
		victoryVoice = GetComponents<AudioSource> () [6];
		huuu = GetComponents<AudioSource> () [7];
		dieVoice = GetComponents<AudioSource> () [8];
		shockVoice = GetComponents<AudioSource> () [9];
	}
	
	private void StartAttack (Transform enemy, Vector3 playerCoord, Transform player)
	{
		if (transform != player)
			return;
						
		AttackDirection (enemy.position);
		targetEnemy = enemy;
	}
	
	private void Shift () //called by AttackDown anim only
	{
		originalPos = transform.position;
		
		iTween.MoveBy (gameObject, iTween.Hash (
			"amount", shift,
			"time", 0.25f,
			"easetype", iTween.EaseType.easeInOutQuad));
	}
		
	private void ShiftBack ()
	{ 
		iTween.MoveTo (gameObject, iTween.Hash (
			"position", originalPos, 
			"time", 0.25f,
			"easetype", iTween.EaseType.easeInOutQuad));
	}
	
	private void AttackDirection (Vector3 enemyPosition)
	{
		if (enemyPosition.x > transform.position.x) {
			animator.Play ("AttackSide");
			ball.transform.localPosition = sidePosition;
			
			
		} else if (enemyPosition.x < transform.position.x) {
			
			gameObject.SendMessage ("Flip");
			
			animator.Play ("AttackSide");
			ball.transform.localPosition = sidePosition;
			
			
		} else if (enemyPosition.y > transform.position.y) {
			
			animator.Play ("AttackUp");
			ball.transform.localPosition = upPosition;
			
			
		} else {
			animator.Play ("AttackDown");
			ball.transform.localPosition = downPosition;
			
		}
	}
		
	private void InitProjectile ()
	{		
		ball.transform.localScale = new Vector3 (0.8f, 0.8f, 0);		
	}
		
	private void LaunchBall ()
	{
		InitProjectile ();
		ballParticleSystem.enableEmission = true;
		
		iTween.MoveTo (ball, iTween.Hash (
				"position", targetEnemy.transform.position, 
				"time", 0.2f,
				"oncomplete", "HideProjectile",
				"oncompletetarget", gameObject,
				"easetype", iTween.EaseType.linear));
	}
		
	private void HideProjectile ()
	{							
		iTween.ScaleTo (ball.gameObject, iTween.Hash (
			    "scale", new Vector3 (0, 0, 0), 
			    "time", 0.3f,
			    "oncompletetarget", gameObject,
			    "oncomplete", "ResetProjectile"));

		ballParticleSystem.enableEmission = false;

		GameObject ballBurstClone = Instantiate (ballBurst, targetEnemy.transform.position, Quaternion.identity) as GameObject;
		
		Destroy (ballBurstClone, 3);
	}

	private void ResetProjectile ()
	{
		ball.transform.localPosition = Vector3.zero;
	}

	private void PlayFlySound ()
	{
		if (!flySound.isPlaying)
			flySound.Play ();
	}

	private void PlayHopSound ()
	{
		hopSound.Play ();
	}

	private void PlayAttackSound ()
	{
		attackSound.Play ();
	}

	private void PlayVictoryVoice ()
	{
		victoryVoice.Play ();
	}

	private void PlayHuuu ()
	{
		huuu.Play ();
	}

	private void PlayDieVoice ()
	{
		dieVoice.Play ();
	}

	private void PlayShockVoice ()
	{
		shockVoice.Play ();
	}

	private void StopFLySound ()
	{
		flySound.Stop ();
		landSound.Play ();
	}
}
