using UnityEngine;
using UnityEngine.AI;

public class NavTest : MonoBehaviour
{
	private NavMeshAgent agent;

	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
	}

	private void Update()
	{
		if ((bool)PlayerMovement.Instance)
		{
			Vector3 position = PlayerMovement.Instance.transform.position;
			if (agent.isOnNavMesh)
			{
				agent.destination = position;
				MonoBehaviour.print("goin");
			}
		}
	}
}
