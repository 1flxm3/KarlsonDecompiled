using UnityEngine;

public class RotateObject : MonoBehaviour
{
	private void Update()
	{
		base.transform.Rotate(Vector3.right, 40f * Time.deltaTime);
	}
}
