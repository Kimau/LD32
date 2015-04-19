using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour {

	public GamePiece m_basePiece;
	public GameObject m_backboard;
	public int m_width;
	public int m_height;
	public GamePiece m_HACKHORRIDSHOOTER;
	public List<Ball> m_ballsOnBoard;
	GamePiece[] m_board;
	GamePiece m_sel;
	GamePieceData[] m_electronPositions;
	float m_timeSinceTick = 0.0f;

	// Use this for initialization
	void Start () {
		m_board = new GamePiece[m_width*m_height];
		m_electronPositions = new GamePieceData[ m_width * m_height ];
		for ( int i = 0; i < (m_width*m_height); ++i )
		{
			m_electronPositions[i] = new GamePieceData();
		}

		for (int x=0; x < m_width; ++x) {
			for (int y=0; y < m_height; ++y) {

				// HACK :: Insert one shooter
				if((x + y) == 0) {
					Place(m_HACKHORRIDSHOOTER,x,y);
				}
				else if(Random.value > 0.6) {
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

	void Update () {
		m_timeSinceTick += Time.deltaTime;

		while (m_timeSinceTick > 1.0f) {
			m_timeSinceTick -= 1.0f;

			TickElec();

			for (int i = 0; i < m_board.Length; i++) {
				if(m_board[i])
					m_board[i].GameTick();
			}

			TickBalls ();
		}

	}

	public void EventBallSpawn(Ball b) {
		m_ballsOnBoard.Add (b);
		Debug.Log ("Ball Added");
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
				Debug.Log("Destroy Ball");
				destroyBall.Push(i);
			}
		}

		while (destroyBall.Count > 0)
			m_ballsOnBoard.RemoveAt (destroyBall.Pop ());
	}

	public bool DoBall(Ball b) {
		Vector3 bp = transform.InverseTransformPoint(b.transform.position);
		int bx = Mathf.FloorToInt(bp.x+0.5f);
		int by = Mathf.FloorToInt(bp.y+0.5f);

		if (OnBoard (bx, by) == false)
			return false;
		
		if (m_board [bx + by * m_width] == null)
			return false;
		
		GamePiece p = m_board[bx+by*m_width];
		p.AffectBall(b);
		
		bp.x = bp.x - (bx*1.0f);
		bp.y = bp.y - (by*1.0f);
		
		if((bp.y > 0.0f) && (p.m_pipe [0] == 0))
			return false;
		if((bp.x > 0.0f) && (p.m_pipe [1] == 0))
			return false;
		if((bp.y < 0.0f) && (p.m_pipe [2] == 0))
			return false;
		if((bp.x < 0.0f) && (p.m_pipe [3] == 0))
			return false;
			
		b.transform.position += DIR[b.m_travelDir];
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

	void TickElec() {


		GamePieceData[] electronPositionsCopy = m_electronPositions.Clone() as GamePieceData[];

		for ( int i = 0; i < (m_width*m_height); ++i )
		{
			m_electronPositions[i] = GamePieceData.zero;
		}

		Vector2 piecePos = new Vector2();
		for ( int x = 0; x < m_width; ++x )
		{
			for ( int y = 0; y < m_height; ++y )
			{
				piecePos.x = x;
				piecePos.y = y;

				GamePiece piece = m_board[ x + y * m_width ];
				GamePieceData electron = electronPositionsCopy[ x + y * m_width ];

				// Simple directions (non-diagonals)
				for ( int i = 0; i < 5; ++i )
				{
					if ( electron[i] != 0 && ( i == 4 || piece.m_wire[ i ] != 0 ) )
					{
						for ( int j = 0; j < 4; ++j )
						{
							if ( i != j && piece.m_wire[ j ] != 0 )
							{
								outputToDirection( piecePos, j );
							}
						}
					}
				}

				// Diagonal checks
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
					// if electron at position
					if ( electron[i] != 0 )
					{
						// For each possible diagonal
						for ( int j = 0; j < 2; ++j )
						{
							// if diagonal exists
							int wireTest = (int)kDirectionToPossibleWires[i][j];
							if ( piece.m_wire[ wireTest + 4 ] != 0 )
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

	void OnDrawGizmos() {
		if ( m_electronPositions != null && m_electronPositions.Length != 0 )
		{
			Gizmos.color = Color.cyan;

			for ( int i = 0; i < (m_width*m_height); ++i )
			{
				for ( int j = 0; j < 4; ++j )
				{
					if ( m_electronPositions[i][j] != 0.0f )
					{
						float kOffsetSize = 0.25f;
						Vector3[] offsets =
						{
							new Vector3( 0.0f, kOffsetSize, 0.0f ),
							new Vector3( kOffsetSize, 0.0f, 0.0f ),

							new Vector3( 0.0f, -kOffsetSize, 0.0f ),
							new Vector3( -kOffsetSize, 0.0f, 0.0f )

						};
						Gizmos.DrawSphere( m_board[i].transform.position + offsets[j], 0.1f );
					}
				}
			}

		}
	}
}
