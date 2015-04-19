using UnityEngine;
using System.Collections;

public class GP_LoadMechanism : GamePiece {

	public void OnElectricTrigger()
	{
		Debug.Log( "Load mechanism triggered" );
		GameBoard gameBoard = GetGameBoard();
		if ( gameBoard != null )
		{
			gameBoard.BroadcastMessage( "OnLoadMechanism" );
		}
	}
}
