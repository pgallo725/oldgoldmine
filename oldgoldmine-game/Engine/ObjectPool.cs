using System;
using System.Collections.Generic;

namespace OldGoldMine.Engine
{
    /// <summary>
    /// Common interface implemented by all classes that can be used with an ObjectPool.
    /// </summary>
    public abstract class IPoolable : ICloneable
    {
        /// <summary>
        /// Flag indicating if this instance is actively being used or is available in the ObjectPool.
        /// </summary>
        public bool IsActive { get; set; } = true;

        public abstract object Clone();
    }


    class ObjectPool<T> where T : IPoolable, new()
    {
        private readonly List<T> pool;
        private readonly T prototype;

        /// <summary>
        /// The amount of objects of type T stored in this ObjectPool.
        /// </summary>
        public int Size { get { return pool.Count; } }


        /// <summary>
        /// Create an ObjectPool of the requested size and fill it with instances of type T.
        /// </summary>
        /// <param name="prototype">The archetype for all object instances stored in this pool.</param>
        /// <param name="size">The amount of objects of type T stored in this pool.</param>
        public ObjectPool(T prototype, int size)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException("The size must be a positive value (size > 0).");

            this.pool = new List<T>(size);
            this.prototype = prototype;
            this.prototype.IsActive = false;

            // Fill the collection with 'size' copies of the provided object
            for (int i = 0; i < size; i++)
                pool.Add(prototype.Clone() as T);
        }

        /// <summary>
        /// Retrieve an available object instance from the ObjectPool.
        /// </summary>
        /// <returns>A reference to the instance of type T extracted from the pool.</returns>
        public T GetOne()
        {
            for (int i = 0; i < pool.Count; i++)
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

            return newObj;
        }

        /// <summary>
        /// Retrieve multiple object instances from the ObjectPool.
        /// </summary>
        /// <param name="amount">The number of objects of type T that have to be retrieved from the pool.</param>
        /// <returns>An array containing 'n' elements of type T extracted from the pool.</returns>
        public T[] GetMany(int amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException("The amount must be a positive value (amount > 0).");

            T[] pooledObjs = new T[amount];
            int found = 0;

            // Get the requested number of instances from the pool
            for (int i = 0; i < pool.Count && found < amount; i++)
            {
                if (!pool[i].IsActive)
                {
                    pool[i].IsActive = true;
                    pooledObjs[found] = pool[i];
                    found++;
                }
            }

            // Allocate additional items if required
            while (found < amount)
            {
                T newObj = prototype.Clone() as T;
                newObj.IsActive = true;

                pool.Add(newObj);

                pooledObjs[found] = newObj;
                found++;
            }

            return pooledObjs;
        }

        /// <summary>
        /// Remove all inactive objects from the pool, to possibly reduce its size.
        /// </summary>
        public void TrimExcess()
        {
            for (int i = 0; i < pool.Count; i++)
            {
                // Avoid O(n) cost of removal by moving the element at the end of the list
                // and then deleting it without requiring to shift all other items
                if (!pool[i].IsActive)
                {
                    pool[i] = pool[pool.Count];
                    pool.RemoveAt(pool.Count - 1);
                    i -= 1;
                }
            }
        }

    }
}