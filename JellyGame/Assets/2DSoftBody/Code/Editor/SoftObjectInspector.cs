using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SoftObject))]
public class SoftObjectInspector : Editor
{
	SerializedProperty fillingProp;
	SerializedProperty jointsRadiusProp;
	SerializedProperty distanceProp;
	SerializedProperty frequencyProp;
	SerializedProperty dampingRationProp;
	SerializedProperty autoMassProp;
	SerializedProperty massProp;
	SerializedProperty angularDragProp;
	SerializedProperty linearDragProp;
	SerializedProperty jointsPhysicMaterial;
	SerializedProperty jointsLayer;

	[MenuItem ("Component/Physics 2D/Soft Object")]
	static void AddSoftObject()
	{
		Selection.activeGameObject.AddComponent<SoftObject>();
	}

	void OnEnable()
	{
		fillingProp = serializedObject.FindProperty("Filling");
		jointsRadiusProp = serializedObject.FindProperty("JointRadius");
		distanceProp = serializedObject.FindProperty("Distance");
		frequencyProp = serializedObject.FindProperty("Frequency");
		dampingRationProp = serializedObject.FindProperty("DampingRation");
		autoMassProp = serializedObject.FindProperty("AutoMass");
		massProp = serializedObject.FindProperty("Mass");
		angularDragProp = serializedObject.FindProperty("AngularDrag");
		linearDragProp = serializedObject.FindProperty("LinearDrag");
		jointsPhysicMaterial = serializedObject.FindProperty("JointsPhysicMaterial");
		jointsLayer = serializedObject.FindProperty("JointsLayer");
	}

	private void LimitPropertyValue(SerializedProperty prop, float minValue, float maxValue)
	{
		if (prop.floatValue < minValue)
		{
			prop.floatValue = minValue;
		}
		if (prop.floatValue > maxValue)
		{
			prop.floatValue = maxValue;
		}
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUILayout.PropertyField(fillingProp, new GUIContent("Fill Type"));
		EditorGUILayout.Slider(jointsRadiusProp, 0.0001f, GetMaxRadius(), new GUIContent("Joint Radius"));
		distanceProp.floatValue = EditorGUILayout.FloatField(new GUIContent("Distance"), distanceProp.floatValue);
		LimitPropertyValue(distanceProp, 0.005f, 1000000f);
		frequencyProp.floatValue = EditorGUILayout.FloatField(new GUIContent("Frequency"), frequencyProp.floatValue);
		LimitPropertyValue(frequencyProp, 0, 1000000f);
		dampingRationProp.floatValue = EditorGUILayout.FloatField(new GUIContent("Damping Ratio"), dampingRationProp.floatValue);
		LimitPropertyValue(dampingRationProp, 0, 1f);
#if UNITY_5_3_OR_NEWER
		autoMassProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Use Auto Mass"), autoMassProp.boolValue);
#endif
		if (!autoMassProp.boolValue)
		{
			massProp.floatValue = EditorGUILayout.FloatField(new GUIContent("Mass"), massProp.floatValue);
			LimitPropertyValue(massProp, 0.0001f, 1000000f);
		}
		angularDragProp.floatValue = EditorGUILayout.FloatField(new GUIContent("Angular Drag"), angularDragProp.floatValue);
		LimitPropertyValue(angularDragProp, 0f, 1000000f);
		linearDragProp.floatValue = EditorGUILayout.FloatField(new GUIContent("Linear Drag"), linearDragProp.floatValue);
		LimitPropertyValue(linearDragProp, 0f, 1000000f);
		EditorGUILayout.PropertyField(jointsPhysicMaterial, new GUIContent("Joints Physic Material"));
		jointsLayer.intValue = EditorGUILayout.LayerField("Joints Layer", jointsLayer.intValue);
		serializedObject.ApplyModifiedProperties();
	}

	private float GetMaxRadius()
	{
		var softObject = (SoftObject)target;
		var meshFilter = softObject.GetComponent<MeshFilter>();
		if (meshFilter != null && meshFilter.sharedMesh != null)
		{
			var size = meshFilter.sharedMesh.bounds.size;
			return ((size.x > size.y) ? size.y : size.x) / 2f;
		}
		else
		{
			return 100f;
		}
	}

	private int GetVertsCount()
	{
		var softObject = (SoftObject)target;
		var meshFilter = softObject.GetComponent<MeshFilter>();
		if (meshFilter != null && meshFilter.sharedMesh != null)
		{
			return meshFilter.sharedMesh.vertices.Length;
		}
		else
		{
			return 4;
		}
	}
}