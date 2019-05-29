using System.Collections.Generic;

namespace oldgoldmine_game.Engine
{
    class ObjectPool<T> where T : Poolable, new()
    {

        int size;
        List<T> pool;
        T prototype;

        public ObjectPool(T prototype, int size)
        {
            this.size = size;
            this.pool = new List<T>();
            this.prototype = prototype;

            for (int i = 0; i < size; i++)
            {
                pool.Add(prototype);
            }
        }

        T Get()
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
            pool.Add(prototype);
            pool[size - 1].IsActive = true;
            return pool[size - 1];
        }
    }
}