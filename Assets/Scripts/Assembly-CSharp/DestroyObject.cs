using UnityEngine;

public class DestroyObject : MonoBehaviour
{
	public float time;

	private void Start()
	{
		Invoke("DestroySelf", time);
	}

	private void DestroySelf()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
