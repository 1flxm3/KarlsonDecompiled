using UnityEngine;

public class Respawn : MonoBehaviour
{
	public Transform respawnPoint;

	private void OnTriggerEnter(Collider other)
	{
		MonoBehaviour.print(other.gameObject.layer);
		if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			Transform root = other.transform.root;
			root.transform.position = respawnPoint.position;
			root.GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
	}
}
