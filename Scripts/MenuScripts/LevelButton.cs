using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
	public int levelId;

	private int buttonState = 0;

	void Start ()
	{
		buttonState = GameManager.manager.GetLevelButtonInfo (levelId);

		if (buttonState == 1) {

			GetComponent<Image> ().enabled = true;
			GetComponent<Button> ().enabled = true;
			transform.GetChild (1).GetComponent<Image> ().enabled = false;

		} else if (buttonState == 2) {

			GetComponent<Image> ().enabled = true;
			GetComponent<Button> ().enabled = true;
			transform.GetChild (0).GetComponent<Image> ().enabled = true;
			transform.GetChild (1).GetComponent<Image> ().enabled = false;
		}
	}
	

}
