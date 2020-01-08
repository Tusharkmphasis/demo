using System;
using System.Collections;

[Serializable]
public class Tile 
{
	private int _x;
	private int _y;
	
	public int X
	{
		get {return this._x;}
		set {this._x = value;}
	}
	
	public int Y
	{
		get {return this._y;}
		set {this._y = value;}
	}
	
	public Tile(int x, int y)
	{
		this.X = x;
		this.Y = y;
	}
	
	public override bool Equals(Object obj)
	{
		if(obj == null) return false;
		
		Tile otherTile = obj as Tile;
		if(otherTile == null)
		{
			return false;	
		}
		else
		{
			return this.X == otherTile.X && this.Y == otherTile.Y;
		}
	}
	
	public override int GetHashCode() {
      return this.X ^ this.Y;
   	}
	
	public bool Equals(Tile t)
	{
		if(t == null)
		{
			return false;	
		}
		else
		{
			return this.X == t.X && this.Y == t.Y;
		}
	}
	
}
