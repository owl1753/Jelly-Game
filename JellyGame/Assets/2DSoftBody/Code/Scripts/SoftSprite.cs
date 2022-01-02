using UnityEngine;

[ExecuteInEditMode()]
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class SoftSprite : MonoBehaviour
{
	public Texture Sprite;
	public Vector2 Scale = Vector2.one;
	public Material SpriteMaterial;
	public float PixelPerMeter = 100f;
	[Range(0.05f, float.MaxValue)]
	public float Density;
	public Color Color = Color.white;

	private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;
	private MaterialPropertyBlock propertyBlock;
	private const string MainTexShaderField = "_MainTex";

	private void Awake()
	{
		meshFilter = GetComponent<MeshFilter>();
		meshRenderer = GetComponent<MeshRenderer>();
		CreateSpriteMaterial();
		CreateMesh();
	}

	private void CreateSpriteMaterial()
	{
		if (SpriteMaterial == null)
		{
			var shader = Shader.Find("2DSB/VertexColor");
			SpriteMaterial = new Material(shader);
			meshRenderer.sharedMaterial = SpriteMaterial;
		}
	}

	private void CreateMesh()
	{
		UpdateMesh();
		UpdateMaterialTexture();
	}

	private void UpdateMesh()
	{
		if (Sprite == null)
		{
			return;
		}

		var mesh = new Mesh();
		var size = new Vector3((Sprite.width / PixelPerMeter) * Scale.x, (Sprite.height / PixelPerMeter) * Scale.y);
		size = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
		var offset = size / 2f;
		var maxDensity = ((size.x > size.y) ? size.y : size.x) / 4f;
		if (Density == 0 || Density > maxDensity)
		{
			Density = maxDensity;
		}
		var jointsHorizontalCount = (int)Mathf.Round(size.x / Density / 2f) - 1;
		var jointsVerticalCount = (int)Mathf.Round(size.y / Density / 2f) - 1;
		var verticesCount = (jointsHorizontalCount + 1) * (jointsVerticalCount + 1);
		if (jointsHorizontalCount <= 0 || jointsVerticalCount <= 0 || verticesCount <= 0 || verticesCount >= 65535)
		{
			return;
		}
		var vertices = new Vector3[verticesCount];
		var triangles = new int[jointsHorizontalCount * jointsVerticalCount * 6];
		var uv = new Vector2[verticesCount];
		var colors = new Color[verticesCount];
		for (int y = 0, i = 0; y <= jointsVerticalCount; y++)
		{
			for (int x = 0; x <= jointsHorizontalCount; x++, i++)
			{
				vertices[i] = new Vector3(size.x * x / jointsHorizontalCount, size.y * y / jointsVerticalCount, 0) - offset;
				uv[i] = new Vector3(x / (float)jointsHorizontalCount, y / (float)jointsVerticalCount, 0);
				colors[i] = Color;
			}
		}

		for (int y = 0, ti = 0, vi = 0; y < jointsVerticalCount; y++, vi++)
		{
			for (int x = 0; x < jointsHorizontalCount; x++, ti+=6, vi++)
			{
				triangles[ti + 0] = vi;
				triangles[ti + 1] = vi + jointsHorizontalCount + 2;
				triangles[ti + 2] = vi + 1;
				triangles[ti + 3] = vi + jointsHorizontalCount + 1;
				triangles[ti + 4] = vi + jointsHorizontalCount + 2;
				triangles[ti + 5] = vi;
			}
		}

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.colors = colors;
		meshFilter.sharedMesh = mesh;
	}

	private void UpdateMaterialTexture()
	{
		CreateSpriteMaterial();
		UpdatePropertyBlock();
	}

	private void UpdatePropertyBlock()
	{
		if (Sprite == null)
			return;

		if (propertyBlock == null)
		{
			propertyBlock = new MaterialPropertyBlock();
		}
		meshRenderer.GetPropertyBlock(propertyBlock);
		propertyBlock.SetTexture(MainTexShaderField, Sprite);
		meshRenderer.SetPropertyBlock(propertyBlock);
	}

	public void ForceUpdate()
	{
		UpdateMesh();
		UpdateMaterialTexture();
	}
}