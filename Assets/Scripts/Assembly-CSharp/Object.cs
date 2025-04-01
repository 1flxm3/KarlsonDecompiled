using UnityEngine;

public class Object : MonoBehaviour
{
	private bool ready = true;

	private bool hitReady = true;

	private void OnCollisionEnter(Collision other)
	{
		float num = other.relativeVelocity.magnitude * 0.025f;
		if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && hitReady && num > 0.8f)
		{
			hitReady = false;
			Vector3 normalized = GetComponent<Rigidbody>().velocity.normalized;
			UnityEngine.Object.Instantiate(PrefabManager.Instance.enemyHitAudio, other.contacts[0].point, Quaternion.identity);
			((RagdollController)other.transform.root.GetComponent(typeof(RagdollController))).MakeRagdoll(normalized * 350f);
			Rigidbody component = other.gameObject.GetComponent<Rigidbody>();
			if ((bool)component)
			{
				component.AddForce(normalized * 1100f);
			}
			((Enemy)other.transform.root.GetComponent(typeof(Enemy))).DropGun(Vector3.up);
		}
		if (ready)
		{
			ready = false;
			AudioSource component2 = UnityEngine.Object.Instantiate(PrefabManager.Instance.objectImpactAudio, base.transform.position, Quaternion.identity).GetComponent<AudioSource>();
			Rigidbody component3 = GetComponent<Rigidbody>();
			float num2 = 1f;
			if ((bool)component3)
			{
				num2 = component3.mass;
			}
			if (num2 < 0.3f)
			{
				num2 = 0.5f;
			}
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			_ = component2.volume;
			if (num > 1f)
			{
				num = 1f;
			}
			component2.volume = num * num2;
			Invoke("GetReady", 0.1f);
		}
	}

	private void GetReady()
	{
		ready = true;
	}
}
