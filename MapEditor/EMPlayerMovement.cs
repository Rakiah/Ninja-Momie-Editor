using UnityEngine;
using System.Collections;

public class EMPlayerMovement : MonoBehaviour 
{

	public float speed = 20.0f;
	public float JumpSpeed = 500.0f;
	public float gravity = 10;

	private float h;
	private bool grounded;
	private Vector3 moveDirection;

	CharacterController controller;

	void Start () 
	{
		controller = this.GetComponent<CharacterController>();
	}

	void FixedUpdate ()
	{
		h = Input.GetAxis("Horizontal");

		if (h > 0.0f) this.transform.localEulerAngles = new Vector3(0, 90, 0);
		else if (h < 0.0f) this.transform.localEulerAngles = new Vector3(0, -90, 0);
	
		moveDirection.x = h * speed;
		
		if (grounded)
		{
			if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow))
			{
				moveDirection.y = JumpSpeed;
			}
		}
		else moveDirection.y -= gravity * Time.deltaTime;

		grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
	}
}
