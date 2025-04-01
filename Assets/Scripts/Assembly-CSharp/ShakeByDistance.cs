using EZCameraShake;
using UnityEngine;

public class ShakeByDistance : MonoBehaviour
{
	public GameObject Player;

	public float Distance = 10f;

	private CameraShakeInstance _shakeInstance;

	private void Start()
	{
		_shakeInstance = CameraShaker.Instance.StartShake(2f, 14f, 0f);
	}

	private void Update()
	{
		float num = Vector3.Distance(Player.transform.position, base.transform.position);
		_shakeInstance.ScaleMagnitude = 1f - Mathf.Clamp01(num / Distance);
	}
}
