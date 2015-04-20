using UnityEngine;
using System.Collections;

public class GP_Diode : GamePiece {

	public int m_allowedElectronEntry = 0;

	public void OnElectricTrigger()
	{
		GameBoard gameBoard = GetGameBoard();
		if ( gameBoard != null )
		{
			Debug.Log( "Diode" );
			int transformedElectronEntry = ( d.r / 90 + m_allowedElectronEntry ) % 4;
			GamePieceData electron = gameBoard.GetElectron( d.x, d.y );
			Debug.Log( transformedElectronEntry.ToString() );
			for ( int i = 0; i < 5; ++i )
			{
				// Remove all unallowed electrons! :)
				if ( i != transformedElectronEntry )
				{
					electron[ i ] = 0;
				}

				gameBoard.SetElectron( d.x, d.y, electron );
			}
		}
	}
}
