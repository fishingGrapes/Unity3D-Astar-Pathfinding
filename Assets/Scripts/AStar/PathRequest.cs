using UnityEngine;
using System;

namespace Pathfinding
{
    public struct PathRequest
    {
        public Vector3 PathStart;
        public Vector3 PathEnd;
        public Action<Vector3[], bool> OnPathProcessed;

        public PathRequest(Vector3 PathStart, Vector3 PathEnd, Action<Vector3[], bool> OnPathProcessed)
        {
            this.PathStart = PathStart;
            this.PathEnd = PathEnd;
            this.OnPathProcessed = OnPathProcessed;
        }
    }
}