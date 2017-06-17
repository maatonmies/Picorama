using UnityEngine;
using System.Collections;

public class PathTileAnim : MonoBehaviour
{

	public delegate void IntroTileAnimComplete ();
	public static event IntroTileAnimComplete OnIntroTileAnimComplete;

	private tk2dTileMap tileMap;
	private Hashtable turnTweenParams;
	private Hashtable moveTweenParams;

	private Vector2 thisCoord;
			
	void OnEnable ()
	{
		CameraScriptedAnimations.OnCameraIntroTurnComplete += InitAnim;
	}

	void OnDisable ()
	{
		CameraScriptedAnimations.OnCameraIntroTurnComplete -= InitAnim;
	}

	void Start ()
	{		
		tileMap = GameObject.Find ("TileMap").GetComponent<tk2dTileMap> ();
		tileMap.PrefabsRoot = null;

		int x, y;
		tileMap.GetTileAtPosition (transform.position, out x, out y);
		thisCoord = new Vector2 ((float)x, (float)y);
				
		GetComponent<Renderer> ().enabled = false;
		int random = (int)Random.Range (10, 20);
		float randomX = Random.Range (-1000, 1000);
		float randomY = Random.Range (-1000, 1000);

		turnTweenParams = iTween.Hash (
			    "amount", new Vector3 (random, random, random),
			    "time", 0.8f,
			    "eastype", iTween.EaseType.linear,
			    "oncomplete", "AdjustRotation");

		moveTweenParams = iTween.Hash (
			    "position", new Vector3 (randomX, randomY, 0),
			    "time", 0.8f,
				"eastype", iTween.EaseType.spring);
	}
	
	private void InitAnim ()
	{
		float multiplier = (thisCoord.x + thisCoord.y) + thisCoord.x;
		float delay = 0.1f * multiplier;
		Invoke ("TurnTile", delay);
	}	
		
	private void TurnTile ()
	{				
		GetComponent<Renderer> ().enabled = true;

		iTween.RotateBy (gameObject, turnTweenParams);

		iTween.MoveFrom (gameObject, moveTweenParams);	
				
	}
		
	private void AdjustRotation ()
	{
		iTween.RotateTo (gameObject, new Vector3 (0, 0, 0), 1.5f);
		if (OnIntroTileAnimComplete != null)
			OnIntroTileAnimComplete ();
	}	
}
