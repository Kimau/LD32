using UnityEngine;
using System.Collections;

public class GameBoard : MonoBehaviour {

	public GamePiece m_basePiece;
	public GameObject m_backboard;
	public int m_width;
	public int m_height;
	GamePiece[] m_board;
	GamePiece m_sel;

	// Use this for initialization
	void Start () {
		m_board = new GamePiece[m_width*m_height];

		for (int x=0; x < m_width; ++x) {
			for (int y=0; y < m_height; ++y) {
				if(Random.value > 0.6) {
					GamePiece p = Instantiate (m_basePiece).GetComponent<GamePiece> ();
					if(Place(p,x,y) == false)
						Debug.LogError("Fuck piece didn't place");

					// HACK :: Random Stuff
					for(int i=0; i<4; ++i)
						p.m_pipe[i] = (Random.value > 0.3)?1:0;
					for(int i=0; i<8; ++i)
						p.m_wire[i] = (Random.value > 0.3)?1:0;
				}
			}
		}

		m_backboard.transform.parent = transform;
		m_backboard.transform.localScale = new Vector3 (m_width, m_height, 0.0f);
		m_backboard.transform.localPosition = m_backboard.transform.localScale * 0.5f - new Vector3(0.5f, 0.5f, 0.0f);

		transform.position = new Vector3 (m_width * -0.5f, m_height * -0.5f, 0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool OnBoard(int x, int y) {
		return ((x >= 0) && (y >= 0) && (x < m_width) && (y < m_height));
	}

	public void RotateSelectedPiece(int r) {
		if (m_sel)
			m_sel.Rotate (r);
	}


	public bool Select(int x, int y) {
		if(m_sel)
			m_sel.m_selected = 0;
		if (OnBoard (x, y) == false)
			return false;
		if (m_board [x + y * m_width] == null)
			return false;

		m_sel = m_board [x + y * m_width];
		m_sel.m_selected = 1;

		return true;
	}

	public GamePiece Pickup() {
		if (!m_sel)
			return null;

		m_board [m_sel.x + m_sel.y * m_width] = null;
		GamePiece p = m_sel;
		m_sel = null;

		p.transform.parent = null;
		p.name = "Piece_Float";
		return p;
	}

	public bool Place(GamePiece p, int x, int y) {
		if (OnBoard (x, y) == false)
			return false;
		if (m_board [x + y * m_width] != null) 
			return false;

		p.transform.parent = transform;
		p.transform.localPosition = new Vector3(x, y, 0);
		p.x = x;
		p.y = y;
		p.m_selected = 0;
		p.name = "Piece" + x + "_" + y;

		m_board[x + y * m_width] = p;
		return true;
	}
}
