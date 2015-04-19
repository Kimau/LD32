using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(PropSpawnItem))]
public class PropPairDrawer : PropertyDrawer {
	
	// Draw the property inside the given rect
	public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {
		Rect r1, r2;
		r1 = new Rect (position.xMin, position.yMin, position.width*0.15f, position.height);
		r2 = new Rect (position.xMin+position.width * 0.15f, position.yMin, position.width*0.85f, position.height);

		prop.FindPropertyRelative ("m_strength").floatValue = EditorGUI.FloatField (r1, prop.FindPropertyRelative ("m_strength").floatValue);
		EditorGUI.ObjectField (r2, prop.FindPropertyRelative ("m_piece"), new GUIContent(""));
	}
	
	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
		return EditorGUIUtility.singleLineHeight;
	}
}
