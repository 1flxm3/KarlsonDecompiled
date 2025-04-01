using UnityEngine;

public abstract class Pickup : MonoBehaviour, IPickup
{
	protected bool player;

	private bool thrown;

	public float recoil;

	private Transform outline;

	public bool pickedUp { get; set; }

	public bool readyToUse { get; set; }

	private void Awake()
	{
		readyToUse = true;
		outline = base.transform.GetChild(1);
	}

	private void Update()
	{
		_ = pickedUp;
	}

	public void PickupWeapon(bool player)
	{
		pickedUp = true;
		this.player = player;
		outline.gameObject.SetActive(value: false);
	}

	public void Drop()
	{
		readyToUse = true;
		Invoke("DropWeapon", 0.5f);
		thrown = true;
	}

	private void DropWeapon()
	{
		CancelInvoke();
		pickedUp = false;
		outline.gameObject.SetActive(value: true);
	}

	public abstract void Use(Vector3 attackDirection);

	public abstract void OnAim();

	public abstract void StopUse();

	public bool IsPickedUp()
	{
		return pickedUp;
	}

	private void OnCollisionEnter(Collision other)
	{
		if (!thrown)
		{
			return;
		}
		if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
		{
			UnityEngine.Object.Instantiate(PrefabManager.Instance.enemyHitAudio, other.contacts[0].point, Quaternion.identity);
			((RagdollController)other.transform.root.GetComponent(typeof(RagdollController))).MakeRagdoll(-base.transform.right * 60f);
			Rigidbody component = other.gameObject.GetComponent<Rigidbody>();
			if ((bool)component)
			{
				component.AddForce(-base.transform.right * 1500f);
			}
			((Enemy)other.transform.root.GetComponent(typeof(Enemy))).DropGun(Vector3.up);
		}
		thrown = false;
	}
}
