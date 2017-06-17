using UnityEngine;
using System.Collections;

public class PlayersEnemySensor : MonoBehaviour
{
	public delegate void EnemyInRange (Transform enemy,Transform player,Transform sensor);
	public static event EnemyInRange OnEnemyInRange;
		
	public delegate void EnemyOutOfRange (Transform enemy,Transform player,Transform sensor);
	public static event EnemyOutOfRange OnEnemyOutOfRange;

	private bool greengo = false;
	private bool orac = false;


	void Start ()
	{
		greengo = this.transform.parent.parent.name == "Greengo";
		orac = this.transform.parent.parent.name == "Orac";

	}
	void OnEnable ()
	{
		InputManager.OnEnemySelected += DisableSensors;
		PlayerMoveAnimator.OnPlayerFinishedMovement += EnableSensors;
		PlayerMoveAnimator.OnPlayerIsMoving += EnableSensors;

	}
	void OnDisable ()
	{		
		InputManager.OnEnemySelected -= DisableSensors;
		PlayerMoveAnimator.OnPlayerFinishedMovement -= EnableSensors;
		PlayerMoveAnimator.OnPlayerIsMoving -= EnableSensors;

	}
		
	void OnTriggerStay2D (Collider2D other)
	{
		if (other.tag == "Enemy") {

			if (greengo) {

				RaycastHit2D[] wall = new RaycastHit2D [1];
				int layerMaskWalls = 1 << 17;        
				int wallHit = Physics2D.LinecastNonAlloc (other.transform.position, transform.parent.parent.position, wall, layerMaskWalls);

				if (wallHit > 0)
					return;

			} else if (orac) {

				if (other.name == "Enemy2" || other.name == "Enemy3") {

					RaycastHit2D[] wall = new RaycastHit2D [1];
					int layerMaskWalls = 1 << 17;        
					int wallHit = Physics2D.LinecastNonAlloc (other.transform.position, transform.parent.parent.position, wall, layerMaskWalls);
				
					if (wallHit > 0)
						return;
				}
				
			}
			
			if (OnEnemyInRange != null) {
				OnEnemyInRange (other.transform, this.transform.parent.parent, this.transform);
			}
		}
	}
		
	void OnTriggerExit2D (Collider2D other)
	{
		if (other.tag == "Enemy") {

			if (OnEnemyOutOfRange != null) {
				OnEnemyOutOfRange (other.transform, this.transform.parent.parent, this.transform);
			}
		}
	}
		
	private void DisableSensors (Transform enemy, Vector3 playerCoord, Transform player)
	{				
		this.GetComponent<Collider2D> ().enabled = false;						
	}
		
	private void EnableSensors (Transform player, bool oracIsDefending)
	{
		if (player == transform.parent.parent)
			this.GetComponent<Collider2D> ().enabled = true;
	}

	private void EnableSensors (Transform player)
	{
		if (player.name == "Blubba")
			this.GetComponent<Collider2D> ().enabled = true;
	}
}
