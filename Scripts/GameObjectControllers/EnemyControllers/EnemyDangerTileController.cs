using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDangerTileController : MonoBehaviour
{		
	private SpriteRenderer dangerTilePink;
	private SpriteRenderer dangerTileBlue;
	private SpriteRenderer dangerTileDarkBlue;

	private SpriteRenderer dangerTileGreen;
	private SpriteRenderer dangerTileOrange;

	private List<Transform>sensorsTouching;
	private bool senseOrac = false;
	private AudioSource click;
	private bool showDarkBlueTiles = false;

	void OnEnable ()
	{
		PlayersEnemySensor.OnEnemyInRange += BeInRange; 
		PlayersEnemySensor.OnEnemyOutOfRange += BeOutOfRange;
		InputManager.OnPlayerSelected += HideDangerZonesForInactivePlayer;
		InputManager.OnEnemySelected += HideAllTilesOnEnemySelected;
		PlayerDeath.OnPlayerIsDead += HideAllTilesOnPlayerIsDead;
	}

	void OnDisable ()
	{
		PlayersEnemySensor.OnEnemyInRange -= BeInRange; 
		PlayersEnemySensor.OnEnemyOutOfRange -= BeOutOfRange;
		InputManager.OnPlayerSelected -= HideDangerZonesForInactivePlayer;
		InputManager.OnEnemySelected -= HideAllTilesOnEnemySelected;
		PlayerDeath.OnPlayerIsDead -= HideAllTilesOnPlayerIsDead;
	}
		
	void Start ()
	{				
		dangerTilePink = transform.Find ("DangerTilePink").GetComponent<SpriteRenderer> ();
		dangerTileBlue = transform.Find ("DangerTileBlue").GetComponent<SpriteRenderer> ();
		dangerTileDarkBlue = transform.Find ("DangerTileDarkBlue").GetComponent<SpriteRenderer> ();
		dangerTileGreen = transform.Find ("DangerTileGreen").GetComponent<SpriteRenderer> ();
		sensorsTouching = new List<Transform> ();
		click = GetComponent<AudioSource> ();

		if (transform.parent.name == "Enemy2" || transform.parent.name == "Enemy3")
			senseOrac = true;

		if (senseOrac)
			dangerTileOrange = transform.Find ("DangerTileOrange").GetComponent<SpriteRenderer> ();

		HideAllTiles ();
	}

	private void BeInRange (Transform enemy, Transform player, Transform sensor)
	{
		if (enemy == transform.parent && ! sensorsTouching.Contains (sensor)) {
										
			sensorsTouching.Add (sensor);

			click.pitch = Random.Range (0.8f, 1.5f);
			click.panStereo = Random.Range (-0.7f, 0.7f);
						
			if (player.name == "Pinky") {		
				ShowPinkTile ();
				HideBlueTile ();
				HideDarkBlueTile ();
				HideGreenTile ();
				if (senseOrac)
					HideOrangeTile ();
				click.Play ();

			} else if (player.name == "Blubba") {

				if (player.GetComponent<BlubbaController> ().isAlreadyAttacking)
					return;
				ShowBlueTile ();
				HidePinkTile ();
				HideGreenTile ();
				if (senseOrac)
					HideOrangeTile ();
				click.Play ();
				showDarkBlueTiles = true;

			} else if (player.name == "Greengo") {
				ShowGreenTile ();
				HideBlueTile ();
				HideDarkBlueTile ();

				HidePinkTile ();
				if (senseOrac)
					HideOrangeTile ();
				click.Play ();

			} else if (senseOrac && player.name == "Orac") {

				ShowOrangeTile ();
				HidePinkTile ();
				HideBlueTile ();
				HideDarkBlueTile ();

				HideGreenTile ();
				click.Play ();
			}
		}					
	}
	
	private void ShowPinkTile ()
	{
		dangerTilePink.enabled = true;
	}
		
	private void ShowBlueTile ()
	{
		dangerTileBlue.enabled = true;
	}

	private void ShowDarkBlueTile () //called by Blubbas sensor
	{
		if (showDarkBlueTiles)
			dangerTileDarkBlue.enabled = true;
	}
		
	private void ShowGreenTile ()
	{
		dangerTileGreen.enabled = true;
	}

	private void ShowOrangeTile ()
	{
		dangerTileOrange.enabled = true;
	}
		
	private void BeOutOfRange (Transform enemy, Transform player, Transform sensor)
	{
		if (enemy == transform.parent && sensorsTouching.Contains (sensor)) {

			sensorsTouching.Remove (sensor);

			if (sensorsTouching.Count < 1) {

				if (player.name == "Pinky") {			
					HidePinkTile ();

				} else if (player.name == "Blubba") {
					HideBlueTile ();
					HideDarkBlueTile ();

				} else if (player.name == "Greengo") {
					HideGreenTile ();

				} else if (senseOrac && player.name == "Orac") {
					HideOrangeTile ();
				}
			}
		}	
	}

	private void HidePinkTile ()
	{
		if (dangerTilePink != null)

			dangerTilePink.enabled = false;			
	}
	private void HideBlueTile ()
	{
		if (dangerTileBlue != null)
			dangerTileBlue.enabled = false;

		showDarkBlueTiles = false;
	}

	private void HideDarkBlueTile ()
	{
		dangerTileDarkBlue.enabled = false;
	}
	
	private void HideGreenTile ()
	{
		if (dangerTileGreen != null)

			dangerTileGreen.enabled = false;			
	}

	private void HideOrangeTile ()
	{
		if (dangerTileOrange != null)
			
			dangerTileOrange.enabled = false;			
	}
		
	private void HideDangerZonesForInactivePlayer (Transform player)
	{
		sensorsTouching.Clear ();	

		if (player.name == "Pinky") {
			HideBlueTile ();
			HideDarkBlueTile ();
			HideGreenTile ();
			if (senseOrac)
				HideOrangeTile ();
		} else if (player.name == "Blubba") {
			HidePinkTile ();
			HideGreenTile ();
			if (senseOrac)
				HideOrangeTile ();

		} else if (player.name == "Greengo") {
			HidePinkTile ();
			HideBlueTile ();
			HideDarkBlueTile ();
			if (senseOrac)
				HideOrangeTile ();
		} else if (player.name == "Orac") {

			HidePinkTile ();
			HideBlueTile ();
			HideDarkBlueTile ();
			HideGreenTile ();
		}
	}
		
	private void HideAllTiles ()
	{
		sensorsTouching.Clear ();	

		if (senseOrac)
			HideOrangeTile ();

		HideBlueTile ();
		HideGreenTile ();
		HidePinkTile ();
		HideDarkBlueTile ();
	}

	private void HideAllTilesOnPlayerIsDead (Transform player)
	{
		HideAllTiles ();
	}

	private void HideAllTilesOnEnemySelected (Transform enemy, Vector3 playerCoord, Transform player)
	{
		HideAllTiles ();	
	}
}