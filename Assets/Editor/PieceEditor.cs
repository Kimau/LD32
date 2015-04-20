using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(GPData))]
public class PieceDrawer : PropertyDrawer {
	
	// Draw the property inside the given rect
	public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {
		position.height = 100;
		Color s = GUI.backgroundColor;
		InspectPipeData (position, prop);
		GUI.backgroundColor = Color.grey;
		position.y += 100;
		position.height = EditorGUIUtility.singleLineHeight;
		prop.FindPropertyRelative ("r").intValue = EditorGUI.IntSlider (position, "Rotation", prop.FindPropertyRelative ("r").intValue / 90, 0, 3) * 90;
		GUI.backgroundColor = s;
	}

	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
		return 100.0f + EditorGUIUtility.singleLineHeight;
	}

	public void InspectPipeData(Rect r, SerializedProperty prop) {
		Rect r1, r2;
		SplitRect (r, out r1, out r2);
		
		OnPipeGUI (r1, prop.FindPropertyRelative("m_pipe"), true);
		OnWireGUI (r2, prop.FindPropertyRelative("m_wire"), true);
		
		GUI.changed = false;
	}
	
	void SplitRect(Rect r, out Rect r1, out Rect r2) {
		
		r1 = r;
		r1.width *= 0.5f;
		if (r1.width > r1.height)
			r1.width = r1.height;
		if (r1.height > r1.width)
			r1.height = r1.width;
		
		r2 = r1;
		r1.x = r.width * 0.25f - r1.width * 0.5f;
		r2.x = r.width * 0.75f - r2.width * 0.5f;
	}
	
	public void OnPipeGUI(Rect r, SerializedProperty prop, bool isInput = false) {
		Rect[] rInputArr = {
			new Rect (r.xMin + r.width * 0.4f, r.yMin, r.width * 0.2f, r.height * 0.4f),
			new Rect (r.xMin + r.width * 0.6f, r.yMin + r.height * 0.4f, r.width * 0.4f, r.height * 0.2f),
			new Rect (r.xMin + r.width * 0.4f, r.yMin + r.height * 0.6f, r.width * 0.2f, r.height * 0.4f),
			new Rect (r.xMin, r.yMin + r.height * 0.4f, r.width * 0.4f, r.height * 0.2f)
		};
		
		if (isInput) {
			for (int i=0; i<4; ++i) {
				GUI.backgroundColor = (prop.GetArrayElementAtIndex (i).intValue == 0) ? Color.black : Color.blue;
				if (GUI.Button (rInputArr [i], "", GUI.skin.button)) {
					prop.GetArrayElementAtIndex (i).intValue ^= 1;
				}
			}
		} else {
				
			for (int i=0; i<4; ++i) {
				GUI.backgroundColor = (prop.GetArrayElementAtIndex (i).intValue == 0) ? Color.black : Color.blue;
				if (GUI.Button (rInputArr [i], "", GUI.skin.button))
				{
					}
			}
		}
	}
	
	public void OnWireGUI(Rect r, SerializedProperty prop, bool isInput = false) {
		Rect[] rArr = {
			Rect.MinMaxRect(r.xMin+r.width*0.45f, r.yMin+r.height*0.00f, r.xMin+r.width*0.55f, r.yMin+r.height*0.45f),
			Rect.MinMaxRect(r.xMin+r.width*0.55f, r.yMin+r.height*0.45f, r.xMin+r.width*1.00f, r.yMin+r.height*0.55f),
			Rect.MinMaxRect(r.xMin+r.width*0.45f, r.yMin+r.height*0.55f, r.xMin+r.width*0.55f, r.yMin+r.height*1.00f),
			Rect.MinMaxRect(r.xMin+r.width*0.00f, r.yMin+r.height*0.45f, r.xMin+r.width*0.45f, r.yMin+r.height*0.55f),
			new Rect(r.xMin+r.width*0.1f, r.yMin+r.height*0.1f, r.width*0.2f, r.height*0.2f),
			new Rect(r.xMin+r.width*0.7f, r.yMin+r.height*0.1f, r.width*0.2f, r.height*0.2f),
			new Rect(r.xMin+r.width*0.7f, r.yMin+r.height*0.7f, r.width*0.2f, r.height*0.2f),
			new Rect(r.xMin+r.width*0.1f, r.yMin+r.height*0.7f, r.width*0.2f, r.height*0.2f)
		};
		
		if (isInput) {
			for (int i=0; i<8; ++i) {
				GUI.backgroundColor = (prop.GetArrayElementAtIndex (i).intValue == 0) ? Color.black : Color.yellow;
				if (GUI.Button (rArr [i], ""))
					prop.GetArrayElementAtIndex (i).intValue ^= 1;
			}
		} else {
			for (int i=0; i<8; ++i) {
				GUI.backgroundColor = (prop.GetArrayElementAtIndex (i).intValue == 0) ? Color.black : Color.yellow;
				if (GUI.Button (rArr [i], ""))
				{}
			}
		}	
	}
}
