using Microsoft.Xna.Framework;
using System;

namespace SML.Physics
{
    public struct SMRectangle
    {
        public static readonly SMRectangle Empty = default;
        public Vector2 LeftTop;
        public float Width;
        public float Height;
        public readonly Vector2 RightTop => LeftTop + new Vector2(Width, 0);
        public readonly Vector2 LeftBottom => LeftTop + new Vector2(0, Height);
        public readonly Vector2 RightBottom => LeftTop + new Vector2(Width, Height);
        public readonly Vector2 MiddleLeft => LeftTop + new Vector2(0, Height / 2);
        public readonly Vector2 MiddleRight => LeftTop + new Vector2(Width, Height / 2);
        public readonly Vector2 MiddleTop => LeftTop + new Vector2(Width / 2, 0);
        public readonly Vector2 MiddleBottom => LeftTop + new Vector2(Width / 2, Height);
        public readonly Vector2 Center => LeftTop + (new Vector2(Width, Height) / 2);
        public readonly float Left => LeftTop.X;
        public readonly float Right => LeftTop.X + Width;
        public readonly float Top => LeftTop.Y;
        public readonly float Bottom => LeftTop.Y + Height;
        public SMRectangle(float x, float y, float width, float height)
        {
            LeftTop = new Vector2(x, y);
            Width = width;
            Height = height;
        }
        public SMRectangle(Vector2 leftTop, float width, float height)
        {
            LeftTop = leftTop;
            Width = width;
            Height = height;
        }
        public SMRectangle(Vector2 v1, Vector2 v2)
        {
            LeftTop = new(Math.Min(v1.X, v2.X), Math.Min(v1.Y, v2.Y));
            Width = Math.Abs(v1.X - v2.X);
            Height = Math.Abs(v1.Y - v2.Y);
        }
        public SMRectangle Scale(Vector2 center, float scale)
        {
            return new()
            {
                LeftTop = center + ((center - LeftTop) * scale),
                Width = Width * scale,
                Height = Height * scale
            };
        }
        public readonly bool Intersects(SMRectangle value)
        {
            return value.Left < Right && Left < value.Right && value.Top < Bottom && Top < value.Bottom;
        }
        public readonly bool Surround(SMRectangle value)
        {
            return value.Left >= Left && value.Right <= Right && value.Top >= Top && value.Bottom <= Bottom;
        }
        public static SMRectangle operator *(SMRectangle rec, float scale)
        {
            return rec.Scale(rec.Center, scale);
        }
    }
}
