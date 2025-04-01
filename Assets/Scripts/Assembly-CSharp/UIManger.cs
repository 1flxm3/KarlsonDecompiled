using UnityEngine;

public class UIManger : MonoBehaviour
{
	public GameObject gameUI;

	public GameObject deadUI;

	public GameObject winUI;

	public static UIManger Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		gameUI.SetActive(value: false);
	}

	public void StartGame()
	{
		gameUI.SetActive(value: true);
		DeadUI(b: false);
		WinUI(b: false);
	}

	public void GameUI(bool b)
	{
		gameUI.SetActive(b);
	}

	public void DeadUI(bool b)
	{
		deadUI.SetActive(b);
	}

	public void WinUI(bool b)
	{
		winUI.SetActive(b);
		MonoBehaviour.print("setting win UI");
	}
}
