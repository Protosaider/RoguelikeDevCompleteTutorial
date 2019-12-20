using System;

[Flags]
public enum EDirection
{
	None = 0,
	Up = 1 << 0,
	Right = 1 << 1,
	Down = 1 << 2,
	Left = 1 << 3,
    UpRight = 1 << 4, //UpRight = Up | Right, 
    DownRight = 1 << 5, //DownRight = Down | Right,
	DownLeft = 1 << 6, //DownLeft = Down | Left,
	UpLeft = 1 << 7, //UpLeft = Up | Left,
	Diagonals = UpRight | DownRight | DownLeft | UpLeft,
	Cardinals = Up | Right | Down | Left,
}