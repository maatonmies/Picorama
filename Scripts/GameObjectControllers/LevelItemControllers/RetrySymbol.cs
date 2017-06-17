using UnityEngine;
using System.Collections;

public class RetrySymbol : MonoBehaviour
{
	Hashtable rotateTweenParams;
	Hashtable growTweenParams;
	Hashtable shrinkTweenParams;
		
	private Vector3 retryButtonPos;
	private AudioSource bgMusic;
	private AudioSource retrySound;
	private AudioSource unwind;
	private GradientBackground background;
	private GameObject mainCamera;
	public bool shouldPitch = true;

	void OnEnable ()
	{
		PlayerDeath.OnPlayerIsDead += DisplayRetryButton;
		InputManager.OnRetrySelected += Shrink;
		InputManager.OnEnemySelected += RegisterTileToDisplayRetry;
	}
		
	void OnDisable ()
	{
		PlayerDeath.OnPlayerIsDead -= DisplayRetryButton;
		InputManager.OnRetrySelected -= Shrink;
		InputManager.OnEnemySelected -= RegisterTileToDisplayRetry;
		
	}
	void Start ()
	{				

		GetComponent<Collider2D> ().enabled = false;
		rotateTweenParams = iTween.Hash (
				 "amount", new Vector3 (0, 0, -1),
				 "time", 1,
				 "looptype", iTween.LoopType.loop,
				 "easetype", iTween.EaseType.linear,
			"ignoretimescale", true);
				 
				 
		growTweenParams = iTween.Hash (
				 "scale", new Vector3 (0.8f, 0.8f, 0),
				 "time", 0.6f,
				 "easetype", iTween.EaseType.easeOutBack,
			"ignoretimescale", true,
			"oncomplete", "StopTime");
				 
		shrinkTweenParams = iTween.Hash (
			"scale", new Vector3 (0, 0, 0),
			"time", 0.5f,
			"easetype", iTween.EaseType.easeOutBack,
			"ignoretimescale", true);

		bgMusic = GameObject.Find ("GameManager/Music").GetComponents<AudioSource> () [1];
		retrySound = GetComponent<AudioSource> ();
		unwind = GetComponents<AudioSource> () [1];
		background = GameObject.Find ("Background/BgCamera").GetComponent<GradientBackground> ();
		mainCamera = GameObject.Find ("Camera");
		
		Rotate ();
	}
	
	private void Rotate ()
	{
		iTween.RotateBy (gameObject, rotateTweenParams);
	}
		
	private void Shrink (Transform player)
	{
		CancelInvoke ("ZeroTimeScale");
		Time.timeScale = 1;
		
		if (shouldPitch) {
			StopCoroutine ("PitchDown");
			StartCoroutine (PitchUp (bgMusic));
		}

		iTween.ScaleTo (gameObject, shrinkTweenParams);
		retrySound.Play ();
		background.SendMessage ("BackToNormalColors");

		GetComponent<Collider2D> ().enabled = false;

	}
	
	private void Grow ()
	{
		iTween.ScaleTo (gameObject, growTweenParams);
	}
		
	private void RegisterTileToDisplayRetry (Transform enemy, Vector3 playerCoord, Transform player)
	{
		Vector3 playerOffset = player.GetComponent<PlayerMoveAnimator> ().SpriteOffset;
		retryButtonPos = player.position - playerOffset;				
	}
		
	private void DisplayRetryButton (Transform player)
	{						
		transform.position = retryButtonPos;

		if (mainCamera.GetComponent<Camera> ().WorldToScreenPoint (retryButtonPos).x < 0)
			mainCamera.SendMessage ("FocusCameraOnRetryButton", transform);
		else if (mainCamera.GetComponent<Camera> ().WorldToScreenPoint (retryButtonPos).x > Screen.width)
			mainCamera.SendMessage ("FocusCameraOnRetryButton", transform);
		else if (mainCamera.GetComponent<Camera> ().WorldToScreenPoint (retryButtonPos).y < 0)
			mainCamera.SendMessage ("FocusCameraOnRetryButton", transform);
		else if (mainCamera.GetComponent<Camera> ().WorldToScreenPoint (retryButtonPos).y > Screen.height)
			mainCamera.SendMessage ("FocusCameraOnRetryButton", retryButtonPos);

		unwind.Play ();

		if (shouldPitch)
			StartCoroutine (PitchDown (bgMusic));
		
		Grow ();
	}

	private IEnumerator PitchDown (AudioSource sound)
	{
		for (int i=0; i<10; i++) {

			sound.pitch -= 0.06f;
			yield return new WaitForSeconds (0.06f);
		}
	}

	private IEnumerator PitchUp (AudioSource sound)
	{
		for (int i=0; i<10; i++) {
			
			sound.pitch += 0.06f;
			yield return new WaitForSeconds (0.06f);
		}
	}

	private void StopTime ()
	{
		GetComponent<Collider2D> ().enabled = true;
		Invoke ("ZeroTimeScale", 0.4f);
	}

	private void ZeroTimeScale ()
	{
		Time.timeScale = 0;
	}
}
