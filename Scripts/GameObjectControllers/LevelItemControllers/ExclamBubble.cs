using UnityEngine;
using System.Collections;

public class ExclamBubble : MonoBehaviour
{
		public float appearTime = 2;
		public Transform targetPlayer;

		void OnEnable ()
		{
				PlayerIntroAnim.OnPlayerIntroAnimComplete += Init;
				InputManager.OnPlayerSelected += KillSelf;
		}
		void OnDisable ()
		{
				InputManager.OnPlayerSelected -= KillSelf;
				PlayerIntroAnim.OnPlayerIntroAnimComplete -= Init;
		
		}
		
		void Start ()
		{
				iTween.FadeTo (gameObject, 0, 0.1f);
		}
		
		private void Init ()
		{
				StartCoroutine (Appear ());
		}
		
		IEnumerator Appear ()
		{
				yield return new WaitForSeconds (appearTime);
				
				iTween.FadeTo (gameObject, 1, 0.2f);
				iTween.MoveTo (gameObject, iTween.Hash (
				"position", this.transform.position + new Vector3 (0, 20, 0),
				"time", 0.4f,
				"easetype", iTween.EaseType.easeOutSine,
				"looptype", iTween.LoopType.pingPong));
		}
		
		private void KillSelf (Transform player)
		{
				if (player == targetPlayer)
						Destroy (gameObject);
		}
		
	
}
