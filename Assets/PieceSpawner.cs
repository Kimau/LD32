using UnityEngine;
using System.Collections;

public class PieceSpawner : MonoBehaviour {

	public GamePiece m_currPiece;
	public GamePiece[] m_pieceBucket;
	public BoxCollider2D m_box;

	// Use this for initialization
	void Start () {
		m_box = GetComponent<BoxCollider2D> ();


	}
	
	// Update is called once per frame
	void Update () {
		if (m_currPiece == null)
			SpawnPiece ();
	}

	void SpawnPiece ()
	{
		m_currPiece = Instantiate (m_pieceBucket [Mathf.FloorToInt (Random.value * m_pieceBucket.Length)]).GetComponent<GamePiece> ();
		m_currPiece.transform.parent = transform;
		m_currPiece.transform.position = transform.position;
	}
}
