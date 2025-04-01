using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
	public bool playing;

	public bool done;

	public static Game Instance { get; private set; }

	private void Start()
	{
		Instance = this;
		playing = false;
	}

	public void StartGame()
	{
		playing = true;
		done = false;
		Time.timeScale = 1f;
		UIManger.Instance.StartGame();
		Timer.Instance.StartTimer();
	}

	public void RestartGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		Time.timeScale = 1f;
		StartGame();
	}

	public void EndGame()
	{
		playing = false;
	}

	public void NextMap()
	{
		Time.timeScale = 1f;
		int buildIndex = SceneManager.GetActiveScene().buildIndex;
		if (buildIndex + 1 >= SceneManager.sceneCountInBuildSettings)
		{
			MainMenu();
			return;
		}
		SceneManager.LoadScene(buildIndex + 1);
		StartGame();
	}

	public void MainMenu()
	{
		playing = false;
		SceneManager.LoadScene("MainMenu");
		UIManger.Instance.GameUI(b: false);
		Time.timeScale = 1f;
	}

	public void Win()
	{
		playing = false;
		Timer.Instance.Stop();
		Time.timeScale = 0.05f;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		UIManger.Instance.WinUI(b: true);
		float timer = Timer.Instance.GetTimer();
		int num = int.Parse(SceneManager.GetActiveScene().name[0].ToString() ?? "");
		if (int.TryParse(SceneManager.GetActiveScene().name.Substring(0, 2) ?? "", out var result))
		{
			num = result;
		}
		float num2 = SaveManager.Instance.state.times[num];
		if (timer < num2 || num2 == 0f)
		{
			SaveManager.Instance.state.times[num] = timer;
			SaveManager.Instance.Save();
		}
		MonoBehaviour.print("time has been saved as: " + Timer.Instance.GetFormattedTime(timer));
		done = true;
	}
}
