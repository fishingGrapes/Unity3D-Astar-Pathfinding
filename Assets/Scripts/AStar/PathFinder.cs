using UnityEngine;
using System.Collections.Generic;
using System;

namespace Pathfinding
{
    public class PathFinder : MonoBehaviour
    {

        #region Private Variables

        private WorldGrid mGrid;
        private int nTotalNodeCount;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            mGrid = this.GetComponent<WorldGrid>();
            nTotalNodeCount = mGrid.TotalNodeCount;
        }

        #endregion

        #region Public Functions

        public void FindPath(PathRequest Request, Action<PathResult> OnPathProcessed)
        {
            Node mStartNode = mGrid.WorldSpaceToNode(Request.PathStart);
            Node mDestinationNode = mGrid.WorldSpaceToNode(Request.PathEnd);

            Vector3[] vec3_WayPoints = new Vector3[0];
            bool bPathFound = false;

            if (mStartNode == null || mDestinationNode == null)
                return;
            if (mStartNode.Traversable && mDestinationNode.Traversable)
            {

                Heap<Node> heap_OpenSet = new Heap<Node>(nTotalNodeCount);
                HashSet<Node> hset_ClosedSet = new HashSet<Node>();
                List<Node> list_Neighbours = new List<Node>();

                int nNewMovementCost;
                bool bOpenSetContainsNeighbour;

                //Add the Start Node to the Open Set
                heap_OpenSet.Add(mStartNode);

                //Unltil the Open Set is NOT Empty, Execute the following code
                while (heap_OpenSet.Count > 0)
                {
                    //Choose a Current Node from the Open set
                    //current node is the node in open set with the lowest f_Cost
                    Node mCurrentNode = heap_OpenSet.RemoveFirst();
                    hset_ClosedSet.Add(mCurrentNode);

                    if (mCurrentNode == mDestinationNode)
                    {
                        bPathFound = true;
                        break;
                    }

                    //Get the Neighbouring Nodes
                    mGrid.GetNeighbours_NonAlloc(mCurrentNode, list_Neighbours);
                    foreach (Node mNeighbourNode in list_Neighbours)
                    {

                        //If the Neighbouring Node is not Traversable OR
                        //If the Neighbouring Node is already Proceesed, Continue to next Neighbour
                        if (!mNeighbourNode.Traversable || hset_ClosedSet.Contains(mNeighbourNode))
                        {
                            continue;
                        }

                        //Calcualte the new Movement Cost between the Current Node and The neighbour Node
                        nNewMovementCost = mCurrentNode.gCost + this.GetDistance(mCurrentNode, mNeighbourNode) + mNeighbourNode.Weight;
                        bOpenSetContainsNeighbour = heap_OpenSet.Contains(mNeighbourNode);

                        //If the new Movement Cost is less than the Perevioys OR
                        //if the Open Set does not conatin the Neighbour Node, then
                        if (nNewMovementCost < mNeighbourNode.gCost || !bOpenSetContainsNeighbour)
                        {

                            //Upsate the g_cost and h_cost
                            mNeighbourNode.gCost = nNewMovementCost;
                            mNeighbourNode.hCost = this.GetDistance(mNeighbourNode, mDestinationNode);
                            mNeighbourNode.Parent = mCurrentNode;

                            if (!bOpenSetContainsNeighbour)
                            {
                                heap_OpenSet.Add(mNeighbourNode);
                            }
                            else
                            {
                                heap_OpenSet.UpdateItem(mNeighbourNode);
                            }
                        }

                    }

                }
            }

            if (bPathFound)
            {
                vec3_WayPoints = this.RetracePath(mStartNode, mDestinationNode);
            }
            OnPathProcessed(new PathResult(Request.id, vec3_WayPoints, bPathFound, Request.OnPathProcessed));
        }

        #endregion

        #region Private functions
        private Vector3[] RetracePath(Node startNode, Node endNode)
        {
            List<Node> list_PathNodes = new List<Node>();
            Node mCurrentNode = endNode;

            while (mCurrentNode != startNode)
            {
                list_PathNodes.Add(mCurrentNode);
                mCurrentNode = mCurrentNode.Parent;
            }

            Vector3[] vec3_SimplifiedPath = this.SimplifyPath(list_PathNodes);
            Array.Reverse(vec3_SimplifiedPath);

            return vec3_SimplifiedPath;
        }

        private Vector3[] SimplifyPath(List<Node> Path)
        {
            //TODO: Change with respect to 3D
            List<Vector3> list_WayPoints = new List<Vector3>();
            Vector3 vec3_DirectionPrevious = Vector3.zero;

            int nPathLength = Path.Count;

            for (int i = 1; i < nPathLength; i++)
            {
                Vector3 vec3_DirectionCurrent = new Vector3((Path[i - 1].X - Path[i].X), (Path[i - 1].Y - Path[i].Y), (Path[i - 1].Z - Path[i].Z));
                if (vec3_DirectionPrevious != vec3_DirectionCurrent)
                {
                    list_WayPoints.Add(Path[i - 1].WorldPosition);
                    list_WayPoints.Add(Path[i].WorldPosition);
                }

                vec3_DirectionPrevious = vec3_DirectionCurrent;
            }

            //for (int i = 0; i < nPathLength; i++)
            //{
            //    list_WayPoints.Add(Path[i].WorldPosition);
            //}

            return list_WayPoints.ToArray();
        }

        //Distance between Nodes
        private int GetDistance(Node node1, Node node2)
        {
            //if X > Y, then distance = (14 * x + 10 * (X - Y))
            //else, distance = (14 * Y + 10 * (Y - X))
            int nDistanceX = Mathf.Abs(node1.X - node2.X);
            int nDistanceZ = Mathf.Abs(node1.Z - node2.Z);
            int nDistanceY = Mathf.Abs(node1.Y - node2.Y);

            if (nDistanceX > nDistanceZ)
            {
                return (14 * nDistanceZ + 10 * (nDistanceX - nDistanceZ) + 10 * nDistanceY);
            }
            else
            {
                return (14 * nDistanceX + 10 * (nDistanceZ - nDistanceX) + 10 * nDistanceY);

            }
        }

        #endregion

    }
}
