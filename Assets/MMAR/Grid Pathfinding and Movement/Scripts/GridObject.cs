using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MMAR.Grid
{
    public class GridObject : MonoBehaviour
    {
        [SerializeField] Grid grid;
        public Vector2Int position;
        [Tooltip("If true then this object will occupy the grid position and block others to move over this")]
        public bool occupyPosition = false;



        bool placedObject = false;
        private void Start()
        {
            Init();
        }

        private void Init()
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
                placedObject = true;
                Debug.Log("Grid isn't set");
            }
        }
        private void Update()
        {
            if (!placedObject)
            {
                Init();
            }
        }
    }
}
