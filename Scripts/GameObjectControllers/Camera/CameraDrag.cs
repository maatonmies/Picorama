
using UnityEngine;
using System.Collections;

public class CameraDrag : MonoBehaviour
{		
	public bool dragEnabled;

	public float cameraMaxLeft = 640;
	
	public float cameraMaxRight = 640;
	
	public float cameraMaxUp = 940;
	
	public float cameraMaxDown = 503;

	public float scrollSpeed = 100;
	public float dragSpeed = 200;

	private Transform thisTransform;

	private Vector2 scrollVelocity;
	private bool inGame = false;

	void OnEnable ()
	{
		LevelManager.OnStartLevel += InGame;
		LevelManager.OnPlayOutroAnimations += InGame;
		EnemyCounterAttack.OnPlayerGetReadyToDie += DisableDrag;
		InputManager.OnRetrySelected += EnableDrag;
	}

	void OnDisable ()
	{
		LevelManager.OnStartLevel -= InGame;
		LevelManager.OnPlayOutroAnimations -= InGame;
		EnemyCounterAttack.OnPlayerGetReadyToDie -= DisableDrag;
		InputManager.OnRetrySelected -= EnableDrag;

	}

	private void EnableDrag (Transform deadPlayer)
	{
		dragEnabled = true;
	}
	private void DisableDrag (Transform player, Transform enemy)
	{
		dragEnabled = false;
	}

	private void InGame (Transform startingPlayer)
	{
		inGame = !inGame;
	}

	private void InGame ()
	{
		inGame = !inGame;
	}

	void Start ()
	{
		thisTransform = transform;
		scrollVelocity = Vector2.zero;
	}

	private void Drag (Vector2 dragAmount)
	{
		if (dragEnabled) {
			scrollVelocity = dragAmount;
			Vector2 force = Vector2.ClampMagnitude (dragAmount, 100);
			GetComponent<Rigidbody2D> ().AddForce (force * dragSpeed);
		}
	}

	private void Scroll ()
	{
		if (dragEnabled) {
			GetComponent<Rigidbody2D> ().AddForce (scrollVelocity * scrollSpeed);
		}
	}

	void Update ()
	{

		if (inGame && dragEnabled) {

			Vector3 newTransformPos = thisTransform.position;

			if (thisTransform.position.x < cameraMaxLeft) {
			
				newTransformPos.x = cameraMaxLeft;
			
			} else if (thisTransform.position.x > cameraMaxRight) {
			
				newTransformPos.x = cameraMaxRight;
			} 
		
			if (thisTransform.position.y < cameraMaxDown) {
			
				newTransformPos.y = cameraMaxDown;
			
			} else if (transform.position.y > cameraMaxUp) {
			
				newTransformPos.y = cameraMaxUp;
			} 

			thisTransform.position = newTransformPos;
		}
	}
}
