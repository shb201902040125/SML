using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SML.Physics
{
    public class Transform
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Scale = Vector3.One;
        public Quaternion Rotation = Quaternion.Identity;
        public Transform Parent = null;
        public Vector3 ApplyTo(Vector3 vector)
        {
            vector = Vector3.Transform((vector - Position) * Scale, Rotation) + Position;
            return Parent?.ApplyTo(vector) ?? vector;
        }
        public void ApplyTo(Vector3 vector, ref Vector3 buffer)
        {
            vector = Vector3.Transform((vector - Position) * Scale, Rotation) + Position;
            if (Parent is null)
            {
                buffer = vector;
            }
            else
            {
                Parent.ApplyTo(vector, ref buffer);
            }
        }
        public Vector3[] ApplyTo(Vector3[] vectors)
        {
            Vector3[] result = new Vector3[vectors.Length];
            for (int i = 0; i < vectors.Length; i++)
            {
                ApplyTo(vectors[i], ref result[i]);
            }
            return result;
        }
        public void ApplyTo(Vector3[] vectors, Vector3[] buffer, int start = 0)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(buffer.Length, start + vectors.Length);
            for (int i = 0; i < vectors.Length; i++)
            {
                ApplyTo(vectors[i], ref buffer[start+i]);
            }
        }
    }
}
