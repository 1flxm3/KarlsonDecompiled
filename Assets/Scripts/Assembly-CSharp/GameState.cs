using Audio;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameState : MonoBehaviour
{
	public GameObject ppVolume;

	public PostProcessProfile pp;

	private MotionBlur ppBlur;

	public bool graphics = true;

	public bool muted;

	public bool blur = true;

	public bool shake = true;

	public bool slowmo = true;

	private float sensitivity = 1f;

	private float volume;

	private float music;

	public float fov = 1f;

	public float cameraShake = 1f;

	public static GameState Instance { get; private set; }

	private void Start()
	{
		Instance = this;
		ppBlur = pp.GetSetting<MotionBlur>();
		graphics = SaveManager.Instance.state.graphics;
		shake = SaveManager.Instance.state.cameraShake;
		blur = SaveManager.Instance.state.motionBlur;
		slowmo = SaveManager.Instance.state.slowmo;
		muted = SaveManager.Instance.state.muted;
		sensitivity = SaveManager.Instance.state.sensitivity;
		music = SaveManager.Instance.state.music;
		volume = SaveManager.Instance.state.volume;
		fov = SaveManager.Instance.state.fov;
		UpdateSettings();
	}

	public void SetGraphics(bool b)
	{
		graphics = b;
		ppVolume.SetActive(b);
		SaveManager.Instance.state.graphics = b;
		SaveManager.Instance.Save();
	}

	public void SetBlur(bool b)
	{
		blur = b;
		if (b)
		{
			ppBlur.shutterAngle.value = 160f;
		}
		else
		{
			ppBlur.shutterAngle.value = 0f;
		}
		SaveManager.Instance.state.motionBlur = b;
		SaveManager.Instance.Save();
	}

	public void SetShake(bool b)
	{
		shake = b;
		if (b)
		{
			cameraShake = 1f;
		}
		else
		{
			cameraShake = 0f;
		}
		SaveManager.Instance.state.cameraShake = b;
		SaveManager.Instance.Save();
	}

	public void SetSlowmo(bool b)
	{
		slowmo = b;
		SaveManager.Instance.state.slowmo = b;
		SaveManager.Instance.Save();
	}

	public void SetSensitivity(float s)
	{
		float num = (sensitivity = Mathf.Clamp(s, 0f, 5f));
		if ((bool)PlayerMovement.Instance)
		{
			PlayerMovement.Instance.UpdateSensitivity();
		}
		SaveManager.Instance.state.sensitivity = num;
		SaveManager.Instance.Save();
	}

	public void SetMusic(float s)
	{
		float musicVolume = (music = Mathf.Clamp(s, 0f, 1f));
		if ((bool)Music.Instance)
		{
			Music.Instance.SetMusicVolume(musicVolume);
		}
		SaveManager.Instance.state.music = musicVolume;
		SaveManager.Instance.Save();
		MonoBehaviour.print("music saved as: " + music);
	}

	public void SetVolume(float s)
	{
		float num2 = (AudioListener.volume = (volume = Mathf.Clamp(s, 0f, 1f)));
		SaveManager.Instance.state.volume = num2;
		SaveManager.Instance.Save();
	}

	public void SetFov(float f)
	{
		float num = (fov = Mathf.Clamp(f, 50f, 150f));
		if ((bool)MoveCamera.Instance)
		{
			MoveCamera.Instance.UpdateFov();
		}
		SaveManager.Instance.state.fov = num;
		SaveManager.Instance.Save();
	}

	public void SetMuted(bool b)
	{
		AudioManager.Instance.MuteSounds(b);
		muted = b;
		SaveManager.Instance.state.muted = b;
		SaveManager.Instance.Save();
	}

	private void UpdateSettings()
	{
		SetGraphics(graphics);
		SetBlur(blur);
		SetSensitivity(sensitivity);
		SetMusic(music);
		SetVolume(volume);
		SetFov(fov);
		SetShake(shake);
		SetSlowmo(slowmo);
		SetMuted(muted);
	}

	public bool GetGraphics()
	{
		return graphics;
	}

	public float GetSensitivity()
	{
		return sensitivity;
	}

	public float GetVolume()
	{
		return volume;
	}

	public float GetMusic()
	{
		return music;
	}

	public float GetFov()
	{
		return fov;
	}

	public bool GetMuted()
	{
		return muted;
	}
}
