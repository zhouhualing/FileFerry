using System;
using System.Collections;

namespace WD.Library.Utils
{
   
    public class ModifyIterator : IEnumerable
    {
        internal class ModifyEnumerator : IEnumerator
        {
            protected ArrayList objs
            {
                get; set;
            }
            
            protected int index;
            
            internal ModifyEnumerator(IEnumerator copy)
            {
                ModifyEnumerator chainedEnumerator = copy as ModifyEnumerator;

                if (chainedEnumerator != null)
                {
                    objs = chainedEnumerator.objs;
                }
                else
                {
                    objs = new ArrayList();

                    while (copy.MoveNext())
                    {
                        objs.Add(copy.Current);
                    }

                    IDisposable disposable = copy as IDisposable;

                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }

                index = -1;
            }

            public void Reset()
            {
                index = -1;
            }

            public bool MoveNext()
            {
                index++;

                if (index == objs.Count)
                    return false;

                return true;
            }

            ///---------------------------------------------------------------------------
            /// <summary>
            /// Return the object at the current index.
            /// </summary>
            ///---------------------------------------------------------------------------
            public object Current
            { get { return objs[index]; } }
        }
        
        protected IEnumerable enumerable;
        
        public ModifyIterator(IEnumerable enumerable)
        {
            this.enumerable = enumerable;
        }
        
        public IEnumerator GetEnumerator()
        {
            return new ModifyEnumerator(enumerable.GetEnumerator());
        }
    }
}
