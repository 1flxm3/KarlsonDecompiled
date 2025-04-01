using UnityEngine;

public class ExplosiveBullet : MonoBehaviour
{
	private Rigidbody rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		UnityEngine.Object.Instantiate(PrefabManager.Instance.thumpAudio, base.transform.position, Quaternion.identity);
	}

	private void OnCollisionEnter(Collision other)
	{
		UnityEngine.Object.Destroy(base.gameObject);
		UnityEngine.Object.Instantiate(PrefabManager.Instance.explosion, base.transform.position, Quaternion.identity);
	}

	private void Update()
	{
		rb.AddForce(Vector3.up * Time.deltaTime * 1000f);
	}
}
