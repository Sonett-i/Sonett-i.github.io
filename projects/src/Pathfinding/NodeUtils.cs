using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NodeUtils
{
    public static float gCost(GridTile a, GridTile b)
	{
		float cost = 10;

		Vector2Int diff = (b.gridPosition - a.gridPosition);
		if (diff.x != 0 && diff.y != 0) // Diagonal
			cost = 14;

		return cost;
	}

	public static float GetDistance(GridTile a, GridTile b)
	{
		int dstX = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
		int dstY = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);

		float distance = 14 * dstX + 10 * (dstY - dstX);

		if (dstX > dstY)
			distance = 14 * dstY + 10 * (dstX - dstY);


		return distance * b.tileCosts[b.tileType];
	}
}
