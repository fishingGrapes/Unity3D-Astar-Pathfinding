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
    private bool bDrawPathGizmos = false;


    private Transform mUnitTransform;
    private Path mPath;

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

            mPath = new Path(WayPoints, mUnitTransform.position, fTurnDistance);

            Timing.KillCoroutines("WalkThePath");
            Timing.RunCoroutine(WalkThePath());

        }
    }

    IEnumerator<float> WalkThePath()
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

    private void OnDrawGizmos()
    {
        if (bDrawPathGizmos)
        {
            if (mPath != null)
            {
                mPath.DrawWithGizmos();
            }
        }
    }

    #endregion


}
