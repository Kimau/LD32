using UnityEngine;
using System.Collections;

public class GP_Relay : GamePiece {

	public int m_RelayControlElectronPosition = 0; // position of the electron that controls the relay
	public bool m_RelayActive = true;

	public void OnElectricTrigger()
	{
		GameBoard gameBoard = GetGameBoard();
		if ( gameBoard != null )
		{
			int transformedElectronPosition = ( currentRot / 90 + m_RelayControlElectronPosition ) % 4;
			GamePieceData electron = gameBoard.GetElectron( d.x, d.y );

			if ( electron[ transformedElectronPosition ] == 1 )
			{
				Debug.Log( "Toggling relay" );
				m_RelayActive = !m_RelayActive;
			}

			int electronPath0 = (transformedElectronPosition - 1) % 4;
			int electronPath1 = (transformedElectronPosition + 1) % 4;
			Debug.Log( electronPath0.ToString() + "," + electronPath1.ToString() );
			for ( int i = 0; i < 5; ++i )
			{
				if ( m_RelayActive )
				{
					if ( i != electronPath0 && i != electronPath1 )
					{
						electron[ i ] = 0;
					}
				}

				else
				{
					electron[ i ] = 0;
				}
			}
			gameBoard.SetElectron( d.x, d.y, electron );
		}
	}
}
