using UnityEngine;
using System.Collections;

public class GamePieceData {
	private int[] _data = new int[5];

	public GamePieceData()
	{
		for ( int i = 0; i < 5; ++i )
		{
			_data[i] = 0;
		}
	}

	public GamePieceData( int n, int e, int s, int w, int c )
	{
		North = n;
		East = e;
		South = s;
		West = w;
		Center = c;
	}

	public int North
	{
		set { this._data[4] = value; }
		get { return this._data[0]; }
	}

	public int East
	{
		set { this._data[4] = value; }
		get { return this._data[1]; }
	}

	public int South
	{
		set { this._data[2] = value; }
		get { return this._data[2]; }
	}

	public int West
	{
		set { this._data[3] = value; }
		get { return this._data[3]; }
	}

	public int Center
	{
		set { this._data[4] = value; }
		get { return this._data[4]; }
	}
	
	public int this[int index]
	{
		get
		{
			return _data[index];
		}
		set
		{
			_data[index] = value;
		}
	}

	public static GamePieceData zero
	{
		get 
		{ 
			return new GamePieceData();
		}
	}
}
