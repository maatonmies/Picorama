using UnityEngine;
using System.Collections;

public class ExitController : MonoBehaviour
{
	public float vortexRotateSpeed = 100;
	public float vortexPulseSpeed = 100;
	public float vortexPulseAmount = 1.2f;

	public Transform exitBeam;
	public Transform vortex;
				
	public delegate void ExitIsReady (Vector3 exitPos);
	public static event ExitIsReady OnExitIsReady;

	public delegate void ExitReached ();
	public static event ExitReached OnExitReached;
		
	private Hashtable rotateTweenParams;
	private Hashtable scaleTweenParams;
	private Vector3 originalScale;
	private Behaviour halo;
	private ParticleSystem pulseWaves;
	
	private bool finalCharge = false;
	private bool exitIsReady = false;

	private AudioSource vortexChime;
	private AudioSource vortexChimeShort;

	private AudioSource vortexSpin;
	private AudioSource victory;

	private GradientBackground background;

	void OnEnable ()
	{
		LevelManager.OnNoEnemiesLeft += GetReadyForFinalCharge;
		EnemyFlare.OnFlareReachedExit += Charge;
		InputManager.OnExitSelected += DenyAccess;
		CameraScriptedAnimations.OnCameraOutroAnimComplete += FadeOutVortexSound;

	}
	
	void OnDisable ()
	{
		LevelManager.OnNoEnemiesLeft -= GetReadyForFinalCharge;
		EnemyFlare.OnFlareReachedExit -= Charge;
		InputManager.OnExitSelected -= DenyAccess;
		CameraScriptedAnimations.OnCameraOutroAnimComplete -= FadeOutVortexSound;

	}
		
	void Start ()
	{
		pulseWaves = transform.FindChild ("ExitPulseWaves").GetComponent<ParticleSystem> ();
		pulseWaves.enableEmission = false;
						
		originalScale = transform.localScale;
		transform.localScale = new Vector3 (0, 0, 0);

		halo = (Behaviour)GetComponent ("Halo");
		halo.enabled = false;
				
		rotateTweenParams = new Hashtable ();
		rotateTweenParams.Add ("name", "exitRot");
		rotateTweenParams.Add ("amount", new Vector3 (0, 0, 360));
		rotateTweenParams.Add ("speed", vortexRotateSpeed);
		rotateTweenParams.Add ("easetype", iTween.EaseType.linear);
		rotateTweenParams.Add ("looptype", iTween.LoopType.loop);
		
		scaleTweenParams = new Hashtable ();
		scaleTweenParams.Add ("name", "exitPulse");
		scaleTweenParams.Add ("amount", new Vector3 (vortexPulseAmount, vortexPulseAmount, 0));
		scaleTweenParams.Add ("speed", vortexPulseSpeed);
		scaleTweenParams.Add ("easetype", iTween.EaseType.easeInOutSine);
		scaleTweenParams.Add ("looptype", iTween.LoopType.pingPong);

		vortexChime = GetComponents<AudioSource> () [0];
		vortexSpin = GetComponents<AudioSource> () [1];
		vortexChimeShort = GetComponents<AudioSource> () [2];
		victory = GetComponents<AudioSource> () [3];

		background = GameObject.Find ("Background/BgCamera").GetComponent<GradientBackground> ();

				
		IntroSpin ();
	}

	private void IntroSpin ()
	{
		iTween.RotateBy (gameObject, new Vector3 (0, 0, 15), 5);
		
		iTween.ScaleTo (gameObject, iTween.Hash (
			"scale", originalScale, 
			"time", 3,
			"easetype", iTween.EaseType.linear,
			"oncomplete", "NormalSpin"));
	}

	private void NormalSpin ()
	{
		vortexChimeShort.Play ();
		iTween.RotateAdd (gameObject, rotateTweenParams);
		iTween.ScaleBy (gameObject, scaleTweenParams);
	}

