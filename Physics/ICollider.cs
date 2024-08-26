using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SML.Physics
{
    public interface ICollider<out T>
    {
        public T BindTarget { get; }
        public Vector2[] Vectors { get; }
        public Quaternion Transform { get; }
        public virtual SMRectangle Envelope
        {
            get
            {
                if (Vectors?.Length < 3)
                {
                    return SMRectangle.Empty;
                }
                float xLeft = float.MaxValue, xRight = float.MinValue;
                float yTop = float.MaxValue, yBottom = float.MinValue;
                foreach (Vector2 v in Vectors)
                {
                    if(v.X < xLeft)
                    {
                        xLeft = v.X;
                    }
                    if (v.Y < yTop)
                    {
                        yTop = v.Y;
                    }
                    if (v.X > xRight)
                    {
                        xRight = v.X;
                    }
                    if (v.Y > yBottom)
                    {
                        yBottom = v.Y;
                    }
                }
                return new SMRectangle(xLeft, yTop, xRight - xLeft, yBottom - yTop);
            }
        }
    }
}
