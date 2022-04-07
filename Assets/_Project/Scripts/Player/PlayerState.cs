using UnityEngine;

namespace Player
{
	public class PlayerState : MonoBehaviour
	{
		// State management
		public enum playerStates {idle, running, jumping, falling}
		public static playerStates state;

		// Physics reference
		private Rigidbody2D playerBody;

		// Animation Component
		private Animator playerAnimator;

		void Awake()
		{
			playerBody = GetComponent<Rigidbody2D>();
			playerAnimator = GetComponent<Animator>();
		}

		void Update()
		{
			updatePlayerState();
		}

		private void updatePlayerState()
		{
			if(Mathf.Approximately(playerBody.velocity.x, 0f) && Mathf.Approximately(playerBody.velocity.y, 0f))
			{
				state = playerStates.idle;
			}

			if((playerBody.velocity.x > .1f || playerBody.velocity.x < -.1f) && Mathf.Approximately(playerBody.velocity.y, 0f))
			{        
				state = playerStates.running;
			}
			
			if(playerBody.velocity.y > .1f)
			{
				state = playerStates.jumping;
			}
			
			if(playerBody.velocity.y < -.1f)
			{
				state = playerStates.falling;
			}

			playerAnimator.SetInteger("state", (int)state);
		}
	}
}
