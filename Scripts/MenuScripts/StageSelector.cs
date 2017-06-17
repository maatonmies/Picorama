using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StageSelector : MonoBehaviour
{
	public GameObject stageSelectorUI;
	private AudioSource tapSound;

	void Start ()
	{
		tapSound = GetComponents<AudioSource> () [0];

		iTween.MoveFrom (stageSelectorUI, iTween.Hash (
			"position", new Vector3 (stageSelectorUI.transform.position.x, stageSelectorUI.transform.position.y + 500, 0),
			"time", 1.5f,
			"easetype", iTween.EaseType.spring));
	}

	public void OnButtonTap (int levelToLoad)
	{
		tapSound.Play ();

		iTween.MoveTo (stageSelectorUI, iTween.Hash (
			"position", stageSelectorUI.transform.position + new Vector3 (0, 900, 0), 
			"time", 1, 
			"easetype", iTween.EaseType.easeInOutBack, 
			"oncompletetarget", gameObject, 
			"oncomplete", "LoadLevel",
			"oncompleteparams", levelToLoad));
	}

	private void LoadLevel (int level)
	{
		GameManager.manager.SendMessage ("LoadSceneWithTransition", level);
		Destroy (gameObject, 0.6f);
	}
}
