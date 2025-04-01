using UnityEngine;
using UnityEngine.UI;

public class GE_UIResponder : MonoBehaviour
{
	public string m_PackageTitle = "-";

	public string m_TargetURL = "www.unity3d.com";

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Text Package Title");
		if (gameObject != null)
		{
			gameObject.GetComponent<Text>().text = m_PackageTitle;
		}
	}

	private void Update()
	{
	}

	public void OnButton_AssetName()
	{
		Application.OpenURL(m_TargetURL);
	}
}
