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

        public T Get()
        {
            for (int i = 0; i < size; i++)
            {
                if (!pool[i].IsActive)
                {
                    pool[i].IsActive = true;
                    return pool[i];
                }
            }

            size++;

            T pooledObj = prototype.Clone() as T;
            pooledObj.IsActive = true;

            pool.Add(pooledObj);

            return pooledObj;
        }

        public T[] GetMany(int n)
        {
            // TODO: internally build an array of available objects (inactive or allocated) and return it
            return new T[1]{ null };
        }

        public void Trim()
        {
            // TODO: discard all inactive objects to reduce size
        }

    }
}