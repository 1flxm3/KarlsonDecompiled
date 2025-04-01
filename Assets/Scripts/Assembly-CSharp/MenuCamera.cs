using EZCameraShake;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
	private Vector3 startPos;

	private Vector3 options = new Vector3(0f, 3.6f, 8f);

	private Vector3 play = new Vector3(1f, 4.6f, 5.5f);

	private Vector3 about = new Vector3(1f, 5.5f, 5.5f);

	private Vector3 desiredPos;

	private Vector3 posVel;

	private Vector3 startRot;

	private Vector3 playRot;

	private Vector3 aboutRot;

	private Quaternion desiredRot;

	private void Start()
	{
		startPos = base.transform.position;
		desiredPos = startPos;
		options += startPos;
		play += startPos;
		about += startPos;
		CameraShaker.Instance.StartShake(1f, 0.04f, 0.1f);
		startRot = Vector3.zero;
		playRot = new Vector3(0f, 90f, 0f);
		aboutRot = new Vector3(-90f, 0f, 0f);
	}

	private void Update()
	{
		base.transform.position = Vector3.SmoothDamp(base.transform.position, desiredPos, ref posVel, 0.4f);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, desiredRot, Time.deltaTime * 4f);
	}

	public void Options()
	{
		desiredPos = options;
	}

	public void Main()
	{
		desiredPos = startPos;
		desiredRot = Quaternion.Euler(startRot);
	}

	public void Play()
	{
		desiredPos = play;
		desiredRot = Quaternion.Euler(playRot);
	}

	public void About()
	{
		desiredPos = about;
		desiredRot = Quaternion.Euler(aboutRot);
	}
}
