using UnityEngine;
using UnityEditor;

public class SoftObjectMenu : Editor
{
	[MenuItem ("GameObject/2D Object/2D Soft Object")]
	static void CreateSoftObject()
	{
		GameObject softObject = new GameObject("SoftObject");
		softObject.AddComponent<SoftSprite>();
		softObject.AddComponent<SoftObject>();
	}
}