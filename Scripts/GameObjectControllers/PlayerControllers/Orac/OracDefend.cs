using UnityEngine;
using System.Collections;

public class OracDefend : MonoBehaviour
{	
	public delegate void OracFinishedDefend ();
	public static event OracFinishedDefend OnOracFinishedDefend;
	
	public GameObject smokeFX;
	public GameObject sparkle;
	
	private Animator animator;
	private int prevSortingOrder;
	private Vector3 smokeFXPos;
	private Vector3 sparklePos;

	private Vector3 smokeLeft;
	private Vector3 smokeRight;
	private Vector3 smokeUp;
	private Vector3 smokeDown;

	private Vector3 sparkleLeft;
	private Vector3 sparkleRight;
	private Vector3 sparkleUp;
	private Vector3 sparkleDown;

	private AudioSource steps;
	private AudioSource defendVoice;
	private AudioSource victoryVoice;
	private AudioSource huuu;
	private bool alreadyDefending = false;

	void OnEnable ()
	{
		EnemyCounterAttack.OnEnemyStartCounterAttack += Defend;
	}
	void OnDisable ()
	{
		EnemyCounterAttack.OnEnemyStartCounterAttack -= Defend;
	}

	void Start ()
	{
		animator = GetComponent<Animator> ();
		smokeFXPos = Vector3.zero;
		sparklePos = Vector3.zero;

		smokeLeft = new Vector3 (-50, -7);
		smokeRight = new Vector3 (53, 12);
		smokeUp = new Vector3 (0, 116);
		smokeDown = new Vector3 (0, -30);

		sparkleLeft = new Vector3 (-18, 5);
		sparkleRight = new Vector3 (18, 5);
		sparkleDown = new Vector3 (0, -40);
		sparkleUp = new Vector3 (0, 80);

		steps = gameObject.GetComponents<AudioSource> () [3];

		defendVoice = gameObject.GetComponents<AudioSource> () [6];
		victoryVoice = gameObject.GetComponents<AudioSource> () [4];
		huuu = gameObject.GetComponents<AudioSource> () [5];
	}
	
	private void Defend (Vector3 playerCoord, Transform player, Transform enemy, Vector3 enemyPos, bool blockedByOrac)
	{
		if (! blockedByOrac || alreadyDefending)
			return;
			
		prevSortingOrder = GetComponent<Renderer> ().sortingOrder;

		GetComponent<Renderer> ().sortingOrder -= 1;
		alreadyDefending = true;

		if (enemy.name == "Enemy2") {

			if (enemyPos.x < transform.position.x) {

				animator.Play ("DefendBlitzLeft");
				smokeFXPos = smokeLeft;
				sparklePos = sparkleLeft;

			} else if (enemyPos.x > transform.position.x) {
			
				animator.Play ("DefendBlitzRight");
				smokeFXPos = smokeRight;
				sparklePos = sparkleRight;

			
			} else if (enemyPos.y > transform.position.y) {

				animator.Play ("DefendBlitzUp");
				smokeFXPos = smokeUp;
				sparklePos = sparkleUp;

			
			
			} else if (enemyPos.y < transform.position.y) {
			
				animator.Play ("DefendBlitzDown");
				smokeFXPos = smokeDown;
				sparklePos = sparkleDown;

			}

		} else if (enemy.name == "Enemy3") {

			if (enemyPos.x < transform.position.x) {
				
				animator.Play ("DefendFireballLeft");
				smokeFXPos = smokeLeft;
				sparklePos = sparkleLeft;
				
			} else if (enemyPos.x > transform.position.x) {
				
				animator.Play ("DefendFireballRight");
				smokeFXPos = smokeRight;
				sparklePos = sparkleRight;
				
				
			} else if (enemyPos.y > transform.position.y) {
				
				animator.Play ("DefendFireballUp");
				smokeFXPos = smokeUp;
				sparklePos = sparkleUp;
				
			} else if (enemyPos.y < transform.position.y) {
				
				animator.Play ("DefendFireballDown");
				smokeFXPos = smokeDown;
				sparklePos = sparkleDown;
				
			}
		}
	}

	private void ResetSortingOrder ()
	{
		GetComponent<Renderer> ().sortingOrder = prevSortingOrder;
	}

	private void Smoke ()
	{
		GameObject smokeFXClone = Instantiate (smokeFX, transform.position + smokeFXPos, Quaternion.identity) as GameObject;
		Destroy (smokeFXClone.gameObject, 5);
	}

	private void Sparkle ()
	{
		GameObject sparkleClone = Instantiate (sparkle, transform.position + sparklePos, Quaternion.identity) as GameObject;
		Destroy (sparkleClone.gameObject, 5);
	}

	private void PlaySteps ()
	{
		if (!steps.isPlaying) {
			steps.volume = 1;
			steps.Play ();
		}
	}

	private void StopSteps ()
	{
		StartCoroutine (FadeOut (steps));
	}

	private void PlayDefendVoice ()
	{
		defendVoice.Play ();
	}
	private void PlayVictoryVoice ()
	{
		victoryVoice.Play ();
	}
	private void PlayHuuu ()
	{
		huuu.Play ();
	}
	
	private void ReportOracFinishedDefend ()
	{		
		if (OnOracFinishedDefend != null)
			OnOracFinishedDefend ();

		alreadyDefending = false;
	}

	private IEnumerator FadeOut (AudioSource sound)
	{
		for (int i= 10; i > 0; i --) {
			
			sound.volume -= 0.15f;
			yield return new WaitForSeconds (0.001f);
			
		}
		sound.Stop ();
	}
}
