using UnityEngine;

public class Barrel : MonoBehaviour
{
	private bool done;

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
		{
			Explosion explosion = (Explosion)UnityEngine.Object.Instantiate(PrefabManager.Instance.explosion, base.transform.position, Quaternion.identity).GetComponentInChildren(typeof(Explosion));
			UnityEngine.Object.Destroy(base.gameObject);
			CancelInvoke();
			done = true;
			Bullet bullet = (Bullet)other.gameObject.GetComponent(typeof(Bullet));
			if ((bool)bullet && bullet.player)
			{
				explosion.player = bullet.player;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
		{
			done = true;
			Invoke("Explode", 0.2f);
		}
	}

	private void Explode()
	{
		UnityEngine.Object.Instantiate(PrefabManager.Instance.explosion, base.transform.position, Quaternion.identity);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
