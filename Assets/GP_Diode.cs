using UnityEngine;
using System.Collections;

public class GP_Diode : GamePiece {

	public int m_allowedElectronEntry = 0;

	public void OnElectricTrigger()
	{
		GameBoard gameBoard = GetGameBoard();
		if ( gameBoard != null )
		{
			int transformedElectronEntry = ( currentRot / 90 + m_allowedElectronEntry ) % 4;
			GamePieceData electron = gameBoard.GetElectron( d.x, d.y );
			for ( int i = 0; i < 5; ++i )
			{
				// Remove all unallowed electrons! :)
				if ( i != transformedElectronEntry )
				{
					electron[ i ] = 0;
				}
			}
		}
	}
}
