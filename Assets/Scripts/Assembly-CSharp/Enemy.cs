using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
	private float hipSpeed = 3f;

	private float headAndHandSpeed = 4f;

	private Transform target;

	public LayerMask objectsAndPlayer;

	private NavMeshAgent agent;

	private bool spottedPlayer;

	private Animator animator;

	public GameObject startGun;

	public Transform gunPosition;

	private Weapon gunScript;

	public GameObject currentGun;

	private float attackSpeed;

	private bool readyToShoot;

	private RagdollController ragdoll;

	public Transform leftArm;

	public Transform rightArm;

	public Transform head;

	public Transform hips;

	public Transform player;

	private bool takingAim;

	private void Start()
	{
		ragdoll = (RagdollController)GetComponent(typeof(RagdollController));
		animator = GetComponentInChildren<Animator>();
		agent = GetComponent<NavMeshAgent>();
		GiveGun();
	}

	private void LateUpdate()
	{
		FindPlayer();
		Aim();
	}

	private void Aim()
	{
		if (!(currentGun == null) && !ragdoll.IsRagdoll() && animator.GetBool("Aiming"))
		{
			Vector3 vector = target.transform.position - base.transform.position;
			if (Vector3.Angle(base.transform.forward, vector) > 70f)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(vector), Time.deltaTime * hipSpeed);
			}
			head.transform.rotation = Quaternion.Slerp(head.transform.rotation, Quaternion.LookRotation(vector), Time.deltaTime * headAndHandSpeed);
			rightArm.transform.rotation = Quaternion.Slerp(head.transform.rotation, Quaternion.LookRotation(vector), Time.deltaTime * headAndHandSpeed);
			leftArm.transform.rotation = Quaternion.Slerp(head.transform.rotation, Quaternion.LookRotation(vector), Time.deltaTime * headAndHandSpeed);
			if (readyToShoot)
			{
				gunScript.Use(target.position);
				readyToShoot = false;
				Invoke("Cooldown", attackSpeed + Random.Range(attackSpeed, attackSpeed * 5f));
			}
		}
	}

	private void FindPlayer()
	{
		FindTarget();
		if (!agent || !target)
		{
			return;
		}
		Vector3 normalized = (target.position - base.transform.position).normalized;
		RaycastHit[] array = Physics.RaycastAll(base.transform.position + normalized, normalized, (int)objectsAndPlayer);
		if (array.Length < 1)
		{
			return;
		}
		bool flag = false;
		float num = 1001f;
		float num2 = 1000f;
		for (int i = 0; i < array.Length; i++)
		{
			int layer = array[i].collider.gameObject.layer;
			if (!(array[i].collider.transform.root.gameObject.name == base.gameObject.name) && layer != LayerMask.NameToLayer("TransparentFX"))
			{
				if (layer == LayerMask.NameToLayer("Player"))
				{
					num = array[i].distance;
					flag = true;
				}
				else if (array[i].distance < num2)
				{
					num2 = array[i].distance;
				}
			}
		}
		if (!flag)
		{
			return;
		}
		if (num2 < num && num != 1001f)
		{
			readyToShoot = false;
			if (animator.GetBool("Running") && agent.remainingDistance < 0.2f)
			{
				animator.SetBool("Running", value: false);
				spottedPlayer = false;
			}
			if (spottedPlayer && agent.isOnNavMesh && !animator.GetBool("Running"))
			{
				MonoBehaviour.print("oof");
				takingAim = false;
				agent.destination = target.transform.position;
				animator.SetBool("Running", value: true);
				animator.SetBool("Aiming", value: false);
				readyToShoot = false;
			}
		}
		else if (!takingAim && !animator.GetBool("Aiming"))
		{
			if (!spottedPlayer)
			{
				spottedPlayer = true;
			}
			Invoke("TakeAim", Random.Range(0.3f, 1f));
			takingAim = true;
		}
	}

	private void TakeAim()
	{
		animator.SetBool("Running", value: false);
		animator.SetBool("Aiming", value: true);
		CancelInvoke();
		Invoke("Cooldown", Random.Range(0.3f, 1f));
		if ((bool)agent && agent.isOnNavMesh)
		{
			agent.destination = base.transform.position;
		}
	}

	private void GiveGun()
	{
		if (!(startGun == null))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(startGun);
			UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
			gunScript = (Weapon)gameObject.GetComponent(typeof(Weapon));
			gunScript.PickupWeapon(player: false);
			gameObject.transform.parent = gunPosition;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			currentGun = gameObject;
			attackSpeed = gunScript.GetAttackSpeed();
		}
	}

	private void Cooldown()
	{
		readyToShoot = true;
	}

	private void FindTarget()
	{
		if (!(target != null) && (bool)PlayerMovement.Instance)
		{
			target = PlayerMovement.Instance.playerCam;
		}
	}

	public void DropGun(Vector3 dir)
	{
		if (!(gunScript == null))
		{
			gunScript.Drop();
			Rigidbody rigidbody = currentGun.AddComponent<Rigidbody>();
			rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			currentGun.transform.parent = null;
			rigidbody.AddForce(dir, ForceMode.Impulse);
			float num = 10f;
			rigidbody.AddTorque(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * num);
			gunScript = null;
		}
	}

	public bool IsDead()
	{
		return ragdoll.IsRagdoll();
	}
}
