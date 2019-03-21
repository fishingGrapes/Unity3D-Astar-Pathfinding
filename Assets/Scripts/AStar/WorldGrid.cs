using Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    //[ExecuteInEditMode]
    public class WorldGrid : MonoBehaviour
    {

        #region Private Variables

        [SerializeField]
        private bool displayGridGizmos = false;

        [Header("World")]
        [SerializeField]
        private World mWorld = null;

        [Header("Terrains")]
        [SerializeField]
        private Terrain[] Terrains = null;

        private float fNoderadius;
        private float fNodeDiameter;
        private int nGridColumns;
        private int nGridRows;
        private int nGridDepth;

        private Vector3 vec3_GridDimensions;
        private Vector3 vec3_GridCenter;
        private Vector3 vec3_WorldBottomLeft;

        private Node[,,] mGrid;
        //The Walkable Layers in this particular World
        private LayerMask mask_WalkableMask = 0;
        //A Map from Walkable Mask to Penalty
        private Dictionary<int, int> map_TerrainMaskToWeight = null;

        public int TotalNodeCount
        {
            get { return (nGridColumns * nGridRows * nGridDepth); }
        }

        //Testing or changed inn Future
        private LayerMask mask_UnWalkableMask;
        private int nMaxWeight = int.MinValue, nMinWeight = int.MaxValue;


        #endregion

        #region Unity Callbacks

        private void Awake()
        {

            fNodeDiameter = mWorld.NodeDimension;
            fNoderadius = fNodeDiameter * 0.5f;
            vec3_GridDimensions = mWorld.WorldDimension;

            //In a XZ Plane
            nGridRows = Mathf.RoundToInt(vec3_GridDimensions.z / fNodeDiameter);
            nGridColumns = Mathf.RoundToInt(vec3_GridDimensions.x / fNodeDiameter);
            nGridDepth = Mathf.RoundToInt(vec3_GridDimensions.y / fNodeDiameter);


            mask_UnWalkableMask = mWorld.UnWalkableMask;
            vec3_GridCenter = mWorld.WorldCenter;

            map_TerrainMaskToWeight = new Dictionary<int, int>();
            foreach (Terrain mTerrain in Terrains)
            {
                mask_WalkableMask.value += mTerrain.LayerMask.value;

                int nTerrainMask = Mathf.RoundToInt(Mathf.Log(mTerrain.LayerMask.value, 2));
                if (!map_TerrainMaskToWeight.ContainsKey(nTerrainMask))
                {
                    map_TerrainMaskToWeight.Add(nTerrainMask, mTerrain.Weight);
                }
            }

            this.Generate();
            this.BlurWeightMap(3);
            //Testing
        }


        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(vec3_GridCenter, new Vector3(vec3_GridDimensions.x, vec3_GridDimensions.y, vec3_GridDimensions.z));


            if (displayGridGizmos)
            {
                if (mGrid != null)
                {
                    foreach (Node mNode in mGrid)
                    {
                        if (mNode != null)
                        {
                            Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(nMinWeight, nMaxWeight, mNode.Weight));
                            Gizmos.color = mNode.Traversable ? Gizmos.color : Color.red;
                            Gizmos.DrawCube(mNode.WorldPosition, VectorExtensions.Multiply(Vector3.one, fNodeDiameter));
                        }
                    }
                }
            }
        }

        #endregion


        #region Other Methods

        public void Generate()
        {
            //Rows are arranged along Y-Axis while Columns along X-Axis
            mGrid = new Node[nGridColumns, nGridDepth, nGridRows];

            Vector3 vec3_OffsetX = VectorExtensions.Multiply(Vector3.right, (vec3_GridDimensions.x * 0.5f));
            Vector3 vec3_OffsetZ = VectorExtensions.Multiply(Vector3.forward, (vec3_GridDimensions.z * 0.5f));
            Vector3 vec3_OffsetY = VectorExtensions.Multiply(Vector3.up, (vec3_GridDimensions.y * 0.5f));

            //Get th Bottom Left of the World
            vec3_WorldBottomLeft = vec3_GridCenter - (vec3_OffsetX + vec3_OffsetZ);

            //Iterate through each point in the array and fill its positional data(for now)
            //* * * *
            //* * * *
            //* * * *
            //* * * * 
            //vec3_WorldBottomLeft is the lower and most left VERTEX (NOT NODE)

            for (int y = 0; y < nGridDepth; y++)
            {
                for (int x = 0; x < nGridColumns; x++)
                {
                    for (int z = 0; z < nGridRows; z++)
                    {
                        vec3_OffsetX = VectorExtensions.Multiply(Vector3.right, (x * fNodeDiameter + fNoderadius));
                        vec3_OffsetZ = VectorExtensions.Multiply(Vector3.forward, (z * fNodeDiameter + fNoderadius));
                        vec3_OffsetY = VectorExtensions.Multiply(Vector3.up, (y * fNodeDiameter));

                        Vector3 vec3_NodePosition = vec3_WorldBottomLeft + vec3_OffsetX + vec3_OffsetZ + vec3_OffsetY;
                        Vector3 vec3_HalfExtends = new Vector3(fNoderadius, fNoderadius, fNoderadius);
                        bool bTraversable = !(Physics.CheckBox(vec3_NodePosition, vec3_HalfExtends, Quaternion.identity, mask_UnWalkableMask));
                        //bool bTraversable = !(Physics.CheckSphere(vec3_NodePosition, fNoderadius, mask_UnWalkableMask));

                        int nWeight = 0;

                        //Create a Ray that starts 50 units above the Node Position, directed straight down
                        Ray mRay = new Ray(vec3_NodePosition + Vector3.up, Vector3.down);
                        RaycastHit mHit;
                        if (Physics.Raycast(mRay, out mHit, 1, mask_WalkableMask))
                        {
                            map_TerrainMaskToWeight.TryGetValue(mHit.collider.gameObject.layer, out nWeight);
                        }


                        //TODO! : Maybe Chnage SO it is Not Traversable instead of Null
                        if (mHit.collider != null)
                        {
                            //if (y >= 5)
                            //    Debug.Log(mHit.collider.name);

                            //TODO: Add Penalty Based on Obstcale 
                            if (!bTraversable)
                            {
                                nWeight += 14;
                            }

                            //TODO: Make this a Parameterized Constructor
                            mGrid[x, y, z] = new Node();

                            mGrid[x, y, z].WorldPosition = vec3_NodePosition;
                            mGrid[x, y, z].Traversable = bTraversable;
                            mGrid[x, y, z].X = x;
                            mGrid[x, y, z].Y = y;
                            mGrid[x, y, z].Z = z;

                            mGrid[x, y, z].Weight = nWeight;
                        }
                        else
                        {
                            mGrid[x, y, z] = null;
                        }

                    }
                }
            }
        }

        public Node WorldSpaceToNode(Vector3 WorldPosition)
        {
            //Thsi Equation makes it possibel to Center the Grid anywhere and still
            //get Accurate Results
            Vector3 vec3_localPosition = (WorldPosition - vec3_WorldBottomLeft);

            float percentX = (vec3_localPosition.x) / vec3_GridDimensions.x;
            float percentY = (vec3_localPosition.y) / vec3_GridDimensions.y;
            float percentZ = (vec3_localPosition.z) / vec3_GridDimensions.z;

            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);
            percentZ = Mathf.Clamp01(percentZ);

            int x = Mathf.RoundToInt((nGridColumns - 1) * percentX);
            //int y = Mathf.RoundToInt((nGridDepth - 1) * percentY);
            int y = Mathf.RoundToInt(nGridDepth * percentY);
            int z = Mathf.RoundToInt((nGridRows - 1) * percentZ);

            if (mGrid[x, y, z] == null)
                return null;
            return mGrid[x, y, z];
        }

        public void GetNeighbours_NonAlloc(Node mNode, List<Node> list_Neighbours)
        {
            list_Neighbours.Clear();
            int nNeighbourX, nNeighbourZ, nNeighbourY;


            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int z = -1; z <= 1; z++)
                    {

                        if (x == 0 && z == 0 && y == 0)
                            continue;

                        if (z != 0 && y != 0)
                            continue;

                        if (Mathf.Abs(x) != 0 && y != 0)
                            continue;


                        nNeighbourX = mNode.X + x;
                        nNeighbourZ = mNode.Z + z;
                        nNeighbourY = mNode.Y + y;


                        if (nNeighbourY >= 0 && nNeighbourY < nGridDepth)
                        {
                            if (nNeighbourX >= 0 && nNeighbourX < nGridColumns)
                            {
                                if (nNeighbourZ >= 0 && nNeighbourZ < nGridRows)
                                {
                                    if (mGrid[nNeighbourX, nNeighbourY, nNeighbourZ] == null)
                                        continue;
                                    list_Neighbours.Add(mGrid[nNeighbourX, nNeighbourY, nNeighbourZ]);
                                }

                            }
                        }

                    }
                }

            }


        }

        #endregion

        #region Private Methods

        private void BlurWeightMap(int nBlurSize)
        {
            //Kernel Size must be odd
            int nKernelSize = (nBlurSize * 2) + 1;
            //How many squares between central square and the edge of the kernel
            int nkernelExtends = (nKernelSize - 1) / 2;

            int[,] nHorizontalPass_Weights = new int[nGridColumns, nGridRows];
            int[,] nVerticalPass_Weights = new int[nGridColumns, nGridRows];

            for (int y = 0; y < nGridDepth; y++)
            {
                //Horizontal pass
                for (int z = 0; z < nGridRows; z++)
                {
                    //For First column of every Row , do this blur
                    for (int x = -nkernelExtends; x <= nkernelExtends; x++)
                    {

                        int nSampleX = Mathf.Clamp(x, 0, nkernelExtends);
                        if (mGrid[nSampleX, y, z] == null)
                            continue;
                        nHorizontalPass_Weights[0, z] += mGrid[nSampleX, y, z].Weight;
                    }

                    int nRemoveIndex, nAddIndex;
                    for (int x = 1; x < nGridColumns; x++)
                    {
                        nRemoveIndex = Mathf.Clamp(x - nkernelExtends - 1, 0, nGridColumns);
                        nAddIndex = Mathf.Clamp(x + nkernelExtends, 0, nGridColumns - 1);
                        if (mGrid[nAddIndex, y, z] == null || mGrid[nRemoveIndex, y, z] == null)
                            continue;
                        nHorizontalPass_Weights[x, z] = nHorizontalPass_Weights[x - 1, z] - mGrid[nRemoveIndex, y, z].Weight + mGrid[nAddIndex, y, z].Weight;
                    }
                }


                //Vertical Pass
                for (int x = 0; x < nGridColumns; x++)
                {
                    //For First column of every Row , do this blur
                    for (int z = -nkernelExtends; z <= nkernelExtends; z++)
                    {
                        int nSampleY = Mathf.Clamp(z, 0, nkernelExtends);
                        nVerticalPass_Weights[x, 0] += nHorizontalPass_Weights[x, nSampleY];
                    }
                    int nBlurredWeight = Mathf.RoundToInt((float)nVerticalPass_Weights[x, 0] / Mathf.Pow(nKernelSize, 2));


                    if (mGrid[x, y, 0] == null)
                        continue;
                    else
                        mGrid[x, y, 0].Weight = nBlurredWeight;

                    int nRemoveIndex, nAddIndex;
                    for (int z = 1; z < nGridRows; z++)
                    {
                        if (mGrid[x, y, 0] == null)
                            continue;

                        nRemoveIndex = Mathf.Clamp(z - nkernelExtends - 1, 0, nGridColumns);
                        nAddIndex = Mathf.Clamp(z + nkernelExtends, 0, nGridColumns - 1);

                        nVerticalPass_Weights[x, z] = nVerticalPass_Weights[x, z - 1] - nHorizontalPass_Weights[x, nRemoveIndex] + nHorizontalPass_Weights[x, nAddIndex];

                        nBlurredWeight = Mathf.RoundToInt((float)nVerticalPass_Weights[x, z] / Mathf.Pow(nKernelSize, 2));
                        mGrid[x, y, z].Weight = nBlurredWeight;

                        if (nBlurredWeight > nMaxWeight)
                        {
                            nMaxWeight = nBlurredWeight;
                        }
                        if (nBlurredWeight < nMinWeight)
                        {
                            nMinWeight = nBlurredWeight;
                        }
                    }
                }
            }
        }

        #endregion




    }
}
