using UnityEngine;

namespace Player
{
	public class PlayerMovement : MonoBehaviour
	{
		// PLAYER COMPONENTS
		private Rigidbody2D playerBody;
		private CapsuleCollider2D playerCollider;

		// JUMP SETUP
		private float coyoteTimer; // used for jump forgiveness after leaving the ground
		private float jumpBufferTimer; // used for queueing a jump

		// INPUT PARAMS (A/D and >/<)
		private float horizontalMovement;

		// MOVEMENT GATES
		private bool isFacingRight = true; // used to detect facing direction
		private bool isJumping = false; // used to move jumping to FixedUpdate()
		private bool queuedJump = false; // used for queueing a jump

		void Awake()
		{		
			playerBody = GetComponent<Rigidbody2D>();
			playerCollider = GetComponent<CapsuleCollider2D>();
		}

		void Start()
		{
			setGravityScale(PlayerData.GRAVITY); // sets default gravity
		}

		void Update()
		{
			horizontalMovement = Input.GetAxisRaw("Horizontal");

			checkFacingDirection(Mathf.Abs(PlayerArm.armAngle) < 90f); // checks facing direction based on mouse rotation

			checkForJump(); // checks keys for jump while also handling coyote time and jump queueing

			if (playerBody.velocity.y >= 0)
			{
				setGravityScale(PlayerData.GRAVITY); // sets default gravity when jumping
			}
			else
			{
				setGravityScale(PlayerData.GRAVITY * PlayerData.FALL_GRAVITY_MULT); // sets higher gravity when falling
			}
		}

		void FixedUpdate()
		{
			// checks for jump
			if(isJumping)
			{
				jumpPlayer();
			}

			movePlayer();

			// Applies different drag depending on the situation
			if(PlayerState.state ==  PlayerState.playerStates.jumping || PlayerState.state ==  PlayerState.playerStates.falling)
			{
				Drag(PlayerData.AIR_DRAG);
			}
			else
			{
				Drag(PlayerData.GROUND_DRAG);
			}
		}

		private void movePlayer()
		{
			float targetSpeed = horizontalMovement * PlayerData.MOVE_SPEED; // can be 0, -MOVE_SPEED, or +MOVE_SPEED

			// takes the difference to find out how much force should be applied. Larger differences mean larger forces (such as when the player wants to turn)
			float speedDiff = targetSpeed - playerBody.velocity.x;

			// How fast the player should accelerate. Equivalent to how we accelerate when we start to run.
			float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? PlayerData.RUN_ACCEL : PlayerData.RUN_DECELL;

			// determines the curve at which the player speeds up. Used to make movements feel more natural.
			float velocityPow;
			if (Mathf.Abs(targetSpeed) < 0.01f)
			{
				velocityPow = PlayerData.STOP_POWER;
			}
			else if (Mathf.Abs(playerBody.velocity.x) > 0 && (Mathf.Sign(targetSpeed) != Mathf.Sign(playerBody.velocity.x)))
			{
				velocityPow = PlayerData.TURN_POWER;
			}
			else
			{
				velocityPow = PlayerData.ACCEL_POWER;
			}

			// force to be applied
			float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velocityPow) * Mathf.Sign(speedDiff);

			playerBody.AddForce(movement * Vector2.right);
		}

		private void checkForJump()
		{
			bool grounded = isGrounded(); // ground check
			jumpBufferTimer -= Time.deltaTime;

			if(grounded)
			{
				playerBody.sharedMaterial.friction = 0.4f;
			}
			else
			{
				playerBody.sharedMaterial.friction = 0f;
			}

			// Coyote time controller
			if(grounded)
			{
				coyoteTimer = PlayerData.COYOTE_TIME;
			}
			else if(coyoteTimer > -1f)
			{
				coyoteTimer -= Time.deltaTime;
			}

			#region QUEUED JUMPING
			if(Input.GetKeyDown(KeyCode.Space) && !queuedJump && (PlayerState.state ==  PlayerState.playerStates.falling || PlayerState.state ==  PlayerState.playerStates.jumping))
			{
				queuedJump = true;
				jumpBufferTimer = PlayerData.JUMP_QUEUE;
			} 
			else if(jumpBufferTimer > 0f)
			{
				jumpBufferTimer -= Time.deltaTime;
			}

			// used to separate queued jumping from regular jump. If a jump is queued and conditions are met,
			// a jump should automatically be executed the next time the player is grounded.
			if(queuedJump)
			{
				if(grounded && jumpBufferTimer > 0f)
				{
					isJumping = true;
					// Debug.Log("GROUNDED, DO A JUMP");
				}
			}
			#endregion

			if(Input.GetKeyUp(KeyCode.Space) && (grounded || coyoteTimer > 0f))
			{
				isJumping = true;
			}
		}

		private void jumpPlayer()
		{
			playerBody.AddForce(new Vector2(0f, PlayerData.JUMP_FORCE), ForceMode2D.Impulse);

			// reset values
			isJumping = false;
			queuedJump = false;
			// chargeTimer = 0f;
			coyoteTimer = 0f;
		}

		private void checkFacingDirection(bool isMovingRight)
		{
			if(isMovingRight != isFacingRight)
			{
				Turn();
			}
		}

		private void Turn()
		{
			// Negative scale implementation of turning
			Vector3 scale = transform.localScale;
			scale.x *= -1;
			transform.localScale = scale;

			isFacingRight = !isFacingRight;

			// Euler Angle implementation of turning
			// if(isFacingRight)
			// {
			// 	transform.rotation = Quaternion.Euler(transform.rotation.x, 0f, transform.rotation.z);
			// }
			// else
			// {
			// 	transform.rotation = Quaternion.Euler(transform.rotation.x, 180f, transform.rotation.z);
			// }
		}
		
		private void setGravityScale(float scale)
		{
			playerBody.gravityScale = scale;
		}

		// determines direction to apply drag in and applies it.
		private void Drag(float amount)
		{
			Vector2 force = amount * playerBody.velocity.normalized;
			force.x = Mathf.Min(Mathf.Abs(playerBody.velocity.x), Mathf.Abs(force.x)); 
			force.y = Mathf.Min(Mathf.Abs(playerBody.velocity.y), Mathf.Abs(force.y));
			force.x *= Mathf.Sign(playerBody.velocity.x);
			force.y *= Mathf.Sign(playerBody.velocity.y);

			playerBody.AddForce(-force, ForceMode2D.Impulse);
		}

		// Raycast grounding test for both of the player's "feet"
		private bool isGrounded()
		{
			GameObject isLeftGrounded = collisionDetector(playerCollider.bounds.min, Vector2.down, PlayerData.GROUND_BUFFER);
			GameObject isRightGrounded = collisionDetector(new Vector2(playerCollider.bounds.max.x, playerCollider.bounds.min.y), Vector2.down, PlayerData.GROUND_BUFFER);

			return(isLeftGrounded || isRightGrounded);
		}

		// Raycast function, returns the gameObject belonging to the collider the raycast hits.
		private GameObject collisionDetector(Vector2 origin, Vector2 direction, float distance){
			RaycastHit2D rayCast = Physics2D.Raycast(origin, direction, distance);
			GameObject objectHit = rayCast.collider?.gameObject;

			return objectHit;
		}
	}
}
