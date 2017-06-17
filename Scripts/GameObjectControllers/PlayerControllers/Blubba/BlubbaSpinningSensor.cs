using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlubbaSpinningSensor : MonoBehaviour
{
	private float rotationRate = 0.25f;
	private float next = 0;
	private Quaternion rotation;

	void Start ()
	{
		rotation = Quaternion.identity;
	}


	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Enemy") {
			other.BroadcastMessage ("ShowDarkBlueTile");

		}
	}
	void OnTriggerExit2D (Collider2D other)
	{
		if (other.tag == "Enemy") {
			other.BroadcastMessage ("HideDarkBlueTile");
		}
	}

	void FixedUpdate ()
	{
		if (Time.time > next) {
			next = Time.time + rotationRate;
			rotation.eulerAngles += new Vector3 (0, 0, 90);
			transform.localRotation = rotation;
		}

	}
	
}
