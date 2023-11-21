using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace MMAR.Grid
{
    public class Character : GridObject
    {
        UnityEvent<Character> onCharacterCLicked;
        public float MovingSpeed=10;
        int pathIndex=1;
        public List<PathItem> path = new List<PathItem>();
        public List<Vector2Int> pathPoints;
        [HideInInspector]
        public string id;
        public override void Start()
        {
            base.Start();
            id= System.Guid.NewGuid().ToString();
        }
        public void MoveTO(Vector2Int position)
        {
            Debug.Log("Finding position " + position);
            grid.FindPath(this,position);
        }
        public void SetPath(List<Vector2Int> pathPoints)
        {
            this.pathPoints = pathPoints;
        }
        //Required a collider in the character gameobject to function this
        private void OnMouseDown()
        {
            onCharacterCLicked?.Invoke(this);
        }
        //public override void Update()
        //{
        //    base.Update();
        //    if(pathPoints.Count > 0)
        //    {
        //        path.Clear();
        //        foreach (var item in pathPoints)
        //        {
        //            var worldPosition = grid.GetWorldPosition(item.x, item.y);
        //            worldPosition.y = transform.position.y;
        //            path.Add(new(worldPosition, item));
        //        }
        //        pathPoints.Clear();
                
        //    }
        //    if (pathIndex < path.Count)
        //    {
        //        if (path[pathIndex].realWorldPosition.y == 0)
        //        {
        //            path[pathIndex].realWorldPosition.y = transform.position.y;
        //        }

        //        transform.position = Vector3.MoveTowards(transform.position, path[pathIndex].realWorldPosition, MovingSpeed * Time.deltaTime);
        //        if (Vector3.Distance(transform.position, path[pathIndex].realWorldPosition) == 0)
        //        {
        //            pathIndex++;
        //        }
        //    }
        //}
    }
    [System.Serializable]
    public class PathItem
    {
        public Vector3 realWorldPosition;
        public Vector2Int gridPosition;
        public PathItem(Vector3 realWorldPosition, Vector2Int gridPosition)
        {
            this.realWorldPosition = realWorldPosition;
            this.gridPosition = gridPosition;
        }
        public PathItem()
        {

        }
    }
}
