using UnityEngine;
using UnityEngine.SceneManagement;

public class Managers : MonoBehaviour
{
	public static Managers Instance { get; private set; }

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		SceneManager.LoadScene("MainMenu");
		Time.timeScale = 1f;
		Application.targetFrameRate = 240;
		QualitySettings.vSyncCount = 0;
	}
}
