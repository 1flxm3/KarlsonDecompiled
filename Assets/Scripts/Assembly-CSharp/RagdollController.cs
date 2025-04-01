using UnityEngine;
using UnityEngine.AI;

public class RagdollController : MonoBehaviour
{
	private CharacterJoint[] c;

	private Vector3[] axis;

	private Vector3[] anchor;

	private Vector3[] swingAxis;

	public GameObject hips;

	private float[] mass;

	public GameObject[] limbs;

	private bool isRagdoll;

	public Transform leftArm;

	public Transform rightArm;

	public Transform head;

	public Transform hand;

	public Transform hand2;

	private void Start()
	{
		MakeStatic();
	}

	private void LateUpdate()
	{
	}

	public void MakeRagdoll(Vector3 dir)
	{
		if (!isRagdoll)
		{
			UnityEngine.Object.Destroy(GetComponent<NavMeshAgent>());
			UnityEngine.Object.Destroy(GetComponent("NavTest"));
			isRagdoll = true;
			UnityEngine.Object.Destroy(GetComponent<Rigidbody>());
			GetComponentInChildren<Animator>().enabled = false;
			for (int i = 0; i < limbs.Length; i++)
			{
				AddRigid(i, dir);
				limbs[i].gameObject.layer = LayerMask.NameToLayer("Object");
				limbs[i].AddComponent(typeof(Object));
			}
		}
	}

	private void AddRigid(int i, Vector3 dir)
	{
		GameObject gameObject = limbs[i];
		Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
		rigidbody.mass = mass[i];
		rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		rigidbody.AddForce(dir);
		if (i != 0)
		{
			CharacterJoint characterJoint = gameObject.AddComponent<CharacterJoint>();
			characterJoint.autoConfigureConnectedAnchor = true;
			characterJoint.connectedBody = FindConnectedBody(i);
			characterJoint.axis = axis[i];
			characterJoint.anchor = anchor[i];
			characterJoint.swingAxis = swingAxis[i];
		}
	}

	private Rigidbody FindConnectedBody(int i)
	{
		int num = 0;
		if (i == 2)
		{
			num = 1;
		}
		if (i == 4)
		{
			num = 3;
		}
		if (i == 7)
		{
			num = 6;
		}
		if (i == 9)
		{
			num = 8;
		}
		if (i == 10)
		{
			num = 5;
		}
		return limbs[num].GetComponent<Rigidbody>();
	}

	private void MakeStatic()
	{
		int num = limbs.Length;
		c = new CharacterJoint[num];
		Rigidbody[] array = new Rigidbody[num];
		mass = new float[num];
		for (int i = 0; i < limbs.Length; i++)
		{
			array[i] = limbs[i].GetComponent<Rigidbody>();
			mass[i] = array[i].mass;
			c[i] = limbs[i].GetComponent<CharacterJoint>();
		}
		axis = new Vector3[num];
		anchor = new Vector3[num];
		swingAxis = new Vector3[num];
		for (int j = 0; j < c.Length; j++)
		{
			if (!(c[j] == null))
			{
				axis[j] = c[j].axis;
				anchor[j] = c[j].anchor;
				swingAxis[j] = c[j].swingAxis;
				UnityEngine.Object.Destroy(c[j]);
			}
		}
		Rigidbody[] array2 = array;
		for (int k = 0; k < array2.Length; k++)
		{
			UnityEngine.Object.Destroy(array2[k]);
		}
	}

	public bool IsRagdoll()
	{
		return isRagdoll;
	}
}
