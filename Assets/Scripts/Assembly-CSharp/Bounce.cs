using UnityEngine;

public class Bounce : MonoBehaviour
{
	private void OnCollisionEnter(Collision other)
	{
		MonoBehaviour.print("yeet");
		_ = (bool)other.gameObject.GetComponent<Rigidbody>();
	}
}
