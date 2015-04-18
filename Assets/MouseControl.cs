using UnityEngine;
using System.Collections;

public class MouseControl : MonoBehaviour {

	GameBoard m_board;
	GamePiece m_dragPiece;
	bool m_mouseDown;
	Vector3 m_startDrag;

	// Use this for initialization
	void Start () {
		m_board = FindObjectOfType<GameBoard> ();

	}
	
	// Update is called once per frame
	void Update () {

		// Mouse Position
		Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		wp.z = -1.0f;
		Vector3 bp = m_board.transform.InverseTransformPoint(wp) + new Vector3(0.5f,0.5f,0.0f);
		
		int x = Mathf.FloorToInt(bp.x);
		int y = Mathf.FloorToInt(bp.y);
		bool onBoard = m_board.OnBoard (x, y);


		// Dragging
		if(m_mouseDown) {
			if((m_dragPiece == null) && (m_startDrag - wp).sqrMagnitude > 0.1f) {
				m_dragPiece = m_board.Pickup();	
			}
		}

		if(m_dragPiece)
			m_dragPiece.transform.position = wp;

		// Mouse Buttons
		if (Input.GetMouseButtonDown (0)) {
			m_mouseDown = true;

			if(onBoard) {
				
				if(m_dragPiece) {
					if(m_board.Place(m_dragPiece,x,y))
						m_dragPiece = null;
				}
				else
					m_board.Select (x, y);

				m_startDrag = wp;
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			m_mouseDown = false;

			if(m_dragPiece && m_board.Place(m_dragPiece,x,y))
				m_dragPiece = null;
		}


		// Keyboard Controls
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			if (m_dragPiece)
				m_dragPiece.Rotate (90);
			else 
				m_board.RotateSelectedPiece (90);
		}
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			if (m_dragPiece)
				m_dragPiece.Rotate (-90);
			else 
				m_board.RotateSelectedPiece (-90);
		}
	}
}
