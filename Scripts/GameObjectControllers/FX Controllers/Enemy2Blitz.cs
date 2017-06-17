using UnityEngine;
using System.Collections;

public class Enemy2Blitz : MonoBehaviour
{
		private Hashtable tweenParamsMoveUp;
		private Hashtable tweenParamsMoveDown;
		private Hashtable tweenParamsMoveLeft;
		private Hashtable tweenParamsMoveRight;
		
		private tk2dTileMap tileMap;
		private Vector3 parentCoords;
		
		private Animator animator;
		
		private Vector3 blitzDirection;
		private Vector3 leftDirection;
		private Vector3 rightDirection;
		private Vector3 upDirection;
		private Vector3 downDirection;
		
		private Quaternion blitzRotation;
		private Quaternion leftRotation;
		private Quaternion rightRotation;
		private Quaternion upRotation;
		private Quaternion downRotation;

		private bool oracBlock = false;
		private AudioSource laserFx;

		void OnEnable ()
		{
				EnemyCounterAttack.OnEnemyStartCounterAttack += AssignBlitzStartPosAndRotation;
		}
		void OnDisable ()
		{
				EnemyCounterAttack.OnEnemyStartCounterAttack -= AssignBlitzStartPosAndRotation;
		
		}

		void Start ()
		{
				tileMap = GameObject.Find ("TileMap").GetComponent<tk2dTileMap> ();
				
				int CoordX, CoordY;
				tileMap.GetTileAtPosition (transform.parent.position, out CoordX, out CoordY);
				
				parentCoords = new Vector3 (CoordX, CoordY);
				
				animator = GetComponent<Animator> ();
				
				blitzDirection = Vector3.zero;
				blitzRotation = Quaternion.identity;
				
				leftDirection = new Vector3 (-250, 14, 0);
				rightDirection = new Vector3 (-250, 14, 0);
				upDirection = new Vector3 (2, 95, 0);
				downDirection = new Vector3 (10, -45, 0);
				
				leftRotation = Quaternion.Euler (0, 0, -70);
				rightRotation = Quaternion.Euler (0, 0, 290);
				upRotation = Quaternion.Euler (0, 0, 180);
				downRotation = Quaternion.Euler (0, 0, 0);

				laserFx = gameObject.GetComponent<AudioSource> ();
		}
		
		
		private void StartBlitzAnim ()
		{
				transform.localPosition = blitzDirection;
				transform.localRotation = blitzRotation;

				if (oracBlock) {

						if (blitzDirection == upDirection) {
				
								animator.Play ("BlitzBlockedUp");		
				
						} else if (blitzDirection == downDirection) {

								GetComponent<Renderer>().sortingOrder = 0;

								animator.Play ("BlitzBlockedDown");	

						} else
								animator.Play ("BlitzBlocked");		

				} else {
						if (blitzDirection == upDirection || blitzDirection == downDirection) {

								animator.Play ("BlitzVertical");		
				
						} else {
								animator.Play ("Blitz");		
						}
				}
		}
	
		private void AssignBlitzStartPosAndRotation (Vector3 playerCoord, Transform player, Transform enemy, Vector3 enemyPos, bool blockedByOrac)
		{
				if (enemy != transform.parent)
						return;

				oracBlock = blockedByOrac;
						
				Vector3 playerCoords = playerCoord;
				
				if (playerCoords.x > parentCoords.x) {
				
						blitzDirection = rightDirection;
						blitzRotation = rightRotation;
						
				} else if (playerCoords.x < parentCoords.x) {
				
						blitzDirection = leftDirection;
						blitzRotation = leftRotation;
						
				} else if (playerCoords.y < parentCoords.y) {
				
						blitzDirection = downDirection;
						blitzRotation = downRotation;
						
				} else if (playerCoords.y > parentCoords.y) {
				
						blitzDirection = upDirection;
						blitzRotation = upRotation;
				}
		}

		private void ResetSortingOrder ()
		{
				GetComponent<Renderer>().sortingOrder = 10;
		}

		private void PlayLaserFx ()
		{
				laserFx.Play ();
		}
}
