
using UnityEngine;
using System.Collections;

[RequireComponent (typeof(MatrixBlender))]

public class CameraScriptedAnimations : MonoBehaviour
{		
	//public float introTurnAnimationTime = 1f;
		
	public float introZoomDelay = 2.5f;
	public float pinchZoomAnimTime = 0.3f;
	public float introZoomAnimationTime = 1f;
	public float panAnimationTime = 3f;
	public float moveAnimationTime = 0.8f;
	public float outroAnimationTime = 3f;
	public Vector3 startPos;
	public Transform exit;
	public float fieldOfView = 41f;
	public float fieldOfViewFar = 41f;
	public float near = .3f;
	public float far = 1000f;
	public float orthographicSizeNear = 300f;
	public float orthographicSizeFar = 500f;
	public float orthographicSizeVeryFar = 800f;
	public bool focusOnExitOnStart = false;
	public bool focusOnEnemyOnStart = false;
	public bool focusOnPlayerOnStart = false;
	public Transform focusedEnemy;
	public Transform focusedPlayer;
	
			
	public delegate void CameraIntroTurnComplete ();

	public static event CameraIntroTurnComplete OnCameraIntroTurnComplete;

	public delegate void CameraIntroZoomComplete ();

	public static event CameraIntroZoomComplete OnCameraIntroZoomComplete;
		
	public delegate void CameraIntroPanComplete ();

	public static event CameraIntroPanComplete OnCameraIntroPanComplete;
		
	public delegate void CameraOutroAnimComplete ();

	public static event CameraOutroAnimComplete OnCameraOutroAnimComplete;
		
	private MatrixBlender blender;
	private Vector3 exitPosition;
	private Matrix4x4 orthoNear;
	private Matrix4x4 orthoFar;
	private Matrix4x4 orthoVeryFar;
	private Matrix4x4 perspective;
	private Matrix4x4 originalMat;
	private CameraDrag cameraDrag;
	private float maxRight;
	private float maxLeft;
	private float maxUp;
	private float maxDown;
	private float aspect;
	private bool zoomedIn = true;
	private bool zoomedOut = false;
	private bool zoomedOutFull = false;
	private AudioSource zoomInFx;
	private AudioSource zoomOutFx;
	private AudioSource panFx;
	private AudioSource bellFx;
	private AudioSource introSound;

	void OnEnable ()
	{
		LevelManager.OnNoEnemiesLeft += PanToExitOnNoEnemiesLeft;
		LevelManager.OnPlayOutroAnimations += CameraOutroAnim;
		
	}
	
	void OnDisable ()
	{
		LevelManager.OnNoEnemiesLeft -= PanToExitOnNoEnemiesLeft;

		LevelManager.OnPlayOutroAnimations -= CameraOutroAnim;

	}
		
	void Start ()
	{

		originalMat = GetComponent<Camera> ().projectionMatrix;
		cameraDrag = GetComponent<CameraDrag> ();

		maxRight = cameraDrag.cameraMaxRight;
		maxLeft = cameraDrag.cameraMaxLeft;
		maxUp = cameraDrag.cameraMaxUp;
		maxDown = cameraDrag.cameraMaxDown;

		zoomInFx = GetComponents<AudioSource> () [0];
		zoomOutFx = GetComponents<AudioSource> () [1];
		panFx = GetComponents<AudioSource> () [2];
		bellFx = GetComponents<AudioSource> () [3];
		introSound = GetComponents<AudioSource> () [4];

		aspect = (float)Screen.width / (float)Screen.height;
				
		orthoNear = Matrix4x4.Ortho (-orthographicSizeNear * aspect, orthographicSizeNear * aspect, -orthographicSizeNear, orthographicSizeNear, near, far);
		orthoFar = Matrix4x4.Ortho (-orthographicSizeFar * aspect, orthographicSizeFar * aspect, -orthographicSizeFar, orthographicSizeFar, near, far);
		orthoVeryFar = Matrix4x4.Ortho (-orthographicSizeVeryFar * aspect, orthographicSizeVeryFar * aspect, -orthographicSizeVeryFar, orthographicSizeVeryFar, near, far);
		
		perspective = Matrix4x4.Perspective (fieldOfView, aspect, near, far);

		blender = (MatrixBlender)GetComponent (typeof(MatrixBlender));
								
		exitPosition = exit.position;						
				
		blender.BlendToMatrix (perspective, 0.01f);
		
		Invoke ("InitIntroZoom", 0.5f);
	}
		
