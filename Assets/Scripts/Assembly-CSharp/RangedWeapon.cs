using System.Collections.Generic;
using Audio;
using UnityEngine;

public class RangedWeapon : Weapon
{
	public GameObject projectile;

	public float pushBackForce;

	public float force;

	public float accuracy;

	public int bullets;

	public float boostRecoil;

	private Transform guntip;

	private Rigidbody rb;

	private Collider[] projectileColliders;

	private new void Start()
	{
		base.Start();
		rb = GetComponent<Rigidbody>();
		guntip = base.transform.GetChild(0);
	}

	public override void Use(Vector3 attackDirection)
	{
		if (base.readyToUse && base.pickedUp)
		{
			SpawnProjectile(attackDirection);
			Recoil();
			base.readyToUse = false;
			Invoke("GetReady", attackSpeed);
		}
	}

	public override void OnAim()
	{
	}

	public override void StopUse()
	{
	}

	private void SpawnProjectile(Vector3 attackDirection)
	{
		Vector3 vector = guntip.position - guntip.transform.right / 4f;
		Vector3 normalized = (attackDirection - vector).normalized;
		List<Collider> list = new List<Collider>();
		if (player)
		{
			PlayerMovement.Instance.GetRb().AddForce(base.transform.right * boostRecoil, ForceMode.Impulse);
		}
		for (int i = 0; i < bullets; i++)
		{
			UnityEngine.Object.Instantiate(PrefabManager.Instance.muzzle, vector, Quaternion.identity);
			GameObject gameObject = UnityEngine.Object.Instantiate(projectile, vector, base.transform.rotation);
			Rigidbody componentInChildren = gameObject.GetComponentInChildren<Rigidbody>();
			projectileColliders = gameObject.GetComponentsInChildren<Collider>();
			RemoveCollisionWithPlayer();
			componentInChildren.transform.rotation = base.transform.rotation;
			Vector3 vector2 = normalized + (guntip.transform.up * Random.Range(0f - accuracy, accuracy) + guntip.transform.forward * Random.Range(0f - accuracy, accuracy));
			componentInChildren.AddForce(componentInChildren.mass * force * vector2);
			Bullet bullet = (Bullet)gameObject.GetComponent(typeof(Bullet));
			if (bullet != null)
			{
				Color col = Color.red;
				if (player)
				{
					col = Color.blue;
					Gun.Instance.Shoot();
					if (bullet.explosive)
					{
						UnityEngine.Object.Instantiate(PrefabManager.Instance.thumpAudio, base.transform.position, Quaternion.identity);
					}
					else
					{
						AudioManager.Instance.PlayPitched("GunBass", 0.3f);
						AudioManager.Instance.PlayPitched("GunHigh", 0.3f);
						AudioManager.Instance.PlayPitched("GunLow", 0.3f);
					}
					componentInChildren.AddForce(componentInChildren.mass * force * vector2);
				}
				else
				{
					UnityEngine.Object.Instantiate(PrefabManager.Instance.gunShotAudio, base.transform.position, Quaternion.identity);
				}
				bullet.SetBullet(damage, pushBackForce, col);
				bullet.player = player;
			}
			foreach (Collider item in list)
			{
				Physics.IgnoreCollision(item, projectileColliders[0]);
			}
			list.Add(projectileColliders[0]);
		}
	}

	private void GetReady()
	{
		base.readyToUse = true;
	}

	private void Recoil()
	{
	}

	private void RemoveCollisionWithPlayer()
	{
		Collider[] array = ((!player) ? base.transform.root.GetComponentsInChildren<Collider>() : new Collider[1] { PlayerMovement.Instance.GetPlayerCollider() });
		for (int i = 0; i < array.Length; i++)
		{
			for (int j = 0; j < projectileColliders.Length; j++)
			{
				Physics.IgnoreCollision(array[i], projectileColliders[j], ignore: true);
			}
		}
	}
}
