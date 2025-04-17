using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AstarAgent : MonoBehaviour
{
	#region ExternalRefs
	TextMeshProUGUI txt_IterationCount;
    TileGrid grid;

    Agent agent;
    UI_Controller controller;
	#endregion

	#region Config

	[Header("Node Information")]
    [SerializeField] List<GridTile> openNodes = new List<GridTile>();
    [SerializeField] List<GridTile> closedNodes = new List<GridTile>();
    [SerializeField] GridTile currentNode;
	#endregion

	#region Methods

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
        OpenNode(grid.startTile);
        grid.startTile.g_cost = 0;
        grid.startTile.h_cost = 0;

        StartCoroutine(PathFind());
    }

    IEnumerator PathFind()
    {
        GridTile startNode = grid.startTile;
        GridTile endNode = grid.endTile;

        txt_IterationCount = GameObject.Find("IterationCount").GetComponent<TextMeshProUGUI>();

        yield return new WaitForSeconds(Time.deltaTime);

        bool goalReached = false;
        int i = 0;

        while (!goalReached)
		{
            controller.UpdateText("A*\n\nIteration: " + i);
            agent.step = true;

            // Get tile in openNodes list/heap with smallest f cost.
            GridTile currentTile = GetTileWithMinDistance(openNodes);

            if (currentTile == null)
            {
                break; // Maze not solvable :'( No more nodes left in OpenNodes
            }

            // Close the node and remove from list/heap.
            CloseNode(currentTile);
            currentTile.SetColor(Color.gray);

            if (currentTile == endNode) //if the current tile is the goal node, we've reached our destination.
			{
                goalReached = true;
                break;
			}

            List<GridTile> neighbors = grid.GetNeighbors(currentTile);

            foreach (GridTile neighbor in neighbors)
			{
                agent.step = false;
                if (neighbor.tileType == GridTile.TileType.TILETYPE_OBSTACLE || neighbor.state == GridTile.State.Closed)
				{
                    continue;
				}
                else
				{
                    int newCostToNeighbor = (int)currentTile.g_cost + (int)NodeUtils.GetDistance(currentTile, neighbor);

                    if (newCostToNeighbor < neighbor.g_cost || !openNodes.Contains(neighbor))
                    {
                        neighbor.g_cost = newCostToNeighbor;
                        neighbor.h_cost = NodeUtils.GetDistance(neighbor, endNode);
                        neighbor.parent = currentTile;
                        neighbor.UpdateLabels(true);

                        // Open the node if it is closed.
                        if (!openNodes.Contains(neighbor))
                        {
                            OpenNode(neighbor);
                        }
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

            // If step by step debugging, or automatic.
            if (agent.navType == Agent.NavType.Manual)
            {
                yield return new WaitUntil(() => agent.step);
                agent.step = false;
            }
            else
            {
                yield return new WaitForSeconds(agent.interval);
            }
            i++;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        List<GridTile> path = RetracePath();

        yield return new WaitForSeconds(agent.interval);
        agent.goalReached = true;
    }

    // Iterate through node parent pointers from end node until there are no nodes left in heap.
    List<GridTile> RetracePath()
	{
        List<GridTile> path = new List<GridTile>();

        GridTile node = grid.endTile;

        while (node != null)
		{
            path.Add(node);
            node.SetColor(Color.green);
            node = node.parent;
		}

        return path;
	}

    // Iterates through input GridTile list and returns tile with smallest f cost.
    public GridTile GetTileWithMinDistance(List<GridTile> tiles)
    {
        GridTile minTile = null;
        float minDist = float.MaxValue;

        foreach (GridTile tile in tiles)
        {
            if (tile.f_cost < minDist)
            {
                minTile = tile;
                minDist = tile.f_cost;
            }
        }

        return minTile;
    }
	#endregion

	#region Unity
	private void Start()
	{
        controller = GameObject.FindObjectOfType<UI_Controller>();
        agent = this.GetComponent<Agent>();
	}
	#endregion
}