	private void InitIntroZoom ()
	{						
		BroadcastCameraIntroTurnComplete ();
		Invoke ("IntroZoom", introZoomDelay);
		introSound.PlayDelayed (0.5f);
	}
		
	private void IntroZoom ()
	{
		if (focusOnExitOnStart) {

			float targetX = exit.position.x;
			float targetY = exit.position.y;
			
			if (targetX > maxRight)
				targetX = maxRight;
			else if (targetX < maxLeft)
				targetX = maxLeft;
			
			if (targetY > maxUp)
				targetY = maxUp;
			else if (targetY < maxDown)
				targetY = maxDown;
			
			Vector3 targetPos = new Vector3 (targetX, targetY, -800);

			iTween.MoveTo (gameObject, iTween.Hash (
				"position", targetPos,
				"time", introZoomAnimationTime,
				"easetype", iTween.EaseType.spring,
				"oncomplete", "IntroPan"));

		} else if (focusOnEnemyOnStart) {

			float targetX = focusedEnemy.position.x;
			float targetY = focusedEnemy.position.y;
			
			if (targetX > maxRight)
				targetX = maxRight;
			else if (targetX < maxLeft)
				targetX = maxLeft;
			
			if (targetY > maxUp)
				targetY = maxUp;
			else if (targetY < maxDown)
				targetY = maxDown;
			
			Vector3 targetPos = new Vector3 (targetX, targetY, -800);

			iTween.MoveTo (gameObject, iTween.Hash (
				"position", targetPos,
				"time", introZoomAnimationTime,
				"easetype", iTween.EaseType.spring,
				"oncomplete", "IntroPan"));
			
		} else if (focusOnPlayerOnStart) {

			float targetX = focusedPlayer.position.x;
			float targetY = focusedPlayer.position.y;
			
			if (targetX > maxRight)
				targetX = maxRight;
			else if (targetX < maxLeft)
				targetX = maxLeft;
			
			if (targetY > maxUp)
				targetY = maxUp;
			else if (targetY < maxDown)
				targetY = maxDown;
			
			Vector3 targetPos = new Vector3 (targetX, targetY, -800);

			iTween.MoveTo (gameObject, iTween.Hash (
				"position", targetPos,
				"time", introZoomAnimationTime,
				"easetype", iTween.EaseType.spring,
				"oncomplete", "IntroPan"));
			
		} else {
			iTween.MoveTo (gameObject, iTween.Hash (
				"position", new Vector3 (transform.position.x, transform.position.y, -800),
				"time", introZoomAnimationTime,
				"easetype", iTween.EaseType.spring,
				"oncomplete", "IntroPan"));
		}

		if (introSound.isPlaying && Application.loadedLevel < 30)
			StartCoroutine (FadeOut (introSound));

		zoomInFx.Play ();
	}
		
	private void IntroPan ()
	{
		BroadcastCameraIntroZoomComplete ();

		blender.BlendToMatrix (orthoNear, 0.1f);

		iTween.MoveTo (gameObject, iTween.Hash (
			"position", startPos,
			"time", panAnimationTime,
			"easetype", iTween.EaseType.easeInOutQuint,
			"oncomplete", "BroadcastCameraIntroPanComplete"));

		if (panAnimationTime > 0)
			panFx.Play ();
	}
	
	private void BroadcastCameraIntroTurnComplete ()
	{
		if (OnCameraIntroTurnComplete != null)
			OnCameraIntroTurnComplete ();
	}

	private void BroadcastCameraIntroZoomComplete ()
	{
		if (OnCameraIntroZoomComplete != null)
			OnCameraIntroZoomComplete ();
	}
		
	private void BroadcastCameraIntroPanComplete ()
	{
		if (OnCameraIntroPanComplete != null)
			OnCameraIntroPanComplete ();
		bellFx.Play ();
	}

