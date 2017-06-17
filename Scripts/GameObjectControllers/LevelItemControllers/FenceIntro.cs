using UnityEngine;
using System.Collections;

public class FenceIntro : MonoBehaviour
{
	private bool alreadyVisible = false;
	private AudioSource fx;

	void OnEnable ()
	{
		PlayerIntroAnim.OnPlayerIntroAnimComplete += AppearOnMap;
		LevelManager.OnNoEnemiesLeft += DiableAll;
	}
	
	void OnDisable ()
	{
		PlayerIntroAnim.OnPlayerIntroAnimComplete -= AppearOnMap;
		LevelManager.OnNoEnemiesLeft -= DiableAll;

	}
	
	void Start ()
	{
		foreach (Transform child in transform)
			child.gameObject.SetActive (false);
			
		fx = GetComponent<AudioSource> ();
	}
	
	private void AppearOnMap ()
	{
		if (!alreadyVisible) {
			alreadyVisible = true;
			
			fx.Play ();
			
			foreach (Transform child in transform) {
				child.gameObject.SetActive (true);
				iTween.FadeFrom (child.gameObject, 0f, 1f);
			}
		}
	}

	private void DiableAll ()
	{
		foreach (Transform child in transform) {
			child.gameObject.SetActive (false);
			iTween.FadeFrom (child.gameObject, 0f, 1f);
		}
	}
}