	private void DenyAccess (Transform player)
	{
		if (!exitIsReady) {
			player.SendMessage ("Nono", player);
			HaloOn ();

			if (! vortexChimeShort.isPlaying)
				vortexChimeShort.Play ();
		}

	}

	private void Charge ()
	{
		if (! finalCharge) {

			background.SendMessage ("FlashToLightBg");

			iTween.RotateBy (gameObject, iTween.Hash (
				"amount", new Vector3 (0, 0, 3),
				"time", 1,
				"easetype", iTween.EaseType.easeInOutQuint));
			
			HaloOn ();
			vortexSpin.mute = false;
			vortexChimeShort.Play ();
			vortexSpin.Play ();
			Invoke ("MuteVortexSpin", 1.1f);
			
		} else {
			
			Instantiate (exitBeam, transform.position, Quaternion.identity);
			background.SendMessage ("ChangeToLightBg");
			halo.enabled = true;
			pulseWaves.enableEmission = true;
			
			iTween.RotateBy (gameObject, iTween.Hash (
				"amount", new Vector3 (0, 0, 50),
				"time", 2,
				"easetype", iTween.EaseType.easeInQuint,
				"oncomplete", "FastSpin"));
			
			HaloOn ();
			vortexSpin.mute = false;
			vortexChime.Play ();
			vortexChimeShort.Play ();
			vortexSpin.Play ();
		}
	}

	private void MuteVortexSpin ()
	{
		vortexSpin.mute = true;
	}

	private void HaloOn ()
	{
		halo.enabled = true;
		Invoke ("HaloOff", 0.3f);
	}
	
	private void HaloOff ()
	{
		if (! finalCharge) {
			halo.enabled = false;
		}
	}
	
	private void FastSpin ()
	{	
		if (OnExitIsReady != null)
			OnExitIsReady (transform.position);

		rotateTweenParams.Remove ("speed");
		rotateTweenParams.Add ("speed", vortexRotateSpeed * 2);
		iTween.RotateAdd (gameObject, rotateTweenParams);

		iTween.ScaleBy (gameObject, scaleTweenParams);

		exitIsReady = true;
		gameObject.GetComponent<SpriteRenderer> ().enabled = false;
	}

	private void GetReadyForFinalCharge ()
	{
		finalCharge = true;
		BoxCollider2D boxCollider = (BoxCollider2D)GetComponent<Collider2D> ();
		boxCollider.size = new Vector2 (40, 40);
		victory.Play ();
	}
	
	void OnTriggerEnter2D (Collider2D other)
	{
		if (exitIsReady) {
			if (other.tag == "Player") {
						

					
				if (other.name == "Greengo" || other.name == "Orac") {
					StartCoroutine (FadeOut (other.gameObject.GetComponents<AudioSource> () [3]));
				}


				GetComponent<Collider2D> ().enabled = false;
				pulseWaves.enableEmission = false;

				if (OnExitReached != null) 
					OnExitReached ();

				Instantiate (vortex, transform.position, Quaternion.identity);
				StartCoroutine (PitchUp (vortexChime));
				iTween.ScaleTo (gameObject, iTween.Hash (
								"scale", Vector3.zero,
								"time", 4,
								"easetype", iTween.EaseType.easeInOutSine));

			}
		}
	}

	private IEnumerator PitchUp (AudioSource sound)
	{
		for (int i= 0; i < 30; i ++) {
			
			sound.pitch += 0.02f;
			sound.volume -= 0.03f;
			yield return new WaitForSeconds (0.1f);
			
		}
		sound.Stop ();
	}
	
	private IEnumerator FadeOut (AudioSource sound)
	{
		for (int i= 10; i > 0; i --) {
			
			sound.volume -= 0.15f;
			yield return new WaitForSeconds (0.5f);
			
		}
	}

	private void FadeOutVortexSound ()
	{
		FadeOut (vortexChime);
	}
}
