using Microsoft.Xna.Framework;
using System;

namespace SML.Physics
{
    public struct Collider
    {
        public object BindTarget;
        public Vector2[] Vectors;
        public Matrix Transform;
        public readonly Vector2[] GetTransformedVectors()
        {
            Vector2[] transformedVectors = new Vector2[Vectors.Length];
            for (int i = 0; i < Vectors.Length; i++)
            {
                transformedVectors[i] = Vector2.Transform(Vectors[i], Transform);
            }
            return transformedVectors;
        }
        public readonly SMRectangle GetEnvelope()
        {
            Vector2[] transformedVectors = GetTransformedVectors();
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (Vector2 vector in transformedVectors)
            {
                if (vector.X < minX)
                {
                    minX = vector.X;
                }

                if (vector.X > maxX)
                {
                    maxX = vector.X;
                }

                if (vector.Y < minY)
                {
                    minY = vector.Y;
                }

                if (vector.Y > maxY)
                {
                    maxY = vector.Y;
                }
            }
            return new SMRectangle(minX, minY, maxX - minX, maxY - minY);
        }
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(BindTarget, Vectors, Transform);
        }
        #region SpecialCheck
        public readonly bool CheckCollisionSpecial(Collider other)
        {
            return Vectors is null || other.Vectors is null
                ? Vectors == other.Vectors
                : Vectors.Length == 1 && other.Vectors.Length == 1
                ? Vectors[0] == other.Vectors[0]
                : Vectors.Length == 1 && other.Vectors.Length == 2
                ? IsPointOnLine(Vectors[0], other.Vectors[0], other.Vectors[1])
                : Vectors.Length == 2 && other.Vectors.Length == 1
                ? IsPointOnLine(other.Vectors[0], Vectors[0], Vectors[1])
                : IsLinesCross(Vectors[0], Vectors[1], other.Vectors[0], other.Vectors[1]);
        }
        private static bool IsLinesCross(Vector2 line1Start, Vector2 line1End, Vector2 line2Start, Vector2 line2End)
        {
            float denominator = ((line1End.X - line1Start.X) * (line2End.Y - line2Start.Y)) - ((line1End.Y - line1Start.Y) * (line2End.X - line2Start.X));
            if (denominator == 0)
            {
                return !(line1Start == line2Start || line1Start == line2End);
            }
            float numerator1 = ((line1Start.Y - line2Start.Y) * (line2End.X - line2Start.X)) - ((line1Start.X - line2Start.X) * (line2End.Y - line2Start.Y));
            float numerator2 = ((line1Start.Y - line2Start.Y) * (line1End.X - line1Start.X)) - ((line1Start.X - line2Start.X) * (line1End.Y - line1Start.Y));
            float r = numerator1 / denominator;
            float s = numerator2 / denominator;
            return r >= 0 && r <= 1 && s >= 0 && s <= 1;
        }
        private static bool IsPointOnLine(Vector2 point, Vector2 lineStart, Vector2 LineEnd)
        {
            return point == lineStart || point == LineEnd
|| (lineStart != LineEnd
&& (point.X - lineStart.X) / (LineEnd.X - lineStart.X) == (point.Y - lineStart.Y) / (LineEnd.Y - lineStart.Y) && (point - lineStart).Length() < (LineEnd - lineStart).Length());
        }
        #endregion
        #region SAT
        public readonly bool CheckCollisionSAT(Collider other)
        {
            if ((Vectors?.Length ?? 0) < 3 || (other.Vectors?.Length ?? 0) < 3)
            {
                return CheckCollisionSpecial(other);
            }
            Vector2[] transformedVectors = GetTransformedVectors();
            Vector2[] otherTransformedVectors = other.GetTransformedVectors();

            Vector2[] axes = GetAxes(transformedVectors);
            Vector2[] otherAxes = GetAxes(otherTransformedVectors);

            foreach (Vector2 axis in axes)
            {
                if (!IsOverlapping(axis, transformedVectors, otherTransformedVectors))
                {
                    return false;
                }
            }

            foreach (Vector2 axis in otherAxes)
            {
                if (!IsOverlapping(axis, transformedVectors, otherTransformedVectors))
                {
                    return false;
                }
            }

            return true;
        }
        private static Vector2[] GetAxes(Vector2[] transformedVectors)
        {
            Vector2[] axes = new Vector2[transformedVectors.Length];

            for (int i = 0; i < transformedVectors.Length; i++)
            {
                Vector2 p1 = transformedVectors[i];
                Vector2 p2 = transformedVectors[(i + 1) % transformedVectors.Length];
                Vector2 edge = p2 - p1;
                axes[i] = new Vector2(-edge.Y, edge.X);
            }

            return axes;
        }
        private static bool IsOverlapping(Vector2 axis, Vector2[] transformedVectors1, Vector2[] transformedVectors2)
        {
            Project(axis, transformedVectors1, out float min1, out float max1);
            Project(axis, transformedVectors2, out float min2, out float max2);

            return !(min1 > max2 || min2 > max1);
        }
        private static void Project(Vector2 axis, Vector2[] transformedVectors, out float min, out float max)
        {
            min = Vector2.Dot(transformedVectors[0], axis);
            max = min;

            for (int i = 1; i < transformedVectors.Length; i++)
            {
                float projection = Vector2.Dot(transformedVectors[i], axis);
                if (projection < min)
                {
                    min = projection;
                }
                else if (projection > max)
                {
                    max = projection;
                }
            }
        }
        #endregion
        #region GJK
        public readonly bool CheckCollisionGJK(Collider other)
        {
            if ((Vectors?.Length ?? 0) < 3 || (other.Vectors?.Length ?? 0) < 3)
            {
                return CheckCollisionSpecial(other);
            }
            Vector2[] transformedVectors = GetTransformedVectors();
            Vector2[] otherTransformedVectors = other.GetTransformedVectors();

            Vector2 direction = new(1, 0);
            Vector2[] simplex = new Vector2[3];
            int simplexSize = 0;

            simplex[simplexSize++] = Support(transformedVectors, otherTransformedVectors, direction);
            direction = -simplex[0];

            while (true)
            {
                simplex[simplexSize++] = Support(transformedVectors, otherTransformedVectors, direction);

                if (Vector2.Dot(simplex[simplexSize - 1], direction) <= 0)
                {
                    return false;
                }

                if (HandleSimplex(ref simplex, ref simplexSize, ref direction))
                {
                    return true;
                }
            }
        }
        private static Vector2 Support(Vector2[] transformedVectors, Vector2[] otherTransformedVectors, Vector2 direction)
        {
            Vector2 point1 = GetFarthestPoint(transformedVectors, direction);
            Vector2 point2 = GetFarthestPoint(otherTransformedVectors, -direction);

            return point1 - point2;
        }
        private static Vector2 GetFarthestPoint(Vector2[] vectors, Vector2 direction)
        {
            float maxDot = float.MinValue;
            Vector2 farthestPoint = vectors[0];

            foreach (Vector2 vector in vectors)
            {
                float dot = Vector2.Dot(vector, direction);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    farthestPoint = vector;
                }
            }

