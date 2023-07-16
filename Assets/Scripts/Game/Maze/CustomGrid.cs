using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGrid
{
    private int width;
    private int height;
    private int cellsize;
    private Vector3 originPosition;
    private int[,] gridArray;

    public CustomGrid(int width, int height, int cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellsize = cellSize;
        this.originPosition = originPosition;

        gridArray = new int[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 1000f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 1000f);
            }
        }
    }

    public int GetCellSize()
    {
        return cellsize;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y, 0) * cellsize + originPosition;
    }

    public int[] GetXY(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition - originPosition).x / cellsize);
        int y = Mathf.FloorToInt((worldPosition - originPosition).y / cellsize);

        return new int[] { x, y };
    }
}