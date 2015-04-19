using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamePiece : MonoBehaviour {

	public int x,y,r;
	public int currentRot;
	public int m_selected;
	public int[] m_pipe = {0,0,0,0};
	public int[] m_wire = {0,0,0,0, // north, east, south, west
		0,0,0,0}; // corners: north-east, south-east, south-west, north-west

	Animator m_anim;

	// Use this for initialization
	public virtual void Start () {
		m_anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		m_anim.SetBool ("selected", m_selected==1);
		m_anim.SetInteger ("rot", r);
		m_anim.SetInteger ("presentrot", currentRot);
	}

	public virtual void GameTick() {
	}

	public virtual void AffectBall(Ball b) {
	}
	
	public void Rotate(int rot) {
		// Stupid Check
		if ((rot % 90) > 0) {
			Debug.LogError("90 oly");
			return;
		}

		// Mooosh DATA	
		int step = -rot / 90;
		int[] nPipe = new int[4];
		int[] nWire = new int[8];
		for (int i=0; i<4; ++i) {
			nPipe[i] = m_pipe[(i+4+step)%4];
			nWire[i] = m_wire[(i+8+step)%8];
			nWire[i+4] = m_wire[(i+8+4+step)%8];
		}
		m_pipe = nPipe;
		m_wire = nWire;

		currentRot = rot;
		r = (r + rot + 360) % 360;
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.blue;

		if(m_pipe[0]!=0) Gizmos.DrawCube (transform.position + new Vector3(0.0f,0.25f,0.0f), new Vector3(0.1f, 0.4f, 0.1f));
		if(m_pipe[1]!=0) Gizmos.DrawCube (transform.position + new Vector3(0.25f,0.0f,0.0f), new Vector3(0.4f, 0.1f, 0.1f));
		if(m_pipe[2]!=0) Gizmos.DrawCube (transform.position + new Vector3(0.0f,-0.25f,0.0f), new Vector3(0.1f, 0.4f, 0.1f));
		if(m_pipe[3]!=0) Gizmos.DrawCube (transform.position + new Vector3(-0.25f,0.0f,0.0f), new Vector3(0.4f, 0.1f, 0.1f));

		Gizmos.color = Color.yellow;
		for (int i=0; i<4; ++i)
			if (m_wire [i] != 0) {
			Gizmos.DrawSphere (transform.position + Quaternion.AngleAxis (-5.0f - 90.0f * i, Vector3.forward) * Vector3.up * 0.4f, 0.05f);
			Gizmos.DrawSphere (transform.position + Quaternion.AngleAxis (-5.0f - 90.0f * i, Vector3.forward) * Vector3.up * 0.3f, 0.05f);
			}
		for (int i=4; i<8; ++i)
			if (m_wire [i] != 0) {
				Gizmos.DrawSphere (transform.position + Quaternion.AngleAxis (55.0f - 90.0f * i, Vector3.forward) * Vector3.up * 0.4f, 0.05f);
			Gizmos.DrawSphere (transform.position + Quaternion.AngleAxis (40.0f - 90.0f * i, Vector3.forward) * Vector3.up * 0.4f, 0.05f);
			}

		Gizmos.color = Color.magenta;
		
		if(m_pipe[0]!=0) Gizmos.DrawCube (transform.position + new Vector3(0.0f,0.25f,0.0f), new Vector3(0.1f, 0.4f, 0.1f));
		if(m_pipe[1]!=0) Gizmos.DrawCube (transform.position + new Vector3(0.25f,0.0f,0.0f), new Vector3(0.4f, 0.1f, 0.1f));
		if(m_pipe[2]!=0) Gizmos.DrawCube (transform.position + new Vector3(0.0f,-0.25f,0.0f), new Vector3(0.1f, 0.4f, 0.1f));
		if(m_pipe[3]!=0) Gizmos.DrawCube (transform.position + new Vector3(-0.25f,0.0f,0.0f), new Vector3(0.4f, 0.1f, 0.1f));
	}

}
