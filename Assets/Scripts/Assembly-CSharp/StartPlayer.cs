using UnityEngine;

public class StartPlayer : MonoBehaviour
{
	private void Awake()
	{
		for (int num = base.transform.childCount - 1; num >= 0; num--)
		{
			MonoBehaviour.print("removing child: " + num);
			base.transform.GetChild(num).parent = null;
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
