using UnityEngine;
using System.Collections;

public class Enemy3Fireball : MonoBehaviour
{
	public GameObject explosion;
	public GameObject sparkle;

	private Transform fireBall;
	private Vector3 fireballUpPos;
	private Vector3 fireballDownPos;
	private Vector3 fireballSidePos;

	private Vector3 targetPos;

	private bool playExplosion = true;

	private bool oracIsFacingLeft = false;
	private bool oracIsFacingRight = false;
	private bool oracIsFacingUp = false;
	private bool oracIsFacingDown = false;

	private Vector3 oracsMouthLeft;
	private Vector3 oracsMouthRight;
	private Vector3 oracsMouthUp;
	private Vector3 oracsMouthDown;

	private Vector3 originalScale;
	private AudioSource fireballSound;
	
	void OnEnable ()
	{
		EnemyCounterAttack.OnEnemyStartCounterAttack += InitFireball;
	}

	void OnDisable ()
	{
		EnemyCounterAttack.OnEnemyStartCounterAttack -= InitFireball;
	}
	void Start ()
	{
		fireBall = transform.FindChild ("Fireball");
		fireBall.gameObject.SetActive (false);

		originalScale = fireBall.transform.localScale;

		fireballUpPos = new Vector3 (-10, 47, 0);
		fireballDownPos = new Vector3 (-26, 5, 0);
		fireballSidePos = new Vector3 (-15, 37, 0);

		oracsMouthLeft = new Vector3 (-25, 7);
		oracsMouthRight = new Vector3 (25, 7);
		oracsMouthDown = new Vector3 (0, -40);
		oracsMouthUp = new Vector3 (0, 80);
		
		targetPos = Vector3.zero;
		fireballSound = GetComponents<AudioSource> () [1];
	}
	
	private void InitFireball (Vector3 playerCoord, Transform player, Transform enemy, Vector3 enemyPos, bool blockedByOrac)
	{
		if (enemy != transform)
			return;

		AttackDirection (player.position);

		if (blockedByOrac) {

			Vector3 oracPos = GameObject.Find ("Orac").transform.position;

			if (oracIsFacingLeft)
				targetPos = oracPos + oracsMouthLeft;
			else if (oracIsFacingRight)
				targetPos = oracPos + oracsMouthRight;
			else if (oracIsFacingDown)
				targetPos = oracPos + oracsMouthDown;
			else if (oracIsFacingUp)
				targetPos = oracPos + oracsMouthUp;
			
			playExplosion = false;

		} else {

			targetPos = player.position;
			playExplosion = true;

		}
	}

	private void LaunchFireball ()
	{
		fireBall.gameObject.SetActive (true);
		fireballSound.Play ();

		iTween.MoveTo (fireBall.gameObject, iTween.Hash (
			"position", targetPos,
			"time", 0.5f,
			"oncompletetarget", this.gameObject,
			"oncomplete", "Explosion",
			"easetype", iTween.EaseType.linear));
	}

	private void AttackDirection (Vector3 playerPosition)
	{
		if (playerPosition.x > transform.position.x) {
			fireBall.transform.localPosition = fireballSidePos;

			oracIsFacingLeft = true;
			oracIsFacingRight = false;
			oracIsFacingDown = false;
			oracIsFacingUp = false;
				 
		} else if (playerPosition.x < transform.position.x) {

			fireBall.transform.localPosition = fireballSidePos;

			oracIsFacingRight = true;
			oracIsFacingLeft = false;
			oracIsFacingDown = false;
			oracIsFacingUp = false;

		} else if (playerPosition.y > transform.position.y) {
			
			fireBall.transform.localPosition = fireballUpPos;

			oracIsFacingDown = true;
			oracIsFacingRight = false;
			oracIsFacingLeft = false;
			oracIsFacingUp = false;

		} else if (playerPosition.y < transform.position.y) {

			fireBall.transform.localPosition = fireballDownPos;

			oracIsFacingUp = true;
			oracIsFacingRight = false;
			oracIsFacingDown = false;
			oracIsFacingLeft = false;

		}
	}

	private void Explosion ()
	{

		iTween.ScaleTo (fireBall.gameObject, iTween.Hash (
			"scale", new Vector3 (0, 0, 0),
			"time", 0.2f,
			"oncompletetarget", gameObject,
			"oncomplete", "SetFireballInactive"));

		if (playExplosion) {

			GameObject explosionClone = Instantiate (explosion, targetPos, Quaternion.identity) as GameObject;
			Destroy (explosionClone, 2);
		} else {

			GameObject sparkleClone = Instantiate (sparkle, targetPos, Quaternion.identity) as GameObject;
			Destroy (sparkleClone, 2);
		}
	}

	private void SetFireballInactive ()
	{
		fireBall.gameObject.SetActive (false);
		fireBall.transform.localScale = originalScale;
	}
	
}
