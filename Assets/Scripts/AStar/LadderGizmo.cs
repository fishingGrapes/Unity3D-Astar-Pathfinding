using UnityEngine;
using MEC;
using System.Collections.Generic;

[ExecuteInEditMode]
public class LadderGizmo : MonoBehaviour
{

    #region Private Fields and Properties

    private Transform mParentTransform;
    private BoxCollider[] mParentColliders;
    private float fHeight;
    private Transform mGizmoTransform;
    private int nParentColliderCount;

    #endregion

    #region Unity Callbacks
    void Start()
    {
        mGizmoTransform = this.transform;

        mParentTransform = mGizmoTransform.GetComponentInParent<Transform>();

        Timing.RunCoroutine(UpdateGizmo());
    }

    IEnumerator<float> UpdateGizmo()
    {
        mParentColliders = mParentTransform.GetComponentsInParent<BoxCollider>();
        nParentColliderCount = mParentColliders.Length;

        fHeight = Mathf.Abs(transform.position.y - mParentColliders[nParentColliderCount - 1].center.y);
        mGizmoTransform.localScale = new Vector3(mParentTransform.localScale.x, fHeight, mParentTransform.localScale.z);
        mGizmoTransform.position = new Vector3(mParentTransform.position.x, fHeight * 0.5f, mParentTransform.position.z);

        yield return Timing.WaitForSeconds(0.5f);
    }
    #endregion


}