	private void FocusCameraOnPlayer (Transform player)
	{
		float targetX = player.position.x;
		float targetY = player.position.y;

		if (targetX > maxRight)
			targetX = maxRight;
		else if (targetX < maxLeft)
			targetX = maxLeft;

		if (targetY > maxUp)
			targetY = maxUp;
		else if (targetY < maxDown)
			targetY = maxDown;

		Vector3 targetPos = new Vector3 (targetX, targetY, transform.position.z);
		
		if (targetPos != transform.position && zoomedIn) {
			
			iTween.MoveTo (gameObject, iTween.Hash (
				"position", targetPos,
				"time", moveAnimationTime,
				"easetype", iTween.EaseType.easeInOutSine));
			
		}
	}

	private void FocusCameraOnRetryButton (Transform retryButton)
	{
		float targetX = retryButton.position.x;
		float targetY = retryButton.position.y;
		
		if (targetX > maxRight)
			targetX = maxRight;
		else if (targetX < maxLeft)
			targetX = maxLeft;
		
		if (targetY > maxUp)
			targetY = maxUp;
		else if (targetY < maxDown)
			targetY = maxDown;
		
		Vector3 targetPos = new Vector3 (targetX, targetY, transform.position.z);
		
		if (targetPos != transform.position) {
			
			iTween.MoveTo (gameObject, iTween.Hash (
				"position", targetPos,
				"time", 0.6f,
				"easetype", iTween.EaseType.easeInOutSine));
			
		}
	}

	private void CameraOutroAnim ()
	{
		blender.BlendToMatrix (originalMat, 600);

		iTween.MoveTo (gameObject, iTween.Hash (
			"position", exitPosition,
			"time", outroAnimationTime,
			"easetype", iTween.EaseType.easeInSine,
			"oncomplete", "BroadcastCameraOutroAnimComplete"));
	}
		
	private void BroadcastCameraOutroAnimComplete ()
	{		
		if (OnCameraOutroAnimComplete != null)
			OnCameraOutroAnimComplete ();
	}
		
	private void ZoomOut ()
	{
		if (zoomedIn) {
			zoomOutFx.Play ();
			blender.BlendToMatrix (orthoFar, pinchZoomAnimTime);

			zoomedIn = false;
			zoomedOut = true;

		} else if (zoomedOut) {

			zoomOutFx.Play ();
			blender.BlendToMatrix (orthoVeryFar, pinchZoomAnimTime);
			zoomedOut = false;
			zoomedOutFull = true;

		}
			
	}
		
	private void ZoomIn ()
	{
		if (zoomedOutFull) {
			zoomInFx.Play ();
			blender.BlendToMatrix (orthoFar, pinchZoomAnimTime);
				
			zoomedOutFull = false;
			zoomedOut = true;

		} else if (zoomedOut) {

			zoomInFx.Play ();
			blender.BlendToMatrix (orthoNear, pinchZoomAnimTime);
			
			zoomedOut = false;
			zoomedIn = true;
		}
			
	}

	private void PanToExitOnNoEnemiesLeft ()
	{
		float targetX = exitPosition.x;
		float targetY = exitPosition.y;
		
		if (targetX > maxRight)
			targetX = maxRight;
		else if (targetX < maxLeft)
			targetX = maxLeft;
		
		if (targetY > maxUp)
			targetY = maxUp;
		else if (targetY < maxDown)
			targetY = maxDown;

		Vector3 targetPos = new Vector3 (targetX, targetY, transform.position.z);

		cameraDrag.dragEnabled = false;

		iTween.MoveTo (gameObject, iTween.Hash (
			"position", targetPos,
			"time", 3,
			"easetype", iTween.EaseType.easeInOutSine,
			"oncomplete", "ReenableDrag"));

		if (introSound.isPlaying)
			introSound.Stop ();
	}

	private void ReenableDrag ()
	{
		cameraDrag.dragEnabled = true;
	}

	private IEnumerator FadeOut (AudioSource sound)
	{
		for (int i= 10; i > 0; i --) {
			
			sound.volume -= 0.15f;
			yield return new WaitForSeconds (0.001f);
			
		}
		sound.Stop ();
	}
}
