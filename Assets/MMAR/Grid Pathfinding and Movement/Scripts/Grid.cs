using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

namespace MMAR.Grid
{
    public class Grid : MonoBehaviour
    {
        Node[,] grid;
        [SerializeField] int width = 25;
        [SerializeField] int length = 25;
        [SerializeField] float cellSize = 1;
        [Header("Movement")]
        public bool allowDiagonalMove;
        [SerializeField] LayerMask obstackleLayer;
        [HideInInspector] public bool generated = false;
        Thread pathFindingThread;
        Dictionary<string, PathFindItem> pathFindItems = new();
        List<string> pathFindItemsKeys = new();
        int pathFindItemIndex = 0;
        private void Start()
        {
            GenerateGrid();
            pathFindingThread = new(PathFindingThread);
            
        }
        #region pathFinding
        public void FindPath(Character character,Vector2Int targetPosition)
        {
            //Adding the item
            PathFindItem pathFindItem = new PathFindItem();
            pathFindItem.character = character;
            pathFindItem.targetPosition = targetPosition;
            pathFindItems[character.id] = pathFindItem;
            //if (pathFindItemsKeys.Count != pathFindItems.Count)
            //{
            //    pathFindItemsKeys.Clear();
            //    pathFindItemsKeys.AddRange(pathFindItems.Keys);
            //}
            //if (pathFindItemIndex > pathFindItemsKeys.Count)
            //{
            //    pathFindItemIndex = 0;
            //}
            //Debug.Log(pathFindItemsKeys[pathFindItemIndex]);
            //Running the thread
            if (!pathFindingThread.IsAlive)
            {
                pathFindingThread = new(PathFindingThread);
                pathFindingThread.Start();
            }
        }
        private void PathFindingThread()
        {
            while (pathFindItems.Count > 0)
            {
                #region checking data
                if (pathFindItemsKeys.Count != pathFindItems.Count)
                {
                    pathFindItemsKeys.Clear();
                    pathFindItemsKeys.AddRange(pathFindItems.Keys);
                }
                if(pathFindItemIndex>= pathFindItemsKeys.Count)
                {
                    pathFindItemIndex = 0;
                }
                #endregion

                if (pathFindItemIndex < pathFindItemsKeys.Count&&pathFindItems.TryGetValue(pathFindItemsKeys[pathFindItemIndex],out var curItem))
                {
                    //At start
                    if(curItem.paths.Count == 0)
                    {
                        if(CheckBoundry(curItem.targetPosition))
                        {
                            var output = GetAvailableAroundPositions(curItem.character.position, new Vector2Int(-1, -1));
                            for (var i = 0; i < output.Count; i++)
                            {
                                List<Vector2Int> pathPoints= new List<Vector2Int>();
                                pathPoints.Add(curItem.character.position);
                                pathPoints.Add(output[i]);
                                if (output[i] == curItem.targetPosition)
                                {
                                    curItem.character.SetPath(pathPoints);
                                    pathFindItems.Remove(pathFindItemsKeys[pathFindItemIndex]);
                                    break;
                                }
                                else
                                {
                                    curItem.paths.Add(pathPoints);
                                }
                            }
                            Debug.Log("Found output "+output.Count);
                        }
                        else
                        {
                            Debug.Log(curItem.character.name + " target position " + curItem.targetPosition + " is our of grid");
                            pathFindItems.Remove(pathFindItemsKeys[pathFindItemIndex]);
                        }
                    }
                    else
                    {
                        if (curItem.pathIndex >= curItem.paths.Count)
                        {
                            curItem.pathIndex = 0;
                            curItem.pathLengthLimit = 0;
                        }
                        if(curItem.pathIndex < curItem.paths.Count)
                        {
                            var currentPathList = curItem.paths[curItem.pathIndex];
                            if (curItem.pathLengthLimit == 0)
                            {
                                curItem.pathLengthLimit = currentPathList.Count;
                            }
                            if (currentPathList.Count <= curItem.pathLengthLimit)
                            {
                                curItem.paths.RemoveAt(curItem.pathIndex);
                                var output = GetAvailableAroundPositions(currentPathList[currentPathList.Count - 1], currentPathList[currentPathList.Count - 2]);
                                for (var i = 0; i < output.Count; i++)
                                {
                                    List<Vector2Int> pathPoints = new List<Vector2Int>();
                                    pathPoints.AddRange(currentPathList);
                                    pathPoints.Add(output[i]);
                                    if (output[i] == curItem.targetPosition)
                                    {

                                        curItem.character.SetPath(pathPoints);
                                        pathFindItems.Remove(pathFindItemsKeys[pathFindItemIndex]);

                                        break;
                                    }
                                    else
                                    {
                                        curItem.paths.Add(pathPoints);
                                    }
                                }
                            }
                            
                        }
                        //Debug.Log(curItem.pathIndex);
                        curItem.pathIndex++;
                        //else
                        //{
                        //    Debug.Log("Past index "+curItem.PathIndex);
                        //}
                    }
                }
                else
                {
                    pathFindItemsKeys.Clear();
                    pathFindItemsKeys.AddRange(pathFindItems.Keys);
                }
                #region finishing tasks
                //Debug.Log(pathFindItems.Count);
                pathFindItemIndex++;
                #endregion
                //Debug.Log(pathFindItems.Count);
            }
        }
        #endregion
        private void OnDestroy()
        {
            if(pathFindingThread.IsAlive) { pathFindingThread.Abort(); }
        }
        public List<Vector2Int> GetAvailableAroundPositions(Vector2Int position,Vector2Int lastPosition)
        {
            List<Vector2Int > result = new List<Vector2Int>();
            bool xtrue=false;
            bool ytrue=false;
            if (position.x > 0)
            {
                var tempPosition=position;
                tempPosition.x--;
                if (grid[tempPosition.x, tempPosition.y].passable)
                {
                    if(allowDiagonalMove)
                    {
                        xtrue = true;
                    }
                    if (tempPosition != lastPosition)
                    {
                        result.Add(tempPosition);
                    }
                }
            }
            if (position.y > 0)
            {
                var tempPosition = position;
                tempPosition.y--;
                if (grid[tempPosition.x, tempPosition.y].passable)
                {
                    if (allowDiagonalMove)
                    {
                        ytrue = true;
                    }
                    if (tempPosition != lastPosition)
                    {
                        result.Add(tempPosition);
                    }
                }
            }
            if(xtrue&&ytrue)
            {
                var tempPosition = position;
                tempPosition.x--;
                tempPosition.y--;
                if (grid[tempPosition.x, tempPosition.y].passable)
                {
                    if (tempPosition != lastPosition)
                    {
                        result.Add(tempPosition);
                    }
                }
            }
            xtrue = false;
            ytrue = false;

            if (position.x < length-1)
            {
                var tempPosition = position;
                tempPosition.x++;
                if (grid[tempPosition.x, tempPosition.y].passable)
                {
                    if (allowDiagonalMove)
                    {
                        xtrue = true;
                    }
                    if (tempPosition != lastPosition)
                    {
                        result.Add(tempPosition);
                    }
                }
            }
            if (position.y < width-1)
            {
                var tempPosition = position;
                tempPosition.y++;
                if (grid[tempPosition.x, tempPosition.y].passable)
                {
                    if (allowDiagonalMove)
                    {
                        ytrue = true;
                    }
                    if (tempPosition != lastPosition)
                    {
                        result.Add(tempPosition);
                    }
                }
            }
            if (xtrue && ytrue)
            {
                var tempPosition = position;
                tempPosition.x++;
                tempPosition.y++;
                if (grid[tempPosition.x, tempPosition.y].passable)
                {
                    if (tempPosition != lastPosition)
                    {
                        result.Add(tempPosition);
                    }
                }
            }
            
            return result;
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
                    Vector3 worldPosition = GetWorldPosition(x, y);
                    bool passable = !Physics.CheckBox(worldPosition, Vector3.one / 2 * cellSize, Quaternion.identity, obstackleLayer);
                    if (!generated)
                    {
                        grid[x, y] = new();
                        grid[x, y].passable = passable;
                    }

                }
            }
            generated = true;
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
            worldPosition -= transform.position;
            return new((int)(worldPosition.x / cellSize), (int)(worldPosition.z / cellSize));

        }
        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(transform.position.x + (x * cellSize), 0, transform.position.z + (y * cellSize));
        }

        internal void PlaceObject(Vector2Int positionOnGrid, GridObject gridObject, bool checkBoundry = false)
        {
            if (checkBoundry || CheckBoundry(positionOnGrid))
            {
                grid[positionOnGrid.x, positionOnGrid.y].gridObjdect = gridObject;
            }
            else
            {
                Debug.Log(gridObject.gameObject.name + " out of the grid boundry");
            }

        }
        public bool CheckBoundry(Vector2Int position)
        {
            if (position.x < 0 || position.x >= length || position.y < 0 || position.y > width)
            {
                return false;
            }
            return true;
        }
        internal GridObject GetPlacedObject(Vector2Int gridPosition)
        {
            GridObject gridObject = grid[gridPosition.x, gridPosition.y].gridObjdect;
            return gridObject;
        }
    }
    public class Node
    {
        public bool passable;
        internal GridObject gridObjdect;
    }
    public class PathFindItem
    {
        public Character character;
        public Vector2Int targetPosition;
        public List<List<Vector2Int>> paths=new();
        public int pathIndex = 0,pathLengthLimit=0;
    }
}
