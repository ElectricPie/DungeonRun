using System;

namespace ElectricPie.Collections
{
    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex { get; set; }
    }
    
    public class Heap<T> where T : IHeapItem<T>
    {
        private readonly T[] m_items;

        public int Count { get; private set; } = 0;

        public Heap(int maxHeapSize)
        {
            m_items = new T[maxHeapSize];
        }

        public void Add(T item)
        {
            item.HeapIndex = Count;
            m_items[Count] = item;
            SortUp(item);
            Count++;
        }

        public T RemoveFirst()
        {
            T firstItem = m_items[0];
            Count--;

            m_items[0] = m_items[Count];
            m_items[0].HeapIndex = 0;

            SortDown(m_items[0]);
            
            return firstItem;
        }

        public bool Contains(T item)
        {
            return Equals(m_items[item.HeapIndex], item);
        }

        public void UpdateItem(T item)
        {
            SortUp(item);
        }

        private void Swap(T a, T b)
        {
            m_items[a.HeapIndex] = b;
            m_items[b.HeapIndex] = a;

            (a.HeapIndex, b.HeapIndex) = (b.HeapIndex, a.HeapIndex);
        }
        
        private void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;
            while (true)
            {
                T parentItem = m_items[parentIndex];
                if (item.CompareTo(parentItem) > 0)
                {
                    Swap(item, parentItem);
                }
                else
                {
                    break;
                }

                parentIndex = (item.HeapIndex - 1) / 2;
            }   
        }

        private void SortDown(T item)
        {
            while (true)
            {
                int childIndexLeft = item.HeapIndex * 2 + 1;
                int childIndexRight = item.HeapIndex * 2 + 2;

                if (childIndexLeft < Count)
                {
                    int swapIndex = childIndexLeft;

                    if (childIndexRight < Count)
                    {
                        if (m_items[childIndexLeft].CompareTo(m_items[childIndexRight]) < 0)
                        {
                            swapIndex = childIndexRight;
                        }
                    }

                    if (item.CompareTo(m_items[swapIndex]) < 0)
                    {
                        Swap(item, m_items[swapIndex]);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }
    }
}