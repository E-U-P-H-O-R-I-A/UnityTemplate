using System.Collections.Generic;
using UnityEngine;

namespace Utility.GridEditor
{
    public class Grid : MonoBehaviour
    {
        private List<Cell> cells;
        
        public void Initialize(List<Cell> cells) => 
            this.cells = cells;
    }
}