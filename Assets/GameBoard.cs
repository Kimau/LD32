using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour {

	public int m_width;
	public int m_height;
	public List<Ball> m_ballsOnBoard;
	public List<GP_Handle> m_handlesOnBoard;
	GamePiece[] m_board;
	GamePiece m_sel;
	GamePieceData[] m_electronPositions;
	float m_timeSinceTick = 0.0f;
	float m_electronTickTime = 0.0f;

	public bool IsPieceSelected() {
		return (m_sel != null);
	}


	public void SnapePiecesFromTransforms() {
		GamePiece[] pArr = GetComponentsInChildren<GamePiece>();
		for (int i = 0; i < pArr.Length; i++) {
			pArr[i].transform.localPosition = new Vector3(
				Mathf.Round(pArr[i].transform.localPosition.x),
				Mathf.Round(pArr[i].transform.localPosition.y),
				0.0f);
			pArr[i].d.x = Mathf.FloorToInt(pArr[i].transform.localPosition.x);
			pArr[i].d.y = Mathf.FloorToInt(pArr[i].transform.localPosition.y);
			pArr[i].m_isFixed = true;

			Debug.Log (pArr[i].d.x + ":" + pArr[i].d.y);
		}
	}

	// Use this for initialization
	void Start () {	
		m_board = new GamePiece[m_width*m_height];
		m_electronPositions = new GamePieceData[ m_width * m_height ];
		for ( int i = 0; i < (m_width*m_height); ++i )
		{
			m_electronPositions[i] = new GamePieceData();
		}

		// Get premade pieces
		SnapePiecesFromTransforms ();
		GamePiece[] pArr = GetComponentsInChildren<GamePiece> ();
		for (int i = 0; i < pArr.Length; i++) {
			if(Place(pArr[i], pArr[i].d.x, pArr[i].d.y) == false)
				Destroy(pArr[i].gameObject); // Destroy on fail
		}
	}

	void Update () {
		m_timeSinceTick += Time.deltaTime;
		m_electronTickTime += Time.deltaTime;

		while ( m_electronTickTime > 1.0f )
		{
			m_electronTickTime -= 1.0f;
			TickElec();
		}

		while (m_timeSinceTick > 0.1f) {
			m_timeSinceTick -= 0.1f;

			for (int i = 0; i < m_board.Length; i++) {
				if(m_board[i])
					m_board[i].GameTick();
			}

			TickBalls ();
		}



	}

	public void EventBallSpawn(Ball b) {
		m_ballsOnBoard.Add (b);
	}

	static Vector3[] DIR;	// BECAUSE I HATE MYSELF DAMNIT - Claire

	public void TickBalls() {
		if(DIR == null)
			DIR = new Vector3[]{
			new Vector3(0.0f, 0.1f, 0.0f),
			new Vector3(0.1f, 0.0f, 0.0f),
			new Vector3(0.0f,-0.1f, 0.0f),
			new Vector3(-0.1f, 0.0f, 0.0f)};

		Stack<int> destroyBall = new Stack<int> ();
		for (int i=0; i<m_ballsOnBoard.Count; ++i) {
			Ball b = m_ballsOnBoard[i];
			if(DoBall(b) == false) {
				Destroy(b.gameObject);
				destroyBall.Push(i);
			}
		}

		while (destroyBall.Count > 0)
			m_ballsOnBoard.RemoveAt (destroyBall.Pop ());
	}

	bool IsBallOnPipe (Vector3 bp, int bx, int by, out GamePiece p)
	{
		p = null;
		if (OnBoard (bx, by) == false)
			return false;
		if (m_board [bx + by * m_width] == null)
			return false;
		p = m_board [bx + by * m_width];
		if ((bp.y > 0.0f) && (p.d.m_pipe [0] == 0))
			return false;
		if ((bp.x > 0.0f) && (p.d.m_pipe [1] == 0))
			return false;
		if ((bp.y < 0.0f) && (p.d.m_pipe [2] == 0))
			return false;
		if ((bp.x < 0.0f) && (p.d.m_pipe [3] == 0))
			return false;
		return true;
	}

	public bool DoBall(Ball b) {
		Vector3 bp = transform.InverseTransformPoint(b.transform.position);
		int bx = Mathf.FloorToInt(bp.x+0.5f);
		int by = Mathf.FloorToInt(bp.y+0.5f);

		bp.x = bp.x - (bx*1.0f);
		bp.y = bp.y - (by*1.0f);

		GamePiece p;
		if (IsBallOnPipe (bp, bx, by, out p) == false)
			return false;

		p.AffectBall(b);

		// Check if we are moving into Unknown Area
		bp += DIR[b.m_travelDir];

		while (bp.y > 0.5f) {// Check North 
			by += 1; 
			bp.y -= 1.0f;
		}
		while (bp.x > 0.5f) {// Check East
			bx += 1;
			bp.x -= 1.0f;
		}
		if (bp.y < -0.5f) {// Check South 
			by -= 1;
			bp.y += 1.0f;
		}
		if (bp.x < -0.5f) {// Check West 
			bx -= 1;
			bp.x += 1.0f;
		}

		GamePiece np;
		if (IsBallOnPipe (bp, bx, by, out np)) { // Continue On
			b.transform.position += DIR [b.m_travelDir] * b.m_speed;
		}
		else {
			if (p != np)
			{
				if(np != null)
				{
					Destroy(np.gameObject);
					m_board[bx + by*m_width]=null;
				}
				return false; // BOOM
			}

			int left = (b.m_travelDir + 3) % 4;
			int right = (b.m_travelDir + 5) % 4;
			if (p.d.m_pipe [left] != 0) {
				b.m_travelDir = left;
				b.transform.position = np.transform.position + DIR [b.m_travelDir] * bp.magnitude;
			} else if (p.d.m_pipe [right] != 0) {
				b.m_travelDir = right;
				b.transform.position = np.transform.position + DIR [b.m_travelDir] * bp.magnitude;
			} else {
				return false;
			}
		}
		return true;
	}

	void outputToDirection( Vector2 piecePos, int dir )
	{
		Vector2[] kDirections =
		{
			new Vector2( 0.0f, 1.0f ),
			new Vector2( 1.0f, 0.0f ),
			new Vector2( 0.0f, -1.0f ),
			new Vector2( -1.0f, 0.0f )
		};

		Vector2 otherPiecePos = piecePos + kDirections[ dir ];
		
		if ( OnBoard( (int)otherPiecePos.x, (int)otherPiecePos.y ) && m_board[ (int)otherPiecePos.x + (int)otherPiecePos.y * m_width ] != null )
		{
			//GamePiece otherPiece = m_board[ (int)otherPiecePos.x + (int)otherPiecePos.y * m_width ];
			
			int inverseDir = ( dir + 6 ) % 4;
			
			//if ( otherPiece.m_wire[ inverseJ ] != 0 || false ) // no need for this check because of confirming own's line existence
			{
				m_electronPositions[ (int)otherPiecePos.x + (int)otherPiecePos.y * m_width ][inverseDir] = 1;
			}
		}
	}

	// Update electron movements on non diagonal tiles
	void UpdateElecSimple( Vector2 piecePos, GamePiece piece, GamePieceData electron )
	{
		for ( int i = 0; i < 5; ++i )
		{
			// if electron and wire exist in position, or center electron and any wire (center is an exception)
			if ( electron[i] != 0 && ( i == 4 || piece.d.m_wire[ i ] != 0 ) )
			{
				for ( int j = 0; j < 4; ++j )
				{
					// make sure electron isn't going backwards
					if ( i != j && piece.d.m_wire[ j ] != 0 )
					{
						// output to new tile
						outputToDirection( piecePos, j );
					}
				}
			}
		}
	}

	void UpdateElecDiagonal( Vector2 piecePos, GamePiece piece, GamePieceData electron )
	{
		//
		// Diagonal checks helper tables
		//

		Vector2[] kDirectionToPossibleWires =
		{
			// corners: north-east, south-east, south-west, north-west
			new Vector2( 0.0f, 3.0f ),
			new Vector2( 0.0f, 1.0f ),
			new Vector2( 1.0f, 2.0f ),
			new Vector2( 2.0f, 3.0f )
		};

		Vector2[] kWireToDirections =
		{
			// corners: north-east, south-east, south-west, north-west
			new Vector2( 0.0f, 1.0f ),
			new Vector2( 1.0f, 2.0f ),
			new Vector2( 2.0f, 3.0f ),
			new Vector2( 3.0f, 0.0f )
		};
		
		for ( int i = 0; i < 4; ++i )
		{
			// If electron is at position
			if ( electron[i] != 0 )
			{
				// For each possible diagonal
				for ( int j = 0; j < 2; ++j )
				{
					// if diagonal exists
					int wireTest = (int)kDirectionToPossibleWires[i][j];
					if ( piece.d.m_wire[ wireTest + 4 ] != 0 )
					{
						// There is a wire! Hurray. Now to get the output direction...
						if ( electron[4] != 0 )
						{
							// If center electron is set, output to both directions
							outputToDirection( piecePos, (int)kWireToDirections[wireTest][0] );
							outputToDirection( piecePos, (int)kWireToDirections[wireTest][1] );
						}
						
						else
						{
							// Output to one direction only
							int outputDirection = (int)kWireToDirections[wireTest][0];
							if ( outputDirection == i )
							{
								outputDirection = (int)kWireToDirections[wireTest][1];
							}
							
							outputToDirection( piecePos, outputDirection );
						}
					}
				}
			}
		}
	}

	// Update electrons logic (movement and event trigger to tiles for activation)
	void TickElec()
	{
		// Copy the data to avoid working on an updated tile
		GamePieceData[] electronPositionsCopy = m_electronPositions.Clone() as GamePieceData[];

		// Zero the main data
		for ( int i = 0; i < (m_width*m_height); ++i )
		{
			m_electronPositions[i] = GamePieceData.zero;
		}

		// Pos variable
		Vector2 piecePos = new Vector2();
		for ( int x = 0; x < m_width; ++x )
		{
			for ( int y = 0; y < m_height; ++y )
			{
				// Set the pos variable
				piecePos.x = x;
				piecePos.y = y;

				// Get the piece and electron
				GamePiece piece = m_board[ x + y * m_width ];
				GamePieceData electron = electronPositionsCopy[ x + y * m_width ];

				// Safety check piece actually exists
				if ( piece == null )
				{
					// No piece, reset electron
					for ( int i = 0; i < 5; ++i )
						electron[ i ] = 0;

					continue;
				}

				// Process Simple directions (non-diagonals)
				UpdateElecSimple( piecePos, piece, electron );

				// Process Diagonal directions
				UpdateElecDiagonal( piecePos, piece, electron );
			}
		}

		// Trigger all game pieces with new electrons on them
		for ( int i = 0; i < (m_width*m_height); ++i )
		{
			if ( m_board[i] != null ) // safety
			{
				GamePieceData elec = m_electronPositions[i];
				for ( int j = 0; j < 4; ++j )
				{
					// if any electron at all, send trigger once
					if ( elec[j] != 0 )
					{
						m_board[ i ].SendMessage( "OnElectricTrigger" );
						break;
					}
				}
			}
		}
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
		if (m_board [x + y * m_width].m_isFixed)
			return false;

		m_sel = m_board [x + y * m_width];
		m_sel.m_selected = 1;

		return true;
	}

	public GamePiece Pickup() {
		if (!m_sel || m_sel.m_isFixed)
			return null;

		SpriteRenderer spriteRenderer = m_sel.gameObject.GetComponent<SpriteRenderer>();
		Vector3 bounds = spriteRenderer.bounds.size;
		for ( int i = 0; i < (int)bounds.x; ++i )
		{
			for ( int j = 0; j < (int)bounds.y; ++j )
			{
				m_board [(m_sel.d.x+i) + (m_sel.d.y+j) * m_width] = null;
			}
		}

		m_board [m_sel.d.x + m_sel.d.y * m_width] = null;
		GamePiece p = m_sel;
		m_sel = null;

		if ( p.GetType() == typeof(GP_Handle) )
		{
			m_handlesOnBoard.Remove( p as GP_Handle );
		}

		p.m_selected = 0;
		p.transform.parent = null;
		p.name = "Piece_Float";
		return p;
	}

	public bool Place(GamePiece p, int x, int y) 
	{
		Vector3 offset = Vector3.zero;
		SpriteRenderer spriteRenderer = p.gameObject.GetComponent<SpriteRenderer>();
		Vector3 bounds = spriteRenderer.bounds.size;
		offset.x = (bounds.x - 1.0f) * 0.5f;
		offset.y = (bounds.y - 1.0f) * 0.5f;
		
		for ( int i = 0; i < (int)bounds.x; ++i )
		{
			for ( int j = 0; j < (int)bounds.y; ++j )
			{
				if (OnBoard (x + i, y + j) == false)
					return false;
				if (m_board [(x+i) + (y+j) * m_width] != null) 
					return false;
			}
		}

		p.transform.parent = transform;
		p.transform.localPosition = new Vector3(x + offset.x, y + offset.y, 0);
		p.d.x = x;
		p.d.y = y;
		p.m_selected = 0;
		p.name = "Piece" + x + "_" + y;

		for ( int i = 0; i < (int)bounds.x; ++i )
		{
			for ( int j = 0; j < (int)bounds.y; ++j )
			{
				m_board [(x+i) + (y+j) * m_width] = p;
			}
		}
		m_board[x + y * m_width] = p;

		if ( p.GetType() == typeof(GP_Handle) )
		{
			m_handlesOnBoard.Add( p as GP_Handle );
		}

		return true;
	}

	void OnDrawGizmos() {

		// SHOW GRID
		Gizmos.color = Color.white;
		Vector3 offset = Vector3.one * -0.5f;
		for (int x = 0; x <= m_width; ++x) {
			Gizmos.DrawLine(transform.TransformPoint(new Vector3(x,0,-10)+offset),
			                transform.TransformPoint(new Vector3(x,m_width,-10)+offset));
		}
		for (int y = 0; y <= m_height; ++y) {
			Gizmos.DrawLine(transform.TransformPoint(new Vector3(0,y,-10)+offset),
			                transform.TransformPoint(new Vector3(m_height,y,-10)+offset));
		}


		/// DEBUG ELECTRON STUFF
		if ( m_electronPositions != null && m_electronPositions.Length != 0 )
		{
			Gizmos.color = Color.cyan;

			for ( int i = 0; i < (m_width*m_height); ++i )
			{
				if ( m_board[i] != null )
				{
					for ( int j = 0; j < 5; ++j )
					{
						if ( m_electronPositions[i][j] != 0 )
						{
							float kOffsetSize = 0.25f;
							Vector3[] offsets =
							{
								new Vector3( 0.0f, kOffsetSize, 0.0f ),
								new Vector3( kOffsetSize, 0.0f, 0.0f ),

								new Vector3( 0.0f, -kOffsetSize, 0.0f ),
								new Vector3( -kOffsetSize, 0.0f, 0.0f ),

								Vector3.zero

							};
							Gizmos.DrawSphere( m_board[i].transform.position + offsets[j], 0.1f );
						}
					}
				}
			}

		}
	}

	public void SetElectron( int x, int y, GamePieceData electron )
	{
		if ( OnBoard( x, y ) )
		{
			m_electronPositions[ x + y * m_width ] = electron;
		}
	}

	public GamePieceData GetElectron( int x, int y )
	{
		if ( OnBoard( x, y ) )
		{
			return m_electronPositions[ x + y * m_width ];
		}

		Debug.LogError( "GetElectron (" + x.ToString() + "," + y.ToString() + ") is out of bounds." );

		return null;
	}

	bool EvaluateForFloorFill( Vector2 pos, bool[] floodFill )
	{
		if ( OnBoard( (int)pos.x, (int)pos.y ) == false )
			return false;

		if ( floodFill[ (int)pos.x + (int)pos.y * m_width ] )
			return false;

		if ( m_board[ (int)pos.x + (int)pos.y * m_width ] == null )
			return false;

		floodFill[ (int)pos.x + (int)pos.y * m_width ] = true;
		return true;

	}

	public void OnTriggerEvent()
	{
		// Flood fill to find anything that touches the handle so it isn't destroyed
		bool[] floodFill = new bool[ m_width * m_height ];
		List< Vector2 > positions = new List<Vector2>();
		foreach ( GP_Handle handle in m_handlesOnBoard )
		{
			floodFill[ handle.d.x + handle.d.y * m_width ] = true;
			positions.Add( new Vector2( handle.d.x, handle.d.y ) );
		}

		while ( positions.Count > 0 )
		{
			Vector2 pos = positions[0];
			positions.RemoveAt(0);

			Vector2 neighbourPos0 = new Vector2( pos.x, pos.y + 1.0f );
			if ( EvaluateForFloorFill( neighbourPos0, floodFill ) )
			{
				positions.Add( neighbourPos0 );
			}

			Vector2 neighbourPos1 = new Vector2( pos.x, pos.y - 1.0f );
			if ( EvaluateForFloorFill( neighbourPos1, floodFill ) )
			{
				positions.Add( neighbourPos1 );
			}

			Vector2 neighbourPos2 = new Vector2( pos.x + 1.0f, pos.y );
			if ( EvaluateForFloorFill( neighbourPos2, floodFill ) )
			{
				positions.Add( neighbourPos2 );
			}

			Vector2 neighbourPos3 = new Vector2( pos.x - 1.0f, pos.y );
			if ( EvaluateForFloorFill( neighbourPos3, floodFill ) )
			{
				positions.Add( neighbourPos3 );
			}
		}

		for ( int x = 0; x < m_width; ++x )
		{
			for ( int y = 0; y < m_height; ++y )
			{
				if ( floodFill[ x + y * m_width ] == false && m_board[ x + y * m_width ] != null )
				{
					Destroy( m_board[ x + y * m_width ].gameObject );
					m_board[ x + y * m_width ] = null;
				}
			}
		}
	}

}
