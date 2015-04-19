using UnityEngine;
using System.Collections;

public class GP_InfiniteBullet : GamePiece {
	public int m_shootDir;
	public Ball m_ballPrefab;
	public int m_ticksBetweenShots = 5;
	int m_ticksSinceSpawn = 0;

	// Use this for initialization
	public override void Start () {
		base.Start ();

		// HACK :: 
		for (int i=0; i<4; ++i)
			m_pipe [i] = 0;
		m_shootDir = Mathf.FloorToInt (Random.value * 4.0f);
		m_pipe [m_shootDir] = 1;
	}

	public override void GameTick() {
		m_ticksSinceSpawn += 1;
		if(m_ticksSinceSpawn >= m_ticksBetweenShots)
		{
			m_ticksSinceSpawn = 0;

			Ball b = Instantiate(m_ballPrefab).GetComponent<Ball>();
			b.transform.position = transform.position;
			b.m_travelDir = m_shootDir;
			b.m_speed = 1.0f;
			SendMessageUpwards("EventBallSpawn", b);
		}
	}
}
