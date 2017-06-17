using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerPathController))]
[RequireComponent(typeof(PlayerMoveAnimator))]

public class PinkyController : MonoBehaviour
{
	private Vector3 originalPos;
		
	private Vector3 leftShift;
	private Vector3 rightShift;
	private Vector3 upShift;
	private Vector3 downShift;
		
	private Vector3 shift;
	private Animator animator;
	private int prevSortingOrder = 0;

	private AudioSource[] sounds;
	private AudioSource jumpFX;
	private AudioSource tickleFX;
	private AudioSource victoryVoice;
	private AudioSource attackVoice;
	private AudioSource giggle;
	private AudioSource dieVoice;
	private AudioSource huu;
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
				
		originalPos = Vector3.zero;
								
		leftShift = new Vector3 (-64, 0, 0);
		rightShift = new Vector3 (64, 0, 0);
		upShift = new Vector3 (0, 64, 0);
		downShift = new Vector3 (0, -45, 0);
				
		shift = Vector3.zero;

		sounds = gameObject.GetComponents<AudioSource> ();

		jumpFX = sounds [1];
		tickleFX = sounds [2];
		victoryVoice = sounds [4];
		attackVoice = sounds [6];
		giggle = sounds [7];
		dieVoice = sounds [8];
		huu = sounds [9];
		shockVoice = sounds [10];
	}

	private void StartAttack (Transform enemy, Vector3 playerCoord, Transform player)
	{
		if (transform != player)
			return;

		shift = AttackDirection (enemy, enemy.position);
		attackVoice.Play ();
	}

	private void ShiftToEnemyTile ()
	{
		originalPos = transform.position;
		prevSortingOrder = GetComponent<Renderer> ().sortingOrder;
		GetComponent<Renderer> ().sortingOrder += 1;

		iTween.MoveBy (gameObject, iTween.Hash (
				"amount", shift,
				"time", 0.25f,
				"easetype", iTween.EaseType.easeInOutQuad));
	}
		
	private Vector3 AttackDirection (Transform enemy, Vector3 enemyPosition)
	{		
		if (enemyPosition.x > transform.position.x) {
			animator.Play ("AttackSide");
			return rightShift;
						
		} else if (enemyPosition.x < transform.position.x) {
				
			gameObject.SendMessageUpwards ("Flip");
									
			animator.Play ("AttackSide");
			return leftShift;
						
		} else if (enemyPosition.y > transform.position.y) {
				
			animator.Play ("AttackUp");
			
			return upShift;
		} else {
			animator.Play ("AttackDown");
			
			if (enemy.name == "Enemy2" || enemy.name == "Enemy3")
				return Vector3.zero;
			else
				return downShift;
		}
	}
		
	private void ShiftBack ()
	{ 
		iTween.MoveTo (gameObject, iTween.Hash (
				"position", originalPos, 
				"time", 0.25f,
				"easetype", iTween.EaseType.easeInOutQuad,
			    "oncomplete", "SortingOrderCheck"));
		giggle.Play ();
	}

	private void SortingOrderCheck ()
	{
		GetComponent<Renderer> ().sortingOrder = prevSortingOrder;
	}

	private void PlayJumpFx ()
	{
		jumpFX.Play ();
	}

	private void PlayTickleFx ()
	{
		tickleFX.Play ();
	}

	private void PlayVictoryVoice ()
	{
		victoryVoice.Play ();
	}

	private void PlayDieVoice ()
	{
		dieVoice.Play ();
	}

	private void PlayHuu ()
	{
		huu.Play ();
	}

	private void PlayShockVoice ()
	{
		shockVoice.Play ();
	}
		
}
