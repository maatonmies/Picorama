using UnityEngine;
using System.Collections;

public class BGLight : MonoBehaviour
{
		void Start ()
		{
				iTween.ScaleFrom (gameObject, new Vector3 (0, 0, 0), 4);

				iTween.RotateBy (gameObject, iTween.Hash (
			    "amount", new Vector3 (0, 0, 1),
			    "time", 10,
			    "easetype", iTween.EaseType.linear,
			    "looptype", iTween.LoopType.loop));
		}

}
