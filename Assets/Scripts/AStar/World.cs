using UnityEngine;

namespace Pathfinding
{
    [CreateAssetMenu(fileName = "newGrid", menuName = "A*/World", order = 1)]
    public class World : ScriptableObject
    {
        public LayerMask UnWalkableMask;

        public Vector3 WorldDimension;
        public Vector3 WorldCenter;


        [Tooltip("The Diameter of Each Node. The Higher The Value, the Lower the Resolution.")]
        [Range(0, 10)]
        public float NodeDimension;

    }
}
