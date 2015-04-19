using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(GamePieceData))]
public class IngredientDrawer : PropertyDrawer {
	
	// Draw the property inside the given rect
	public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {
		position.height = 100;
		InspectPipeData (prop);
	}

	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
		return 100.0f;
	}

	public void InspectPipeData(SerializedProperty prop) {
		EditorGUILayout.BeginHorizontal();
		prop.FindPropertyRelative("x").intValue = EditorGUILayout.IntField(prop.FindPropertyRelative("x").intValue);
		prop.FindPropertyRelative("y").intValue = EditorGUILayout.IntField(prop.FindPropertyRelative("y").intValue);
		prop.FindPropertyRelative("r").intValue = EditorGUILayout.IntField(prop.FindPropertyRelative("r").intValue);
		EditorGUILayout.EndHorizontal();
		Rect r = GUILayoutUtility.GetRect (100, 100, 100, 100);
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
		Rect[] rArr = {
			new Rect (r.xMin + r.width * 0.4f, r.yMin, r.width * 0.2f, r.height * 0.6f),
			new Rect (r.xMin + r.width * 0.4f, r.yMin + r.height * 0.4f, r.width * 0.6f, r.height * 0.2f),
			new Rect (r.xMin + r.width * 0.4f, r.yMin + r.height * 0.4f, r.width * 0.2f, r.height * 0.6f),
			new Rect (r.xMin, r.yMin + r.height * 0.4f, r.width * 0.6f, r.height * 0.2f),
			new Rect (r.xMin + r.width * 0.4f, r.yMin + r.height * 0.4f, r.width * 0.2f, r.height * 0.2f)
		};
		
		if (isInput) {
			Rect[] rInputArr = {
				new Rect (r.xMin + r.width * 0.4f, r.yMin, r.width * 0.2f, r.height * 0.6f),
				new Rect (r.xMin + r.width * 0.4f, r.yMin + r.height * 0.4f, r.width * 0.6f, r.height * 0.2f),
				new Rect (r.xMin + r.width * 0.4f, r.yMin + r.height * 0.4f, r.width * 0.2f, r.height * 0.6f),
				new Rect (r.xMin, r.yMin + r.height * 0.4f, r.width * 0.6f, r.height * 0.2f)
			};
			
			for (int i=0; i<4; ++i) {
				if (GUI.Button (rInputArr [i], ""))
				{ 
					prop.GetArrayElementAtIndex(i).intValue ^= 1;
				}
			}
		}

		bool isPipe = false;
		for (int i=0; i<4; ++i) {
			if(prop.GetArrayElementAtIndex(i).intValue != 0) {
			EditorGUI.DrawRect (rArr [i], Color.blue);
				isPipe = true;
			}
			else {
				EditorGUI.DrawRect (rArr [i], Color.black);

			}
		}
		EditorGUI.DrawRect (rArr[4], (isPipe)?Color.blue:Color.black);
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
			for(int i=0; i<8; ++i) {
				if (GUI.Button (rArr [i], ""))
					prop.GetArrayElementAtIndex(i).intValue ^= 1;
			}
		}
		
		EditorGUI.DrawRect (r, Color.grey);
		for(int i=0; i<8; ++i)
			EditorGUI.DrawRect (rArr[i], (prop.GetArrayElementAtIndex(i).intValue==1)?Color.yellow:Color.black);;
	}
}
