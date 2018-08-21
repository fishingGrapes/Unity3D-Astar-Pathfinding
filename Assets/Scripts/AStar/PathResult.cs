using UnityEngine;
using System;

namespace Pathfinding
{
    public struct PathResult
    {

        public Vector3[] Path;
        public bool Success;
        public Action<Vector3[], bool> OnPathProcessed;

        public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> onPathProcessed)
        {
            this.Path = path;
            this.Success = success;
            this.OnPathProcessed = onPathProcessed;
        }
    }
}
