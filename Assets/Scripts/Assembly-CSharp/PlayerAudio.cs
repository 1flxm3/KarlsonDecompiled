using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
	private Rigidbody rb;

	public AudioSource wind;

	public AudioSource foley;

	private float currentVol;

	private float volVel;

	private void Start()
	{
		rb = PlayerMovement.Instance.GetRb();
	}

	private void Update()
	{
		if (!rb)
		{
			return;
		}
		float num = rb.velocity.magnitude;
		if (PlayerMovement.Instance.grounded)
		{
			if (num < 20f)
			{
				num = 0f;
			}
			num = (num - 20f) / 30f;
		}
		else
		{
			num = (num - 10f) / 30f;
		}
		if (num > 1f)
		{
			num = 1f;
		}
		num *= 1f;
		currentVol = Mathf.SmoothDamp(currentVol, num, ref volVel, 0.2f);
		if (PlayerMovement.Instance.paused)
		{
			currentVol = 0f;
		}
		foley.volume = currentVol;
		wind.volume = currentVol * 0.5f;
	}
}
