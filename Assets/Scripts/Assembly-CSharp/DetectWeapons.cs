using System.Collections.Generic;
using Audio;
using UnityEngine;

public class DetectWeapons : MonoBehaviour
{
	public Transform weaponPos;

	private List<GameObject> weapons;

	private bool hasGun;

	private GameObject gun;

	private Pickup gunScript;

	private float speed = 10f;

	private Quaternion desiredRot = Quaternion.Euler(0f, 90f, 0f);

	private Vector3 desiredPos = Vector3.zero;

	private Vector3 posVel;

	private float throwForce = 1000f;

	private Vector3 scale;

	public void Pickup()
	{
		if (!hasGun && HasWeapons())
		{
			gun = GetWeapon();
			gunScript = (Pickup)gun.GetComponent(typeof(Pickup));
			if (gunScript.pickedUp)
			{
				gun = null;
				gunScript = null;
				return;
			}
			UnityEngine.Object.Destroy(gun.GetComponent<Rigidbody>());
			scale = gun.transform.localScale;
			gun.transform.parent = weaponPos;
			gun.transform.localScale = scale;
			hasGun = true;
			gunScript.PickupWeapon(player: true);
			AudioManager.Instance.Play("GunPickup");
			gun.layer = LayerMask.NameToLayer("Equipable");
		}
	}

	public void Shoot(Vector3 dir)
	{
		if (hasGun)
		{
			gunScript.Use(dir);
		}
	}

	public void StopUse()
	{
		if (hasGun)
		{
			gunScript.StopUse();
		}
	}

	public void Throw(Vector3 throwDir)
	{
		if (hasGun && !gun.GetComponent<Rigidbody>())
		{
			gunScript.StopUse();
			hasGun = false;
			Rigidbody rigidbody = gun.AddComponent<Rigidbody>();
			rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			rigidbody.maxAngularVelocity = 20f;
			rigidbody.AddForce(throwDir * throwForce);
			rigidbody.AddRelativeTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f) * 0.4f), ForceMode.Impulse);
			gun.layer = LayerMask.NameToLayer("Gun");
			gunScript.Drop();
			gun.transform.parent = null;
			gun.transform.localScale = scale;
			gun = null;
			gunScript = null;
		}
	}

	public void Fire(Vector3 dir)
	{
		gunScript.Use(dir);
	}

	private void Update()
	{
		if (hasGun)
		{
			gun.transform.localRotation = Quaternion.Slerp(gun.transform.localRotation, desiredRot, Time.deltaTime * speed);
			gun.transform.localPosition = Vector3.SmoothDamp(gun.transform.localPosition, desiredPos, ref posVel, 1f / speed);
			gunScript.OnAim();
		}
	}

	private void Start()
	{
		weapons = new List<GameObject>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Gun") && !weapons.Contains(other.gameObject))
		{
			weapons.Add(other.gameObject);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Gun") && weapons.Contains(other.gameObject))
		{
			weapons.Remove(other.gameObject);
		}
	}

	public GameObject GetWeapon()
	{
		if (weapons.Count == 1)
		{
			return weapons[0];
		}
		GameObject result = null;
		float num = float.PositiveInfinity;
		foreach (GameObject weapon in weapons)
		{
			float num2 = Vector3.Distance(base.transform.position, weapon.transform.position);
			if (num2 < num)
			{
				num = num2;
				result = weapon;
			}
		}
		return result;
	}

	public void ForcePickup(GameObject gun)
	{
		gunScript = (Pickup)gun.GetComponent(typeof(Pickup));
		this.gun = gun;
		if (gunScript.pickedUp)
		{
			gun = null;
			gunScript = null;
			return;
		}
		UnityEngine.Object.Destroy(gun.GetComponent<Rigidbody>());
		scale = gun.transform.localScale;
		gun.transform.parent = weaponPos;
		gun.transform.localScale = scale;
		hasGun = true;
		gunScript.PickupWeapon(player: true);
		gun.layer = LayerMask.NameToLayer("Equipable");
	}

	public float GetRecoil()
	{
		return gunScript.recoil;
	}

	public bool HasWeapons()
	{
		return weapons.Count > 0;
	}

	public bool IsGrappler()
	{
		if (!gun)
		{
			return false;
		}
		return gun.GetComponent(typeof(Grappler));
	}

	public Vector3 GetGrapplerPoint()
	{
		if (IsGrappler())
		{
			return ((Grappler)gun.GetComponent(typeof(Grappler))).GetGrapplePoint();
		}
		return Vector3.zero;
	}

	public Pickup GetWeaponScript()
	{
		return gunScript;
	}

	public bool HasGun()
	{
		return hasGun;
	}
}
