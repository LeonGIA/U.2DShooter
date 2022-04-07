using UnityEngine;

namespace Player
{
	public class PlayerArm : MonoBehaviour
	{
		public static float armAngle;
		[SerializeField] private GameObject player;

		void FixedUpdate()
		{
			armAngle = findAngleToMouse();
			rotateArm(armAngle); // rotates arm to follow mouse position
		}

		#region ARM ROTATION
		// Finds angle to mouse position
		private float findAngleToMouse()
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

			mousePos.Normalize();

			float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

			return angle;
		}

		// Applies a rotation to the player's arm based on an angle
		private void rotateArm(float angle)
		{
			transform.rotation = Quaternion.Euler(0f, 0f, angle);

			if(angle < -90f || angle > 90f)
			{
				if(player.transform.eulerAngles.y == 0f)	
				{
					transform.localRotation = Quaternion.Euler(180f, 0f, -angle);
				}
				else if(player.transform.eulerAngles.y == 180f)
				{
					transform.localRotation = Quaternion.Euler(180f, 180f, -angle);
				}
			}
		}
		#endregion
	}
}