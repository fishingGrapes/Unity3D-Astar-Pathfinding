
namespace Pathfinding
{
    public class Heap<T> where T : IHeapItem<T>
    {
        private T[] mItems;

        private int nCurrentItemCount;
        public int Count { get { return nCurrentItemCount; } }


        #region public Methods
        public Heap(int maximumHeapSize)
        {
            mItems = new T[maximumHeapSize];
        }

        public void Add(T Item)
        {
            //Add the Item to the End of the Heap
            //And Start Sorting up
            Item.HeapIndex = nCurrentItemCount;
            mItems[nCurrentItemCount] = Item;

            this.SortUp(Item);
            nCurrentItemCount += 1;
        }

        public T RemoveFirst()
        {
            //mItems[0] has the item with highest Priority (i.e) Lowest f_cost
            T mFirstItem = mItems[0];

            //mItems[nCurrentItemCount -1] is the Node at the end of the Heap and Insert it at the top
            //and we start Sorting Down
            nCurrentItemCount -= 1;
            mItems[0] = mItems[nCurrentItemCount];
            mItems[0].HeapIndex = 0;
            this.SortDown(mItems[0]);


            return mFirstItem;
        }

        public bool Contains(T Item)
        {
            //If the Item with the HeapIndex Exists in the Heap
            return Equals(mItems[Item.HeapIndex], Item);
        }

        public void UpdateItem(T Item)
        {
            //Incase of PathFinding We're ogoing to Update the Heap only if we have a Lower f_cost
            //Or Higher Priority, thus we only call SortUp Here
            //For Other Cases a SortDown May be Necessary
            this.SortUp(Item);
        }

        #endregion

        #region Private Methods

        private void SortUp(T mCurrentItem)
        {
            //Predefined Formula : parentIndex = (currentItems's HeapIndex -1) / 2;
            int nParentIndex = (mCurrentItem.HeapIndex - 1) / 2;

            //Continues Until the Insertes Item cannot be sorted up anymore with the parent
            while (true)
            {
                //get the Parent of the current Item
                T mParentItem = mItems[nParentIndex];

                //The result is positive if the item  has lower f_cost than the parent item
                //zero if equal f_costs
                //Negative if higher g_cost
                if (mCurrentItem.CompareTo(mParentItem) > 0)
                {
                    this.Swap(mCurrentItem, mParentItem);
                }
                else
                {
                    break;
                }
                nParentIndex = (mCurrentItem.HeapIndex - 1) / 2;
            }
        }

        private void SortDown(T mCurrentItem)
        {
            int nChildIndex_Left, nChildIndex_Right;
            int nSwapIndex;
            while (true)
            {
                //Predefined Forumlas:
                //Left Child Index = CurrentItem's HeapIndex * 2 + 1 ;
                //Right Child Index = CurrentItem's HeapIndex * 2 + 2 ;
                nChildIndex_Left = mCurrentItem.HeapIndex * 2 + 1;
                nChildIndex_Right = mCurrentItem.HeapIndex * 2 + 2;
                nSwapIndex = 0;

                //If Left Child Exists,
                if (nChildIndex_Left < nCurrentItemCount)
                {
                    //Initially set the swapIndex to the left Child's HeapIndex
                    nSwapIndex = nChildIndex_Left;

                    //If Right Child Exists,
                    if (nChildIndex_Right < nCurrentItemCount)
                    {
                        //If, Priority of Left Child is LOWER than the priority of Right Child
                        if (mItems[nChildIndex_Left].CompareTo(mItems[nChildIndex_Right]) < 0)
                        {
                            nSwapIndex = nChildIndex_Right;
                        }
                    }

                    //If Parent, mCurrentItem, has Lower priority than one of it's Children
                    if (mCurrentItem.CompareTo(mItems[nSwapIndex]) < 0)
                    {
                        this.Swap(mCurrentItem, mItems[nSwapIndex]);
                    }
                    else
                    {
                        //If Parent, mCurrentItem, has Higher Priority than its children
                        return;
                    }
                }
                else
                {
                    //If Parent, mCurrentItem,  Doesn't contain any Children
                    return;
                }

            }
        }

        private void Swap(T itemA, T itemB)
        {
            mItems[itemA.HeapIndex] = itemB;
            mItems[itemB.HeapIndex] = itemA;

            int nItemAIndex = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = nItemAIndex;
        }

        #endregion
    }
}
