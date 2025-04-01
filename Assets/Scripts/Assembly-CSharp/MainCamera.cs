using UnityEngine;

public class MainCamera : MonoBehaviour
{
	private void Awake()
	{
		if ((bool)SlowmoEffect.Instance)
		{
			SlowmoEffect.Instance.NewScene(GetComponent<AudioLowPassFilter>(), GetComponent<AudioDistortionFilter>());
		}
	}
}
