using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using MEC;

public class Unit : MonoBehaviour
{

    #region Private Fields and Properties

    [SerializeField]
    private Transform Target = null;

    [SerializeField]
    private float fMovementSpeed = 10f;
    [SerializeField]
    private float fTurnDistance = 3f;
    [SerializeField]
    private float fTurnSpeed = 0.1f;

    [SerializeField]
    private bool DrawPathGizmos = false;
    [SerializeField]
    private bool SmoothPath = false;

    [SerializeField]
    private float fSpeed = 5f;


    private Transform mUnitTransform;
    private Path mPath;

    private Vector3[] vec3_WayPoints;
    private int nTargetIndex;

    #endregion

    #region Unity Callbacks
    void Start()
    {
        mUnitTransform = this.transform;
        PathFactory.RequestPath(new PathRequest(mUnitTransform.position, Target.position, OnPathProcessed));
    }

    #endregion

    #region Private Methods

    private void OnPathProcessed(Vector3[] WayPoints, bool bPathFound)
    {
        if (bPathFound)
        {

            if (SmoothPath)
            {
                mPath = new Path(WayPoints, mUnitTransform.position, fTurnDistance);

                Timing.KillCoroutines("WalkTheSmoothPath");
                Timing.RunCoroutine(WalkTheSmoothPath());
            }
            else
            {
                vec3_WayPoints = WayPoints;
                nTargetIndex = 0;

                Timing.KillCoroutines("WalkThePath");
                Timing.RunCoroutine(WalkThePath());
            }

        }
    }

    IEnumerator<float> WalkTheSmoothPath()
    {

        //TODO: !Implement Sharp Turns long Y-Axis instead of Smooth Lerps
        int nPathIndex = 0;
        bool bFollowingPath = true;
        mUnitTransform.LookAt(mPath.LookPoints[0]);

        while (bFollowingPath)
        {
            Vector2 vec2_Position2D = new Vector2(mUnitTransform.position.x, mUnitTransform.position.z);
            while (mPath.TurnBoundaries[nPathIndex].HasCrossedLine(vec2_Position2D))
            {
                if (nPathIndex == mPath.FinalLineIndex)
                {
                    bFollowingPath = false;
                    break;
                }
                else
                {
                    nPathIndex += 1;
                }
            }

            if (bFollowingPath)
            {
                //TODO: Avoid Lerping in Z-Axis
                Quaternion quat_TargetRotation = Quaternion.LookRotation(mPath.LookPoints[nPathIndex] - mUnitTransform.position);
                mUnitTransform.rotation = Quaternion.Lerp(mUnitTransform.rotation, quat_TargetRotation, fTurnSpeed * Time.deltaTime);
                mUnitTransform.Translate(Vector3.forward * Time.deltaTime * fMovementSpeed, Space.Self);
            }

            yield return Timing.WaitForOneFrame;

        }
    }

    IEnumerator<float> WalkThePath()
    {
        Vector3 currentWaypoint = vec3_WayPoints[0];

        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                nTargetIndex += 1;
                if (nTargetIndex >= vec3_WayPoints.Length)
                {
                    yield break;
                }
                currentWaypoint = vec3_WayPoints[nTargetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, fSpeed * Time.deltaTime);
            yield return Timing.WaitForOneFrame;

        }
    }

    private void OnDrawGizmos()
    {
        if (DrawPathGizmos)
        {
            if (SmoothPath)
            {
                if (mPath != null)
                {
                    mPath.DrawWithGizmos();
                }
            }
            else
            {
                if (vec3_WayPoints != null)
                {
                    for (int i = nTargetIndex; i < vec3_WayPoints.Length; i++)
                    {
                        Gizmos.color = Color.black;
                        Gizmos.DrawCube(vec3_WayPoints[i], Vector3.one);

                        if (i == nTargetIndex)
                        {
                            Gizmos.DrawLine(mUnitTransform.position, vec3_WayPoints[i]);
                        }
                        else
                        {
                            Gizmos.DrawLine(vec3_WayPoints[i - 1], vec3_WayPoints[i]);
                        }
                    }
                }
            }
        }

    }





    #endregion


}
