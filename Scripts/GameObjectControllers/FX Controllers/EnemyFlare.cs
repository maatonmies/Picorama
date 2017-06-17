using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyFlare : MonoBehaviour
{
		private Vector3 exitPos;
		private List<Vector3> animPathList;
		private Vector3[] animPathArray;

		public GameObject flareBurst;

		public delegate void FlareReachedExit ();
	
		public static event FlareReachedExit OnFlareReachedExit;

		void Start ()
		{
				exitPos = GameObject.Find ("Exit").transform.position;

				animPathList = new List<Vector3> ();
				Vector3 thisPos = transform.position;

				animPathList.Add (thisPos);

				Vector3 pathVector1 = exitPos + new Vector3 (0, 200);
				Vector3 pathVector2 = exitPos + new Vector3 (-200, 0);
				Vector3 pathVector3 = exitPos + new Vector3 (0, -200);
				Vector3 pathVector4 = exitPos + new Vector3 (200, 0);
				Vector3 pathVector5 = exitPos + new Vector3 (0, 100);
				Vector3 pathVector6 = exitPos + new Vector3 (-100, 0);
				Vector3 pathVector7 = exitPos + new Vector3 (0, -100);
				Vector3 pathVector8 = exitPos + new Vector3 (100, 0);

				Vector3 pathVector9 = thisPos + new Vector3 (0, 200);
				Vector3 pathVector10 = thisPos + new Vector3 (200, 0);
				Vector3 pathVector11 = thisPos + new Vector3 (0, -200);
				Vector3 pathVector12 = thisPos + new Vector3 (-200, 0);
				Vector3 pathVector13 = thisPos + new Vector3 (0, 100);
				Vector3 pathVector14 = thisPos + new Vector3 (100, 0);
				Vector3 pathVector15 = thisPos + new Vector3 (0, -100);
				Vector3 pathVector16 = thisPos + new Vector3 (-100, 0);

				animPathList.Add (pathVector9);
				animPathList.Add (pathVector10);
				animPathList.Add (pathVector11);
				animPathList.Add (pathVector12);
				animPathList.Add (pathVector13);
				animPathList.Add (pathVector14);
				animPathList.Add (pathVector15);
				animPathList.Add (pathVector16);

				animPathList.Add (pathVector1);
				animPathList.Add (pathVector2);
				animPathList.Add (pathVector3);
				animPathList.Add (pathVector4);
				animPathList.Add (pathVector5);
				animPathList.Add (pathVector6);
				animPathList.Add (pathVector7);
				animPathList.Add (pathVector8);

				animPathList.Add (exitPos);

				animPathArray = animPathList.ToArray ();

				Fly ();
		}
	
		private void Fly ()
		{
				iTween.MoveTo (gameObject, iTween.Hash (
			               "position", exitPos,
				           "time", 2,
				           "path", animPathArray,
				           "easeType", iTween.EaseType.easeInOutSine,
					       "oncomplete", "Disappear"));

		}

		private void Disappear ()
		{
				if (OnFlareReachedExit != null)
						OnFlareReachedExit ();

				GameObject flareBurstClone = Instantiate (flareBurst, exitPos, Quaternion.identity) as GameObject;

				Destroy (flareBurstClone, 6);
				GetComponent<ParticleSystem>().enableEmission = false;
				Destroy (gameObject, 6);
		}
}
