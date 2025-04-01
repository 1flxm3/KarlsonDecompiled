using UnityEngine;

public class Music : MonoBehaviour
{
	private AudioSource music;

	private float multiplier;

	private float desiredVolume;

	private float vel;

	public static Music Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		music = GetComponent<AudioSource>();
		music.volume = 0.04f;
		multiplier = 1f;
	}

	private void Update()
	{
		desiredVolume = 0.016f * multiplier;
		if (Game.Instance.playing)
		{
			desiredVolume = 0.6f * multiplier;
		}
		music.volume = Mathf.SmoothDamp(music.volume, desiredVolume, ref vel, 0.6f);
	}

	public void SetMusicVolume(float f)
	{
		multiplier = f;
	}
}
