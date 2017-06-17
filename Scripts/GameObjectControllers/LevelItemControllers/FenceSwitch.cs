using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FenceSwitch : MonoBehaviour
{
	private AudioSource fx;
	private Collider2D playerOnSwitch;

	void Start ()
	{
		fx = GetComponent<AudioSource> ();
		playerOnSwitch = null;
	}
	
	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player" && playerOnSwitch == null) {
		
			fx.Play ();
			transform.parent.GetChild (0).GetComponent<FenceBeam> ().SendMessage ("HideBeam");
			other.SendMessage ("SteppingOnSwitch", transform.parent);
			playerOnSwitch = other;
		}
	}
	
	void OnTriggerExit2D (Collider2D other)
	{
		if (other == playerOnSwitch) {
			
			fx.Play ();
			transform.parent.GetChild (0).GetComponent<FenceBeam> ().SendMessage ("ShowBeam");
			other.SendMessage ("SteppingOffSwitch", transform.parent);
			playerOnSwitch = null;

		}
	}
}
