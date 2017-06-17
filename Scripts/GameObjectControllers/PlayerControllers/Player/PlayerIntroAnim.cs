using UnityEngine;
using System.Collections;

public class PlayerIntroAnim : MonoBehaviour
{
	public delegate void PlayerIntroAnimComplete ()	;	
	public static event PlayerIntroAnimComplete OnPlayerIntroAnimComplete;	
	
	public Transform playerStartPoint;
		
	private Hashtable tweenParams;
	private Hashtable rotateTweenParams;
		
	private bool introAnimisPlaying = false;
		
	private Animator animator;
		
	void OnEnable ()
	{
		PathTileAnim.OnIntroTileAnimComplete += InitIntro;								
	}
	
	void OnDisable ()
	{
		PathTileAnim.OnIntroTileAnimComplete -= InitIntro;								
		
	}
	void Start ()
	{				
		tweenParams = iTween.Hash (
				"position", playerStartPoint.position,
				"time", 1,
				"easetype", iTween.EaseType.easeInOutQuad);	
				
		animator = GetComponent<Animator> ();	
				
		rotateTweenParams = new Hashtable ();
		rotateTweenParams.Add ("amount", new Vector3 (0, 0, 5));
		rotateTweenParams.Add ("time", 1);
		rotateTweenParams.Add ("easetype", iTween.EaseType.linear);
		rotateTweenParams.Add ("oncomplete", "AdjustRotationAndBroadcastIntroAnimComplete");
	}	
		
	private void InitIntro ()
	{
		if (! introAnimisPlaying) {
			float randomDelay = Random.Range (1, 2);
			Invoke ("PlayIntro", randomDelay);
			introAnimisPlaying = true;
		}
	}

	private void PlayIntro ()
	{
		iTween.MoveTo (gameObject, tweenParams);
		iTween.RotateBy (gameObject, rotateTweenParams);
		animator.Play ("ArriveToScene");
	}
		
	private void AdjustRotationAndBroadcastIntroAnimComplete ()
	{
		iTween.RotateTo (gameObject, new Vector3 (0, 0, 0), 0.1f);
				
		if (OnPlayerIntroAnimComplete != null)
			OnPlayerIntroAnimComplete ();
	}
		
}
