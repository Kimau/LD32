using UnityEngine;
using System.Collections;

[System.Serializable]
public class PropSpawnItem : System.Object {
	
	public float m_strength = 1.0f;
	public GamePiece m_piece = null;
}

public class PropSpawner : SpawnerBase {

	public PropSpawnItem[] m_pieceBucket;

	// Update is called once per frame
	void Update () {
		if (m_currPiece == null)
			SpawnPiece ();
	} 
	
	protected override void SpawnPiece ()
	{
		int i;
		float m_total = 0.0f;
		for (i = 0; i < m_pieceBucket.Length; i++) {
			m_total += m_pieceBucket[i].m_strength;
		}

		i = 0;
		float f = Random.value * m_total;
		while (f > m_pieceBucket[i].m_strength) {
			f -= m_pieceBucket[i].m_strength;
			i += 1;
			if(i >= m_pieceBucket.Length)
				i =0;
		}

		m_currPiece = Instantiate (m_pieceBucket[i].m_piece).GetComponent<GamePiece> ();
		m_currPiece.transform.parent = transform;
		m_currPiece.transform.position = transform.position;
	}
}
