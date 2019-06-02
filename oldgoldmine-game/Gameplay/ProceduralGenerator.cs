using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.Engine;


namespace oldgoldmine_game.Gameplay
{
    public class ProceduralGenerator
    {
        private readonly float railSegmentLength;
        private readonly float popupDistance;

        float nextRailsGenerationPosition = 0f;


        ObjectPool<GameObject3D> railsPool;


        public ProceduralGenerator(in Queue<GameObject3D> rails, Model rail, float railLength,
            in Queue<Collectible> collectibles, Model collectible, float popupDistance = 100f)
        {
            this.railSegmentLength = railLength;
            this.popupDistance = popupDistance;
            this.railsPool = new ObjectPool<GameObject3D>(new GameObject3D(rail), 10);

            for (int i = 0; i <= (int)(popupDistance / railLength); i++)
            {
                GameObject3D newRails = railsPool.GetOne();
                newRails.EnableLightingModel();
                newRails.Position = new Vector3(0f, 0f, nextRailsGenerationPosition);
                rails.Enqueue(newRails);
                nextRailsGenerationPosition += railSegmentLength;
            }
        }

        public void Update(Vector3 playerPosition, in Queue<GameObject3D> rails, in Queue<Collectible> collectibles)
        {
            if (playerPosition.Z >= nextRailsGenerationPosition - popupDistance)
            {
                GenerateNextRails(rails);
            }
        }

        public void GenerateNextRails(in Queue<GameObject3D> rails)
        {
            if (rails.Count > 0)
                rails.Dequeue().IsActive = false;

            GameObject3D newRails = railsPool.GetOne();
            newRails.EnableLightingModel();
            newRails.Position = new Vector3(0f, 0f, nextRailsGenerationPosition);
            rails.Enqueue(newRails);
            nextRailsGenerationPosition += railSegmentLength;
        }
    }
}