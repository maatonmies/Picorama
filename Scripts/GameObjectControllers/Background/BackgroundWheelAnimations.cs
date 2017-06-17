
//BACKGROUND WHEEL ANIMATIONS

//BEHAVIOUR CLASS FOR ORNAMENTAL WHEEL OBJECTS IN THE BACKGROUND 
// * PLAYS AND INTRO ANIMATION AT THE START OF EACH LEVEL
// * PLAYS ANIMATION OF THE WHEELS DURING GAMEPLAY
// * PLAYS AN OUTRO ANIMATION AT THE END OF EACH LEVEL

using UnityEngine;
using System.Collections;

public class BackgroundWheelAnimations : MonoBehaviour
{
		public float rotationSpeed = 100;	
		public float startMoveTime = 2;	
		public float endMoveTime = 5;	
	
		public Transform startPos;
	
		void OnEnable ()
		{
				LevelManager.OnPlayOutroAnimations += MoveOut;
		}
	
		void OnDisable ()
		{
				LevelManager.OnPlayOutroAnimations -= MoveOut;
		}
		
		void Start ()
		{				
				iTween.MoveFrom (gameObject, iTween.Hash (
						
						"position", startPos.position, 
						
						"time", startMoveTime,
			       "easetype", iTween.EaseType.easeInOutSine));
						
						
						
				int randomValue = Random.Range (0, 2); 
		
				int randomDir = (randomValue > 0) ? 1 : -1; 
		
				iTween.RotateBy (gameObject, iTween.Hash (
			
			"amount", new Vector3 (0, 0, randomDir),
			
			"speed", rotationSpeed,
			
			"looptype", iTween.LoopType.loop,
			
			"easetype", iTween.EaseType.linear));
		}		
		
		private void MoveOut ()
		{		
				iTween.MoveTo (gameObject, iTween.Hash (
			
			"position", startPos, 
			
			"time", endMoveTime,
		               
		    "easetype", iTween.EaseType.easeInOutSine));
		}
}