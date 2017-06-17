using UnityEngine;
using System.Collections;

public class MoveTargetXController : MonoBehaviour
{		
	private ParticleSystem flashRing;
	private Transform pinkySymbol;
	private Transform blubbaSymbol;
	private Transform greengoSymbol;
	private Transform oracSymbol;

	private Hashtable tweenParamsGrow;
	private Hashtable tweenParamsShrink;
	private Hashtable tweenParamsPulse;
	private AudioSource fx;
				
	void OnEnable ()
	{
		InputManager.OnMoveTargetSelected += ShowFlashRing;
	}
	void OnDisable ()
	{		
		InputManager.OnMoveTargetSelected -= ShowFlashRing;
	}
		
	void Start ()
	{
		flashRing = GetComponent<ParticleSystem> ();
		flashRing.Stop ();

		pinkySymbol = transform.FindChild ("PinkySymbol");
		blubbaSymbol = transform.FindChild ("BlubbaSymbol");
		greengoSymbol = transform.FindChild ("GreengoSymbol");
		oracSymbol = transform.FindChild ("OracSymbol");

		tweenParamsGrow = iTween.Hash (
			"scale", new Vector3 (0.3f, 0.3f, 1),
			"time", 0.2f,
			"easetype", iTween.EaseType.spring,
			"oncompletetarget", gameObject,
			"oncomplete", "Pulse");

		tweenParamsShrink = iTween.Hash (
			"scale", new Vector3 (0, 0, 1),
			"time", 0.2f,
			"easetype", iTween.EaseType.spring);

		tweenParamsPulse = iTween.Hash (
			"scale", new Vector3 (0.2f, 0.2f, 1),
			"time", 0.3f,
			"easetype", iTween.EaseType.easeInOutSine,
			"looptype", iTween.LoopType.pingPong);

		fx = gameObject.GetComponent<AudioSource> ();

	}

	private void ShowFlashRing (Vector3 targetTileCoord, Vector3 targetTilePos, Transform player)
	{		
		transform.position = targetTilePos;
		Vector3 newPos = new Vector3 (targetTilePos.x + 64, targetTilePos.y + 64);
		flashRing.transform.position = newPos;
		flashRing.Play ();

		fx.Play ();

		ShowSymbol (player, newPos);
	}

	private void Pulse (Transform symbol)
	{
		iTween.ScaleTo (symbol.gameObject, tweenParamsPulse);
	}

	private void ShowSymbol (Transform player, Vector3 position)
	{

		if (player.name == "Pinky") {

			pinkySymbol.position = position;

			if (tweenParamsGrow.ContainsKey ("oncompleteparams"))
				tweenParamsGrow.Remove ("oncompleteparams");

			tweenParamsGrow.Add ("oncompleteparams", pinkySymbol);
			iTween.ScaleTo (pinkySymbol.gameObject, tweenParamsGrow);

		} else if (player.name == "Blubba") {

			blubbaSymbol.position = position;

			if (tweenParamsGrow.ContainsKey ("oncompleteparams"))
				tweenParamsGrow.Remove ("oncompleteparams");

			tweenParamsGrow.Add ("oncompleteparams", blubbaSymbol);
			iTween.ScaleTo (blubbaSymbol.gameObject, tweenParamsGrow);

		} else if (player.name == "Greengo") {

			greengoSymbol.position = position;

			if (tweenParamsGrow.ContainsKey ("oncompleteparams"))
				tweenParamsGrow.Remove ("oncompleteparams");

			tweenParamsGrow.Add ("oncompleteparams", greengoSymbol);
			iTween.ScaleTo (greengoSymbol.gameObject, tweenParamsGrow);

		} else if (player.name == "Orac") {

			oracSymbol.position = position;

			if (tweenParamsGrow.ContainsKey ("oncompleteparams"))
				tweenParamsGrow.Remove ("oncompleteparams");

			tweenParamsGrow.Add ("oncompleteparams", oracSymbol);
			iTween.ScaleTo (oracSymbol.gameObject, tweenParamsGrow);
		}
	}

	private void HideSymbol (Transform player)
	{
		if (tweenParamsGrow.ContainsKey ("oncompleteparams"))
			tweenParamsGrow.Remove ("oncompleteparams");

		if (player.name == "Pinky") {

			iTween.ScaleTo (pinkySymbol.gameObject, tweenParamsShrink);

		} else if (player.name == "Blubba") {

			iTween.ScaleTo (blubbaSymbol.gameObject, tweenParamsShrink);
			
		} else if (player.name == "Greengo") {

			iTween.ScaleTo (greengoSymbol.gameObject, tweenParamsShrink);
			
		} else if (player.name == "Orac") {

			iTween.ScaleTo (oracSymbol.gameObject, tweenParamsShrink);
		}
	}

	private void UnreachableMoveTarget (Transform player)
	{
		StartCoroutine (HideSymbolOnUnreachableMoveTarget (player));
	}

	IEnumerator HideSymbolOnUnreachableMoveTarget (Transform player)
	{
		yield return new WaitForSeconds (0.4f);
		HideSymbol (player);
	}

	void OnTriggerEnter2D (Collider2D other)
	{

		if (other.tag == "Player") {

			HideSymbol (other.transform);
		}
	}
}
