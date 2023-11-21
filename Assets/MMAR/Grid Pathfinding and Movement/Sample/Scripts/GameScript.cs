using MMAR.Grid;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    [SerializeField] MMAR.Grid.Grid targetGrid;
    [SerializeField] LayerMask terrainLayerMask;
    [SerializeField] Character selectedCharacter;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit,50, terrainLayerMask))
            {
                var targetGridPosition=targetGrid.GetGridPosition(hit.point);
                selectedCharacter.MoveTO(targetGridPosition);
            }
        }
    }
}
