using System;

public class Grid<T>
{
	public event Action<T> OnItemSettled;
	public T[] Items;
	public Int32 Width;
	public Int32 Height;

	public Int32 Index(Int32 x, Int32 y) => y * Width + x;

	public Grid(Int32 width, Int32 height)
	{
		Width = width;
		Height = height;
		Items = new T[width * height];
	}

	public Boolean IsOutside(Int32 x, Int32 y) => x < 0 || x >= Width || y < 0 || y >= Height;

	public virtual Boolean SetItemSafe(Int32 x, Int32 y, T item)
	{
		if (IsOutside(x, y))
			return false;

		Items[y * Width + x] = item;
		OnItemSettled?.Invoke(item);

		return true;
	}
	public virtual Boolean SetItem(Int32 x, Int32 y, T item)
	{
		Items[y * Width + x] = item;
		OnItemSettled?.Invoke(item);

		return true;
	}
	public virtual T GetItemSafe(Int32 x, Int32 y) => IsOutside(x, y) ? default : Items[y * Width + x];
	public virtual T GetItem(Int32 x, Int32 y) => Items[y * Width + x];

	/*
	public virtual Boolean SetItemSafe(Int32 x, Int32 y, T item)
	{
		if (IsOutside(x, y))
			return false;

        Items[Index(x, y)] = item;
		OnItemSettled?.Invoke(item);

		return true;
	}
    public virtual Boolean SetItem(Int32 x, Int32 y, T item)
	{
		Items[Index(x, y)] = item;
		OnItemSettled?.Invoke(item);

		return true;
	}
	public virtual T GetTileSafe(Int32 x, Int32 y) => IsOutside(x, y) ? default : Items[Index(x, y)];
	public virtual T GetTile(Int32 x, Int32 y) => Items[Index(x, y)];
	*/
	//public T this[Int32 x, Int32 y]
	//{
	//	get => Items[y * Width + x];
	//	set => Items[y * Width + x] = value;
	//}

}