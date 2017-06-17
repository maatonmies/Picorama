using UnityEngine;
using System.Collections;

public class FenceBeam : MonoBehaviour
{	
	private AudioSource powerDown;
	private AudioSource powerUp;

	void Start ()
	{
		powerDown = GetComponent<AudioSource> ();
		powerUp = GetComponents<AudioSource> () [1];

	}
	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player") {
			other.gameObject.SendMessage ("ShockedByFence");
		}
	}
	
	private void HideBeam ()
	{
		iTween.FadeTo (gameObject, 0f, 0.5f);
		GetComponent<Collider2D> ().enabled = false;
		powerDown.Play ();

	}

	private void ShowBeam ()
	{
		iTween.FadeTo (gameObject, 1f, 0.5f);
		GetComponent<Collider2D> ().enabled = true;	
		powerUp.Play ();

	}
}
