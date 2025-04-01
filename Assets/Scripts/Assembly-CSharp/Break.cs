using UnityEngine;

public class Break : MonoBehaviour
{
	public GameObject replace;

	private bool done;

	private void OnCollisionEnter(Collision other)
	{
		if (done || other.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			return;
		}
		Rigidbody component = other.gameObject.GetComponent<Rigidbody>();
		if (!component || !(component.velocity.magnitude > 18f))
		{
			return;
		}
		if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			if (!PlayerMovement.Instance.IsCrouching())
			{
				return;
			}
			PlayerMovement.Instance.Slowmo(0.35f, 0.8f);
			BreakDoor(component);
		}
		BreakDoor(component);
	}

	private void BreakDoor(Rigidbody rb)
	{
		Vector3 velocity = rb.velocity;
		float magnitude = velocity.magnitude;
		if (magnitude > 20f)
		{
			float num = magnitude / 20f;
			velocity /= num;
		}
		Rigidbody[] componentsInChildren = UnityEngine.Object.Instantiate(replace, base.transform.position, base.transform.rotation).GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].velocity = velocity * 1.5f;
		}
		UnityEngine.Object.Instantiate(PrefabManager.Instance.destructionAudio, base.transform.position, Quaternion.identity);
		UnityEngine.Object.Destroy(base.gameObject);
		done = true;
	}
}
