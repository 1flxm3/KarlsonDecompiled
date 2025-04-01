using UnityEngine;

namespace Audio
{
	public class AudioManager : MonoBehaviour
	{
		public Sound[] sounds;

		public Sound[] footsteps;

		public Sound[] wallrun;

		public Sound[] jumps;

		public AudioLowPassFilter filter;

		private float desiredFreq = 500f;

		private float velFreq;

		private float freqSpeed = 0.2f;

		public bool muted;

		public static AudioManager Instance { get; set; }

		private void Awake()
		{
			Instance = this;
			Sound[] array = sounds;
			foreach (Sound sound in array)
			{
				sound.source = base.gameObject.AddComponent<AudioSource>();
				sound.source.clip = sound.clip;
				sound.source.loop = sound.loop;
				sound.source.volume = sound.volume;
				sound.source.pitch = sound.pitch;
				sound.source.bypassListenerEffects = sound.bypass;
			}
			array = footsteps;
			foreach (Sound sound2 in array)
			{
				sound2.source = base.gameObject.AddComponent<AudioSource>();
				sound2.source.clip = sound2.clip;
				sound2.source.loop = sound2.loop;
				sound2.source.volume = sound2.volume;
				sound2.source.pitch = sound2.pitch;
				sound2.source.bypassListenerEffects = sound2.bypass;
			}
			array = wallrun;
			foreach (Sound sound3 in array)
			{
				sound3.source = base.gameObject.AddComponent<AudioSource>();
				sound3.source.clip = sound3.clip;
				sound3.source.loop = sound3.loop;
				sound3.source.volume = sound3.volume;
				sound3.source.pitch = sound3.pitch;
				sound3.source.bypassListenerEffects = sound3.bypass;
			}
			array = jumps;
			foreach (Sound sound4 in array)
			{
				sound4.source = base.gameObject.AddComponent<AudioSource>();
				sound4.source.clip = sound4.clip;
				sound4.source.loop = sound4.loop;
				sound4.source.volume = sound4.volume;
				sound4.source.pitch = sound4.pitch;
				sound4.source.bypassListenerEffects = sound4.bypass;
			}
		}

		private void Update()
		{
		}

		public void MuteSounds(bool b)
		{
			if (b)
			{
				AudioListener.volume = 0f;
			}
			else
			{
				AudioListener.volume = 1f;
			}
			muted = b;
		}

		public void PlayButton()
		{
			if (muted)
			{
				return;
			}
			Sound[] array = sounds;
			foreach (Sound sound in array)
			{
				if (sound.name == "Button")
				{
					sound.source.pitch = 0.8f + Random.Range(-0.03f, 0.03f);
					break;
				}
			}
			Play("Button");
		}

		public void PlayPitched(string n, float v)
		{
			if (muted)
			{
				return;
			}
			Sound[] array = sounds;
			foreach (Sound sound in array)
			{
				if (sound.name == n)
				{
					sound.source.pitch = 1f + Random.Range(0f - v, v);
					break;
				}
			}
			Play(n);
		}

		public void MuteMusic()
		{
			Sound[] array = sounds;
			foreach (Sound sound in array)
			{
				if (sound.name == "Song")
				{
					sound.source.volume = 0f;
					break;
				}
			}
		}

		public void SetVolume(float v)
		{
			Sound[] array = sounds;
			foreach (Sound sound in array)
			{
				if (sound.name == "Song")
				{
					sound.source.volume = v;
					break;
				}
			}
		}

		public void UnmuteMusic()
		{
			Sound[] array = sounds;
			foreach (Sound sound in array)
			{
				if (sound.name == "Song")
				{
					sound.source.volume = 1.15f;
					break;
				}
			}
		}

		public void Play(string n)
		{
			if (muted && n != "Song")
			{
				return;
			}
			Sound[] array = sounds;
			foreach (Sound sound in array)
			{
				if (sound.name == n)
				{
					sound.source.Play();
					break;
				}
			}
		}

		public void PlayFootStep()
		{
			if (!muted)
			{
				int num = Random.Range(0, footsteps.Length - 1);
				footsteps[num].source.Play();
			}
		}

		public void PlayLanding()
		{
			if (!muted)
			{
				int num = Random.Range(0, wallrun.Length - 1);
				wallrun[num].source.Play();
			}
		}

		public void PlayJump()
		{
			if (!muted)
			{
				int num = Random.Range(0, jumps.Length - 1);
				Sound sound = jumps[num];
				if ((bool)sound.source)
				{
					sound.source.Play();
				}
			}
		}

		public void Stop(string n)
		{
			Sound[] array = sounds;
			foreach (Sound sound in array)
			{
				if (sound.name == n)
				{
					sound.source.Stop();
					break;
				}
			}
		}

		public void SetFreq(float freq)
		{
			desiredFreq = freq;
		}
	}
}
