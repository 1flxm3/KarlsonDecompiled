using System;
using Audio;
using EZCameraShake;
using UnityEngine;

public class Movement : MonoBehaviour
{
	public GameObject spawnWeapon;

	private float sensitivity = 50f;

	private float sensMultiplier = 1f;

	private bool dead;

	public PhysicMaterial deadMat;

	public Transform playerCam;

	public Transform orientation;

	public Transform gun;

	private float xRotation;

	public Rigidbody rb;

	private float moveSpeed = 4500f;

	private float walkSpeed = 20f;

	private float runSpeed = 10f;

	public bool grounded;

	public Transform groundChecker;

	public LayerMask whatIsGround;

	private bool readyToJump;

	private float jumpCooldown = 0.2f;

	private float jumpForce = 550f;

	private float x;

	private float y;

	private bool jumping;

	private bool sprinting;

	private bool crouching;

	public LineRenderer lr;

	private Vector3 grapplePoint;

	private SpringJoint joint;

	private Vector3 normalVector;

	private Vector3 wallNormalVector;

	private bool wallRunning;

	private Vector3 wallRunPos;

	private DetectWeapons detectWeapons;

	public ParticleSystem ps;

	private ParticleSystem.EmissionModule psEmission;

	private Collider playerCollider;

	public bool paused;

	public LayerMask whatIsGrabbable;

	private Rigidbody objectGrabbing;

	private Vector3 previousLookdir;

	private Vector3 grabPoint;

	private float dragForce = 700000f;

	private SpringJoint grabJoint;

	private LineRenderer grabLr;

	private Vector3 myGrabPoint;

	private Vector3 myHandPoint;

	private Vector3 endPoint;

	private Vector3 grappleVel;

	private float offsetMultiplier;

	private float offsetVel;

	private float distance;

	private float actualWallRotation;

	private float wallRotationVel;

	private float desiredX;

	private float wallRunRotation;

	private bool airborne;

	private bool touchingGround;

	public LayerMask whatIsHittable;

	private float desiredTimeScale = 1f;

	private float timeScaleVel;

