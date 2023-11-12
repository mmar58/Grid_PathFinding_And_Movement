using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    [SerializeField] Grid targetGrid;
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        Vector2Int positionOnGrid=targetGrid.GetGridPosition(transform.position);
        targetGrid.PlaceObject(positionOnGrid,this);
    }
}
