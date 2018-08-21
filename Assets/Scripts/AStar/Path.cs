using UnityEngine;

namespace Pathfinding
{
    public class Path
    {
        public readonly Vector3[] LookPoints;
        public readonly Line[] TurnBoundaries;
        public readonly int FinalLineIndex;

        //WayPoints Does/nt Include the Starting Node
        //So a Separate Parameter for it
        public Path(Vector3[] WayPoints, Vector3 StartPoint, float TurnDistance)
        {
            LookPoints = WayPoints;
            TurnBoundaries = new Line[LookPoints.Length];
            FinalLineIndex = TurnBoundaries.Length - 1;


            Vector2 vec2_PreviousPoint = this.ToVector2(StartPoint);
            int nPathLength = LookPoints.Length;
            for (int i = 0; i < nPathLength; i++)
            {
                Vector2 vec2_CurrentPoint = ToVector2(LookPoints[i]);
                Vector2 vec2_DirectionToCurrentPoint = (vec2_CurrentPoint - vec2_PreviousPoint).normalized;
                Vector2 vec2_TurnBoundaryPoint = (i == FinalLineIndex) ? vec2_CurrentPoint : vec2_CurrentPoint - (vec2_DirectionToCurrentPoint * TurnDistance);
                TurnBoundaries[i] = new Line(vec2_TurnBoundaryPoint, vec2_PreviousPoint - (vec2_DirectionToCurrentPoint * TurnDistance));

                vec2_PreviousPoint = vec2_TurnBoundaryPoint;
            }
        }

        public void DrawWithGizmos()
        {
            Gizmos.color = Color.black;
            foreach (Vector3 Point in LookPoints)
            {
                Gizmos.DrawCube(Point + Vector3.up, Vector3.one);
            }

            Gizmos.color = Color.white;
            foreach (Line line in TurnBoundaries)
            {
                line.DrawWithGizmos(10);
            }
        }

        private Vector2 ToVector2(Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }
    }
}
