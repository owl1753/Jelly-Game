using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Unity에서는 opengl처럼 GL_TRIANGLE_STRIP, GL_TRIANGLE_FAN을 지원하지 않는듯함.
// 오직. GL_TRIANGLES 삼각형 조합만 지원하는갑다.

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class TestCustomMesh : MonoBehaviour
{
	// 버텍스 list
	public Vector3[] vertices;

	// 색상 정보 list
	public List<Color> listColor = new List<Color>();

	// 정점 인덱스 정보 list
	public int[] triangles;

	// UV 정보 list
	public Vector2[] uv;
	// 메쉬
	private Mesh mesh;
	

	public Material mat;

	public SoftBody softBody;

	public int xSize, ySize;

	public float offset;

	private void Start()
	{
		Generate();
		ApplyMesh();
	}
    private void Update()
    {
		ApplyMesh();
    }
    private void Generate()
	{
		// MeshFilter로 부터 메쉬 데이터 획득
		mesh = GetComponent<MeshFilter>().mesh;
		mesh.name = "Procedural Grid";

		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

		meshRenderer.material = mat;

		//meshRenderer.material.mainTextureScale = new Vector2(1f/(xSize + 1), 1f/(ySize + 1));
		meshRenderer.material.SetColor("_Color", Color.white);
		meshRenderer.material.SetTexture("Texture", Resources.Load("Textures/Heart") as Texture);

		xSize = softBody.col - 1;
		ySize = softBody.row - 1;

		vertices = new Vector3[(xSize + 1) * (ySize + 1)];
		uv = new Vector2[vertices.Length];
		for (int i = 0, y = 0; y <= ySize; y++)
		{
			for (int x = 0; x <= xSize; x++, i++)
			{
				vertices[i] = new Vector3(x, y);
				uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
			}
		}
		mesh.vertices = vertices;
		mesh.uv = uv;

		triangles = new int[xSize * ySize * 6];
		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
		{
			for (int x = 0; x < xSize; x++, ti += 6, vi++)
			{
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
				triangles[ti + 5] = vi + xSize + 2;
			}
		}
		mesh.triangles = triangles;
	}

	void ApplyMesh()
	{

        for (int i = 0; i < softBody.row; i++)
        {
            for (int j = 0; j < softBody.col; j++)
            {
				vertices[i*softBody.row + j] = softBody.points[i, j].transform.localPosition + ((softBody.points[i, j].transform.position-transform.position)*offset);
            }
        }

        
		mesh.Clear();

		// 정점정보와 컬러, 인덱스, UV정보 배열을 셋팅
		mesh.vertices = vertices;
		mesh.colors = listColor.ToArray();
		mesh.triangles = triangles;
		mesh.uv = uv;

		// 노말 벡터 계산
		mesh.RecalculateNormals();
	}
}