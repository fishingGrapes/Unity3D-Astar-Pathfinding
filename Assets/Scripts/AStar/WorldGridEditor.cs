#if UNITY_EDITOR


using UnityEngine;
using UnityEditor;

namespace Pathfinding
{

    [CustomEditor(typeof(WorldGrid))]
    public class WorldGridEditor : Editor
    {

        private WorldGrid mWorldGrid;


        private void OnEnable()
        {
            mWorldGrid = (WorldGrid)target;

        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Generate World Grid"))
            {
                mWorldGrid.Generate();
            }
        }

    }
}

#endif
