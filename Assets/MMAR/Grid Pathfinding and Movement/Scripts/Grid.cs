using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    Node[,] grid;
    [SerializeField] int width = 25;
    [SerializeField] int length = 25;
    [SerializeField] float cellSize = 1;
    [SerializeField] bool autoUpdateGridOnSizeChanged = false;
    [SerializeField] LayerMask obstackleLayer;
    int lastWidth = 0, lastLength = 0;
    float lastCellSize=0;
    [Header("Util")]
    [SerializeField] GameObject gridItemPrefab;
    [SerializeField] Transform gridItemsParent;
    [SerializeField] bool DebugThis = false;
    private void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        grid = new Node[length, width];
        CheckPassableTerrain();
    }

    void CheckPassableTerrain()
    {
        
        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < length; x++)
            {
                Vector3 worldPosition= GetWorldPosition(x,y);
                bool passable=!Physics.CheckBox(worldPosition,Vector3.one/2*cellSize,Quaternion.identity,obstackleLayer);
                grid[x, y] = new();
                grid[x,y].passable = passable;
            }
        }
    }
    #region Maintaning Grid Items
    IEnumerator ClearAllGridItemsNumarator()
    {
        while(gridItemsParent.childCount > 0)
        {
            Destroy(gridItemsParent.GetChild(0));
            yield return new WaitForSecondsRealtime(.001f);
        }
    }
    public void ResizeGrid()
    {
        #region Setting the lasts
        lastCellSize = cellSize;
        lastLength = length;
        lastWidth = width;
        #endregion
        StartCoroutine(ResizeGridNumarator());
    }
    IEnumerator ResizeGridNumarator()
    {
        yield return ClearAllGridItemsNumarator();
        var firstOne = Instantiate(gridItemPrefab, gridItemsParent);
        
    }
    #endregion
    private void OnValidate()
    {
        if(autoUpdateGridOnSizeChanged)
        {
            if(lastCellSize != cellSize || width != lastWidth || length != lastLength)
            {
                ResizeGrid();
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (grid == null)
        {
            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < length; x++)
                {
                    Vector3 pos = GetWorldPosition(x, y);
                    //Gizmos.color = grid[x, y].passable ? Color.green : Color.red;
                    Gizmos.DrawCube(pos, Vector3.one / 4);
                }
            }
        }
        else
        {
            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < length; x++)
                {
                    Vector3 pos = GetWorldPosition(x, y);
                    Gizmos.color = grid[x, y].passable ? Color.green : Color.red;
                    Gizmos.DrawCube(pos, Vector3.one / 4);
                }
            }
        }
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        worldPosition-=transform.position;
        return new((int)(worldPosition.x/cellSize),(int)(worldPosition.z / cellSize));

    }
    Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(transform.position.x + (x * cellSize), 0, transform.position.z + (y * cellSize));
    }

    internal void PlaceObject(Vector2Int positionOnGrid, GridObject gridObject)
    {
        if (CheckBoundry(positionOnGrid))
        {
            grid[positionOnGrid.x, positionOnGrid.y].gridObjdect = gridObject;
        }
        else {
            Debug.Log(gridObject.gameObject.name + " out of the grid boundry");
        }
        
    }
    public bool CheckBoundry(Vector2Int position)
    {
        if (position.x < 0 || position.x >= length || position.y < 0||position.y>width)
        {
            return false;
        }
        return true;
    }
    internal GridObject GetPlacedObject(Vector2Int gridPosition)
    {
        GridObject gridObject= grid[gridPosition.x, gridPosition.y].gridObjdect;
        return gridObject;
    }
    void DebugLog(object msg)
    {
        if (DebugThis)
        {
            Debug.Log(msg);
        }
    }
}
