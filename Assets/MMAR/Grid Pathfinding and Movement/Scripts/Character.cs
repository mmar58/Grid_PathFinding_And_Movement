using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MMAR.Grid
{
    public class Character : GridObject
    {
        UnityEvent<Character> onCharacterCLicked;
        public void MoveTO(Vector2Int position)
        {
            
        }
        //Required a collider in the character gameobject to function this
        private void OnMouseDown()
        {
            onCharacterCLicked.Invoke(this);
        }
    }
}
