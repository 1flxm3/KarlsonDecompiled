using TMPro;
using UnityEngine;

public class PlayUI : MonoBehaviour
{
	public TextMeshProUGUI[] maps;

	private void Start()
	{
		float[] times = SaveManager.Instance.state.times;
		for (int i = 0; i < maps.Length; i++)
		{
			MonoBehaviour.print("i: " + times[i]);
			maps[i].text = Timer.Instance.GetFormattedTime(times[i]);
		}
	}
}
