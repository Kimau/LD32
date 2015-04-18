using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(GamePiece))]
public class PieceEditor  : Editor  {

	public override void OnInspectorGUI() {


		foreach(var t in targets)
			InspectPipe(GamePiece)t);

	}

	public void InspectPipe(GamePiece p) {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(p.name);
		p.x = EditorGUILayout.IntField(p.x);
		p.y = EditorGUILayout.IntField(p.y);
		p.r = EditorGUILayout.IntField(p.r);
		EditorGUILayout.EndHorizontal();
		Rect r = GUILayoutUtility.GetRect (100, 100, 100, 100);
		Rect r1, r2;
		SplitRect (r, out r1, out r2);
		
		OnPipeGUI (r1, p, true);
		OnWireGUI (r2, p, true);
		
		
		if (GUI.changed)
			EditorUtility.SetDirty (target);
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

	public void OnPipeGUI(Rect r, GamePiece p, bool isInput = false) {
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
					p.m_pipe [i] ^= 1;
			}
		}

		for(int i=0; i<4; ++i)
			EditorGUI.DrawRect (rArr[i], (p.m_pipe[i]==1)?Color.blue:Color.black);
		EditorGUI.DrawRect (rArr[4], ((p.m_pipe[0] | p.m_pipe[1] | p.m_pipe[2] | p.m_pipe[3]) != 0)?Color.blue:Color.black);
	}

	public void OnWireGUI(Rect r, GamePiece p, bool isInput = false) {
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
					p.m_wire [i] ^= 1;
			}
		}

		EditorGUI.DrawRect (r, Color.grey);
		for(int i=0; i<8; ++i)
			EditorGUI.DrawRect (rArr[i], (p.m_wire[i]==1)?Color.yellow:Color.black);;
	}

	public override void OnPreviewGUI(Rect r, GUIStyle background) {
		if (r.width > r.height)
			r.width = r.height;

		OnPipeGUI (r, (GamePiece)target);
		OnWireGUI (r, (GamePiece)target);
	}

}
