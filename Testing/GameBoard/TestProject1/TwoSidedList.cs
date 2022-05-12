using System;
using System.Collections;
using System.Collections.Generic;

namespace ScenesFolders.MainGame
{
    public class TwoSidedList<T>
    {
        private List<T> positiveList;
        private List<T> negativeList;

        public TwoSidedList()
        {
            positiveList = new List<T>();
            negativeList = new List<T>();
        }
        
        public int Count => positiveList.Count + negativeList.Count;

        public IEnumerable<T> Values
        {
            get
            {
                foreach (var element in negativeList)
                    yield return element;
                foreach (var element in positiveList)
                    yield return element;
            }
        }
        
        public void AddRight(T element) => positiveList.Add(element);
        public void AddLeft(T element) => negativeList.Add(element);
        
        public T this[int index]
        {
            get
            {
                return index >= 0 ? positiveList[index] : negativeList[-index - 1];
            }
            set
            {
                if (index >= 0)
                    positiveList[index] = value;
                else
                    negativeList[-index - 1] = value;
            }
        }
    }
}