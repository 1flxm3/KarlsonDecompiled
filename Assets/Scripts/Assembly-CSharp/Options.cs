using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
	public TextMeshProUGUI sens;

	public TextMeshProUGUI volume;

	public TextMeshProUGUI music;

	public TextMeshProUGUI fov;

	public TextMeshProUGUI[] sounds;

	public TextMeshProUGUI[] graphics;

	public TextMeshProUGUI[] shake;

	public TextMeshProUGUI[] slowmo;

	public TextMeshProUGUI[] blur;

	public Slider sensS;

	public Slider volumeS;

	public Slider musicS;

	public Slider fovS;

	private void OnEnable()
	{
		UpdateList(graphics, GameState.Instance.GetGraphics());
		UpdateList(shake, GameState.Instance.shake);
		UpdateList(slowmo, GameState.Instance.slowmo);
		UpdateList(blur, GameState.Instance.blur);
		sensS.value = GameState.Instance.GetSensitivity();
		volumeS.value = GameState.Instance.GetVolume();
		musicS.value = GameState.Instance.GetMusic();
		fovS.value = GameState.Instance.GetFov();
		MonoBehaviour.print(GameState.Instance.GetMusic());
		UpdateSensitivity();
		UpdateFov();
		UpdateVolume();
		UpdateMusic();
	}

	public void ChangeGraphics(bool b)
	{
		GameState.Instance.SetGraphics(b);
		UpdateList(graphics, b);
	}

	public void ChangeBlur(bool b)
	{
		GameState.Instance.SetBlur(b);
		UpdateList(blur, b);
	}

	public void ChangeShake(bool b)
	{
		GameState.Instance.SetShake(b);
		UpdateList(shake, b);
	}

	public void ChangeSlowmo(bool b)
	{
		GameState.Instance.SetSlowmo(b);
		UpdateList(slowmo, b);
	}

	public void UpdateSensitivity()
	{
		float value = sensS.value;
		GameState.Instance.SetSensitivity(value);
		sens.text = $"{value:F2}";
	}

	public void UpdateVolume()
	{
		float num = (AudioListener.volume = volumeS.value);
		GameState.Instance.SetVolume(num);
		volume.text = $"{num:F2}";
	}

	public void UpdateMusic()
	{
		float value = musicS.value;
		GameState.Instance.SetMusic(value);
		music.text = $"{value:F2}";
	}

	public void UpdateFov()
	{
		float value = fovS.value;
		GameState.Instance.SetFov(value);
		fov.text = string.Concat(value);
	}

	private void UpdateList(TextMeshProUGUI[] list, bool b)
	{
		if (!b)
		{
			list[1].color = Color.white;
			list[0].color = (Color.clear + Color.white) / 2f;
		}
		else
		{
			list[1].color = (Color.clear + Color.white) / 2f;
			list[0].color = Color.white;
		}
	}
}
