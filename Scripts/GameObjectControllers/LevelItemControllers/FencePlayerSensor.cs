using UnityEngine;
using System.Collections;

public class FencePlayerSensor : MonoBehaviour
{
	public delegate void PlayerEnteredFence (Transform player,Transform fence);
	public static event PlayerEnteredFence OnPlayerEnteredFence;
	
	public delegate void PlayerExitedFence (Transform player,Transform fence);
	public static event PlayerExitedFence OnPlayerExitedFence;		
	
	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player") {
			
			if (OnPlayerEnteredFence != null)
				OnPlayerEnteredFence (other.transform, transform.parent);
		}
	}
	
	void OnTriggerExit2D (Collider2D other)
	{
		if (other.tag == "Player") {
			
			if (OnPlayerExitedFence != null)
				OnPlayerExitedFence (other.transform, transform.parent);
		}
	}
}
