using UnityEngine;
using System.Collections;

public class PieceSpawner : SpawnerBase {

	public GamePiece[] m_pieceBucket;

	// Update is called once per frame
	void Update () {
		if (m_currPiece == null)
			SpawnPiece ();
	}

	protected override void SpawnPiece ()
	{
		m_currPiece = Instantiate (m_pieceBucket [Mathf.FloorToInt (Random.value * m_pieceBucket.Length)]).GetComponent<GamePiece> ();
		m_currPiece.transform.parent = transform;
		m_currPiece.transform.position = transform.position;
	}
}
