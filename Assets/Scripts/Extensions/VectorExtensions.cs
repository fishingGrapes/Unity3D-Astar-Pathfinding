using UnityEngine;

namespace Extensions
{
    public class VectorExtensions
    {

        public static Vector3 Multiply(Vector3 vector, float scalar)
        {
            Vector3 result;
            result.x = vector.x * scalar;
            result.y = vector.y * scalar;
            result.z = vector.z * scalar;
            return result;
        }

        public static Vector2 Multiply(Vector2 vector, float scalar)
        {
            Vector2 result;
            result.x = vector.x * scalar;
            result.y = vector.y * scalar;
            return result;
        }

        public static Vector3 Direction(Vector3 From, Vector3 To, bool Normalized = true)
        {
            return Normalized ? new Vector3(To.x - From.x, To.y - From.y, To.z - From.z).normalized : new Vector3(To.x - From.x, To.y - From.y, To.z - From.z);
        }

        public static Vector2 Direction(Vector2 From, Vector2 To, bool Normalized = true)
        {
            return Normalized ? new Vector3(To.x - From.x, To.y - From.y).normalized : new Vector3(To.x - From.x, To.y - From.y);
        }
    }
}
