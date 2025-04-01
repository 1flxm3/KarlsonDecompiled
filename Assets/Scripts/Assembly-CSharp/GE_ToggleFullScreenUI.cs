using UnityEngine;
using UnityEngine.UI;

public class GE_ToggleFullScreenUI : MonoBehaviour
{
	private int m_DefWidth;

	private int m_DefHeight;

	private void Start()
	{
		m_DefWidth = Screen.width;
		m_DefHeight = Screen.height;
		if (!Application.isEditor)
		{
			if (Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer)
			{
				base.gameObject.SetActive(value: true);
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}

	private void Update()
	{
	}

	public void OnButton_ToggleFullScreen()
	{
		if (Application.isEditor)
		{
			if (!base.gameObject.activeSelf)
			{
				return;
			}
			base.gameObject.GetComponent<Button>().interactable = false;
			{
				foreach (Transform item in base.transform)
				{
					item.gameObject.SetActive(value: true);
				}
				return;
			}
		}
		Screen.fullScreen = !Screen.fullScreen;
		if (!Screen.fullScreen)
		{
			Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fullscreen: true);
		}
		else
		{
			Screen.SetResolution(m_DefWidth, m_DefHeight, fullscreen: false);
		}
	}
}
