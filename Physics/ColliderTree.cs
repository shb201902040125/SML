using SML.Common;
using System.Collections.Generic;

namespace SML.Physics
{
    public class ColliderTree
    {
        private SparseSet<object, Collider> _colliders;
        private ColliderTreeNode _root;
        public ColliderTree(SMRectangle region, int maxCapacity)
        {
            _colliders = new(maxCapacity);
            _root = new(this, region);
        }
        public ColliderTree(SMRectangle region, SparseSet<object, Collider> colliders)
        {
            _colliders = colliders;
            _root = new(this, region);
            foreach (object obj in _colliders.Keys)
            {
                SMRectangle envelope = _colliders[obj].GetEnvelope();
                _root.Insert(ref _colliders[obj], ref envelope);
            }
        }
        public bool Insert(Collider collider)
        {
            if (!_colliders.Add(collider.BindTarget, collider))
            {
                return false;
            }
            SMRectangle envelope = collider.GetEnvelope();
            if (!_root.Insert(ref collider, ref envelope))
            {
                _colliders.Remove(collider.BindTarget);
                return false;
            }
            return true;
        }
        public bool Remove(Collider collider)
        {
            if (!_colliders.Remove(collider))
            {
                return false;
            }
            SMRectangle envelope = collider.GetEnvelope();
            return _root.Remove(ref collider, ref envelope);
        }
        public List<Collider> GetCollideWith(Collider collider, bool useSAT = true)
        {
            HashSet<object> list = [];
            SMRectangle envelope = collider.GetEnvelope();
            _root.GetCollideWith(ref collider, ref envelope, list, useSAT);
            List<Collider> result = [];
            foreach (object obj in list)
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
            private ColliderTree _tree;
            private SMRectangle _region;
            private HashSet<object> _objects;
            private ColliderTreeNode[] _children;
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
                foreach (ColliderTreeNode child in _children)
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
                    foreach (ColliderTreeNode child in _children)
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
                    foreach (ColliderTreeNode child in _children)
                    {
                        if (child._region.Surround(envelope))
                        {
                            child.GetCollideWith(ref collider, ref envelope, list, useSAT);
                            return;
                        }
                    }
                }
                foreach (object obj in _objects)
                {
                    Collider c = _tree._colliders[obj];
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
                    foreach (ColliderTreeNode child in _children)
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