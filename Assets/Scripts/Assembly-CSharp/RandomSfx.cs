using UnityEngine;

public class RandomSfx : MonoBehaviour
{
	public AudioClip[] sounds;

	private void Awake()
	{
		AudioSource component = GetComponent<AudioSource>();
		component.clip = sounds[Random.Range(0, sounds.Length - 1)];
		component.playOnAwake = true;
		component.pitch = 1f + Random.Range(-0.3f, 0.1f);
		component.enabled = true;
	}
}
