using UnityEngine;
using System.Collections;

public class CameraMoveTrigger : MonoBehaviour
{
		public delegate void CameraMoveToTarget (Vector3 target);
		public static event CameraMoveToTarget OnCameraMoveToTarget;

		public Vector3 target;

		void OnEnable ()
		{
				LevelManager.OnStartLevel += EnableCollider;
		}

		void OnDisable ()
		{
				LevelManager.OnStartLevel -= EnableCollider;
		
		}

		void Start ()
		{
				GetComponent<Collider2D>().enabled = false;
		}

		private void EnableCollider (Transform startingPlayer)
		{
				GetComponent<Collider2D>().enabled = true;
		}
	
		void OnTriggerEnter2D (Collider2D other)
		{				
				if (other.tag == "Player") {

						other.gameObject.SendMessage ("RegisterLastCameraPosForPlayer", target);
								
						if (OnCameraMoveToTarget != null)
								OnCameraMoveToTarget (target);	
				}
		}
}
