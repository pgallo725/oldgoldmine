using System;
using System.Collections.Generic;

namespace oldgoldmine_game.Engine
{

    public abstract class Poolable : ICloneable
    {
        bool active = true;
        public bool IsActive { get { return active; } set { active = value; } }

        public abstract object Clone();
    }


    class ObjectPool<T> where T : Poolable, new()
    {
        int size;
        List<T> pool;
        T prototype;

        public int Size { get { return size; } }


        public ObjectPool(T prototype, int size)
        {
            this.size = size;
            this.pool = new List<T>();
            this.prototype = prototype;
            this.prototype.IsActive = false;

            for (int i = 0; i < size; i++)
            {
                T pooledObj = prototype.Clone() as T;
                pooledObj.IsActive = false;

                pool.Add(pooledObj);
            }
        }

        public T GetOne()
        {
            for (int i = 0; i < size; i++)
            {
                if (!pool[i].IsActive)
                {
                    pool[i].IsActive = true;
                    return pool[i];
                }
            }

            T newObj = prototype.Clone() as T;
            newObj.IsActive = true;

            pool.Add(newObj);
            size++;

            return newObj;
        }

        public T[] GetMany(int n)
        {
            T[] pooledObjs = new T[n];
            int found = 0;

            for (int i = 0; i < size && found < n; i++)
            {
                if (!pool[i].IsActive)
                {
                    pool[i].IsActive = true;
                    pooledObjs[found] = pool[i];
                    found++;
                }
            }

            while (found < n)
            {
                T newObj = prototype.Clone() as T;
                newObj.IsActive = true;

                pool.Add(newObj);
                size++;

                pooledObjs[found] = newObj;
                found++;
            }

            return pooledObjs;
        }

        public void Trim()
        {
            for (int i = 0; i < size; i++)
            {
                if (!pool[i].IsActive)
                    pool.RemoveAt(i);
            }
        }

    }
}