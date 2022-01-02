using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SoftSprite))]
public class SoftSpriteInspector : Editor
{
	SerializedProperty textureProp;
	SerializedProperty scaleProp;
	SerializedProperty materialProp;
	SerializedProperty pixelPerMeterProp;
	SerializedProperty densityProp;
	SerializedProperty colorProp;

	private Object textureVal;
	private Vector2 scaleVal;
	private Object materialVal;
	private float pixelPerMeterVal;
	private float densityVal;
	private Color colorVal;

	[MenuItem ("Component/Rendering/Soft Sprite")]
	static void AddSoftSprite()
	{
		Selection.activeGameObject.AddComponent<SoftSprite>();
	}

	void Awake()
	{
		Undo.undoRedoPerformed += UpdateSprite;
	}

	void OnEnable()
	{
		textureProp = serializedObject.FindProperty("Sprite");
		scaleProp = serializedObject.FindProperty("Scale");
		materialProp = serializedObject.FindProperty("SpriteMaterial");
		pixelPerMeterProp = serializedObject.FindProperty("PixelPerMeter");
		densityProp = serializedObject.FindProperty("Density");
		colorProp = serializedObject.FindProperty("Color");

		SetValues();
	}

	private void SetValues()
	{
		textureVal = textureProp.objectReferenceValue;
		scaleVal = scaleProp.vector2Value;
		materialVal = materialProp.objectReferenceValue;
		pixelPerMeterVal = pixelPerMeterProp.floatValue;
		densityVal = densityProp.floatValue;
		colorVal = colorProp.colorValue;
	}

	private bool CheckForUpdate()
	{
		if (Application.isPlaying)
			return false;
		if (textureProp.objectReferenceValue != textureVal || scaleProp.vector2Value != scaleVal || materialProp.objectReferenceValue != materialVal || 
		    pixelPerMeterProp.floatValue != pixelPerMeterVal || densityProp.floatValue != densityVal || colorProp.colorValue != colorVal)
		{
			SetValues();
			return true;
		}
		return false;
	}

	private void UpdateSprite()
	{
		var softSprite = (SoftSprite)target;
		if (softSprite != null && softSprite.gameObject.activeInHierarchy)
		{
			softSprite.ForceUpdate();
		}
	}

	private float GetMaxRadius()
	{
		var softSprite = (SoftSprite)target;
		if (softSprite.Sprite != null)
		{
			var size = new Vector3((softSprite.Sprite.width / softSprite.PixelPerMeter) * softSprite.Scale.x, (softSprite.Sprite.height / softSprite.PixelPerMeter) * softSprite.Scale.y);
			size = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
			return ((size.x > size.y) ? size.y : size.x) / 4f;
		}
		else
		{
			return 1f;
		}
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		if (textureProp.objectReferenceValue != null)
		{
			EditorGUILayout.TextArea(string.Empty, GUI.skin.horizontalSlider, GUILayout.Height(EditorGUIUtility.singleLineHeight));
			var rect = GUILayoutUtility.GetRect(160, 120, GUILayout.ExpandWidth(true));
			GUI.DrawTexture(rect, (Texture)textureProp.objectReferenceValue, ScaleMode.ScaleToFit);
			EditorGUILayout.TextArea(string.Empty, GUI.skin.horizontalSlider, GUILayout.Height(EditorGUIUtility.singleLineHeight));
		}
		EditorGUILayout.PropertyField(textureProp, new GUIContent("Texture"));
		scaleProp.vector2Value = EditorGUILayout.Vector2Field("Scale", scaleProp.vector2Value);
		EditorGUILayout.PropertyField(materialProp, new GUIContent("Material"));
		EditorGUILayout.Slider(pixelPerMeterProp, 1f, 100f, new GUIContent("Pixel Per Meter"));
		EditorGUILayout.Slider(densityProp, 0.05f, GetMaxRadius(), new GUIContent("Density"));
		colorProp.colorValue = EditorGUILayout.ColorField("Color", colorProp.colorValue);
		serializedObject.ApplyModifiedProperties();

		if (CheckForUpdate())
		{
			UpdateSprite();
		}

		if (GUILayout.Button("Update"))
		{
			UpdateSprite();
		}
	}
}