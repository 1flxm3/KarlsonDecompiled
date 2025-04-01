using Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{
	private void Start()
	{
	}

	public void LoadMap(string s)
	{
		if (!s.Equals(""))
		{
			SceneManager.LoadScene(s);
			Game.Instance.StartGame();
		}
	}

	public void Exit()
	{
		Application.Quit(0);
	}

	public void ButtonSound()
	{
		AudioManager.Instance.Play("Button");
	}

	public void Youtube()
	{
		Application.OpenURL("https://youtube.com/danidev");
	}

	public void Twitter()
	{
		Application.OpenURL("https://twitter.com/DaniDevYT");
	}

	public void Facebook()
	{
		Application.OpenURL("https://www.facebook.com/DWSgames");
	}

	public void Discord()
	{
		Application.OpenURL("https://discord.gg/P53pFtR");
	}

	public void Steam()
	{
		Application.OpenURL("https://store.steampowered.com/app/1228610/Karlson");
	}

	public void EvanYoutube()
	{
		Application.OpenURL("https://www.youtube.com/user/EvanKingAudio");
	}

	public void EvanTwitter()
	{
		Application.OpenURL("https://twitter.com/EvanKingAudio");
	}
}
