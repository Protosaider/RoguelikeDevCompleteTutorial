using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FovMap : Grid<Boolean>
{
	public FovMap(Int32 width, Int32 height) : base(width, height) { }
}

public class LightMap : Grid<Single>
{
	public LightMap(Int32 width, Int32 height) : base(width, height) { }
}

public class FieldOfView : MonoBehaviour
{
	public FovMap Map;
	public LightMap LightMap; //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

	public FieldOfView(Int32 width, Int32 height)
	{
		Map = new FovMap(width, height);
		LightMap = new LightMap(width, height); //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	}

	public void Clear(Vector2Int origin, Int32 fovRadius) => Clear(origin.x, origin.y, fovRadius);

	public void Clear(Int32 originX, Int32 originY, Int32 fovRadius)
	{
		Map.SetItem(originX, originY, false);

		if (fovRadius <= 0)
			return;

		RectInt fovArea = new RectInt(0, 0, Map.Width - 1, Map.Height - 1);

		fovArea = fovArea.Intersection(
			new RectInt(originX - fovRadius - 1, originY - fovRadius - 1, fovRadius * 2 + 3, fovRadius * 2 + 3)
		);

		for (var y = fovArea.yMin; y <= fovArea.yMax; y++)
			for (var x = fovArea.xMin; x <= fovArea.xMax; x++)
			{
				Map.SetItem(x, y, false);
				LightMap.SetItem(x, y, 0); //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			}

	}

	public void Recompute(TileMap map, Vector2Int origin, Int32 fovRadius) =>
		Recompute(map, origin.x, origin.y, fovRadius);


    public void Recompute(TileMap map, Int32 originX, Int32 originY, Int32 fovRadius)
    {
        Map.SetItem(originX, originY, true);

        if (fovRadius <= 0)
            return;
		for (uint octant = 0; octant < 8; octant++)
			AdamMilazzoAlgorithm(octant, originX, originY, fovRadius, 1, new Vector2Int(1, 1), new Vector2Int(1, 0), map, Distances.ManhattanNormalized);
    }

	void AdamMilazzoAlgorithm(uint octant, Int32 originX, Int32 originY, int fovRadius, uint x, Vector2Int slopeTop,
		Vector2Int slopeBottom, TileMap map,
		Func<Vector2Int, Vector2Int, Int32> getDistance) =>
		AdamMilazzoAlgorithm(octant, (UInt32)originX, (UInt32)originY, fovRadius, x, new Slope((UInt32)slopeTop.x, (UInt32)slopeTop.y),
			new Slope((UInt32)slopeBottom.x, (UInt32)slopeBottom.y), map,
			(x0, y0, x1, y1) => getDistance(new Vector2Int(x0, y0), new Vector2Int(x1, y1)));

	void AdamMilazzoAlgorithm(uint octant, UInt32 originX, UInt32 originY, int fovRadius, uint x, Slope top,
		Slope bottom, TileMap map,
		Func<Int32, Int32, Int32, Int32, Int32> getDistance)
	{
        // throughout this function there are references to various parts of tiles. a tile's coordinates refer to its
        // center, and the following diagram shows the parts of the tile and the vectors from the origin that pass through
        // those parts. given a part of a tile with vector u, a vector v passes above it if v > u and below it if v < u
        //    g         center:        y / x
        // a------b   a top left:      (y*2+1) / (x*2-1)   i inner top left:      (y*4+1) / (x*4-1)
        // |  /\  |   b top right:     (y*2+1) / (x*2+1)   j inner top right:     (y*4+1) / (x*4+1)
        // |i/__\j|   c bottom left:   (y*2-1) / (x*2-1)   k inner bottom left:   (y*4-1) / (x*4-1)
        //e|/|  |\|f  d bottom right:  (y*2-1) / (x*2+1)   m inner bottom right:  (y*4-1) / (x*4+1)
        // |\|__|/|   e middle left:   (y*2) / (x*2-1)
        // |k\  /m|   f middle right:  (y*2) / (x*2+1)     a-d are the corners of the tile
        // |  \/  |   g top center:    (y*2+1) / (x*2)     e-h are the corners of the inner (wall) diamond
        // c------d   h bottom center: (y*2-1) / (x*2)     i-m are the corners of the inner square (1/2 tile width)
        //    h
        for (; x <= (uint)fovRadius; x++) // (x <= (uint)rangeLimit) == (rangeLimit < 0 || x <= rangeLimit)
        {
            // compute the Y coordinates of the top and bottom of the sector. we maintain that top > bottom
            uint topY;
            if (top.X == 1) // if top == ?/1 then it must be 1/1 because 0/1 < top <= 1/1. this is special-cased because top
            {              // starts at 1/1 and remains 1/1 as long as it doesn't hit anything, so it's a common case
                topY = x;
            }
            else // top < 1
            {
                // get the tile that the top vector enters from the left. since our coordinates refer to the center of the
                // tile, this is (x-0.5)*top+0.5, which can be computed as (x-0.5)*top+0.5 = (2(x+0.5)*top+1)/2 =
                // ((2x+1)*top+1)/2. since top == a/b, this is ((2x+1)*a+b)/2b. if it enters a tile at one of the left
                // corners, it will round up, so it'll enter from the bottom-left and never the top-left
                topY = ((x * 2 - 1) * top.Y + top.X) / (top.X * 2); // the Y coordinate of the tile entered from the left
                                                                    // now it's possible that the vector passes from the left side of the tile up into the tile above before
                                                                    // exiting from the right side of this column. so we may need to increment topY
                if (BlocksLight(x, topY, octant, originX, originY, map)) // if the tile blocks light (i.e. is a wall)...
                {
                    // if the tile entered from the left blocks light, whether it passes into the tile above depends on the shape
                    // of the wall tile as well as the angle of the vector. if the tile has does not have a beveled top-left
                    // corner, then it is blocked. the corner is beveled if the tiles above and to the left are not walls. we can
                    // ignore the tile to the left because if it was a wall tile, the top vector must have entered this tile from
                    // the bottom-left corner, in which case it can't possibly enter the tile above.
                    //
                    // otherwise, with a beveled top-left corner, the slope of the vector must be greater than or equal to the
                    // slope of the vector to the top center of the tile (x*2, topY*2+1) in order for it to miss the wall and
                    // pass into the tile above
                    if (top.GreaterOrEqual(x * 2, topY * 2 + 1) && !BlocksLight(x, topY + 1, octant, originX, originY, map))
                        topY++;
                }
                else // the tile doesn't block light
                {
                    // since this tile doesn't block light, there's nothing to stop it from passing into the tile above, and it
                    // does so if the vector is greater than the vector for the bottom-right corner of the tile above. however,
                    // there is one additional consideration. later code in this method assumes that if a tile blocks light then
                    // it must be visible, so if the tile above blocks light we have to make sure the light actually impacts the
                    // wall shape. now there are three cases: 1) the tile above is clear, in which case the vector must be above
                    // the bottom-right corner of the tile above, 2) the tile above blocks light and does not have a beveled
                    // bottom-right corner, in which case the vector must be above the bottom-right corner, and 3) the tile above
                    // blocks light and does have a beveled bottom-right corner, in which case the vector must be above the
                    // bottom center of the tile above (i.e. the corner of the beveled edge).
                    // 
                    // now it's possible to merge 1 and 2 into a single check, and we get the following: if the tile above and to
                    // the right is a wall, then the vector must be above the bottom-right corner. otherwise, the vector must be
                    // above the bottom center. this works because if the tile above and to the right is a wall, then there are
                    // two cases: 1) the tile above is also a wall, in which case we must check against the bottom-right corner,
                    // or 2) the tile above is not a wall, in which case the vector passes into it if it's above the bottom-right
                    // corner. so either way we use the bottom-right corner in that case. now, if the tile above and to the right
                    // is not a wall, then we again have two cases: 1) the tile above is a wall with a beveled edge, in which
                    // case we must check against the bottom center, or 2) the tile above is not a wall, in which case it will
                    // only be visible if light passes through the inner square, and the inner square is guaranteed to be no
                    // larger than a wall diamond, so if it wouldn't pass through a wall diamond then it can't be visible, so
                    // there's no point in incrementing topY even if light passes through the corner of the tile above. so we
                    // might as well use the bottom center for both cases.
                    uint ax = x * 2; // center
                    if (BlocksLight(x + 1, topY + 1, octant, originX, originY, map))
                        ax++; // use bottom-right if the tile above and right is a wall
                    if (top.Greater(ax, topY * 2 + 1))
                        topY++;
                }
            }

            uint bottomY;
            if (bottom.Y == 0) // if bottom == 0/?, then it's hitting the tile at Y=0 dead center. this is special-cased because
            {                 // bottom.Y starts at zero and remains zero as long as it doesn't hit anything, so it's common
                bottomY = 0;
            }
            else // bottom > 0
            {
                bottomY = ((x * 2 - 1) * bottom.Y + bottom.X) / (bottom.X * 2); // the tile that the bottom vector enters from the left
                                                                                // code below assumes that if a tile is a wall then it's visible, so if the tile contains a wall we have to
                                                                                // ensure that the bottom vector actually hits the wall shape. it misses the wall shape if the top-left corner
                                                                                // is beveled and bottom >= (bottomY*2+1)/(x*2). finally, the top-left corner is beveled if the tiles to the
                                                                                // left and above are clear. we can assume the tile to the left is clear because otherwise the bottom vector
                                                                                // would be greater, so we only have to check above
                if (bottom.GreaterOrEqual(x * 2, bottomY * 2 + 1) && BlocksLight(x, bottomY, octant, originX, originY, map) &&
                   !BlocksLight(x, bottomY + 1, octant, originX, originY, map))
                {
                    bottomY++;
                }
            }

            // go through the tiles in the column now that we know which ones could possibly be visible
            int wasOpaque = -1; // 0:false, 1:true, -1:not applicable
            for (uint y = topY; (int)y >= (int)bottomY; y--) // use a signed comparison because y can wrap around when decremented
            {
                if (fovRadius < 0 || getDistance(0, 0, (int)x, (int)y) <= fovRadius) // skip the tile if it's out of visual range
                {
                    bool isOpaque = BlocksLight(x, y, octant, originX, originY, map);
                    // every tile where topY > y > bottomY is guaranteed to be visible. also, the code that initializes topY and
                    // bottomY guarantees that if the tile is opaque then it's visible. so we only have to do extra work for the
                    // case where the tile is clear and y == topY or y == bottomY. if y == topY then we have to make sure that
                    // the top vector is above the bottom-right corner of the inner square. if y == bottomY then we have to make
                    // sure that the bottom vector is below the top-left corner of the inner square
                    bool isVisible =
                      isOpaque || ((y != topY || top.Greater(x * 4 + 1, y * 4 - 1)) && (y != bottomY || bottom.Less(x * 4 - 1, y * 4 + 1)));
                    // NOTE: if you want the algorithm to be either fully or mostly symmetrical, replace the line above with the
                    // following line (and uncomment the Slope.LessOrEqual method). the line ensures that a clear tile is visible
                    // only if there's an unobstructed line to its center. if you want it to be fully symmetrical, also remove
                    // the "isOpaque ||" part and see NOTE comments further down
                    // bool isVisible = isOpaque || ((y != topY || top.GreaterOrEqual(y, x)) && (y != bottomY || bottom.LessOrEqual(y, x)));
                    if (isVisible)
                        SetVisible(x, y, octant, originX, originY, fovRadius, getDistance);

                    // if we found a transition from clear to opaque or vice versa, adjust the top and bottom vectors
                    if (x != fovRadius) // but don't bother adjusting them if this is the last column anyway
                    {
                        if (isOpaque)
                        {
                            if (wasOpaque == 0) // if we found a transition from clear to opaque, this sector is done in this column,
                            {                  // so adjust the bottom vector upward and continue processing it in the next column
                                               // if the opaque tile has a beveled top-left corner, move the bottom vector up to the top center.
                                               // otherwise, move it up to the top left. the corner is beveled if the tiles above and to the left are
                                               // clear. we can assume the tile to the left is clear because otherwise the vector would be higher, so
                                               // we only have to check the tile above
                                uint nx = x * 2, ny = y * 2 + 1; // top center by default
                                                                 // NOTE: if you're using full symmetry and want more expansive walls (recommended), comment out the next line
                                if (BlocksLight(x, y + 1, octant, originX, originY, map))
                                    nx--; // top left if the corner is not beveled
                                if (top.Greater(nx, ny)) // we have to maintain the invariant that top > bottom, so the new sector
                                {                       // created by adjusting the bottom is only valid if that's the case
                                                        // if we're at the bottom of the column, then just adjust the current sector rather than recursing
                                                        // since there's no chance that this sector can be split in two by a later transition back to clear
                                    if (y == bottomY)
                                    { bottom = new Slope(nx, ny); break; } // don't recurse unless necessary
                                    else
										AdamMilazzoAlgorithm(octant, originX, originY, fovRadius, x + 1, top, new Slope(nx, ny), map, getDistance);
                                }
                                else // the new bottom is greater than or equal to the top, so the new sector is empty and we'll ignore
                                {    // it. if we're at the bottom of the column, we'd normally adjust the current sector rather than
                                    if (y == bottomY)
                                        return; // recursing, so that invalidates the current sector and we're done
                                }
                            }
                            wasOpaque = 1;
                        }
                        else
                        {
                            if (wasOpaque > 0) // if we found a transition from opaque to clear, adjust the top vector downwards
                            {
                                // if the opaque tile has a beveled bottom-right corner, move the top vector down to the bottom center.
                                // otherwise, move it down to the bottom right. the corner is beveled if the tiles below and to the right
                                // are clear. we know the tile below is clear because that's the current tile, so just check to the right
                                uint nx = x * 2, ny = y * 2 + 1; // the bottom of the opaque tile (oy*2-1) equals the top of this tile (y*2+1)
                                                                 // NOTE: if you're using full symmetry and want more expansive walls (recommended), comment out the next line
                                if (BlocksLight(x + 1, y + 1, octant, originX, originY, map))
                                    nx++; // check the right of the opaque tile (y+1), not this one
                                          // we have to maintain the invariant that top > bottom. if not, the sector is empty and we're done
                                if (bottom.GreaterOrEqual(nx, ny))
                                    return;
                                top = new Slope(nx, ny);
                            }
                            wasOpaque = 0;
                        }
                    }
                }
            }

            // if the column didn't end in a clear tile, then there's no reason to continue processing the current sector
            // because that means either 1) wasOpaque == -1, implying that the sector is empty or at its range limit, or 2)
            // wasOpaque == 1, implying that we found a transition from clear to opaque and we recursed and we never found
            // a transition back to clear, so there's nothing else for us to do that the recursive method hasn't already. (if
            // we didn't recurse (because y == bottomY), it would have executed a break, leaving wasOpaque equal to 0.)
            if (wasOpaque != 0)
                break;
        }
    }



	struct Slope // represents the slope Y/X as a rational number
	{
		public Slope(uint x, uint y) { X = x; Y = y; }

		public bool Greater(uint x, uint y) { return Y * x > X * y; } // this > y/x
		public bool GreaterOrEqual(uint x, uint y) { return Y * x >= X * y; } // this >= y/x
		public bool Less(uint x, uint y) { return Y * x < X * y; } // this < y/x
        public bool LessOrEqual(uint x, uint y) { return Y * x <= X * y; } // this <= y/x

        public readonly uint X, Y;
	}

	// NOTE: the code duplication between BlocksLight and SetVisible is for performance. don't refactor the octant
	// translation out unless you don't mind an 18% drop in speed
	bool BlocksLight(uint x, uint y, uint octant, UInt32 originX, UInt32 originY, TileMap map)
	{
		uint nx = originX, ny = originY;
		switch (octant)
		{
			case 0:
				nx += x;
				ny -= y;
				break;
			case 1:
				nx += y;
				ny -= x;
				break;
			case 2:
				nx -= y;
				ny -= x;
				break;
			case 3:
				nx -= x;
				ny -= y;
				break;
			case 4:
				nx -= x;
				ny += y;
				break;
			case 5:
				nx -= y;
				ny += x;
				break;
			case 6:
				nx += y;
				ny += x;
				break;
			case 7:
				nx += x;
				ny += y;
				break;
		}

		return map.IsOutside((int)nx, (int)ny) || !map.GetItem((int)nx, (int)ny).Cell.IsTransparent;
	}

	void SetVisible(uint x, uint y, uint octant, UInt32 originX, UInt32 originY, Int32 fovRadius, Func<Int32, Int32, Int32, Int32, Int32> getDistance)
	{
		uint nx = originX, ny = originY;
		switch (octant)
		{
			case 0:
				nx += x;
				ny -= y;
				break;
			case 1:
				nx += y;
				ny -= x;
				break;
			case 2:
				nx -= y;
				ny -= x;
				break;
			case 3:
				nx -= x;
				ny -= y;
				break;
			case 4:
				nx -= x;
				ny += y;
				break;
			case 5:
				nx -= y;
				ny += x;
				break;
			case 6:
				nx += y;
				ny += x;
				break;
			case 7:
				nx += x;
				ny += y;
				break;
		}

		Map.SetItemSafe((int)nx, (int)ny, true);
		LightMap.SetItemSafe((int)nx, (int)ny,
            UnityMath.Remap(getDistance((int)originX, (int)originY, (int)nx, (int)ny), 0, fovRadius, 1, 0));
    }


    //  public void Recompute(TileMap map, Int32 originX, Int32 originY, Int32 fovRadius)
    //  {
    //      Map.SetItem(originX, originY, true);

    //      if (fovRadius <= 0)
    //          return;

    //for (var octant = 0; octant < 8; octant++)
    //	ShadowCast(octant, originX, originY, fovRadius, 1, new Vector2Int(1, 1), new Vector2Int(1, 0), map, Distances.ManhattanNormalized);
    //  }

    void ShadowCast(int octant, Int32 originX, Int32 originY, int fovRadius, int x, Vector2Int slopeTop,
		Vector2Int slopeBottom, TileMap map,
		Func<Vector2Int, Vector2Int, Int32> getDistance) =>
		ShadowCast(octant, originX, originY, fovRadius, x, slopeTop, slopeBottom, map,
			(x0, y0, x1, y1) => getDistance(new Vector2Int(x0, y0), new Vector2Int(x1, y1)));

    void ShadowCast(int octant, Int32 originX, Int32 originY, int fovRadius, int x, Vector2Int slopeTop, Vector2Int slopeBottom, TileMap map,
		Func<Int32, Int32, Int32, Int32, Int32> getDistance)
    {
        for (; (uint)x <= (uint)fovRadius; x++) // rangeLimit < 0 || x <= rangeLimit
        {
            // compute the Y coordinates where the top vector leaves the column (on the right) and where the bottom vector
            // enters the column (on the left). this equals (x+0.5)*top+0.5 and (x-0.5)*bottom+0.5 respectively, which can
            // be computed like (x+0.5)*top+0.5 = (2(x+0.5)*top+1)/2 = ((2x+1)*top+1)/2 to avoid floating point math
            int topY = slopeTop.x == 1 ? x : ((x * 2 + 1) * slopeTop.y + slopeTop.x - 1) / (slopeTop.x * 2); // the rounding is a bit tricky, though
            int bottomY = slopeBottom.y == 0 ? 0 : ((x * 2 - 1) * slopeBottom.y + slopeBottom.x) / (slopeBottom.x * 2);

            int wasOpaque = -1; // 0:false, 1:true, -1:not applicable
            for (int y = topY; y >= bottomY; y--)
            {
                int tx = originX, ty = originY;
                switch (octant) // translate local coordinates to map coordinates
                {
                    case 0:
                        tx += x;
                        ty -= y;
                        break;
                    case 1:
                        tx += y;
                        ty -= x;
                        break;
                    case 2:
                        tx -= y;
                        ty -= x;
                        break;
                    case 3:
                        tx -= x;
                        ty -= y;
                        break;
                    case 4:
                        tx -= x;
                        ty += y;
                        break;
                    case 5:
                        tx -= y;
                        ty += x;
                        break;
                    case 6:
                        tx += y;
                        ty += x;
                        break;
                    case 7:
                        tx += x;
                        ty += y;
                        break;
                }

                //bool inRange = fovRadius < 0 || getDistance(originX, originY, tx, ty) <= fovRadius;
                bool inRange = !Map.IsOutside(tx, ty) && (fovRadius < 0 || getDistance(originX, originY, tx, ty) <= fovRadius);
    //            if (inRange)
				//{
				//	Map.SetItem(tx, ty, true);
				//	LightMap.SetItem(tx, ty,
				//		UnityMath.Remap(getDistance(originX, originY, tx, ty), 0, fovRadius, 1, 0));
    //            }
                // NOTE: use the next line instead if you want the algorithm to be symmetrical
                if (inRange && (y != topY || slopeTop.y * x >= slopeTop.x * y) && (y != bottomY || slopeBottom.y * x <= slopeBottom.x * y))
				{
					Map.SetItem(tx, ty, true);
					LightMap.SetItem(tx, ty,
						UnityMath.Remap(getDistance(originX, originY, tx, ty), 0, fovRadius, 1, 0));
                }

                bool isOpaque = !inRange || !map.GetItem(tx, ty).Cell.IsTransparent;
                if (x != fovRadius)
                {
                    if (isOpaque)
                    {
                        if (wasOpaque == 0) // if we found a transition from clear to opaque, this sector is done in this column, so
                        {                  // adjust the bottom vector upwards and continue processing it in the next column.
                            Vector2Int newBottomSlope = new Vector2Int(x * 2 - 1, y * 2 + 1); // (x*2-1, y*2+1) is a vector to the top-left of the opaque tile
                            if (!inRange || y == bottomY)
                            { slopeBottom = newBottomSlope; break; } // don't recurse unless we have to

							ShadowCast(octant, originX, originY, fovRadius, x + 1, slopeTop, newBottomSlope, map, getDistance);
						}
                        wasOpaque = 1;
                    }
                    else // adjust top vector downwards and continue if we found a transition from opaque to clear
                    {    // (x*2+1, y*2+1) is the top-right corner of the clear tile (i.e. the bottom-right of the opaque tile)
                        if (wasOpaque > 0)
                            slopeTop = new Vector2Int(x * 2 + 1, y * 2 + 1);
                        wasOpaque = 0;
                    }
                }
            }

            if (wasOpaque != 0)
                break; // if the column ended in a clear tile, continue processing the current sector
        }
    }


	//public void Recompute(TileMap map, Int32 originX, Int32 originY, Int32 fovRadius)
//{
//	Map.SetItem(originX, originY, true);

//	if (fovRadius <= 0)
//		return;

//	//Octant:  1 = NNW, 2 =NNE, 3=ENE, 4=ESE, 5=SSE, 6=SSW, 7=WSW, 8 = WNW
//       for (var i = 0; i < 8; i++)
//	{
//		ScanOctant(1, i, 1.0, 0.0, map, originX, originY, fovRadius);
//       }
//   }

//   /// <summary>
//   /// Examine the provided octant and calculate the visible cells within it.
//   /// </summary>
//   /// <param name="pDepth">Depth of the scan</param>
//   /// <param name="pOctant">Octant being examined</param>
//   /// <param name="pStartSlope">Start slope of the octant</param>
//   /// <param name="pEndSlope">End slope of the octant</param>
//   protected void ScanOctant(int pDepth, int pOctant, double pStartSlope, double pEndSlope, TileMap map, Int32 originX, Int32 originY, Int32 fovRadius)
//   {
//       int visrange2 = fovRadius * fovRadius;
//       int x = 0;
//       int y = 0;

//       switch (pOctant)
//       {
//           case 1: //nnw
//               y = originY - pDepth;
//               if (y < 0)
//                   return;

//               x = originX - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
//               if (x < 0)
//                   x = 0;

//               while (GetSlope(x, y, originX, originY, false) >= pEndSlope)
//               {
//                   if (GetVisDistance(x, y, originX, originY) <= visrange2)
//                   {
//                       if (!map.GetItem(x, y).Cell.IsTransparent) //current cell blocked
//                       {
//                           if (x - 1 >= 0 && map.GetItem(x - 1, y).Cell.IsTransparent) //prior cell within range AND open => incremenet the depth, adjust the endslope and recurse
//                               ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y + 0.5, originX, originY, false), map, originX, originY, fovRadius);
//                       }
//                       else
//                       {
//                           if (x - 1 >= 0 && !map.GetItem(x - 1, y).Cell.IsTransparent) //prior cell within range AND open => adjust the startslope
//                               pStartSlope = GetSlope(x - 0.5, y - 0.5, originX, originY, false);

//						Map.SetItem(x, y, true);

//						LightMap.SetItem(x, y,
//							UnityMath.Remap(GetVisDistance(x, y, originX, originY), 0, visrange2, 1, 0));
//					}
//                   }
//                   x++;
//               }
//               x--;
//               break;

//           case 2: //nne
//               y = originY - pDepth;
//               if (y < 0)
//                   return;

//               x = originX + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
//               if (x >= map.Width)
//                   x = map.Width - 1;

//               while (GetSlope(x, y, originX, originY, false) <= pEndSlope)
//               {
//                   if (GetVisDistance(x, y, originX, originY) <= visrange2)
//                   {
//                       if (!map.GetItem(x, y).Cell.IsTransparent)
//                       {
//                           if (x + 1 < map.Width && map.GetItem(x + 1, y).Cell.IsTransparent)
//                               ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y + 0.5, originX, originY, false), map, originX, originY, fovRadius);
//                       }
//                       else
//                       {
//                           if (x + 1 < map.Width && !map.GetItem(x + 1, y).Cell.IsTransparent)
//                               pStartSlope = -GetSlope(x + 0.5, y - 0.5, originX, originY, false);

//						Map.SetItem(x, y, true);
//						LightMap.SetItem(x, y,
//							UnityMath.Remap(GetVisDistance(x, y, originX, originY), 0, visrange2, 1, 0));
//					}
//                   }
//                   x--;
//               }
//               x++;
//               break;

//           case 3:
//               x = originX + pDepth;
//               if (x >= map.Width)
//                   return;

//               y = originY - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
//               if (y < 0)
//                   y = 0;

//               while (GetSlope(x, y, originX, originY, true) <= pEndSlope)
//               {
//                   if (GetVisDistance(x, y, originX, originY) <= visrange2)
//                   {
//                       if (!map.GetItem(x, y).Cell.IsTransparent)
//                       {
//                           if (y - 1 >= 0 && map.GetItem(x, y - 1).Cell.IsTransparent)
//                               ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y - 0.5, originX, originY, true), map, originX, originY, fovRadius);
//                       }
//                       else
//                       {
//                           if (y - 1 >= 0 && !map.GetItem(x, y - 1).Cell.IsTransparent)
//                               pStartSlope = -GetSlope(x + 0.5, y - 0.5, originX, originY, true);

//						Map.SetItem(x, y, true);
//						LightMap.SetItem(x, y,
//							UnityMath.Remap(GetVisDistance(x, y, originX, originY), 0, visrange2, 1, 0));
//					}
//                   }
//                   y++;
//               }
//               y--;
//               break;

//           case 4:
//               x = originX + pDepth;
//               if (x >= map.Width)
//                   return;

//               y = originY + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
//               if (y >= map.Height)
//                   y = map.Height - 1;

//               while (GetSlope(x, y, originX, originY, true) >= pEndSlope)
//               {
//                   if (GetVisDistance(x, y, originX, originY) <= visrange2)
//                   {
//                       if (!map.GetItem(x, y).Cell.IsTransparent)
//                       {
//                           if (y + 1 < map.Height && map.GetItem(x, y + 1).Cell.IsTransparent)
//                               ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y + 0.5, originX, originY, true), map, originX, originY, fovRadius);
//                       }
//                       else
//                       {
//                           if (y + 1 < map.Height && !map.GetItem(x, y + 1).Cell.IsTransparent)
//                               pStartSlope = GetSlope(x + 0.5, y + 0.5, originX, originY, true);

//						Map.SetItem(x, y, true);
//						LightMap.SetItem(x, y,
//							UnityMath.Remap(GetVisDistance(x, y, originX, originY), 0, visrange2, 1, 0));
//					}
//                   }
//                   y--;
//               }
//               y++;
//               break;

//           case 5:
//               y = originY + pDepth;
//               if (y >= map.Height)
//                   return;

//               x = originX + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
//               if (x >= map.Width)
//                   x = map.Width - 1;

//               while (GetSlope(x, y, originX, originY, false) >= pEndSlope)
//               {
//                   if (GetVisDistance(x, y, originX, originY) <= visrange2)
//                   {
//                       if (!map.GetItem(x, y).Cell.IsTransparent)
//                       {
//                           if (x + 1 < map.Height && map.GetItem(x + 1, y).Cell.IsTransparent)
//                               ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y - 0.5, originX, originY, false), map, originX, originY, fovRadius);
//                       }
//                       else
//                       {
//                           if (x + 1 < map.Height
//                                   && !map.GetItem(x + 1, y).Cell.IsTransparent)
//                               pStartSlope = GetSlope(x + 0.5, y + 0.5, originX, originY, false);

//						Map.SetItem(x, y, true);
//						LightMap.SetItem(x, y,
//							UnityMath.Remap(GetVisDistance(x, y, originX, originY), 0, visrange2, 1, 0));
//					}
//                   }
//                   x--;
//               }
//               x++;
//               break;

//           case 6:
//               y = originY + pDepth;
//               if (y >= map.Height)
//                   return;

//               x = originX - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
//               if (x < 0)
//                   x = 0;

//               while (GetSlope(x, y, originX, originY, false) <= pEndSlope)
//               {
//                   if (GetVisDistance(x, y, originX, originY) <= visrange2)
//                   {
//                       if (!map.GetItem(x, y).Cell.IsTransparent)
//                       {
//                           if (x - 1 >= 0 && map.GetItem(x - 1, y).Cell.IsTransparent)
//                               ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y - 0.5, originX, originY, false), map, originX, originY, fovRadius);
//                       }
//                       else
//                       {
//                           if (x - 1 >= 0
//                                   && !map.GetItem(x - 1, y).Cell.IsTransparent)
//                               pStartSlope = -GetSlope(x - 0.5, y + 0.5, originX, originY, false);

//						Map.SetItem(x, y, true);
//						LightMap.SetItem(x, y,
//							UnityMath.Remap(GetVisDistance(x, y, originX, originY), 0, visrange2, 1, 0));
//					}
//                   }
//                   x++;
//               }
//               x--;
//               break;

//           case 7:
//               x = originX - pDepth;
//               if (x < 0)
//                   return;

//               y = originY + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
//               if (y >= map.Height)
//                   y = map.Height - 1;

//               while (GetSlope(x, y, originX, originY, true) <= pEndSlope)
//               {
//                   if (GetVisDistance(x, y, originX, originY) <= visrange2)
//                   {
//                       if (!map.GetItem(x, y).Cell.IsTransparent)
//                       {
//                           if (y + 1 < map.Height && map.GetItem(x, y + 1).Cell.IsTransparent)
//                               ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y + 0.5, originX, originY, true), map, originX, originY, fovRadius);
//                       }
//                       else
//                       {
//                           if (y + 1 < map.Height && !map.GetItem(x, y + 1).Cell.IsTransparent)
//                               pStartSlope = -GetSlope(x - 0.5, y + 0.5, originX, originY, true);

//						Map.SetItem(x, y, true);
//						LightMap.SetItem(x, y,
//							UnityMath.Remap(GetVisDistance(x, y, originX, originY), 0, visrange2, 1, 0));
//					}
//                   }
//                   y--;
//               }
//               y++;
//               break;

//           case 8: //wnw

//               x = originX - pDepth;
//               if (x < 0)
//                   return;

//               y = originY - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
//               if (y < 0)
//                   y = 0;

//               while (GetSlope(x, y, originX, originY, true) >= pEndSlope)
//               {
//                   if (GetVisDistance(x, y, originX, originY) <= visrange2)
//                   {
//                       if (!map.GetItem(x, y).Cell.IsTransparent)
//                       {
//                           if (y - 1 >= 0 && map.GetItem(x, y - 1).Cell.IsTransparent)
//                               ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y - 0.5, originX, originY, true), map, originX, originY, fovRadius);
//                       }
//                       else
//                       {
//                           if (y - 1 >= 0 && !map.GetItem(x, y - 1).Cell.IsTransparent)
//                               pStartSlope = GetSlope(x - 0.5, y - 0.5, originX, originY, true);

//						Map.SetItem(x, y, true);
//						LightMap.SetItem(x, y,
//							UnityMath.Remap(GetVisDistance(x, y, originX, originY), 0, visrange2, 1, 0));
//					}
//                   }
//                   y++;
//               }
//               y--;
//               break;
//       }


//       if (x < 0)
//           x = 0;
//       else if (x >= map.Width)
//           x = map.Width - 1;

//       if (y < 0)
//           y = 0;
//       else if (y >= map.Height)
//           y = map.Height - 1;

//       if (pDepth < fovRadius & map.GetItem(x, y).Cell.IsTransparent)
//           ScanOctant(pDepth + 1, pOctant, pStartSlope, pEndSlope, map, originX, originY, fovRadius);
//   }


/// <summary>
/// Get the gradient of the slope formed by the two points
/// </summary>
private double GetSlope(double x1, double y1, double x2, double y2, bool invertSlope)
{
	if (invertSlope)
		return (y1 - y2) / (x1 - x2);

	return (x1 - x2) / (y1 - y2);
}

	private int GetVisDistance(int x1, int y1, int x2, int y2)
	{
		return ((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2));
	}

    //public void Recompute(TileMap map, Int32 originX, Int32 originY, Int32 fovRadius)
    //{
    //	Map.SetItem(originX, originY, true);
    //  LightMap.SetItem(originX, originY, 1);

    //	if (fovRadius <= 0)
    //		return;

    //	RectInt fovArea = new RectInt(0, 0, Map.Width - 1, Map.Height - 1);

    //	fovArea = fovArea.Intersection(
    //		new RectInt(originX - fovRadius, originY - fovRadius, fovRadius * 2 + 1, fovRadius * 2 + 1)
    //	);

    //	for (var x = fovArea.xMin; x <= fovArea.xMax; x++) // cast rays towards the top and bottom of the area
    //	{
    //		BresenhamLine(map, Map, originX, originY, x, fovArea.yMax, Distances.ManhattanNormalized, fovRadius, false);
    //		BresenhamLine(map, Map, originX, originY, x, fovArea.yMin, Distances.ManhattanNormalized, fovRadius, false);
    //	}
    //	for (var y = fovArea.yMin; y <= fovArea.yMax; y++) // and to the left and right
    //	{
    //		BresenhamLine(map, Map, originX, originY, fovArea.xMax, y, Distances.ManhattanNormalized, fovRadius, false);
    //		BresenhamLine(map, Map, originX, originY, fovArea.xMin, y, Distances.ManhattanNormalized, fovRadius, false);
    //	}
    //}

    private void Swap<T>(ref T lhs, ref T rhs)
	{
		T temp = lhs;
		lhs = rhs;
		rhs = temp;
	}

	private void BresenhamLine(TileMap map, FovMap fovMap, Int32 fromX, Int32 fromY, Int32 toX, Int32 toY,
		Func<Vector2Int, Vector2Int, Int32> getDistance, Int32 fovRange, Boolean isObstacleLighted) =>
		BresenhamLine(map, fovMap, fromX, fromY, toX, toY,
			(x0, y0, x1, y1) => getDistance(new Vector2Int(x0, y0), new Vector2Int(x1, y1)), fovRange,
			isObstacleLighted);


    private void BresenhamLine(TileMap map, FovMap fovMap, Int32 fromX, Int32 fromY, Int32 toX, Int32 toY, Func<Int32, Int32, Int32, Int32, Int32> getDistance, Int32 fovRange, Boolean isObstacleLighted)
    {
		Int32 xDiff = toX - fromX, 
			yDiff = toY - fromY, 
			xLen = Math.Abs(xDiff), 
			yLen = Math.Abs(yDiff);

		Int32 deltaX = Math.Sign(xDiff), 
			deltaY = Math.Sign(yDiff) << 16, 
			index = (fromY << 16) + fromX;

		if (xLen < yLen) // make sure we walk along the long axis
		{
			Swap(ref xLen, ref yLen);
			Swap(ref deltaX, ref deltaY);
		}

		Int32 deltaError = yLen * 2, 
			error = -xLen, 
			errorReset = xLen * 2;

		while (--xLen >= 0) // skip the first point (the origin) since it's always visible and should never stop rays
		{
			index += deltaX; // advance down the long axis (could be X or Y)
			error += deltaError;

			if (error > 0)
			{
				error -= errorReset;
				index += deltaY;
			}

			Int32 x = index & 0xFFFF, 
				y = index >> 16;


            if (getDistance(fromX, fromY, x, y) > fovRange)
				return;
			var tile = map.GetItem(x, y);

			if (!tile.Cell.IsTransparent)
			{
				if (isObstacleLighted)
				{
					fovMap.SetItem(x, y, true);
					var distance = getDistance(fromX, fromY, x, y); //!!!!!!!!!!!!!!
					var light = UnityMath.Remap(distance, 0, fovRange, 1, 0);//!!!!!!!!!!!!!!
                    LightMap.SetItem(x, y, light);//!!!!!!!!!!!!!!
                }
				return;
			}
			fovMap.SetItem(x, y, true);
			var dist = getDistance(fromX, fromY, x, y); //!!!!!!!!!!!!!!
			var lightVal = UnityMath.Remap(dist, 0, fovRange, 1, 0);//!!!!!!!!!!!!!!
			LightMap.SetItem(x, y, lightVal);//!!!!!!!!!!!!!!
        }
    }


}
