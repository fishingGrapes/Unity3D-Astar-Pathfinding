using UnityEngine;

namespace Pathfinding
{
    public struct Line
    {

        public float Gradient;
        public float y_Intercept;

        public float Gradient_Perpendicular;

        private const float Gradient_Vertical = 1e5f;
        private Vector2 vec2_PointOnLine_1, vec2_PointOnLine_2;
        private bool bApproachSide;

        public Line(Vector2 PointOnLine, Vector2 PointPerpendicluarToLine)
        {
            float dx = PointOnLine.x - PointPerpendicluarToLine.x;
            float dy = PointOnLine.y - PointPerpendicluarToLine.y;
            bApproachSide = false;

            //If Line is Vertical, then dx = 0
            //So, to avoid Invalid operation, we divide by a very high value
            if (dx == 0)
            {
                Gradient_Perpendicular = Gradient_Vertical;
            }
            else
            {
                Gradient_Perpendicular = dy / dx;
            }

            if (Gradient_Perpendicular == 0)
            {
                Gradient = Gradient_Vertical;
            }
            else
            {
                Gradient = -1 / Gradient_Perpendicular;
            }


            //c = y - mx
            y_Intercept = PointOnLine.y - Gradient * PointOnLine.x;

            vec2_PointOnLine_1 = PointOnLine;
            vec2_PointOnLine_2 = PointOnLine + new Vector2(1, Gradient);
            bApproachSide = GetSide(PointPerpendicluarToLine);
        }


        public void DrawWithGizmos(float Length)
        {
            Vector3 vec3_LineDirection = new Vector3(1, 0, Gradient).normalized;
            Vector3 vec3_LineCenter = new Vector3(vec2_PointOnLine_1.x, 0, vec2_PointOnLine_1.y) + Vector3.up;

            Gizmos.DrawLine(vec3_LineCenter - vec3_LineDirection * Length * 0.5f, vec3_LineCenter + vec3_LineDirection * Length * 0.5f);
        }


        public bool HasCrossedLine(Vector2 Point)
        {
            return GetSide(Point) != bApproachSide;
        }

        private bool GetSide(Vector2 Point)
        {
            return (Point.x - vec2_PointOnLine_1.x) * (vec2_PointOnLine_2.y - vec2_PointOnLine_1.y) > (Point.y - vec2_PointOnLine_1.y) * (vec2_PointOnLine_2.x - vec2_PointOnLine_1.x);
        }
    }
}
