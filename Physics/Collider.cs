using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SML.Physics
{
    public struct Collider
    {
        public object BindTarget;
        public Vector2[] Vectors;
        public Matrix Transform;
        public Vector2[] GetTransformedVectors()
        {
            Vector2[] transformedVectors = new Vector2[Vectors.Length];
            for (int i = 0; i < Vectors.Length; i++)
            {
                transformedVectors[i] = Vector2.Transform(Vectors[i], Transform);
            }
            return transformedVectors;
        }
        public SMRectangle GetEnvelope()
        {
            Vector2[] transformedVectors = GetTransformedVectors();
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (var vector in transformedVectors)
            {
                if (vector.X < minX) minX = vector.X;
                if (vector.X > maxX) maxX = vector.X;
                if (vector.Y < minY) minY = vector.Y;
                if (vector.Y > maxY) maxY = vector.Y;
            }
            return new SMRectangle(minX, minY, maxX - minX, maxY - minY);
        }
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(BindTarget, Vectors, Transform);
        }
        #region SAT
        public bool CheckCollisionSAT(Collider other)
        {
            Vector2[] transformedVectors = GetTransformedVectors();
            Vector2[] otherTransformedVectors = other.GetTransformedVectors();

            Vector2[] axes = GetAxes(transformedVectors);
            Vector2[] otherAxes = GetAxes(otherTransformedVectors);

            foreach (var axis in axes)
            {
                if (!IsOverlapping(axis, transformedVectors, otherTransformedVectors))
                {
                    return false;
                }
            }

            foreach (var axis in otherAxes)
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
            float min1, max1, min2, max2;
            Project(axis, transformedVectors1, out min1, out max1);
            Project(axis, transformedVectors2, out min2, out max2);

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
        public bool CheckCollisionGJK(Collider other)
        {
            Vector2[] transformedVectors = GetTransformedVectors();
            Vector2[] otherTransformedVectors = other.GetTransformedVectors();

            Vector2 direction = new Vector2(1, 0);
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

            foreach (var vector in vectors)
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
            return b * ac - a * bc;
        }
        #endregion
    }
}