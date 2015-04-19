using UnityEngine;
using System.Collections;

public class GP_Trigger : GamePiece {
	

	void OnTriggerEvent() 
	{
		if ( transform.parent != null )
		{
			GameBoard gameBoard = GetGameBoard();
			if ( gameBoard != null )
			{
				gameBoard.SetElectron( x, y, new GamePieceData( 1, 1, 1, 1, 1 ) );
			}
		}
	}
}