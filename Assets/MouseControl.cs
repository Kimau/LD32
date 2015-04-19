using UnityEngine;
using System.Collections;

public class MouseControl : MonoBehaviour {

	GameBoard m_board;
	GamePiece m_dragPiece;
	Vector3 m_draggPieceoffset = Vector3.zero;
	DropBin m_bin;
	Tooltip m_tip;
	SpawnerBase[] m_spawners;
	bool m_mouseDown;
	Vector3 m_startDrag;

	// Use this for initialization
	void Start () {
		m_board = FindObjectOfType<GameBoard> ();
		m_bin = FindObjectOfType<DropBin> ();
		m_spawners = FindObjectsOfType<SpawnerBase> ();
		m_tip = FindObjectOfType<Tooltip> ();

		SetTooltip (null);
	}

	void SetTooltip(GamePiece p) {
		if (m_tip == null)
			return;

		if (p == null) {
			m_tip.gameObject.SetActive(false);
			return;
		}

		m_tip.gameObject.SetActive(true);
		m_tip.m_FriendlyNameText.text = p.m_FriendlyName;
		m_tip.m_FriendlyDescText.text = p.m_FriendlyDesc;
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

				if ( m_dragPiece )
				{
					SpriteRenderer spriteRenderer = m_dragPiece.gameObject.GetComponent<SpriteRenderer>();
					if ( spriteRenderer )
					{
						Vector3 bounds = spriteRenderer.bounds.size;
						m_draggPieceoffset.x = (bounds.x - 1.0f) * 0.5f;
						m_draggPieceoffset.y = (bounds.y - 1.0f) * 0.5f;
					}
				}
			}
		}

		if (m_dragPiece) {
			m_dragPiece.transform.position = wp + m_draggPieceoffset;
		}

		// Mouse Buttons
		if (Input.GetMouseButtonDown (0)) {
			m_mouseDown = true;

			if(onBoard) {
				
				if(m_dragPiece) {
					DropDragPiece(x,y,wp);
				}
				else
					m_board.Select (x, y);

				m_startDrag = wp;
			}
			else 
			{
				foreach ( SpawnerBase spanwer in m_spawners ) 
				{
					if ( spanwer.m_box.OverlapPoint(wp) ) 
					{
						m_board.Select(-1,-1);

						m_startDrag = wp;
						m_dragPiece = spanwer.m_currPiece;
						m_dragPiece.transform.parent = null;
						spanwer.m_currPiece = null;

						SpriteRenderer spriteRenderer = m_dragPiece.gameObject.GetComponent<SpriteRenderer>();
						if ( spriteRenderer )
						{
							Vector3 bounds = spriteRenderer.bounds.size;
							m_draggPieceoffset.x = (bounds.x - 1.0f) * 0.5f;
							m_draggPieceoffset.y = (bounds.y - 1.0f) * 0.5f;
						}
					}
				}
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			m_mouseDown = false;

			if(m_dragPiece)
			{
				DropDragPiece (x,y,wp);
			}
		}

		if (Input.GetMouseButtonUp (1)) {
			if(m_dragPiece) {
				m_dragPiece.Rotate(-90);
			}
			else {
				if(onBoard)
					m_board.Select(x,y);
				m_board.RotateSelectedPiece(-90);
			}
		}

		// Keyboard Controls
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			if (m_dragPiece)
				m_dragPiece.Rotate (90);
			else 
				m_board.RotateSelectedPiece (90);
		
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			m_board.BroadcastMessage( "OnTriggerEvent" );
		}

		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			if (m_dragPiece)
				m_dragPiece.Rotate (-90);
			else 
				m_board.RotateSelectedPiece (-90);
		}

		if (Input.GetKeyUp (KeyCode.Q)) {
			m_board.transform.localScale *= 0.5f;
		}
		if (Input.GetKeyUp (KeyCode.E)) {
			m_board.transform.localScale *= 2.0f;
		}
		if (Input.GetKeyUp (KeyCode.W)) {
			m_board.transform.position += new Vector3(0.0f, -m_board.transform.localScale.z, 0.0f);
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			m_board.transform.position += new Vector3(0.0f, +m_board.transform.localScale.z, 0.0f);
		}
		if (Input.GetKeyUp (KeyCode.A)) {
			m_board.transform.position += new Vector3(-m_board.transform.localScale.z, 0.0f, 0.0f);
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			m_board.transform.position += new Vector3(+m_board.transform.localScale.z, 0.0f,0.0f);
		}

		// Tooltip
		SetTooltip (m_dragPiece);
	}

	void DropDragPiece (int x,int y, Vector3 wp)
	{
		if (m_board.Place (m_dragPiece, x, y))
			m_dragPiece = null;
		if (m_bin.m_box.OverlapPoint (wp)) {
			Destroy (m_dragPiece.gameObject);
			m_dragPiece = null;
		}
	}
}
