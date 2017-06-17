using UnityEngine;
using System.Collections;

public class MainMenuAnimations : MonoBehaviour
{
	public GameObject picoramaLogo;
	public GameObject pinky;
	public GameObject blubba;
	public GameObject greengo;
	public GameObject orac;
	
	public GameObject buttons;
	public GameObject enemies;

	private AudioSource tapSound;

	private GameManager gameManager;

	void Awake ()
	{
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager> ();
	}

	void Start ()
	{
		blubba.SetActive (false);
		pinky.SetActive (false);
		greengo.SetActive (false);

		buttons.GetComponent<Canvas> ().enabled = false;
		enemies.SetActive (false);

		tapSound = GetComponents<AudioSource> () [0];

		iTween.ScaleFrom (picoramaLogo, iTween.Hash ("scale", new Vector3 (0, 0, 0), "time", 1.5f, "easetype", iTween.EaseType.spring, "oncomplete", "LogoScaleCallback", "oncompletetarget", gameObject));
	}

	public void PlayButton ()
	{
		tapSound.Play ();
		StartCoroutine (TransitionToStageSelector ());
	}

	public void InfoButton ()
	{
		tapSound.Play ();
	}

	public void HowToButton ()
	{
		tapSound.Play ();
	}

	public void OptionsButton ()
	{
		tapSound.Play ();
	}
	
	private void LogoScaleCallback ()
	{
		Invoke ("LogoMoveUp", 0.7f);
		pinky.SetActive (true);
		
		iTween.ScaleTo (pinky, iTween.Hash ("scale", new Vector3 (1, 1, 1), "time", 1.5f, "easetype", iTween.EaseType.spring));
		iTween.MoveTo (pinky, iTween.Hash ("position", pinky.transform.position + new Vector3 (0, 90, 0), "time", 1.5f, "easetype", iTween.EaseType.spring));
	}

	private void LogoMoveUp ()
	{
		iTween.MoveTo (picoramaLogo, iTween.Hash ("position", picoramaLogo.transform.position + new Vector3 (0, 100, 0), "time", 2, "easetype", iTween.EaseType.easeInOutBack, "oncomplete", "LogoMoveUpCallback", "oncompletetarget", gameObject));
	}

	private void LogoMoveUpCallback ()
	{
		Invoke ("FriendsAppear", 1);

		iTween.MoveTo (picoramaLogo, iTween.Hash ("position", picoramaLogo.transform.position + new Vector3 (0, 20, 0), "time", 1, "easetype", iTween.EaseType.easeInOutSine, "looptype", iTween.LoopType.pingPong));
		iTween.MoveFrom (buttons, iTween.Hash ("position", buttons.transform.position + new Vector3 (-200, 0, 0), "time", 1f, "easetype", iTween.EaseType.spring, "oncomplete", "EnemiesAppear", "oncompletetarget", gameObject));
		buttons.GetComponent<Canvas> ().enabled = true;

	}

	private void EnemiesAppear ()
	{
		iTween.MoveFrom (enemies, iTween.Hash ("name", "enemiesAppear", "position", enemies.transform.position + new Vector3 (0, -500, 0), "time", 1f, "easetype", iTween.EaseType.spring));
		enemies.SetActive (true);
	}

	private void FriendsAppear ()
	{
		blubba.SetActive (true);
		iTween.MoveTo (blubba, iTween.Hash ("name", "blubbaAppear", "position", blubba.transform.position + new Vector3 (0, 70, 0), "time", 1, "easetype", iTween.EaseType.easeInOutSine));

		greengo.SetActive (true);
		iTween.ScaleTo (greengo, iTween.Hash ("name", "greengoAppearScale", "scale", new Vector3 (0.8f, 0.8f, 1), "time", 1f, "easetype", iTween.EaseType.spring));
		iTween.MoveTo (greengo, iTween.Hash ("name", "greengoAppearMove", "position", greengo.transform.position + new Vector3 (0, 40, 0), "time", 1, "easetype", iTween.EaseType.spring));

		orac.SetActive (true);
		iTween.ScaleTo (orac, iTween.Hash ("name", "oracAppearScale", "scale", new Vector3 (1, 1, 1), "time", 0.7f, "easetype", iTween.EaseType.spring));
		iTween.MoveTo (orac, iTween.Hash ("name", "oracAppearMove", "position", orac.transform.position + new Vector3 (0, 60, 0), "time", 0.7f, "easetype", iTween.EaseType.spring));
	}

	private IEnumerator TransitionToStageSelector ()
	{
		CancelInvoke ("FriendsAppear");

		if (iTween.tweens.Count > 0)
			iTween.Stop (gameObject, true);

		yield return new WaitForFixedUpdate ();

		iTween.MoveTo (picoramaLogo, iTween.Hash ("position", picoramaLogo.transform.position + new Vector3 (0, 500, 0), "time", 1, "easetype", iTween.EaseType.easeInOutBack, "oncompletetarget", gameObject, "oncomplete", "ShowStageSelector"));
		iTween.MoveTo (buttons, iTween.Hash ("position", buttons.transform.position + new Vector3 (0, -200, 0), "time", 1, "easetype", iTween.EaseType.easeInOutBack));
		iTween.MoveTo (enemies, iTween.Hash ("position", enemies.transform.position + new Vector3 (700, 0, 0), "time", 1, "easetype", iTween.EaseType.easeInOutBack));
	}

	private void ShowStageSelector ()
	{
		gameManager.SendMessage ("LoadSceneWithTransition", 2);
		Destroy (gameObject, 0.6f);
	}
}
