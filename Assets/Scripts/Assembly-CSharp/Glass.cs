using EZCameraShake;
using UnityEngine;

public class Glass : MonoBehaviour
{
	public GameObject glass;

	public GameObject glassSfx;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer != LayerMask.NameToLayer("Ground"))
		{
			UnityEngine.Object.Instantiate(glassSfx, base.transform.position, Quaternion.identity);
			glass.SetActive(value: true);
			glass.transform.parent = null;
			glass.transform.localScale = Vector3.one;
			UnityEngine.Object.Destroy(base.gameObject);
			if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
			{
				PlayerMovement.Instance.Slowmo(0.3f, 1f);
			}
			CameraShaker.Instance.ShakeOnce(5f, 3.5f, 0.3f, 0.2f);
		}
	}
}
