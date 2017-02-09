using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace CMaNGOSNET.Common.Collection
{
    public class ObjectPool<T> : IObjectPool where T : class, IDisposable
    {
        public bool CreateObjectOnEmpty
        {
            get;
            private set;
        }

        public Func<T> CreateObjectHandle;
        private Queue<T> pool = null;

        public ObjectPool(bool createObjectOnEmpty = false)
        {
            pool = new Queue<T>();
            CreateObjectOnEmpty = createObjectOnEmpty;
        }

        public void AddObject(T obj)
        {
            if (obj != null)
            {
                lock (pool)
                {
                    pool.Enqueue(obj);
                }
            }

        }

        public T GetObject()
        {
            lock (pool)
            {
                if (pool.Count == 0)
                {
                    if (CreateObjectOnEmpty && CreateObjectHandle != null)
                    {
                        T newObj = CreateObjectHandle();
                        pool.Enqueue(newObj);
                    }
                    else
                    {
                        return null;
                    }
                }

                return pool.Dequeue();
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // so that Dispose(false) isn't called later
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose all owned managed objects
            }

            // Release unmanaged resources
        }
    }
}
