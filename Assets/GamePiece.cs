using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GPData : System.Object {
	
	public int x,y,r;
	
	public int[] m_pipe = {0,0,0,0};
	public int[] m_wire = {0,0,0,0, // north, east, south, west
		0,0,0,0}; // corners: north-east, south-east, south-west, north-west

}

public class GamePiece : MonoBehaviour {

	public GPData d; 
	public string m_FriendlyName = "Balls";
	public string m_FriendlyDesc = "Game ticks this object until it...";
	public bool m_isFixed = false;
	public int m_selected;
	Animator m_anim;

	GPData m_origD; // horrible hack

	// Use this for initialization
	public virtual void Start () {
		m_anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		m_anim.SetBool ("selected", m_selected==1);
		m_anim.SetInteger ("rot", d.r);

		RotateMoooshData();
	}

	public void OnDestroy()
	{
		if ( transform.parent != null )
		{
			GameBoard gameBoard = transform.parent.gameObject.GetComponent<GameBoard>() as GameBoard;
			if ( gameBoard != null )
			{
				gameBoard.SetElectron( d.x, d.y, GamePieceData.zero );
			}
		}
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

		SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		Vector3 bounds = spriteRenderer.bounds.size;
		if ( bounds.x > 1.1f || bounds.y > 1.1f )
		{
			Debug.Log( "Rotation cancelled due to size: " + bounds.ToString() );
			return;
		}

		d.r = (d.r + rot + 360) % 360;
		RotateMoooshData();
	}

	public void RotateMoooshData()
	{
		// FUCKING CLAIRE STOP BEING AN IDIOT!!! This rotation system is retarded
		if (m_origD == null) {
			m_origD = new GPData();
			m_origD.x = d.x;
			m_origD.y = d.y;
			m_origD.r = d.r;
			m_origD.m_pipe = new int[d.m_pipe.Length];
			m_origD.m_wire = new int[d.m_wire.Length];
			for (int i = 0; i < 4; ++i) {
				m_origD.m_pipe [i] = d.m_pipe[i];
				m_origD.m_wire [i] = d.m_wire[i];
				m_origD.m_wire [i + 4] = d.m_wire[i+4];
			}
		}

		// Mooosh DATA	
		int step = -d.r / 90;
		for (int i = 0; i < 4; ++i) {
			d.m_pipe [i] = m_origD.m_pipe [(i + 4 + step) % 4];
			d.m_wire [i] = m_origD.m_wire [(i + 4 + step) % 4];
			d.m_wire [i + 4] = m_origD.m_wire [(i + 4 + step) % 4 + 4];
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.blue;

		if (d == null)
			d = new GPData ();

		if (Application.isPlaying) {

			if (d.m_pipe [0] != 0)
				Gizmos.DrawCube (transform.position + new Vector3 (0.0f, 0.25f, 0.0f), new Vector3 (0.1f, 0.2f, 0.1f));
			if (d.m_pipe [1] != 0)
				Gizmos.DrawCube (transform.position + new Vector3 (0.25f, 0.0f, 0.0f), new Vector3 (0.2f, 0.1f, 0.1f));
			if (d.m_pipe [2] != 0)
				Gizmos.DrawCube (transform.position + new Vector3 (0.0f, -0.25f, 0.0f), new Vector3 (0.1f, 0.2f, 0.1f));
			if (d.m_pipe [3] != 0)
				Gizmos.DrawCube (transform.position + new Vector3 (-0.25f, 0.0f, 0.0f), new Vector3 (0.2f, 0.1f, 0.1f));

			Gizmos.color = Color.yellow;
			for (int i=0; i<4; ++i)
				if (d.m_wire [i] != 0) {
					Gizmos.DrawSphere (transform.position + Quaternion.AngleAxis (-5.0f - 90.0f * i, Vector3.forward) * Vector3.up * 0.4f, 0.05f);
					Gizmos.DrawSphere (transform.position + Quaternion.AngleAxis (-5.0f - 90.0f * i, Vector3.forward) * Vector3.up * 0.3f, 0.05f);
				}
			for (int i=4; i<8; ++i)
				if (d.m_wire [i] != 0) {
					Gizmos.DrawSphere (transform.position + Quaternion.AngleAxis (55.0f - 90.0f * i, Vector3.forward) * Vector3.up * 0.4f, 0.05f);
					Gizmos.DrawSphere (transform.position + Quaternion.AngleAxis (40.0f - 90.0f * i, Vector3.forward) * Vector3.up * 0.4f, 0.05f);
				}
		} else {
			if (d.m_pipe [0] != 0)
				Gizmos.DrawCube (transform.position + transform.TransformDirection(new Vector3 (0.0f, 0.25f, 0.0f)), transform.TransformDirection(new Vector3 (0.1f, 0.2f, 0.1f)));
			if (d.m_pipe [1] != 0)
				                 Gizmos.DrawCube (transform.position + transform.TransformDirection(new Vector3 (0.25f, 0.0f, 0.0f)), transform.TransformDirection(new Vector3 (0.2f, 0.1f, 0.1f)));
			if (d.m_pipe [2] != 0)
				                 Gizmos.DrawCube (transform.position + transform.TransformDirection(new Vector3 (0.0f, -0.25f, 0.0f)), transform.TransformDirection(new Vector3 (0.1f, 0.2f, 0.1f)));
			if (d.m_pipe [3] != 0)
				                 Gizmos.DrawCube (transform.position + transform.TransformDirection(new Vector3 (-0.25f, 0.0f, 0.0f)), transform.TransformDirection(new Vector3 (0.2f, 0.1f, 0.1f)));
			
			Gizmos.color = Color.yellow;
			for (int i=0; i<4; ++i)
			if (d.m_wire [i] != 0) {
				Gizmos.DrawSphere (transform.position + transform.TransformDirection(Quaternion.AngleAxis (-5.0f - 90.0f * i, Vector3.forward) * Vector3.up * 0.4f), 0.05f);
				Gizmos.DrawSphere (transform.position + transform.TransformDirection(Quaternion.AngleAxis (-5.0f - 90.0f * i, Vector3.forward) * Vector3.up * 0.3f), 0.05f);
			}
			for (int i=4; i<8; ++i)
			if (d.m_wire [i] != 0) {
				Gizmos.DrawSphere (transform.position + transform.TransformDirection(Quaternion.AngleAxis (55.0f - 90.0f * i, Vector3.forward) * Vector3.up * 0.4f), 0.05f);
				Gizmos.DrawSphere (transform.position + transform.TransformDirection(Quaternion.AngleAxis (40.0f - 90.0f * i, Vector3.forward) * Vector3.up * 0.4f), 0.05f);
			}
		}
	}

	public GameBoard GetGameBoard()
	{
		if ( transform.parent != null )
		{
			return transform.parent.gameObject.GetComponent<GameBoard>() as GameBoard;
		}

		return null;
	}

}