	public static Movement Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		rb = GetComponent<Rigidbody>();
		MonoBehaviour.print("rb: " + rb);
	}

	private void Start()
	{
		psEmission = ps.emission;
		playerCollider = GetComponent<Collider>();
		detectWeapons = (DetectWeapons)GetComponentInChildren(typeof(DetectWeapons));
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		readyToJump = true;
		wallNormalVector = Vector3.up;
		CameraShake();
		if (spawnWeapon != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(spawnWeapon, base.transform.position, Quaternion.identity);
			detectWeapons.ForcePickup(gameObject);
		}
		if ((bool)GameState.Instance)
		{
			sensMultiplier = GameState.Instance.GetSensitivity();
		}
	}

	private void LateUpdate()
	{
		if (!dead && !paused)
		{
			DrawGrapple();
			DrawGrabbing();
			WallRunning();
		}
	}

	private void FixedUpdate()
	{
		if (!dead && !Game.Instance.done && !paused)
		{
			Move();
		}
	}

	private void Update()
	{
		MyInput();
		if (!dead && !Game.Instance.done && !paused)
		{
			Look();
			DrawGrabbing();
			UpdateTimescale();
			if (base.transform.position.y < -200f)
			{
				KillPlayer();
			}
		}
	}

	private void MyInput()
	{
		if (dead || Game.Instance.done)
		{
			return;
		}
		x = Input.GetAxisRaw("Horizontal");
		y = Input.GetAxisRaw("Vertical");
		jumping = Input.GetButton("Jump");
		sprinting = Input.GetKey(KeyCode.LeftShift);
		crouching = Input.GetKey(KeyCode.LeftControl);
		if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			StartCrouch();
		}
		if (Input.GetKeyUp(KeyCode.LeftControl))
		{
			StopCrouch();
		}
		if (Input.GetKey(KeyCode.Mouse0))
		{
			if (detectWeapons.HasGun())
			{
				detectWeapons.Shoot(HitPoint());
			}
			else
			{
				GrabObject();
			}
		}
		if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			detectWeapons.StopUse();
			if ((bool)objectGrabbing)
			{
				StopGrab();
			}
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			detectWeapons.Pickup();
		}
		if (Input.GetKeyDown(KeyCode.Q))
		{
			detectWeapons.Throw((HitPoint() - detectWeapons.weaponPos.position).normalized);
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Pause();
		}
	}

	private void Pause()
	{
		if (!dead)
		{
			if (paused)
			{
				Time.timeScale = 1f;
				UIManger.Instance.DeadUI(b: false);
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				paused = false;
			}
			else
			{
				paused = true;
				Time.timeScale = 0f;
				UIManger.Instance.DeadUI(b: true);
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}
	}

	private void UpdateTimescale()
	{
		if (!Game.Instance.done && !paused && !dead)
		{
			Time.timeScale = Mathf.SmoothDamp(Time.timeScale, desiredTimeScale, ref timeScaleVel, 0.15f);
		}
	}

	private void GrabObject()
	{
		if (objectGrabbing == null)
		{
			StartGrab();
		}
		else
		{
			HoldGrab();
		}
	}

	private void DrawGrabbing()
	{
		if ((bool)objectGrabbing)
		{
			myGrabPoint = Vector3.Lerp(myGrabPoint, objectGrabbing.position, Time.deltaTime * 45f);
			myHandPoint = Vector3.Lerp(myHandPoint, grabJoint.connectedAnchor, Time.deltaTime * 45f);
			grabLr.SetPosition(0, myGrabPoint);
			grabLr.SetPosition(1, myHandPoint);
		}
	}

	private void StartGrab()
	{
		RaycastHit[] array = Physics.RaycastAll(playerCam.transform.position, playerCam.transform.forward, 8f, whatIsGrabbable);
		if (array.Length < 1)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			MonoBehaviour.print("testing on: " + array[i].collider.gameObject.layer);
			if ((bool)array[i].transform.GetComponent<Rigidbody>())
			{
				objectGrabbing = array[i].transform.GetComponent<Rigidbody>();
				grabPoint = array[i].point;
				grabJoint = objectGrabbing.gameObject.AddComponent<SpringJoint>();
				grabJoint.autoConfigureConnectedAnchor = false;
				grabJoint.minDistance = 0f;
				grabJoint.maxDistance = 0f;
				grabJoint.damper = 4f;
				grabJoint.spring = 40f;
				grabJoint.massScale = 5f;
				objectGrabbing.angularDrag = 5f;
				objectGrabbing.drag = 1f;
				previousLookdir = playerCam.transform.forward;
				grabLr = objectGrabbing.gameObject.AddComponent<LineRenderer>();
				grabLr.positionCount = 2;
				grabLr.startWidth = 0.05f;
				grabLr.material = new Material(Shader.Find("Sprites/Default"));
				grabLr.numCapVertices = 10;
				grabLr.numCornerVertices = 10;
				break;
			}
		}
	}

	private void HoldGrab()
	{
		grabJoint.connectedAnchor = playerCam.transform.position + playerCam.transform.forward * 5.5f;
		grabLr.startWidth = 0f;
		grabLr.endWidth = 0.0075f * objectGrabbing.velocity.magnitude;
		previousLookdir = playerCam.transform.forward;
	}

	private void StopGrab()
	{
		UnityEngine.Object.Destroy(grabJoint);
		UnityEngine.Object.Destroy(grabLr);
		objectGrabbing.angularDrag = 0.05f;
		objectGrabbing.drag = 0f;
		objectGrabbing = null;
	}

	private void StartCrouch()
	{
		float num = 400f;
		base.transform.localScale = new Vector3(1f, 0.5f, 1f);
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y - 0.5f, base.transform.position.z);
		if (rb.velocity.magnitude > 0.1f && grounded)
		{
			rb.AddForce(orientation.transform.forward * num);
			AudioManager.Instance.Play("StartSlide");
			AudioManager.Instance.Play("Slide");
		}
	}

	private void StopCrouch()
	{
		base.transform.localScale = new Vector3(1f, 1.5f, 1f);
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 0.5f, base.transform.position.z);
	}

	private void StopGrapple()
	{
		UnityEngine.Object.Destroy(joint);
		grapplePoint = Vector3.zero;
	}

	private void StartGrapple()
	{
		RaycastHit[] array = Physics.RaycastAll(playerCam.transform.position, playerCam.transform.forward, 70f, whatIsGround);
		if (array.Length >= 1)
		{
			grapplePoint = array[0].point;
			joint = base.gameObject.AddComponent<SpringJoint>();
			joint.autoConfigureConnectedAnchor = false;
			joint.connectedAnchor = grapplePoint;
			joint.spring = 6.5f;
			joint.damper = 2f;
			joint.maxDistance = Vector2.Distance(grapplePoint, base.transform.position) * 0.8f;
			joint.minDistance = Vector2.Distance(grapplePoint, base.transform.position) * 0.25f;
			joint.spring = 4.5f;
			joint.damper = 7f;
			joint.massScale = 4.5f;
			endPoint = gun.transform.GetChild(0).position;
			offsetMultiplier = 2f;
		}
	}

	private void DrawGrapple()
	{
		if (grapplePoint == Vector3.zero || joint == null)
		{
			lr.positionCount = 0;
			return;
		}
		lr.positionCount = 2;
		endPoint = Vector3.Lerp(endPoint, grapplePoint, Time.deltaTime * 15f);
		offsetMultiplier = Mathf.SmoothDamp(offsetMultiplier, 0f, ref offsetVel, 0.1f);
		int num = 100;
		lr.positionCount = num;
		Vector3 position = gun.transform.GetChild(0).position;
		float num2 = Vector3.Distance(endPoint, position);
		lr.SetPosition(0, position);
		lr.SetPosition(num - 1, endPoint);
		float num3 = num2;
		float num4 = 1f;
		for (int i = 1; i < num - 1; i++)
		{
			float num5 = (float)i / (float)num;
			float num6 = num5 * offsetMultiplier;
			float num7 = (Mathf.Sin(num6 * num3) - 0.5f) * num4 * (num6 * 2f);
			Vector3 normalized = (endPoint - position).normalized;
			float num8 = Mathf.Sin(num5 * 180f * ((float)Math.PI / 180f));
			float num9 = Mathf.Cos(offsetMultiplier * 90f * ((float)Math.PI / 180f));
			Vector3 position2 = position + (endPoint - position) / num * i + ((Vector3)(num9 * num7 * Vector2.Perpendicular(normalized)) + offsetMultiplier * num8 * Vector3.down);
			lr.SetPosition(i, position2);
		}
	}

	private void WallRunning()
	{
		if (wallRunning)
		{
			rb.AddForce(-wallNormalVector * Time.deltaTime * moveSpeed);
		}
	}

	private void FootSteps()
	{
		if (!crouching && !dead && (grounded || wallRunning))
		{
			float num = 1.2f;
			float num2 = rb.velocity.magnitude;
			if (num2 > 20f)
			{
				num2 = 20f;
			}
			distance += num2;
			if (distance > 300f / num)
			{
				AudioManager.Instance.PlayFootStep();
				distance = 0f;
			}
		}
	}

	private void Move()
	{
		if (dead)
		{
			return;
		}
		grounded = Physics.OverlapSphere(groundChecker.position, 0.1f, whatIsGround).Length != 0;
		if (!touchingGround)
		{
			grounded = false;
		}
		Vector2 mag = FindVelRelativeToLook();
		float num = mag.x;
		float num2 = mag.y;
		FootSteps();
		CounterMovement(x, y, mag);
		if (readyToJump && jumping)
		{
			Jump();
		}
		float num3 = walkSpeed;
		if (sprinting)
		{
			num3 = runSpeed;
		}
		if (crouching && grounded && readyToJump)
		{
			rb.AddForce(Vector3.down * Time.deltaTime * 2000f);
			return;
		}
		if (x > 0f && num > num3)
		{
			x = 0f;
		}
		if (x < 0f && num < 0f - num3)
		{
			x = 0f;
		}
		if (y > 0f && num2 > num3)
		{
			y = 0f;
		}
		if (y < 0f && num2 < 0f - num3)
		{
			y = 0f;
		}
		float num4 = 1f;
		float num5 = 1f;
		if (!grounded)
		{
			num4 = 0.5f;
		}
		if (grounded && crouching)
		{
			num5 = 0f;
		}
		rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * num4 * num5);
		rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * num4);
		SpeedLines();
	}

	private void SpeedLines()
	{
		float num = Vector3.Angle(rb.velocity, playerCam.transform.forward) * 0.15f;
		if (num < 1f)
		{
			num = 1f;
		}
		float rateOverTimeMultiplier = rb.velocity.magnitude / num;
		if (grounded && !wallRunning)
		{
			rateOverTimeMultiplier = 0f;
		}
		psEmission.rateOverTimeMultiplier = rateOverTimeMultiplier;
	}

	private void CameraShake()
	{
		float num = rb.velocity.magnitude / 9f;
		CameraShaker.Instance.ShakeOnce(num, 0.1f * num, 0.25f, 0.2f);
		Invoke("CameraShake", 0.2f);
	}

	private void ResetJump()
	{
		readyToJump = true;
		MonoBehaviour.print("reset");
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
			if (wallRunning)
			{
				rb.AddForce(wallNormalVector * jumpForce * 1.5f);
			}
			Invoke("ResetJump", jumpCooldown);
			if (wallRunning)
			{
				wallRunning = false;
			}
			AudioManager.Instance.PlayJump();
		}
	}

	private void Look()
	{
		float num = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
		float num2 = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
		desiredX = playerCam.transform.localRotation.eulerAngles.y + num;
		xRotation -= num2;
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);
		FindWallRunRotation();
		actualWallRotation = Mathf.SmoothDamp(actualWallRotation, wallRunRotation, ref wallRotationVel, 0.2f);
		playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, actualWallRotation);
		orientation.transform.localRotation = Quaternion.Euler(0f, desiredX, 0f);
	}

	private void FindWallRunRotation()
	{
		if (!wallRunning)
		{
			wallRunRotation = 0f;
			return;
		}
		_ = new Vector3(0f, playerCam.transform.rotation.y, 0f).normalized;
		new Vector3(0f, 0f, 1f);
		float num = 0f;
		float current = playerCam.transform.rotation.eulerAngles.y;
		if (Math.Abs(wallNormalVector.x - 1f) < 0.1f)
		{
			num = 90f;
		}
		else if (Math.Abs(wallNormalVector.x - -1f) < 0.1f)
		{
			num = 270f;
		}
		else if (Math.Abs(wallNormalVector.z - 1f) < 0.1f)
		{
			num = 0f;
		}
		else if (Math.Abs(wallNormalVector.z - -1f) < 0.1f)
		{
			num = 180f;
		}
		num = Vector3.SignedAngle(new Vector3(0f, 0f, 1f), wallNormalVector, Vector3.up);
		float num2 = Mathf.DeltaAngle(current, num);
		wallRunRotation = (0f - num2 / 90f) * 15f;
	}

	private void CounterMovement(float x, float y, Vector2 mag)
	{
		if (!grounded)
		{
			return;
		}
		float num = 0.2f;
		if (crouching)
		{
			rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * 0.045f);
			return;
		}
		if (Math.Abs(x) < 0.05f || (mag.x < 0f && x > 0f) || (mag.x > 0f && x < 0f))
		{
			rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * (0f - mag.x) * num);
		}
		if (Math.Abs(y) < 0.05f || (mag.y < 0f && y > 0f) || (mag.y > 0f && y < 0f))
		{
			rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * (0f - mag.y) * num);
		}
		if (Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2f) + Mathf.Pow(rb.velocity.z, 2f)) > 20f)
		{
			float num2 = rb.velocity.y;
			Vector3 vector = rb.velocity.normalized * 20f;
			rb.velocity = new Vector3(vector.x, num2, vector.z);
		}
	}

	public Vector2 FindVelRelativeToLook()
	{
		float current = orientation.transform.eulerAngles.y;
		float target = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * 57.29578f;
		float num = Mathf.DeltaAngle(current, target);
		float num2 = 90f - num;
		float magnitude = rb.velocity.magnitude;
		return new Vector2(y: magnitude * Mathf.Cos(num * ((float)Math.PI / 180f)), x: magnitude * Mathf.Cos(num2 * ((float)Math.PI / 180f)));
	}

	private void OnCollisionEnter(Collision other)
	{
		int layer = other.gameObject.layer;
		if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			CameraShaker.Instance.ShakeOnce(5.5f * GameState.Instance.cameraShake, 1.2f, 0.2f, 0.3f);
			if (wallRunning && other.contacts[0].normal.y == -1f)
			{
				MonoBehaviour.print("ROOF");
				return;
			}
			wallNormalVector = other.contacts[0].normal;
			MonoBehaviour.print("nv: " + wallNormalVector);
			AudioManager.Instance.PlayLanding();
			if (Math.Abs(wallNormalVector.y) < 0.1f)
			{
				StartWallRun();
			}
			airborne = false;
		}
		if (layer != LayerMask.NameToLayer("Enemy") || (grounded && !crouching) || rb.velocity.magnitude < 3f)
		{
			return;
		}
		Enemy enemy = (Enemy)other.transform.root.GetComponent(typeof(Enemy));
		if ((bool)enemy && !enemy.IsDead())
		{
			UnityEngine.Object.Instantiate(PrefabManager.Instance.enemyHitAudio, other.contacts[0].point, Quaternion.identity);
			RagdollController ragdollController = (RagdollController)other.transform.root.GetComponent(typeof(RagdollController));
			if (grounded && crouching)
			{
				ragdollController.MakeRagdoll(rb.velocity * 1.2f * 34f);
			}
			else
			{
				ragdollController.MakeRagdoll(rb.velocity.normalized * 250f);
			}
			rb.AddForce(rb.velocity.normalized * 2f, ForceMode.Impulse);
			enemy.DropGun(rb.velocity.normalized * 2f);
		}
	}

	private void StartWallRun()
	{
		if (wallRunning)
		{
			MonoBehaviour.print("stopping since wallrunning");
			return;
		}
		if (touchingGround)
		{
			MonoBehaviour.print("stopping since grounded");
			return;
		}
		MonoBehaviour.print("got through");
		float num = 20f;
		wallRunning = true;
		rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
		rb.AddForce(Vector3.up * num, ForceMode.Impulse);
	}

	private void OnCollisionExit(Collision other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			if (Math.Abs(wallNormalVector.y) < 0.1f)
			{
				MonoBehaviour.print("oof");
				wallRunning = false;
				wallNormalVector = Vector3.up;
			}
			else
			{
				touchingGround = false;
			}
			airborne = true;
		}
		if (other.gameObject.layer == LayerMask.NameToLayer("Object"))
		{
			touchingGround = false;
		}
	}

	private void OnCollisionStay(Collision other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Ground") && Math.Abs(other.contacts[0].normal.y) > 0.1f)
		{
			touchingGround = true;
			wallRunning = false;
		}
		if (other.gameObject.layer == LayerMask.NameToLayer("Object"))
		{
			touchingGround = true;
		}
	}

	public Vector3 GetVelocity()
	{
		return rb.velocity;
	}

	public float GetFallSpeed()
	{
		return rb.velocity.y;
	}

	public Vector3 GetGrapplePoint()
	{
		return detectWeapons.GetGrapplerPoint();
	}

	public Collider GetPlayerCollider()
	{
		return playerCollider;
	}

	public Transform GetPlayerCamTransform()
	{
		return playerCam.transform;
	}

	public Vector3 HitPoint()
	{
		RaycastHit[] array = Physics.RaycastAll(playerCam.transform.position, playerCam.transform.forward, (int)whatIsHittable);
		if (array.Length < 1)
		{
			return playerCam.transform.position + playerCam.transform.forward * 100f;
		}
		if (array.Length > 1)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
				{
					return array[i].point;
				}
			}
		}
		return array[0].point;
	}

	public float GetRecoil()
	{
		return detectWeapons.GetRecoil();
	}

	public void KillPlayer()
	{
		if (!Game.Instance.done)
		{
			CameraShaker.Instance.ShakeOnce(3f * GameState.Instance.cameraShake, 2f, 0.1f, 0.6f);
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			UIManger.Instance.DeadUI(b: true);
			Timer.Instance.Stop();
			dead = true;
			rb.freezeRotation = false;
			playerCollider.material = deadMat;
			detectWeapons.Throw(Vector3.zero);
			paused = false;
			ResetSlowmo();
		}
	}

	public void Respawn()
	{
		detectWeapons.StopUse();
	}

	public void Slowmo(float timescale, float length)
	{
		if (GameState.Instance.shake)
		{
			CancelInvoke("Slowmo");
			desiredTimeScale = timescale;
			Invoke("ResetSlowmo", length);
			AudioManager.Instance.Play("SlowmoStart");
		}
	}

	private void ResetSlowmo()
	{
		desiredTimeScale = 1f;
		AudioManager.Instance.Play("SlowmoEnd");
	}

	public bool IsCrouching()
	{
		return crouching;
	}

	public bool HasGun()
	{
		return detectWeapons.HasGun();
	}

	public bool IsDead()
	{
		return dead;
	}

	public Rigidbody GetRb()
	{
		return rb;
	}
}
