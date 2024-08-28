using Microsoft.Xna.Framework;
using SML.Common;
using System;
using System.Collections.Generic;

namespace SML.Physics
{
    public class ColliderTree
    {
        SparseSet<object, Collider> _colliders;
        ColliderTreeNode _root;
        public ColliderTree(SMRectangle region, int maxCapacity)
        {
            _colliders = new(maxCapacity);
            _root = new(this, region);
        }
        public bool Insert(Collider collider)
        {
            if (!_colliders.Add(collider.BindTarget, collider))
            {
                return false;
            }
            var envelope = collider.GetEnvelope();
            if (!_root.Insert(ref collider, ref envelope))
            {
                _colliders.Remove(collider.BindTarget);
                return false;
            }
            return true;
        }
        public bool Remove(Collider collider)
        {
            if(!_colliders.Remove(collider))
            {
                return false;
            }
            var envelope = collider.GetEnvelope();
            return _root.Remove(ref collider, ref envelope);
        }
        public List<Collider> GetCollideWith(Collider collider,bool useSAT=true)
        {
            HashSet<object> list = [];
            var envelope = collider.GetEnvelope();
            _root.GetCollideWith(ref collider, ref envelope, list,useSAT);
            List<Collider> result = [];
            foreach (var obj in list)
            {
                result.Add(_colliders[obj]);
            }
            return result;
        }
        public void Clean()
        {
            _root.Clean();
        }
        public void Clear()
        {
            _root.Clear();
        }
        private class ColliderTreeNode
        {
            ColliderTree _tree;
            SMRectangle _region;
            HashSet<object> _objects;
            ColliderTreeNode[] _children;
            public ColliderTreeNode(ColliderTree tree, SMRectangle region)
            {
                _tree = tree;
                _region = region;
                _objects = [];
            }
            public bool Insert(ref Collider collider, ref SMRectangle envelope)
            {
                if (!_region.Intersects(envelope))
                {
                    return false;
                }
                SubDivide();
                foreach (var child in _children)
                {
                    if (child._region.Surround(envelope))
                    {
                        return child.Insert(ref collider, ref envelope);
                    }
                }
                _objects.Add(collider.BindTarget);
                return true;
            }
            public bool Remove(ref Collider collider, ref SMRectangle envelope)
            {
                if (!_region.Intersects(envelope))
                {
                    return false;
                }
                if (_children is not null)
                {
                    foreach (var child in _children)
                    {
                        if (child._region.Surround(envelope))
                        {
                            return child.Remove(ref collider, ref envelope);
                        }
                    }
                }
                return _objects.Remove(collider.BindTarget);
            }
            internal void GetCollideWith(ref Collider collider, ref SMRectangle envelope, HashSet<object> list, bool useSAT)
            {
                if (!_region.Intersects(envelope))
                {
                    return;
                }
                if (_children is not null)
                {
                    foreach (var child in _children)
                    {
                        if (child._region.Surround(envelope))
                        {
                            child.GetCollideWith(ref collider, ref envelope, list, useSAT);
                            return;
                        }
                    }
                }
                foreach (var obj in _objects)
                {
                    var c = _tree._colliders[obj];
                    if (useSAT ? c.CheckCollisionSAT(collider) : c.CheckCollisionGJK(collider))
                    {
                        list.Add(obj);
                    }
                }
            }
            public void Clean()
            {
                if (_children is not null)
                {
                    bool cleanChildren = true;
                    foreach (var child in _children)
                    {
                        child.Clean();
                        cleanChildren = cleanChildren && child._objects.Count == 0;
                    }
                    if (cleanChildren)
                    {
                        _children = null;
                    }
                }
            }
            public void Clear()
            {
                _children = null;
                _objects.Clear();
            }
            private void SubDivide()
            {
                _children ??=
                [
                    new (_tree,new SMRectangle(_region.Center,_region.LeftTop)),
                    new (_tree,new SMRectangle(_region.Center,_region.RightTop)),
                    new (_tree,new SMRectangle(_region.Center,_region.LeftBottom)),
                    new (_tree,new SMRectangle(_region.Center,_region.RightBottom))
                ];
            }
        }
    }
}