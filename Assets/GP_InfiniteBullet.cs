using UnityEngine;
using System.Collections;

public class GP_InfiniteBullet : GamePiece {
	public int m_shootDir;

	// Use this for initialization
	void Start () {
		// HACK :: 
		for (int i=0; i<4; ++i)
			m_pipe [i] = 0;
		m_shootDir = Mathf.FloorToInt (Random.value * 4.0f);
		m_pipe [m_shootDir] = 1;
	}
	
	// Update is called once per frame
	void Update () {

	}
}
