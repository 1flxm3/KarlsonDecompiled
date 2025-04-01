using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class SlowmoEffect : MonoBehaviour
{
	public Image blackFx;

	public PostProcessProfile pp;

	private ColorGrading cg;

	private float frequency;

	private float vel;

	private float hue;

	private float hueVel;

	private AudioDistortionFilter af;

	private AudioLowPassFilter lf;

	public static SlowmoEffect Instance { get; private set; }

	private void Start()
	{
		cg = pp.GetSetting<ColorGrading>();
		Instance = this;
	}

	private void Update()
	{
		if (!af || !lf)
		{
			return;
		}
		if (!Game.Instance.playing || !Camera.main)
		{
			if (cg.hueShift.value != 0f)
			{
				cg.hueShift.value = 0f;
			}
			return;
		}
		float timeScale = Time.timeScale;
		float num = (1f - timeScale) * 2f;
		if ((double)num > 0.7)
		{
			num = 0.7f;
		}
		blackFx.color = new Color(1f, 1f, 1f, num);
		float target = PlayerMovement.Instance.GetActionMeter();
		float target2 = 0f;
		if (timeScale < 0.9f)
		{
			target = 400f;
			target2 = -20f;
		}
		frequency = Mathf.SmoothDamp(frequency, target, ref vel, 0.1f);
		hue = Mathf.SmoothDamp(hue, target2, ref hueVel, 0.2f);
		if ((bool)af)
		{
			af.distortionLevel = num * 0.2f;
		}
		if ((bool)lf)
		{
			lf.cutoffFrequency = frequency;
		}
		if ((bool)cg)
		{
			cg.hueShift.value = hue;
		}
		if (!Game.Instance.playing)
		{
			cg.hueShift.value = 0f;
		}
	}

	public void NewScene(AudioLowPassFilter l, AudioDistortionFilter d)
	{
		lf = l;
		af = d;
	}
}
