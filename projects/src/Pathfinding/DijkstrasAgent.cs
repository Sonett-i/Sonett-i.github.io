using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstrasAgent : MonoBehaviour
{
	#region ExternalRefs 
	Agent agent;

	UI_Controller controller;
    TileGrid grid;
	#endregion

	#region Config
	[Header("Node Information")]
	[SerializeField] List<GridTile> openNodes = new List<GridTile>();
	[SerializeField] List<GridTile> closedNodes = new List<GridTile>();
	[SerializeField] GridTile currentNode;
	#endregion

	#region Methods
	GridTile GetOpenNode(GridTile tile)
	{
		if (openNodes.Contains(tile))
		{
			return tile;
		}
		return null;
	}

	bool Exists(List<GridTile> list, GridTile tile)
	{
		foreach (GridTile gridTile in list)
		{
			if (gridTile == tile)
				return true;
		}

		return false;
	}

	void OpenNode(GridTile node)
	{
		if (node.state != GridTile.State.Closed)
		{
			if (!Exists(openNodes, node))
			{
				openNodes.Add(node);
				node.SetState(GridTile.State.Open);
			}
		}
	}

	void CloseNode(GridTile node)
	{
		openNodes.Remove(node);
		closedNodes.Add(node);

		node.SetState(GridTile.State.Closed);
	}

    public void Initialize(TileGrid grid)
	{
		openNodes = new List<GridTile>();
		closedNodes = new List<GridTile>();

        this.grid = grid;

		grid.startTile.g_cost = 0;
		OpenNode(grid.startTile);

		StartCoroutine(PathFind());
	}
	
    IEnumerator PathFind()
	{
		bool goalReached = false;
		int i = 1;
		Debug.Log("Begin");

		yield return new WaitForSeconds(Time.deltaTime);
		while (!goalReached)
		{
			controller.UpdateText("Dijkstra's\n\nIteration: " + i);

			agent.step = false;
			GridTile currentTile = GetTileWithMinDistance(openNodes);

			currentNode = currentTile; // debug in scene view DO NOT USE

			if (currentTile == null)
			{
				break; // Maze is not solvable, no more nodes left in OpenNodes :'(
			}

			if (currentTile.gridPosition == grid.endTile.gridPosition)
			{
				Debug.Log("GOAL REACHED " + i);
				goalReached = true;
				grid.endTile.parent = currentTile;
				break;
			}

			CloseNode(currentTile);
			currentTile.SetColor(Color.gray);

			List<GridTile> neighbors = grid.GetNeighbors(currentTile);

			foreach (GridTile neighbor in neighbors)
			{
				if (neighbor.gridPosition == grid.endTile.gridPosition)
				{
					goalReached = true;
					grid.endTile.parent = currentTile;
					break;
				}

				if (neighbor.state == GridTile.State.Closed || neighbor.tileType == GridTile.TileType.TILETYPE_OBSTACLE)
				{
					continue;
				}
				else
				{
					float g_cost = NodeUtils.GetDistance(currentTile, neighbor);

					if (!Exists(openNodes, neighbor) || currentTile.g_cost + g_cost < neighbor.g_cost)
					{
						agent.step = false;
						OpenNode(neighbor);

						neighbor.parent = currentTile;
						neighbor.g_cost = neighbor.parent.g_cost + g_cost;
						neighbor.UpdateLabels(false);
					}

					neighbor.SetColor(Color.yellow);
					if (agent.navType == Agent.NavType.Manual)
					{
						yield return new WaitUntil(() => agent.step);
						agent.step = false;
					}

					neighbor.ResetColor();
				}
			}
			currentTile.ResetColor();
			i++;

			if (agent.navType == Agent.NavType.Manual)
			{
				yield return new WaitUntil(() => agent.step);
				agent.step = false;
			}
			else
			{
				yield return new WaitForSeconds(agent.interval);
			}
		}

		Debug.Log("Solution found after " + i + "iteration(s).");

		List<GridTile> path = new List<GridTile>();
		GridTile node = grid.endTile;

		while (node != null)
		{
			node = node.parent;
			if (node)
			{
				path.Add(node);
				node.SetColor(Color.green);
			}
		}

		yield return new WaitForSeconds(agent.interval);
		agent.goalReached = true;
	}

	public GridTile GetTileWithMinDistance(List<GridTile> tiles)
	{
		GridTile minTile = null;
		float minDist = float.MaxValue;

		foreach (GridTile tile in tiles)
		{
			if (tile.g_cost < minDist)
			{
				minTile = tile;
				minDist = tile.g_cost;
			}
		}

		return minTile;
	}
	#endregion

	#region Unity
	// Start is called before the first frame update
	void Start()
    {
		controller = GameObject.FindObjectOfType<UI_Controller>();
		agent = this.GetComponent<Agent>();
    }
	#endregion
}

/*
 *  https://www.youtube.com/watch?v=B2mUby28wsw
 */