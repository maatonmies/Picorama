using UnityEngine;
using System.Collections;

public class ParticleSystemSortingOrder : MonoBehaviour
{

		public int sortingOrderNumber = 9;
		
		void Start ()
		{
				GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = "Actors";
				GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = sortingOrderNumber;
		}
}
