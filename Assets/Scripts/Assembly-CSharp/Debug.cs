using TMPro;
using UnityEngine;

public class Debug : MonoBehaviour
{
	public TextMeshProUGUI fps;

	public TMP_InputField console;

	public TextMeshProUGUI consoleLog;

	private bool fpsOn;

	private bool speedOn;

	private float deltaTime;

	private void Start()
	{
		Application.targetFrameRate = 150;
	}

	private void Update()
	{
		Fps();
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			if (console.isActiveAndEnabled)
			{
				CloseConsole();
			}
			else
			{
				OpenConsole();
			}
		}
	}

	private void Fps()
	{
		if (!fpsOn && !speedOn)
		{
			if (!fps.enabled)
			{
				fps.gameObject.SetActive(value: false);
			}
			return;
		}
		if (!fps.gameObject.activeInHierarchy)
		{
			fps.gameObject.SetActive(value: true);
		}
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
		float num = deltaTime * 1000f;
		float num2 = 1f / deltaTime;
		string text = "";
		if (fpsOn)
		{
			text += $"{num:0.0} ms ({num2:0.} fps)";
		}
		if (speedOn)
		{
			text = text + "\nm/s: " + $"{PlayerMovement.Instance.rb.velocity.magnitude:F1}";
		}
		fps.text = text;
	}

	private void OpenConsole()
	{
		console.gameObject.SetActive(value: true);
		console.Select();
		console.ActivateInputField();
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		PlayerMovement.Instance.paused = true;
		Time.timeScale = 0f;
	}

	private void CloseConsole()
	{
		console.gameObject.SetActive(value: false);
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		PlayerMovement.Instance.paused = false;
		Time.timeScale = 1f;
	}

	public void RunCommand()
	{
		string text = console.text;
		TextMeshProUGUI textMeshProUGUI = consoleLog;
		textMeshProUGUI.text = textMeshProUGUI.text + text + "\n";
		if (text.Length < 2 || text.Length > 30 || CountWords(text) != 2)
		{
			console.text = "";
			console.Select();
			console.ActivateInputField();
			return;
		}
		console.text = "";
		string s = text.Substring(text.IndexOf(' ') + 1);
		string text2 = text.Substring(0, text.IndexOf(' '));
		if (!int.TryParse(s, out var result))
		{
			consoleLog.text += "Command not found\n";
			return;
		}
		switch (text2)
		{
		case "fps":
			OpenCloseFps(result);
			break;
		case "fpslimit":
			FpsLimit(result);
			break;
		case "fov":
			ChangeFov(result);
			break;
		case "sens":
			ChangeSens(result);
			break;
		case "speed":
			OpenCloseSpeed(result);
			break;
		case "help":
			Help();
			break;
		}
		console.Select();
		console.ActivateInputField();
	}

	private void Help()
	{
		string text = "The console can be used for simple commands.\nEvery command must be followed by number i (0 = false, 1 = true)\n<i><b>fps 1</b></i>            shows fps\n<i><b>speed 1</b></i>      shows speed\n<i><b>fov i</b></i>             sets fov to i\n<i><b>sens i</b></i>          sets sensitivity to i\n<i><b>fpslimit i</b></i>    sets max fps\n<i><b>TAB</b></i>              to open/close the console\n";
		consoleLog.text += text;
	}

	private void FpsLimit(int n)
	{
		Application.targetFrameRate = n;
		TextMeshProUGUI textMeshProUGUI = consoleLog;
		textMeshProUGUI.text = textMeshProUGUI.text + "Max FPS set to " + n + "\n";
	}

	private void OpenCloseFps(int n)
	{
		fpsOn = n == 1;
		consoleLog.text += "FPS set to " + n == 1 + "\n";
	}

	private void OpenCloseSpeed(int n)
	{
		speedOn = n == 1;
		consoleLog.text += "Speedometer set to " + n == 1 + "\n";
	}

	private void ChangeFov(int n)
	{
		GameState.Instance.SetFov(n);
		TextMeshProUGUI textMeshProUGUI = consoleLog;
		textMeshProUGUI.text = textMeshProUGUI.text + "FOV set to " + n + "\n";
	}

	private void ChangeSens(int n)
	{
		GameState.Instance.SetSensitivity(n);
		TextMeshProUGUI textMeshProUGUI = consoleLog;
		textMeshProUGUI.text = textMeshProUGUI.text + "Sensitivity set to " + n + "\n";
	}

	private int CountWords(string text)
	{
		int num = 0;
		int i;
		for (i = 0; i < text.Length && char.IsWhiteSpace(text[i]); i++)
		{
		}
		while (i < text.Length)
		{
			for (; i < text.Length && !char.IsWhiteSpace(text[i]); i++)
			{
			}
			num++;
			for (; i < text.Length && char.IsWhiteSpace(text[i]); i++)
			{
			}
		}
		return num;
	}
}
