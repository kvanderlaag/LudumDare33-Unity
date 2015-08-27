using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class Controller2D : MonoBehaviour {

	const float skinWidth = 0.01f;
	
	public int horizontalRayCount;
	public int verticalRayCount;

	public LayerMask collisionMask;

	float horizontalRaySpacing;
	float verticalRaySpacing;


	BoxCollider2D col;

	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;

		public void reset() {
			above = below = false;
			left = right = false;
		}
	}

	public CollisionInfo collisions;
	
	struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	RaycastOrigins raycastOrigins;

	public void UpdateRaycastOrigins() {
		Bounds bounds = col.bounds;
		bounds.Expand (skinWidth);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}

	public void CalculateRaySpacing() {
		Bounds bounds = col.bounds;
		bounds.Expand (skinWidth * -2);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

		// Use this for initialization
	void Start () {
		col = GetComponent<BoxCollider2D> ();
		CalculateRaySpacing ();
	}

	public void Move(Vector3 velocity) {
		UpdateRaycastOrigins ();
		collisions.reset ();
		if (velocity.y != 0)
			VerticalCollisions (ref velocity);
		if (velocity.x != 0)
			HorizontalCollisions (ref velocity);

		transform.Translate (velocity);
	}

	void VerticalCollisions(ref Vector3 velocity) {
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;
		
		for (int i = 0; i < verticalRayCount; i ++) {
			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
			
			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength,Color.red);
			
			if (hit) {
				velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;
				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}
	}

	void HorizontalCollisions(ref Vector3 velocity) {
		float directionX = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;
		
		for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
			
			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength,Color.red);
			
			if (hit) {
				velocity.x = (hit.distance - skinWidth) * directionX;
				rayLength = hit.distance;
				collisions.left = directionX == -1;
				collisions.right = directionX == 1;
			}
		}
	}


	// Update is called once per frame
	void Update () {

	}

}
