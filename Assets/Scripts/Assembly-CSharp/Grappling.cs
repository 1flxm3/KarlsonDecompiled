using System;
using UnityEngine;

public class Grappling : MonoBehaviour
{
	private LineRenderer lr;

	public LayerMask whatIsSickoMode;

	private Transform connectedTransform;

	private SpringJoint joint;

	private Vector3 offsetPoint;

	private Vector3 endPoint;

	private Vector3 ropeVel;

	private Vector3 desiredPos;

	private float offsetMultiplier;

	private float offsetVel;

	private bool readyToUse = true;

	public static Grappling Instance { get; set; }

	private void Start()
	{
		Instance = this;
		lr = GetComponentInChildren<LineRenderer>();
		lr.positionCount = 0;
	}

	private void Update()
	{
		DrawLine();
		if (!(connectedTransform == null))
		{
			Vector2 vector = (connectedTransform.position - base.transform.position).normalized;
			Mathf.Atan2(vector.y, vector.x);
			_ = joint == null;
		}
	}

	private void DrawLine()
	{
		if (connectedTransform == null || joint == null)
		{
			ClearLine();
			return;
		}
		desiredPos = connectedTransform.position + offsetPoint;
		endPoint = Vector3.SmoothDamp(endPoint, desiredPos, ref ropeVel, 0.03f);
		offsetMultiplier = Mathf.SmoothDamp(offsetMultiplier, 0f, ref offsetVel, 0.12f);
		int num = 100;
		lr.positionCount = num;
		Vector3 position = base.transform.position;
		lr.SetPosition(0, position);
		lr.SetPosition(num - 1, endPoint);
		float num2 = 15f;
		float num3 = 0.5f;
		for (int i = 1; i < num - 1; i++)
		{
			float num4 = (float)i / (float)num;
			float num5 = num4 * offsetMultiplier;
			float num6 = (Mathf.Sin(num5 * num2) - 0.5f) * num3 * (num5 * 2f);
			Vector3 normalized = (endPoint - position).normalized;
			float num7 = Mathf.Sin(num4 * 180f * ((float)Math.PI / 180f));
			float num8 = Mathf.Cos(offsetMultiplier * 90f * ((float)Math.PI / 180f));
			Vector3 position2 = position + (endPoint - position) / num * i + (Vector3)(num8 * num6 * Vector2.Perpendicular(normalized) + offsetMultiplier * num7 * Vector2.down);
			lr.SetPosition(i, position2);
		}
	}

	private void ClearLine()
	{
		lr.positionCount = 0;
	}

	public void Use(Vector3 attackDirection)
	{
		if (readyToUse)
		{
			ShootRope(attackDirection);
			readyToUse = false;
		}
	}

	public void StopUse()
	{
		if (!(joint == null))
		{
			MonoBehaviour.print("STOPPING");
			connectedTransform = null;
			readyToUse = true;
		}
	}

	private void ShootRope(Vector3 dir)
	{
		RaycastHit[] array = Physics.RaycastAll(base.transform.position, dir, 10f, whatIsSickoMode);
		GameObject gameObject = null;
		RaycastHit raycastHit = default(RaycastHit);
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit2 = array2[i];
			gameObject = raycastHit2.collider.gameObject;
			if (gameObject.layer == LayerMask.NameToLayer("Player"))
			{
				gameObject = null;
				continue;
			}
			raycastHit = raycastHit2;
			break;
		}
		if (!(gameObject == null) && !(raycastHit.collider == null))
		{
			connectedTransform = raycastHit.collider.transform;
			joint = base.gameObject.AddComponent<SpringJoint>();
			Rigidbody component = gameObject.GetComponent<Rigidbody>();
			offsetPoint = raycastHit.point - raycastHit.collider.transform.position;
			joint.connectedBody = gameObject.GetComponent<Rigidbody>();
			if (component == null)
			{
				joint.connectedAnchor = raycastHit.point;
			}
			else
			{
				joint.connectedAnchor = offsetPoint;
			}
			joint.autoConfigureConnectedAnchor = false;
			endPoint = base.transform.position;
			offsetMultiplier = 1f;
		}
	}
}
