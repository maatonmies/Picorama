
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{				
	public Transform startingPlayer;
	private GameObject levelComp;
	public GameObject levelButton;
	private GameObject star;
	private GameObject tapToProceedButton;
	private ParticleSystem starBurst;
	
	public delegate void StartLevel (Transform startingPlayer)	;	
	public static event StartLevel OnStartLevel;	
		
	public delegate void PlayOutroAnimations ()	;	
	public static event PlayOutroAnimations OnPlayOutroAnimations	;
		
	public delegate void NoEnemiesLeft ()	;	
	public static event NoEnemiesLeft OnNoEnemiesLeft;

	private AudioSource woosh;
	private AudioSource moveTargetSound;
	private AudioSource hit;
	private AudioSource bell;
			
	private List<Transform> enemies;
			
	void Start ()
	{
		enemies = new List<Transform> ();
		GameObject[] enemyArray = GameObject.FindGameObjectsWithTag ("Enemy");
		
		foreach (GameObject enemy in enemyArray) {
			
			enemies.Add (enemy.transform);
		}

		woosh = GameObject.Find ("Camera").GetComponents<AudioSource> () [2];
		moveTargetSound = GameObject.Find ("MoveTargetX").GetComponent<AudioSource> ();
		hit = GameObject.Find ("InGameUI").GetComponents<AudioSource> () [2];
		bell = GameObject.Find ("Camera").GetComponents<AudioSource> () [3];
		hit.volume = 1;

		levelComp = GameObject.Find ("InGameUI").transform.Find ("LevelCompleteScreen/LevelCompCanvas/LevelCompHeader").gameObject;
		star = GameObject.Find ("InGameUI").transform.Find ("LevelCompleteScreen/LevelCompCanvas/Star").gameObject;
		starBurst = star.transform.GetChild (0).GetComponent<ParticleSystem> ();
		tapToProceedButton = GameObject.Find ("InGameUI").transform.Find ("LevelCompleteScreen/LevelCompCanvas/TapToProceed").gameObject;

		star.gameObject.SetActive (false);
		tapToProceedButton.gameObject.SetActive (false);
		levelButton.gameObject.SetActive (false);
		levelComp.gameObject.SetActive (false);
					
	}
			
	void OnEnable ()
	{				
		EnemyAnimator.OnEnemyIsDead += CheckProgress;
		
		CameraScriptedAnimations.OnCameraIntroPanComplete += BroadcastStartLevel;
				
		ExitController.OnExitReached += BroadcastPlayOutroAnimations;
				
		CameraScriptedAnimations.OnCameraOutroAnimComplete += LevelCompleteScreen;

	}
		
	void OnDisable ()
	{		
		EnemyAnimator.OnEnemyIsDead -= CheckProgress;
		
		CameraScriptedAnimations.OnCameraIntroPanComplete -= BroadcastStartLevel;
		
		ExitController.OnExitReached -= BroadcastPlayOutroAnimations;
				
		CameraScriptedAnimations.OnCameraOutroAnimComplete -= LevelCompleteScreen;		

	}
		
	private void BroadcastStartLevel ()
	{
		if (startingPlayer == null)
			Debug.LogError ("Please assign startingPlayer to LevelManager!!!");
						
		if (OnStartLevel != null)
			OnStartLevel (startingPlayer);
	}
		
	private void BroadcastPlayOutroAnimations ()
	{
		if (OnPlayOutroAnimations != null)
			OnPlayOutroAnimations ();
	}
		
	private void LevelCompleteScreen ()
	{
		levelComp.gameObject.SetActive (true);

		iTween.ScaleFrom (levelComp, iTween.Hash (
			"scale", new Vector3 (0, 0, 0),
			"time", 0.7f,
			"easetype", iTween.EaseType.spring,
			"oncomplete", "BringInLevelButton",
			"oncompletetarget", gameObject));
	}

	private void BringInLevelButton ()
	{
		levelButton.SetActive (true);

		iTween.MoveFrom (levelButton, iTween.Hash (
			"position", new Vector3 (-2000, 0, 0),
			"time", 0.7f,
			"easetype", iTween.EaseType.spring,
			"oncomplete", "BringInStar",
			"oncompletetarget", gameObject));

		woosh.Play ();
	}

	private void BringInStar ()
	{

		star.SetActive (true);

		iTween.MoveFrom (star, iTween.Hash (

			"position", new Vector3 (2000, 2000, 0),
			"time", 1f,
			"easetype", iTween.EaseType.easeInElastic,
			"oncomplete", "PlayHit",
			"oncompletetarget", gameObject));

	}

	private void PlayHit ()
	{
		hit.volume = 0.5f;
		hit.Play ();

		iTween.ScaleTo (levelButton, iTween.Hash (
			"scale", new Vector3 (0.95f, 0.95f, 0),
			"time", 0.2f,
			"oncomplete", "ScaleBack",
			"oncompletetarget", gameObject));
		starBurst.transform.position = Camera.main.ScreenToWorldPoint (
			new Vector3 (Screen.width / 2, Screen.height / 2, 0));
		starBurst.Play ();
	}

	private void ScaleBack ()
	{
		iTween.ScaleTo (levelButton, iTween.Hash (
			"scale", new Vector3 (1, 1, 0),
			"time", 0.2f));

		bell.Play ();
		Invoke ("ShowTapToProceedButton", 1f);
	}

	private void ShowTapToProceedButton ()
	{
		tapToProceedButton.SetActive (true);

		iTween.ScaleTo (tapToProceedButton, iTween.Hash (
			"scale", new Vector3 (1, 1, 0),
			"time", 0.7f,
			"easetype", iTween.EaseType.easeInOutSine,
			"oncomplete", "PulseTapToProceedText",
			"oncompletetarget", gameObject));
	}

	private void PulseTapToProceedText ()
	{
		iTween.ScaleTo (tapToProceedButton, iTween.Hash (
			"scale", new Vector3 (0.95f, 0.95f, 0),
			"time", 0.5f,
			"easetype", iTween.EaseType.easeInOutSine,
			"loopType", iTween.LoopType.pingPong));
	}

	public void LoadNextLevel ()
	{
		moveTargetSound.Play ();

		GameManager.manager.UnlockLevel (Application.loadedLevel + 1);

		if (Application.loadedLevel < 32) {
			GameManager.manager.SendMessage ("LoadSceneWithTransition", Application.loadedLevel + 1);
		} else
			GameManager.manager.SendMessage ("LoadSceneWithTransition", 1);
	}
		
	private void CheckProgress (Transform enemy, Vector3 playerCoords, Transform player)
	{
		if (enemies.Contains (enemy)) {
				
			enemies.Remove (enemy);
		}

		if (enemies.Count < 1) {
			
			if (OnNoEnemiesLeft != null)
				OnNoEnemiesLeft ();
		}
	}
}
