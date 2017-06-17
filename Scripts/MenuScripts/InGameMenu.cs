using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
	public GameObject sideBar;
	public GameObject menuButton;
	public GameObject shade;
	public GameObject exitDialog;

	private GameManager gameManager;
	private AudioSource selectSound;

	private List<GameObject> playerTokens;
	
	void OnEnable ()
	{
		ExitController.OnExitReached += HideMenuButton;
		InputManager.OnPlayerSelected += DisplayPlayerToken;
		EnemyCounterAttack.OnPlayerGetReadyToDie += HideMenuButton;
		InputManager.OnRetrySelected += ShowMenuButton;
	}

	void OnDisable ()
	{
		ExitController.OnExitReached -= HideMenuButton;
		InputManager.OnPlayerSelected -= DisplayPlayerToken;
		EnemyCounterAttack.OnPlayerGetReadyToDie -= HideMenuButton;
		InputManager.OnRetrySelected -= ShowMenuButton;
	}

	void Start ()
	{
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		selectSound = GetComponents<AudioSource> () [1];
		
		playerTokens = new List<GameObject> ();
		playerTokens.Add (transform.FindChild ("MenuCanvas/PinkyToken").gameObject);
		playerTokens.Add (transform.FindChild ("MenuCanvas/BlubbaToken").gameObject);
		playerTokens.Add (transform.FindChild ("MenuCanvas/GreengoToken").gameObject);
		playerTokens.Add (transform.FindChild ("MenuCanvas/OracToken").gameObject);
		
		foreach (GameObject playerToken in playerTokens)
			playerToken.SetActive (false);
	}
	
	private void DisplayPlayerToken (Transform player)
	{
		foreach (GameObject playerToken in playerTokens)
			playerToken.SetActive (playerToken.name.Contains (player.name));
	}

	public void MenuButton ()
	{
		selectSound.pitch = 1.2f;
		selectSound.Play ();

		iTween.MoveTo (sideBar, iTween.Hash (
			"position", new Vector3 (sideBar.transform.position.x - 200, sideBar.transform.position.y), 
			"time", 1, 
			"easetype", iTween.EaseType.spring,
			"ignoretimescale", true));

		menuButton.GetComponent<Image> ().CrossFadeAlpha (0f, 0.7f, true);
		menuButton.GetComponent<Button> ().enabled = false;
		shade.SetActive (true);
		shade.GetComponent<Image> ().CrossFadeAlpha (0f, 0f, true);
		shade.GetComponent<Image> ().CrossFadeAlpha (0.3f, 0.7f, true);
		foreach (GameObject playerToken in playerTokens) {
			playerToken.GetComponent<Image> ().CrossFadeAlpha (0f, 0.7f, true);
			playerToken.GetComponent<Button> ().enabled = false;
		}
	}
	
	public void ExitToStageSelectorButton ()
	{
		selectSound.pitch = 1f;
		selectSound.Play ();

		iTween.ScaleTo (exitDialog, iTween.Hash (
			"scale", new Vector3 (1, 1, 1), 
			"time", 0.5f, 
			"easetype", iTween.EaseType.spring,
			"ignoretimescale", true));

		shade.GetComponent<Image> ().CrossFadeAlpha (0.5f, 0.7f, true);
	} 

	public void ExitDialogYes ()
	{
		selectSound.pitch = 0.8f;
		
		selectSound.Play ();

		iTween.ScaleTo (exitDialog, iTween.Hash (
			"scale", new Vector3 (0, 0, 0), 
			"time", 0.5f, 
			"easetype", iTween.EaseType.easeInOutBack,
			"oncompletetarget", gameObject,
			"oncomplete", "LoadStageSelector",
			"ignoretimescale", true));
	}

	public void ExitDialogNo ()
	{
		selectSound.pitch = 0.8f;
		selectSound.Play ();

		iTween.ScaleTo (exitDialog, iTween.Hash (
			"scale", new Vector3 (0, 0, 0), 
			"time", 0.5f, 
			"easetype", iTween.EaseType.easeInOutBack,
			"ignoretimescale", true));
		shade.GetComponent<Image> ().CrossFadeAlpha (0.3f, 0.5f, true);
	}
	
	public void HowToButton ()
	{
		selectSound.pitch = 0.8f;

		selectSound.Play ();
	}
	
	public void ShareButton ()
	{
		selectSound.pitch = 0.8f;

		selectSound.Play ();
	}
	
	public void CLosePanelButton ()
	{
		selectSound.pitch = 1f;
		selectSound.Play ();

		shade.GetComponent<Image> ().CrossFadeAlpha (0f, 0.7f, true);
		Invoke ("DisableShade", 0.75f);

		iTween.MoveTo (sideBar, iTween.Hash (
			"position", new Vector3 (sideBar.transform.position.x + 200, sideBar.transform.position.y), 
			"time", 1, 
			"easetype", iTween.EaseType.spring,
			"ignoretimescale", true));

		menuButton.GetComponent<Image> ().CrossFadeAlpha (1f, 0.7f, true);
		menuButton.GetComponent<Button> ().enabled = true;
		foreach (GameObject playerToken in playerTokens) {
			playerToken.GetComponent<Image> ().CrossFadeAlpha (1f, 0.7f, true);
			playerToken.GetComponent<Button> ().enabled = true;
		}
	}

	private void DisableShade ()
	{
		shade.SetActive (false);
	}

	private void HideMenuButton ()
	{
		menuButton.GetComponent<Image> ().CrossFadeAlpha (0f, 0.7f, true);
		menuButton.GetComponent<Button> ().enabled = false;
		foreach (GameObject playerToken in playerTokens) {
			playerToken.GetComponent<Image> ().CrossFadeAlpha (0f, 0.7f, true);
			playerToken.GetComponent<Button> ().enabled = false;
		}
			
	}
	
	private void HideMenuButton (Transform player, Transform enemy)
	{
		menuButton.GetComponent<Image> ().CrossFadeAlpha (0f, 0.7f, true);
		menuButton.GetComponent<Button> ().enabled = false;
		foreach (GameObject playerToken in playerTokens) {
			playerToken.GetComponent<Image> ().CrossFadeAlpha (0f, 0.7f, true);
			playerToken.GetComponent<Button> ().enabled = false;
		}
		
	}
	
	private void ShowMenuButton (Transform deadPlayer)
	{
		menuButton.GetComponent<Image> ().CrossFadeAlpha (1f, 0.7f, true);
		menuButton.GetComponent<Button> ().enabled = true;
		foreach (GameObject playerToken in playerTokens) {
			playerToken.GetComponent<Image> ().CrossFadeAlpha (1f, 0.7f, true);
			playerToken.GetComponent<Button> ().enabled = true;
		}
		
	}

	private void LoadStageSelector ()
	{
		gameManager.SendMessage ("LoadSceneWithTransition", 2);
	}
}
