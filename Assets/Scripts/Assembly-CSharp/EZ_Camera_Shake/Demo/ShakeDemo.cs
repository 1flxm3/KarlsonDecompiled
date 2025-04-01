using EZCameraShake;
using UnityEngine;

namespace EZ_Camera_Shake.Demo
{
	public class ShakeDemo : MonoBehaviour
	{
		private delegate float Slider(float val, string prefix, float min, float max, int pad);

		private Vector3 posInf = new Vector3(0.25f, 0.25f, 0.25f);

		private Vector3 rotInf = new Vector3(1f, 1f, 1f);

		private float magn = 1f;

		private float rough = 1f;

		private float fadeIn = 0.1f;

		private float fadeOut = 2f;

		private bool modValues;

		private bool showList;

		private CameraShakeInstance shake;

		private void OnGUI()
		{
			if (Input.GetKeyDown(KeyCode.R))
			{
				Application.LoadLevel(Application.loadedLevel);
			}
			Slider slider = delegate(float val, string prefix, float min, float max, int pad)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(prefix, GUILayout.MaxWidth(pad));
				val = GUILayout.HorizontalSlider(val, min, max);
				GUILayout.Label(val.ToString("F2"), GUILayout.MaxWidth(50f));
				GUILayout.EndHorizontal();
				return val;
			};
			GUI.Box(new Rect(10f, 10f, 250f, Screen.height - 15), "Make-A-Shake");
			GUILayout.BeginArea(new Rect(29f, 40f, 215f, Screen.height - 40));
			GUILayout.Label("--Position Infleunce--");
			posInf.x = slider(posInf.x, "X", 0f, 4f, 25);
			posInf.y = slider(posInf.y, "Y", 0f, 4f, 25);
			posInf.z = slider(posInf.z, "Z", 0f, 4f, 25);
			GUILayout.Label("--Rotation Infleunce--");
			rotInf.x = slider(rotInf.x, "X", 0f, 4f, 25);
			rotInf.y = slider(rotInf.y, "Y", 0f, 4f, 25);
			rotInf.z = slider(rotInf.z, "Z", 0f, 4f, 25);
			GUILayout.Label("--Other Properties--");
			magn = slider(magn, "Magnitude:", 0f, 10f, 75);
			rough = slider(rough, "Roughness:", 0f, 20f, 75);
			fadeIn = slider(fadeIn, "Fade In:", 0f, 10f, 75);
			fadeOut = slider(fadeOut, "Fade Out:", 0f, 10f, 75);
			GUILayout.Label("--Saved Shake Instance--");
			GUILayout.Label("You can save shake instances and modify their properties at runtime.");
			if (shake == null && GUILayout.Button("Create Shake Instance"))
			{
				shake = CameraShaker.Instance.StartShake(magn, rough, fadeIn);
				shake.DeleteOnInactive = false;
			}
			if (shake != null)
			{
				if (GUILayout.Button("Delete Shake Instance"))
				{
					shake.DeleteOnInactive = true;
					shake.StartFadeOut(fadeOut);
					shake = null;
				}
				if (shake != null)
				{
					GUILayout.BeginHorizontal();
					if (GUILayout.Button("Fade Out"))
					{
						shake.StartFadeOut(fadeOut);
					}
					if (GUILayout.Button("Fade In"))
					{
						shake.StartFadeIn(fadeIn);
					}
					GUILayout.EndHorizontal();
					modValues = GUILayout.Toggle(modValues, "Modify Instance Values");
					if (modValues)
					{
						shake.ScaleMagnitude = magn;
						shake.ScaleRoughness = rough;
						shake.PositionInfluence = posInf;
						shake.RotationInfluence = rotInf;
					}
				}
			}
			GUILayout.Label("--Shake Once--");
			GUILayout.Label("You can simply have a shake that automatically starts and stops too.");
			if (GUILayout.Button("Shake!"))
			{
				CameraShakeInstance cameraShakeInstance = CameraShaker.Instance.ShakeOnce(magn, rough, fadeIn, fadeOut);
				cameraShakeInstance.PositionInfluence = posInf;
				cameraShakeInstance.RotationInfluence = rotInf;
			}
			GUILayout.EndArea();
			GUI.Box(new Rect(height: showList ? (120f + (float)CameraShaker.Instance.ShakeInstances.Count * 130f) : 120f, x: Screen.width - 310, y: 10f, width: 300f), "Shake Instance List");
			GUILayout.BeginArea(new Rect(Screen.width - 285, 40f, 255f, Screen.height - 40));
			GUILayout.Label("All shake instances are saved and stacked as long as they are active.");
			showList = GUILayout.Toggle(showList, "Show List");
			if (showList)
			{
				int num = 1;
				foreach (CameraShakeInstance shakeInstance in CameraShaker.Instance.ShakeInstances)
				{
					string text = shakeInstance.CurrentState.ToString();
					GUILayout.Label("#" + num + ": Magnitude: " + shakeInstance.Magnitude.ToString("F2") + ", Roughness: " + shakeInstance.Roughness.ToString("F2"));
					GUILayout.Label("      Position Influence: " + shakeInstance.PositionInfluence);
					GUILayout.Label("      Rotation Influence: " + shakeInstance.RotationInfluence);
					GUILayout.Label("      State: " + text);
					GUILayout.Label("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
					num++;
				}
			}
			GUILayout.EndArea();
		}
	}
}
