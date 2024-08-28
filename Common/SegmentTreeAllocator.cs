using System;

namespace SML.Common
{
    public class SegmentTreeAllocator
    {
        private int[] tree;
        private int capacity;
        public enum AllocType
        {
            FirstFit,
            BestFit,
            WorstFit
        }
        public SegmentTreeAllocator(int capacity)
        {
            this.capacity = capacity;
            int size = 1;
            while (size < capacity)
            {
                size *= 2;
            }

            tree = new int[2 * size];
            Array.Fill(tree, capacity);
        }
        public int Rent(int size, AllocType allocType)
        {
            return allocType switch
            {
                AllocType.FirstFit => RentFirstFit(0, 0, capacity, size),
                AllocType.BestFit => RentBestFit(0, 0, capacity, size),
                AllocType.WorstFit => RentWorstFit(0, 0, capacity, size),
                _ => -1,
            };
        }
        public void Return(int pos, int size)
        {
            Update(0, 0, capacity, pos, size);
        }
        private int RentFirstFit(int node, int start, int end, int size)
        {
            if (tree[node] < size)
            {
                return -1;
            }

            if (start + 1 == end)
            {
                tree[node] -= size;
                return start;
            }
            int mid = (start + end) / 2;
            int leftResult = RentFirstFit((2 * node) + 1, start, mid, size);
            if (leftResult != -1)
            {
                tree[node] = Math.Max(tree[(2 * node) + 1], tree[(2 * node) + 2]);
                return leftResult;
            }
            int rightResult = RentFirstFit((2 * node) + 2, mid, end, size);
            tree[node] = Math.Max(tree[(2 * node) + 1], tree[(2 * node) + 2]);
            return rightResult;
        }
        private int RentBestFit(int node, int start, int end, int size)
        {
            if (tree[node] < size)
            {
                return -1;
            }

            if (start + 1 == end)
            {
                tree[node] -= size;
                return start;
            }
            int mid = (start + end) / 2;
            int leftResult = RentBestFit((2 * node) + 1, start, mid, size);
            int rightResult = RentBestFit((2 * node) + 2, mid, end, size);
            tree[node] = Math.Max(tree[(2 * node) + 1], tree[(2 * node) + 2]);
            return leftResult != -1 && rightResult != -1
                ? tree[(2 * node) + 1] < tree[(2 * node) + 2] ? leftResult : rightResult
                : leftResult != -1 ? leftResult : rightResult;
        }
        private int RentWorstFit(int node, int start, int end, int size)
        {
            if (tree[node] < size)
            {
                return -1;
            }

            if (start + 1 == end)
            {
                tree[node] -= size;
                return start;
            }
            int mid = (start + end) / 2;
            int leftResult = RentWorstFit((2 * node) + 1, start, mid, size);
            int rightResult = RentWorstFit((2 * node) + 2, mid, end, size);
            tree[node] = Math.Max(tree[(2 * node) + 1], tree[(2 * node) + 2]);
            return leftResult != -1 && rightResult != -1
                ? tree[(2 * node) + 1] > tree[(2 * node) + 2] ? leftResult : rightResult
                : leftResult != -1 ? leftResult : rightResult;
        }
        private void Update(int node, int start, int end, int pos, int size)
        {
            if (start + 1 == end)
            {
                tree[node] += size;
                return;
            }
            int mid = (start + end) / 2;
            if (pos < mid)
            {
                Update((2 * node) + 1, start, mid, pos, size);
            }
            else
            {
                Update((2 * node) + 2, mid, end, pos, size);
            }
            tree[node] = Math.Max(tree[(2 * node) + 1], tree[(2 * node) + 2]);
        }
    }
}