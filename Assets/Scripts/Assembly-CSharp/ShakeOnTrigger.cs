using EZCameraShake;
using UnityEngine;

public class ShakeOnTrigger : MonoBehaviour
{
	private CameraShakeInstance _shakeInstance;

	private void Start()
	{
		_shakeInstance = CameraShaker.Instance.StartShake(2f, 15f, 2f);
		_shakeInstance.StartFadeOut(0f);
		_shakeInstance.DeleteOnInactive = true;
	}

	private void OnTriggerEnter(Collider c)
	{
		if (c.CompareTag("Player"))
		{
			_shakeInstance.StartFadeIn(1f);
		}
	}

	private void OnTriggerExit(Collider c)
	{
		if (c.CompareTag("Player"))
		{
			_shakeInstance.StartFadeOut(3f);
		}
	}
}
