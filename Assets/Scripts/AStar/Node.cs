using UnityEngine;

namespace Pathfinding
{
    public class Node : IHeapItem<Node>
    {
        public bool Traversable;
        public Vector3 WorldPosition;

        public int X;
        public int Y;
        public int Z;

        public int Weight;


        #region Nodal data
        public Node Parent;

        public int gCost;
        public int hCost;

        public int fCost
        {
            get { return (gCost + hCost); }
        }

        #endregion

        #region IHeapItem Implementations

        private int nHeapIndex;
        public int HeapIndex
        {
            get
            {
                return nHeapIndex;
            }

            set
            {
                nHeapIndex = value;
            }
        }

        public int CompareTo(Node other)
        {
            int nCompareResult = fCost.CompareTo(other.fCost);
            if (nCompareResult == 0)
            {
                nCompareResult = hCost.CompareTo(other.hCost);
            }

            //We Multiply by -1 because lower f_cost is Higher Priority
            return (-1 * nCompareResult);
        }

        #endregion
    }


}
