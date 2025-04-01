using UnityEngine;

[ExecuteInEditMode]
public class TextureScaling : MonoBehaviour
{
	private Vector3 _currentScale;

	public float size = 1f;

	private void Start()
	{
		Calculate();
	}

	private void Update()
	{
		Calculate();
	}

	public void Calculate()
	{
		if (!(_currentScale == base.transform.localScale) && !CheckForDefaultSize())
		{
			_currentScale = base.transform.localScale;
			Mesh mesh = GetMesh();
			mesh.uv = SetupUvMap(mesh.uv);
			mesh.name = "Cube Instance";
			if (GetComponent<Renderer>().sharedMaterial.mainTexture.wrapMode != 0)
			{
				GetComponent<Renderer>().sharedMaterial.mainTexture.wrapMode = TextureWrapMode.Repeat;
			}
		}
	}

	private Mesh GetMesh()
	{
		return GetComponent<MeshFilter>().mesh;
	}

	private Vector2[] SetupUvMap(Vector2[] meshUVs)
	{
		float x = _currentScale.x * size;
		float num = _currentScale.z * size;
		float y = _currentScale.y * size;
		meshUVs[2] = new Vector2(0f, y);
		meshUVs[3] = new Vector2(x, y);
		meshUVs[0] = new Vector2(0f, 0f);
		meshUVs[1] = new Vector2(x, 0f);
		meshUVs[7] = new Vector2(0f, 0f);
		meshUVs[6] = new Vector2(x, 0f);
		meshUVs[11] = new Vector2(0f, y);
		meshUVs[10] = new Vector2(x, y);
		meshUVs[19] = new Vector2(num, 0f);
		meshUVs[17] = new Vector2(0f, y);
		meshUVs[16] = new Vector2(0f, 0f);
		meshUVs[18] = new Vector2(num, y);
		meshUVs[23] = new Vector2(num, 0f);
		meshUVs[21] = new Vector2(0f, y);
		meshUVs[20] = new Vector2(0f, 0f);
		meshUVs[22] = new Vector2(num, y);
		meshUVs[4] = new Vector2(x, 0f);
		meshUVs[5] = new Vector2(0f, 0f);
		meshUVs[8] = new Vector2(x, num);
		meshUVs[9] = new Vector2(0f, num);
		meshUVs[13] = new Vector2(x, 0f);
		meshUVs[14] = new Vector2(0f, 0f);
		meshUVs[12] = new Vector2(x, num);
		meshUVs[15] = new Vector2(0f, num);
		return meshUVs;
	}

	private bool CheckForDefaultSize()
	{
		if (_currentScale != Vector3.one)
		{
			return false;
		}
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		UnityEngine.Object.DestroyImmediate(GetComponent<MeshFilter>());
		base.gameObject.AddComponent<MeshFilter>();
		GetComponent<MeshFilter>().sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
		UnityEngine.Object.DestroyImmediate(gameObject);
		return true;
	}
}
