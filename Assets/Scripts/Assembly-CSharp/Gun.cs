using UnityEngine;

public class Gun : MonoBehaviour
{
	private Vector3 rotationVel;

	private float speed = 8f;

	private float posSpeed = 0.075f;

	private float posOffset = 0.004f;

	private Vector3 defaultPos;

	private Vector3 posVel;

	private Rigidbody rb;

	private float rotationOffset;

	private float rotationOffsetZ;

	private float rotVelY;

	private float rotVelZ;

	private Vector3 prevRotation;

	private Vector3 desiredBob;

	private float xBob = 0.12f;

	private float yBob = 0.08f;

	private float zBob = 0.1f;

	private float bobSpeed = 0.45f;

	public static Gun Instance { get; set; }

	private void Start()
	{
		Instance = this;
		defaultPos = base.transform.localPosition;
		rb = PlayerMovement.Instance.GetRb();
	}

	private void Update()
	{
		if (!PlayerMovement.Instance || PlayerMovement.Instance.HasGun())
		{
			MoveGun();
			Vector3 grapplePoint = PlayerMovement.Instance.GetGrapplePoint();
			Quaternion b = Quaternion.LookRotation((PlayerMovement.Instance.GetGrapplePoint() - base.transform.position).normalized);
			rotationOffset += Mathf.DeltaAngle(base.transform.parent.rotation.eulerAngles.y, prevRotation.y);
			if (rotationOffset > 90f)
			{
				rotationOffset = 90f;
			}
			if (rotationOffset < -90f)
			{
				rotationOffset = -90f;
			}
			rotationOffset = Mathf.SmoothDamp(rotationOffset, 0f, ref rotVelY, 0.025f);
			Vector3 vector = new Vector3(0f, rotationOffset, rotationOffset);
			if (grapplePoint == Vector3.zero)
			{
				b = Quaternion.Euler(base.transform.parent.rotation.eulerAngles - vector);
			}
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * speed);
			Vector3 vector2 = PlayerMovement.Instance.FindVelRelativeToLook() * posOffset;
			float num = PlayerMovement.Instance.GetFallSpeed() * posOffset;
			if (num < -0.08f)
			{
				num = -0.08f;
			}
			Vector3 vector3 = defaultPos - new Vector3(vector2.x, num, vector2.y);
			base.transform.localPosition = Vector3.SmoothDamp(base.transform.localPosition, vector3 + desiredBob, ref posVel, posSpeed);
			prevRotation = base.transform.parent.rotation.eulerAngles;
		}
	}

	private void MoveGun()
	{
		if ((bool)rb && PlayerMovement.Instance.grounded)
		{
			if (Mathf.Abs(rb.velocity.magnitude) < 4f)
			{
				desiredBob = Vector3.zero;
				return;
			}
			float x = Mathf.PingPong(Time.time * bobSpeed, xBob) - xBob / 2f;
			float y = Mathf.PingPong(Time.time * bobSpeed, yBob) - yBob / 2f;
			float z = Mathf.PingPong(Time.time * bobSpeed, zBob) - zBob / 2f;
			desiredBob = new Vector3(x, y, z);
		}
	}

	public void Shoot()
	{
		float recoil = PlayerMovement.Instance.GetRecoil();
		Vector3 vector = (Vector3.up / 4f + Vector3.back / 1.5f) * recoil;
		base.transform.localPosition = base.transform.localPosition + vector;
		Quaternion localRotation = Quaternion.Euler(-60f * recoil, 0f, 0f);
		base.transform.localRotation = localRotation;
	}
}
