using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

namespace MMAR.Grid
{
    public class GridObject : MonoBehaviour
    {
        public Grid grid;
        public Vector2Int position;
        [Tooltip("If true then this object will occupy the grid position and block others to move over this")]
        public bool occupyPosition = false;


        bool placedObject = false;
        virtual public void Start()
        {
            Init();
        }
        
        void Init()
        {
            if (grid != null)
            {
                if (grid.generated)
                {
                    position = grid.GetGridPosition(transform.position);
                    grid.PlaceObject(position, this, true);
                    placedObject = true;
                }
            }
            else
            {
                placedObject = false;
                Debug.Log("Grid isn't set");
            }
        }
        virtual public void Update()
        {
            if (!placedObject)
            {
                Init();
            }
        }
    }
}
