using UnityEngine;
using System.Collections;

public class EMCameraMovement : MonoBehaviour 
{

	float InputZ;
	float InputX;
	float InputY;

	public float AcceleratorFactor;
	public float Speed;
	[System.NonSerialized]  public Vector3 MoveDirection;


	[System.NonSerialized]  public bool Locked;

	[System.NonSerialized]  public bool playing;

	[System.NonSerialized] public GameObject playertoFollow;

	public float offsetY = 20.0F;
	public float distanceZ = 100.0f;

	void Update () 
	{
		if (!playing)
		{
			if (!Locked)
			{
				if(!Mathf.Approximately(Input.GetAxis("Horizontal"),0.0f) || !Mathf.Approximately(Input.GetAxis("Vertical"),0.0f))
					AcceleratorFactor += Time.deltaTime;
				else
					AcceleratorFactor = 1.0f;
				InputZ = Input.GetAxis("Mouse ScrollWheel");
				InputX = Input.GetAxis("Horizontal");
				InputY = Input.GetAxis("Vertical");


				this.camera.orthographicSize -= InputZ * 500 * Time.deltaTime;
				this.camera.fieldOfView -= InputZ * 500 * Time.deltaTime;
				MoveDirection.y += InputY * Speed * AcceleratorFactor * Time.deltaTime;
				MoveDirection.x += InputX * Speed * AcceleratorFactor * Time.deltaTime;
				MoveDirection.z = -100;

				if (this.camera.orthographicSize > 60)
				{
					this.camera.orthographicSize = 60;
					this.camera.fieldOfView = 60;
				}
				if (this.camera.orthographicSize < 10)
				{
					this.camera.orthographicSize = 10;
					this.camera.fieldOfView = 10;
				}
				transform.position = MoveDirection;
			}
			else
			{
				InputX = 0;
				InputY = 0;
			}
		}

		else
		{
			this.transform.position = new Vector3(playertoFollow.transform.position.x,playertoFollow.transform.position.y + offsetY,-distanceZ);
			this.camera.orthographicSize = 30;
			this.camera.fieldOfView = 30;
		}
	}
}