            return farthestPoint;
        }
        private static bool HandleSimplex(ref Vector2[] simplex, ref int simplexSize, ref Vector2 direction)
        {
            if (simplexSize == 3)
            {
                Vector2 a = simplex[2];
                Vector2 b = simplex[1];
                Vector2 c = simplex[0];

                Vector2 ab = b - a;
                Vector2 ac = c - a;
                Vector2 ao = -a;

                Vector2 abPerp = TripleProduct(ac, ab, ab);
                Vector2 acPerp = TripleProduct(ab, ac, ac);

                if (Vector2.Dot(abPerp, ao) > 0)
                {
                    simplex[0] = simplex[1];
                    simplex[1] = simplex[2];
                    simplexSize = 2;
                    direction = abPerp;
                }
                else if (Vector2.Dot(acPerp, ao) > 0)
                {
                    simplex[1] = simplex[2];
                    simplexSize = 2;
                    direction = acPerp;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                Vector2 a = simplex[1];
                Vector2 b = simplex[0];
                Vector2 ab = b - a;
                Vector2 ao = -a;
                direction = TripleProduct(ab, ao, ab);
            }

            return false;
        }
        private static Vector2 TripleProduct(Vector2 a, Vector2 b, Vector2 c)
        {
            float ac = Vector2.Dot(a, c);
            float bc = Vector2.Dot(b, c);
            return (b * ac) - (a * bc);
        }
        #endregion
    }
}