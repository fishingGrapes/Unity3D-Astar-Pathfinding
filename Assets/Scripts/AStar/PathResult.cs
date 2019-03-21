using UnityEngine;
using System;

namespace Pathfinding
{
    public struct PathResult
    {

        public Vector3[] Path;
        public bool Success;
        public Action<Guid, Vector3[], bool> OnPathProcessed;
        public Guid RequestID;

        public PathResult(Guid requestID, Vector3[] path, bool success, Action<Guid, Vector3[], bool> onPathProcessed)
        {
            this.Path = path;
            this.Success = success;
            this.OnPathProcessed = onPathProcessed;
            this.RequestID = requestID;
        }
    }
}
