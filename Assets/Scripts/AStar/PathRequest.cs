using UnityEngine;
using System;

namespace Pathfinding
{
    public struct PathRequest
    {
        public Vector3 PathStart;
        public Vector3 PathEnd;
        public Action<Guid, Vector3[], bool> OnPathProcessed;
        public Guid id;

        public PathRequest(Guid requestID, Vector3 PathStart, Vector3 PathEnd, Action<Guid, Vector3[], bool> OnPathProcessed)
        {
            this.PathStart = PathStart;
            this.PathEnd = PathEnd;
            this.OnPathProcessed = OnPathProcessed;
            this.id = requestID;
        }
    }
}