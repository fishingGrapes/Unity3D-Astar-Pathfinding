using UnityEngine;

namespace Pathfinding
{
    [CreateAssetMenu(fileName = "newTerrain", menuName = "A*/Terrain", order = 2)]
    public class Terrain : ScriptableObject
    {

        public LayerMask LayerMask;
        public int Weight;

    }
}
