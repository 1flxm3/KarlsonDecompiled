using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
using UnityEngine;

public class Grappler : Pickup
{
	private Transform tip;

	private bool grappling;

	public LayerMask whatIsGround;

	private Vector3 grapplePoint;

	private SpringJoint joint;

	private LineRenderer lr;

	private Vector3 endPoint;

	private float offsetMultiplier;

	private float offsetVel;

	public GameObject aim;

	private int positions = 100;

	private Vector3 aimVel;

	private Vector3 scaleVel;

	private Vector3 nearestPoint;

	private void Start()
	{
		tip = base.transform.GetChild(0);
		lr = GetComponent<LineRenderer>();
		lr.positionCount = positions;
		aim.transform.parent = null;
		aim.SetActive(value: false);
	}

	public override void Use(Vector3 attackDirection)
	{
		if (grappling)
		{
			return;
		}
		grappling = true;
		Transform playerCamTransform = PlayerMovement.Instance.GetPlayerCamTransform();
		Transform transform = PlayerMovement.Instance.transform;
		RaycastHit[] array = Physics.RaycastAll(playerCamTransform.position, playerCamTransform.forward, 70f, whatIsGround);
		if (array.Length < 1)
		{
			if (nearestPoint == Vector3.zero)
			{
				return;
			}
			grapplePoint = nearestPoint;
		}
		else
		{
			grapplePoint = array[0].point;
		}
		joint = transform.gameObject.AddComponent<SpringJoint>();
		joint.autoConfigureConnectedAnchor = false;
		joint.connectedAnchor = grapplePoint;
		joint.maxDistance = Vector2.Distance(grapplePoint, transform.position) * 0.8f;
		joint.minDistance = Vector2.Distance(grapplePoint, transform.position) * 0.25f;
		joint.spring = 4.5f;
		joint.damper = 7f;
		joint.massScale = 4.5f;
		endPoint = tip.position;
		offsetMultiplier = 2f;
		lr.positionCount = positions;
		AudioManager.Instance.PlayPitched("Grapple", 0.2f);
	}

	public override void OnAim()
	{
		if (grappling)
		{
			return;
		}
		Transform playerCamTransform = PlayerMovement.Instance.GetPlayerCamTransform();
		List<RaycastHit> list = Physics.RaycastAll(playerCamTransform.position, playerCamTransform.forward, 70f, whatIsGround).ToList();
		if (list.Count > 0)
		{
			aim.SetActive(value: false);
			aim.transform.localScale = Vector3.zero;
			return;
		}
		int num = 50;
		int num2 = 10;
		float num3 = 0.035f;
		bool flag = list.Count > 0;
		for (int i = 0; i < num2; i++)
		{
			if (flag)
			{
				break;
			}
			for (int j = 0; j < num; j++)
			{
				float f = (float)Math.PI * 2f / (float)num * (float)j;
				float num4 = Mathf.Cos(f);
				float num5 = Mathf.Sin(f);
				Vector3 vector = playerCamTransform.right * num4 + playerCamTransform.up * num5;
				list.AddRange(Physics.RaycastAll(playerCamTransform.position, playerCamTransform.forward + vector * num3 * i, 70f, whatIsGround));
			}
			if (list.Count > 0)
			{
				flag = true;
				break;
			}
		}
		nearestPoint = FindNearestPoint(list);
		if (list.Count > 0 && !grappling)
		{
			aim.SetActive(value: true);
			aim.transform.position = Vector3.SmoothDamp(aim.transform.position, nearestPoint, ref aimVel, 0.05f);
			Vector3 target = 0.025f * list[0].distance * Vector3.one;
			aim.transform.localScale = Vector3.SmoothDamp(aim.transform.localScale, target, ref scaleVel, 0.2f);
		}
		else
		{
			aim.SetActive(value: false);
			aim.transform.localScale = Vector3.zero;
		}
	}

	private Vector3 FindNearestPoint(List<RaycastHit> hits)
	{
		Transform playerCamTransform = PlayerMovement.Instance.GetPlayerCamTransform();
		Vector3 result = Vector3.zero;
		float num = float.PositiveInfinity;
		for (int i = 0; i < hits.Count; i++)
		{
			if (hits[i].distance < num)
			{
				num = hits[i].distance;
				result = hits[i].collider.ClosestPoint(playerCamTransform.position + playerCamTransform.forward * num);
			}
		}
		return result;
	}

	public override void StopUse()
	{
		UnityEngine.Object.Destroy(joint);
		grapplePoint = Vector3.zero;
		grappling = false;
	}

	private void LateUpdate()
	{
		DrawGrapple();
	}

	private void DrawGrapple()
	{
		if (grapplePoint == Vector3.zero || joint == null)
		{
			lr.positionCount = 0;
			return;
		}
		endPoint = Vector3.Lerp(endPoint, grapplePoint, Time.deltaTime * 15f);
		offsetMultiplier = Mathf.SmoothDamp(offsetMultiplier, 0f, ref offsetVel, 0.1f);
		Vector3 position = tip.position;
		float num = Vector3.Distance(endPoint, position);
		lr.SetPosition(0, position);
		lr.SetPosition(positions - 1, endPoint);
		float num2 = num;
		float num3 = 1f;
		for (int i = 1; i < positions - 1; i++)
		{
			float num4 = (float)i / (float)positions;
			float num5 = num4 * offsetMultiplier;
			float num6 = (Mathf.Sin(num5 * num2) - 0.5f) * num3 * (num5 * 2f);
			Vector3 normalized = (endPoint - position).normalized;
			float num7 = Mathf.Sin(num4 * 180f * ((float)Math.PI / 180f));
			float num8 = Mathf.Cos(offsetMultiplier * 90f * ((float)Math.PI / 180f));
			Vector3 position2 = position + (endPoint - position) / positions * i + ((Vector3)(num8 * num6 * Vector2.Perpendicular(normalized)) + offsetMultiplier * num7 * Vector3.down);
			lr.SetPosition(i, position2);
		}
	}

	public Vector3 GetGrapplePoint()
	{
		return grapplePoint;
	}
}
