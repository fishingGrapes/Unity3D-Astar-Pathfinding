using UnityEngine;
using System.Threading;
using MEC;
using System.Collections.Generic;

namespace Pathfinding
{

    [RequireComponent(typeof(PathFinder))]
    public class PathFactory : MonoBehaviour
    {
        #region Private Variables

        private Queue<PathResult> queue_PathResults;
        private static PathFactory mInstance;
        private PathFinder mPathFinder;


        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            mInstance = this;
            queue_PathResults = new Queue<PathResult>();
            mPathFinder = this.GetComponent<PathFinder>();
        }

        private void Start()
        {
            Timing.RunCoroutine(Tick());
        }
        #endregion


        #region Public Methods



        public static void RequestPath(PathRequest Request)
        {
            ThreadStart mPathFinderThread = delegate
            {
                mInstance.mPathFinder.FindPath(Request, mInstance.OnFinishedPathFinding);
            };
            mPathFinderThread.Invoke();
        }

        //Called from pathFinder Script
        public void OnFinishedPathFinding(PathResult PathResult)
        {
            lock (queue_PathResults)
            {
                queue_PathResults.Enqueue(PathResult);
            }
        }

        #endregion

        #region Private Methods

        private int nResultCount;
        private IEnumerator<float> Tick()
        {
            while (true)
            {
                nResultCount = queue_PathResults.Count;
                if (nResultCount > 0)
                {
                    lock (queue_PathResults)
                    {
                        for (int i = 0; i < nResultCount; i++)
                        {
                            PathResult mPathResult = queue_PathResults.Dequeue();
                            mPathResult.OnPathProcessed(mPathResult.RequestID, mPathResult.Path, mPathResult.Success);
                        }
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }

    #endregion

}



