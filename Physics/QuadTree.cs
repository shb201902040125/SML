using Microsoft.Xna.Framework;
using SML.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SML.Physics
{
    public class QuadTree
    {
        private readonly int _maxDepth;
        private readonly float _minSize;
        private readonly List<Collider> _colliders;
        private readonly SMRectangle _bounds;
        private QuadTree[] _children;
        private int MaxColliderPerQuadTree;
        public QuadTree(SMRectangle bounds, int maxDepth, float minSize, int maxColliderPerQuadTree = 4)
        {
            _bounds = bounds;
            _maxDepth = maxDepth;
            _minSize = minSize;
            _colliders = new List<Collider>();
            _children = null;
            MaxColliderPerQuadTree = maxColliderPerQuadTree;
        }
        public bool Insert(Collider collider)
        {
            if (!_bounds.Intersects(collider.GetEnvelope()))
            {
                return false;
            }

            if (_children == null)
            {
                if (_colliders.Count < MaxColliderPerQuadTree || _maxDepth == 0 || _bounds.Width <= _minSize || _bounds.Height <= _minSize)
                {
                    _colliders.Add(collider);
                    return true;
                }
                Subdivide();
            }

            foreach (var child in _children)
            {
                if (child.Insert(collider))
                {
                    return true;
                }
            }

            return false;
        }
        public bool Remove(Collider collider)
        {
            if (!_bounds.Intersects(collider.GetEnvelope()))
            {
                return false;
            }

            if (_children == null)
            {
                return _colliders.Remove(collider);
            }

            bool removed = false;
            foreach (var child in _children)
            {
                if (child.Remove(collider))
                {
                    removed = true;
                    break;
                }
            }

            if (removed)
            {
                Merge();
            }

            return removed;
        }
        public bool Contains(Collider collider)
        {
            if (!_bounds.Intersects(collider.GetEnvelope()))
            {
                return false;
            }

            if (_children == null)
            {
                return _colliders.Contains(collider);
            }

            foreach (var child in _children)
            {
                if (child.Contains(collider))
                {
                    return true;
                }
            }

            return false;
        }
        public void GetIntersectingColliders(Collider collider, HashSet<Collider> container, bool useSAT = true)
        {
            if (!_bounds.Intersects(collider.GetEnvelope()))
            {
                return;
            }
            foreach (var c in _colliders)
            {
                if (container.Contains(c))
                {
                    continue;
                }
                if (useSAT ? c.CheckCollisionSAT(collider) : c.CheckCollisionGJK(collider))
                {
                    container.Add(c);
                }
            }
            if (_children is not null)
            {
                foreach (var child in _children)
                {
                    child.GetIntersectingColliders(collider, container, useSAT);
                }
            }
        }
        public void Clear()
        {
            _colliders.Clear();
            if (_children != null)
            {
                foreach (var child in _children)
                {
                    child.Clear();
                }
                _children = null;
            }
        }
        private void Subdivide()
        {
            var halfWidth = _bounds.Width / 2;
            var halfHeight = _bounds.Height / 2;
            var center = _bounds.Center;

            _children = new QuadTree[4];
            _children[0] = new QuadTree(new SMRectangle(_bounds.LeftTop, halfWidth, halfHeight), _maxDepth - 1, _minSize);
            _children[1] = new QuadTree(new SMRectangle(new Vector2(center.X, _bounds.LeftTop.Y), halfWidth, halfHeight), _maxDepth - 1, _minSize);
            _children[2] = new QuadTree(new SMRectangle(new Vector2(_bounds.LeftTop.X, center.Y), halfWidth, halfHeight), _maxDepth - 1, _minSize);
            _children[3] = new QuadTree(new SMRectangle(center, halfWidth, halfHeight), _maxDepth - 1, _minSize);
        }
        private void Merge()
        {
            if (_children == null)
            {
                return;
            }

            var totalColliders = _colliders.Count;
            foreach (var child in _children)
            {
                totalColliders += child._colliders.Count;
            }

            if (totalColliders <= MaxColliderPerQuadTree)
            {
                foreach (var child in _children)
                {
                    _colliders.AddRange(child._colliders);
                }
                _children = null;
            }
        }
    }
}
