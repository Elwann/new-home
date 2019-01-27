using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public static float gridSize = 0.25f;

    public Vector2Int size = new Vector2Int();

    public int[,] grid;

    private void Awake()
    {
        grid = new int[size.x, size.y];
        for(int x = 0; x<size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                grid[x, y] = 0;
            }
        }
    }

    public Vector3 GridToPosition(Vector2Int grid)
    {
        return new Vector3(grid.x * gridSize, 0f, 0f - grid.y * gridSize);
    }

    public Vector2Int PositionToGrid(Vector3 position)
    {
        return new Vector2Int(
            Mathf.FloorToInt(position.x / gridSize),
            Mathf.FloorToInt(0f - position.z / gridSize)
            );
    }

    public bool Placable(int x, int y, int width, int height)
    {
        bool placable = true;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (grid[x + i, y + j] > 0)
                {
                    Debug.Log(grid[x + i, y + j]);
                    placable = false;
                }
            }
        }

        return placable;
    }

    public void ChangeGrid(int x, int y, int width, int height, int change)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                grid[x + i, y + j] = Mathf.Max(0, grid[x + i, y + j] + change);
            }
        }
    }

    public Vector3 FindPlace(Item item, Vector3 position)
    {
        Debug.Log("find place");
        Vector2Int pos = PositionToGrid(position);
        
        int cornerX = Mathf.Clamp(pos.x - Mathf.RoundToInt(item.size.x * 0.5f), 0, size.x - item.size.x - 1);
        int cornerY = Mathf.Clamp(pos.y - Mathf.RoundToInt(item.size.y * 0.5f), 0, size.y - item.size.y - 1);

        if(Placable(cornerX, cornerY, item.size.x, item.size.y))
        {
            ChangeGrid(cornerX, cornerY, item.size.x, item.size.y, 1);
            return GridToPosition(new Vector2Int(cornerX, cornerY));
        }

        return Vector3.zero;
    }

    public void RemoveItem(Item item)
    {
        Vector2Int pos = PositionToGrid(item.transform.position);
        ChangeGrid(pos.x, pos.y, item.size.x, item.size.y, 1);
    }
}
