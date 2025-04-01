using System;
using UnityEngine;

public abstract class Actor : MonoBehaviour
{
	public Transform gunPosition;

	public Transform orientation;

	private float xRotation;

	private Rigidbody rb;

	private float accelerationSpeed = 4500f;

	private float maxSpeed = 20f;

	private bool crouching;

	private bool jumping;

	private bool wallRunning;

	protected float x;

	protected float y;

	private Vector3 wallNormalVector = Vector3.up;

	private bool grounded;

	public Transform groundChecker;

	public LayerMask whatIsGround;

	private bool readyToJump;

	private float jumpCooldown = 0.2f;

	private float jumpForce = 500f;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		OnStart();
	}

	private void FixedUpdate()
	{
		Movement();
		RotateBody();
	}

	private void LateUpdate()
	{
		Look();
	}

	private void Update()
	{
		Logic();
	}

	protected abstract void OnStart();

	protected abstract void Logic();

	protected abstract void RotateBody();

	protected abstract void Look();

	private void Movement()
	{
		grounded = Physics.OverlapSphere(groundChecker.position, 0.1f, whatIsGround).Length != 0;
		Vector2 mag = FindVelRelativeToLook();
		float num = mag.x;
		float num2 = mag.y;
		CounterMovement(x, y, mag);
		if (readyToJump && jumping)
		{
			Jump();
		}
		if (crouching && grounded && readyToJump)
		{
			rb.AddForce(Vector3.down * Time.deltaTime * 2000f);
			return;
		}
		if (x > 0f && num > maxSpeed)
		{
			x = 0f;
		}
		if (x < 0f && num < 0f - maxSpeed)
		{
			x = 0f;
		}
		if (y > 0f && num2 > maxSpeed)
		{
			y = 0f;
		}
		if (y < 0f && num2 < 0f - maxSpeed)
		{
			y = 0f;
		}
		rb.AddForce(Time.deltaTime * y * accelerationSpeed * orientation.transform.forward);
		rb.AddForce(Time.deltaTime * x * accelerationSpeed * orientation.transform.right);
	}

	private void Jump()
	{
		if (grounded || wallRunning)
		{
			Vector3 velocity = rb.velocity;
			rb.velocity = new Vector3(velocity.x, 0f, velocity.z);
			readyToJump = false;
			rb.AddForce(Vector2.up * jumpForce * 1.5f);
			rb.AddForce(wallNormalVector * jumpForce * 0.5f);
			Invoke("ResetJump", jumpCooldown);
			if (wallRunning)
			{
				wallRunning = false;
			}
		}
	}

	private void ResetJump()
	{
		readyToJump = true;
	}

	protected void CounterMovement(float x, float y, Vector2 mag)
	{
		if (grounded && !crouching)
		{
			float num = 0.2f;
			if (x == 0f || (mag.x < 0f && x > 0f) || (mag.x > 0f && x < 0f))
			{
				rb.AddForce(accelerationSpeed * num * Time.deltaTime * (0f - mag.x) * orientation.transform.right);
			}
			if (y == 0f || (mag.y < 0f && y > 0f) || (mag.y > 0f && y < 0f))
			{
				rb.AddForce(accelerationSpeed * num * Time.deltaTime * (0f - mag.y) * orientation.transform.forward);
			}
			if (Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2f) + Mathf.Pow(rb.velocity.z, 2f)) > 20f)
			{
				float num2 = rb.velocity.y;
				Vector3 vector = rb.velocity.normalized * 20f;
				rb.velocity = new Vector3(vector.x, num2, vector.z);
			}
		}
	}

	protected Vector2 FindVelRelativeToLook()
	{
		float current = orientation.transform.eulerAngles.y;
		Vector3 velocity = rb.velocity;
		float target = Mathf.Atan2(velocity.x, velocity.z) * 57.29578f;
		float num = Mathf.DeltaAngle(current, target);
		float num2 = 90f - num;
		float magnitude = rb.velocity.magnitude;
		return new Vector2(y: magnitude * Mathf.Cos(num * ((float)Math.PI / 180f)), x: magnitude * Mathf.Cos(num2 * ((float)Math.PI / 180f)));
	}
}
