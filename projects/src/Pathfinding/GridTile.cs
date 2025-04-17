using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridTile : MonoBehaviour
{
    #region NodeStruct
    // Node Information

    public GridTile parent; // The previous node in this branch

    public Vector2Int gridPosition;
    public State state = State.Default;

    // g cost heuristic: used by Dijkstra's and A*: Traversal cost between nodes, and cumulative cost to reach a specific node on a branch.
    public float g_cost = float.MaxValue;

    // h cost heuristic: used by A*: is the distance from the current tile to the end tile.
    public float h_cost = float.MaxValue;

    // f cost heuristic: used by A*, is the sum of the node's g cost and h cost.
    public float f_cost
    {
        get
        {
            return g_cost + h_cost;
        }
    }
    #endregion

    #region ExternalRefs

    SpriteRenderer sr;

    #endregion

    #region Config
    public enum TileType
	{
        TILETYPE_CLEAR,
        TILETYPE_OBSTACLE,
        TILETYPE_START,
        TILETYPE_GOAL,
        TILETYPE_SLOW,
	}

    // Tile cost modifier dictionary. Multiplies distance cost based on float modifier associated with tiletype.
    public Dictionary<TileType, float> tileCosts = new Dictionary<TileType, float>()
    {
        [TileType.TILETYPE_CLEAR] = 1f,
        [TileType.TILETYPE_OBSTACLE] = 0f,
        [TileType.TILETYPE_START] = 1f,
        [TileType.TILETYPE_GOAL] = 1f,
		[TileType.TILETYPE_SLOW] = 1.5f,
    };
    public enum State
	{
        Default,
        Open,
        Closed
	}

    public Dictionary<TileType, Color> tileColours = new Dictionary<TileType, Color>() 
    {
        [TileType.TILETYPE_CLEAR] = Color.white,
        [TileType.TILETYPE_OBSTACLE] = Color.black,
		[TileType.TILETYPE_START] = Color.cyan,
        [TileType.TILETYPE_GOAL] = Color.green,
		[TileType.TILETYPE_SLOW] = Color.red
    };

    public bool slowTile = false;

    public TileType tileType;

    [SerializeField] public TextMeshPro txt_GCost;
    [SerializeField] public TextMeshPro txt_FCost;
    [SerializeField] public TextMeshPro txt_HCost;

    [SerializeField] bool demoTile = false;

    [SerializeField] Color flashColour = Color.yellow;
    [SerializeField] Color visitedColour = Color.gray;

    [SerializeField] Color openColour = Color.magenta;
    [SerializeField] Color closedColour = Color.gray;

	#endregion

	#region Methods

	// Methods

	public void SetText(string text, TextMeshPro textObject)
    {
        textObject.text = text;
    }

    public void UpdateLabels(bool aStar)
    {
        this.SetText(g_cost.ToString(), txt_GCost);
        if (aStar)
		{
            this.SetText(h_cost.ToString(), txt_HCost);
            this.SetText(f_cost.ToString(), txt_FCost);
        }
    }
    public void SetTile(int type)
	{
        this.tileType = (TileType)type;

        if (!demoTile)
		{
            sr.color = tileColours[this.tileType];
        }   
	}

    public void ResetTile()
	{
        this.state = State.Default;
        this.g_cost = float.MaxValue;
        this.h_cost = float.MaxValue;

        this.SetText("", txt_GCost);
        this.SetText("", txt_HCost);
        this.SetText("", txt_FCost);

        this.parent = null;
        sr.color = tileColours[this.tileType];
    }

    public void SetState(State state)
	{
        this.state = state;

        Color col = closedColour;

        if (state == State.Closed)
		{
            col = closedColour;
		}
        else
		{
            col = openColour;
		}

        sr.color = col;
	}

    public float gCost(GridTile tile)
	{
        float cost = 10;

        Vector2Int diff = (tile.gridPosition - this.gridPosition);
        if (diff.x != 0 && diff.y != 0) // Diagonal
            cost = 14;

        return cost;
    }

    public float hCost(GridTile tile)
	{
        return ManhattanCost(tile);
	}

    public float fCost()
	{
        return h_cost + g_cost;
	}

    public float ManhattanCost(GridTile tile)
	{
        int dstX = Mathf.Abs(this.gridPosition.x - tile.gridPosition.x);
        int dstY = Mathf.Abs(this.gridPosition.y - tile.gridPosition.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
   
    Color originalColor;
    public void SetColor(Color color)
	{
        originalColor = sr.color;
        sr.color = color;
	}
    public void ResetColor()
	{
        sr.color = originalColor;
	}

	#endregion

	#region Unity
	// Start is called before the first frame update
	void Start()
    {
        sr = this.GetComponent<SpriteRenderer>();
    }

	#endregion
}
