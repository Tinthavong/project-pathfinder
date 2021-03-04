using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    //Adds the (generic) item to the heap at whatever the current item count/index is. then counts up.
    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirstItem()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0; //I need a refresher on what the heap index might be, but with how data structures work I imagine it's pretty simple
        SortDown(items[0]);
        return firstItem;
    }
    
    //In case the priority needs to change
    public void UpdateItem(T item)
    {
        SortUp(item);
        //For pathfinding priority generally only needs to be increased so SortDown() doesn't need to be used here
    }

    public int Count
    {
        get { return currentItemCount; }
    }
    
    //Heap returns the comparison of whether the item at the heap index is equal to the item being checked (Contains).
    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    //For sorting down between child and parent
    public void SortDown(T item)
    {
        while (true)
        {
            //Left child: 2N + 1 
            //Right child: 2N +2
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0; //A temporary swapIndex 

            //Compares if the child node index is less than the current item count and if it's not then the loop returns
            if (childIndexLeft < currentItemCount)
            {
                // stores the left child's index into the swap index until the right node is compared
                swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount)
                {
                    //Compares both child index nodes according to position
                    //Refresher, less than 0 means that the item has a lower priority
                    //0 means that the item is at the same position
                    //Greater than 0 means that the item has a higher priority
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                //Compares the parent with the highest priority child from above
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    //If the parent has a higher priority than both of its children then it is in the correct position
                    return;
                }
            }
            else
            {
                //If parent has no children then it's already in its correct position
                return;
            }
        }
    }

    //Finds the parent node in a heap
    public void SortUp(T item)
    {
        //Formula for finding the parent node: (N - 1) / 2
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            //Sets the parent item in case swapping needs to occur
            //Swapping values occurs between the child and the parent if CompareTo returns 1
            //The nature of the heap node for the project usually means that swapping occurs only if the child has a smaller value than the parent
            T parentItem = items[parentIndex];
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

    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}
