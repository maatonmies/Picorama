using UnityEngine;
using System.Collections;

public class ShockHalo : MonoBehaviour
{		
		private Behaviour halo;
		private bool orac = false;
		private AudioSource fx;
		
		void Start ()
		{
				halo = (Behaviour)transform.GetChild (1).GetComponent ("Halo");
				orac = this.name == "Orac";

				fx = transform.FindChild ("ShockHalo").GetComponent<AudioSource> ();
		}
		
		void InitFlash ()
		{
				fx.Play ();

				StartCoroutine (Flash ());
		}
		
		IEnumerator Flash ()
		{		
				if (orac) {

						for (int i = 0; i < 8; i++) {

								halo.enabled = !halo.enabled;
								yield return new WaitForSeconds (0.1f);
						}
	

				} else {
						for (int i = 0; i < 4; i++) {
				
								halo.enabled = !halo.enabled;
								yield return new WaitForSeconds (0.1f);
						}
				}
		}
}
