using UnityEngine;

namespace EZCameraShake
{
	public static class CameraShakePresets
	{
		public static CameraShakeInstance Bump => new CameraShakeInstance(2.5f, 4f, 0.1f, 0.75f)
		{
			PositionInfluence = Vector3.one * 0.15f,
			RotationInfluence = Vector3.one
		};

		public static CameraShakeInstance Explosion => new CameraShakeInstance(5f, 10f, 0f, 1.5f)
		{
			PositionInfluence = Vector3.one * 0.25f,
			RotationInfluence = new Vector3(4f, 1f, 1f)
		};

		public static CameraShakeInstance Earthquake => new CameraShakeInstance(0.6f, 3.5f, 2f, 10f)
		{
			PositionInfluence = Vector3.one * 0.25f,
			RotationInfluence = new Vector3(1f, 1f, 4f)
		};

		public static CameraShakeInstance BadTrip => new CameraShakeInstance(10f, 0.15f, 5f, 10f)
		{
			PositionInfluence = new Vector3(0f, 0f, 0.15f),
			RotationInfluence = new Vector3(2f, 1f, 4f)
		};

		public static CameraShakeInstance HandheldCamera => new CameraShakeInstance(1f, 0.25f, 5f, 10f)
		{
			PositionInfluence = Vector3.zero,
			RotationInfluence = new Vector3(1f, 0.5f, 0.5f)
		};

		public static CameraShakeInstance Vibration => new CameraShakeInstance(0.4f, 20f, 2f, 2f)
		{
			PositionInfluence = new Vector3(0f, 0.15f, 0f),
			RotationInfluence = new Vector3(1.25f, 0f, 4f)
		};

		public static CameraShakeInstance RoughDriving => new CameraShakeInstance(1f, 2f, 1f, 1f)
		{
			PositionInfluence = Vector3.zero,
			RotationInfluence = Vector3.one
		};
	}
}
