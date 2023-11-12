using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridControl : MonoBehaviour
{
    [SerializeField] Grid targetGrid;
    [SerializeField] LayerMask terrainLayerMask;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit,50, terrainLayerMask))
            {
                var targetGridPosition=targetGrid.GetGridPosition(hit.point);
                var targetGridObject = targetGrid.GetPlacedObject(targetGridPosition);
                if(targetGridObject == null)
                {
                    Debug.Log(targetGridPosition+" is empty");
                }
                else
                {
                    Debug.Log(targetGridPosition + " has gameobject" + targetGridObject.gameObject.name);
                }
            }
        }
    }
}
