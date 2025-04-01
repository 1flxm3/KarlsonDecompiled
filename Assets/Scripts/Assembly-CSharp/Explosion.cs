using EZCameraShake;
using UnityEngine;

public class Explosion : MonoBehaviour
{
	public bool player;

	private void Start()
	{
		float num = Vector3.Distance(base.transform.position, PlayerMovement.Instance.gameObject.transform.position);
		MonoBehaviour.print(num);
		float num2 = 10f / num;
		if (num2 < 0.1f)
		{
			num2 = 0f;
		}
		CameraShaker.Instance.ShakeOnce(20f * num2 * GameState.Instance.cameraShake, 2f, 0.4f, 0.5f);
		MonoBehaviour.print("ratio: " + num2);
	}

	private void OnTriggerEnter(Collider other)
	{
		int layer = other.gameObject.layer;
		Vector3 normalized = (other.transform.position - base.transform.position).normalized;
		float num = Vector3.Distance(other.transform.position, base.transform.position);
		if (other.gameObject.CompareTag("Enemy"))
		{
			if (other.gameObject.name != "Torso")
			{
				return;
			}
			RagdollController ragdollController = (RagdollController)other.transform.root.GetComponent(typeof(RagdollController));
			if ((bool)ragdollController && !ragdollController.IsRagdoll())
			{
				ragdollController.MakeRagdoll(normalized * 1100f);
				if (player)
				{
					PlayerMovement.Instance.Slowmo(0.35f, 0.5f);
				}
				Enemy enemy = (Enemy)other.transform.root.GetComponent(typeof(Enemy));
				if ((bool)enemy)
				{
					enemy.DropGun(Vector3.up);
				}
			}
			return;
		}
		Rigidbody component = other.gameObject.GetComponent<Rigidbody>();
		if ((bool)component)
		{
			if (num < 5f)
			{
				num = 5f;
			}
			component.AddForce(normalized * 450f / num, ForceMode.Impulse);
			component.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 10f);
			if (layer == LayerMask.NameToLayer("Player"))
			{
				((PlayerMovement)other.transform.root.GetComponent(typeof(PlayerMovement))).Explode();
			}
		}
	}
}
