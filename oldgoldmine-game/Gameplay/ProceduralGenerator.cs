using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.Engine;


namespace oldgoldmine_game.Gameplay
{
    public class ProceduralGenerator
    {
        Random rand = new Random();     // TODO: add seed ?


        private readonly float railSegmentLength;
        private readonly float popupDistance;

        float nextRailsGenerationPosition = 0f;


        private readonly Vector3 leftObstacleOffset = new Vector3(1f, 1.25f, 0f);
        private readonly Vector3 rightObstacleOffset = new Vector3(-1f, 1.25f, 0f);
        private readonly Vector3 bottomObstacleOffset = new Vector3(0f, 0.25f, 0f);
        private readonly Vector3 topObstacleOffset = new Vector3(0f, 3f, 0f);

        private readonly Vector3 normalCollectibleOffset = new Vector3(0f, 1.5f, 0f);
        private readonly Vector3 leftCollectibleOffset = new Vector3(2f, 1.7f, 0f);
        private readonly Vector3 rightCollectibleOffset = new Vector3(-2f, 1.7f, 0f);
        private readonly Vector3 topCollectibleOffset = new Vector3(0f, 4f, 0f);

        float nextGenerationPosition = 0f;


        ObjectPool<GameObject3D> railsPool;
        ObjectPool<Collectible> goldPool;
        //ObjectPool<Obstacle> obstaclesPool;



        public ProceduralGenerator(in Queue<GameObject3D> rails, Model rail, float railLength,
            in Queue<Collectible> collectibles, Model collectible, float collectibleScale,
            float popupDistance = 100f)
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

            this.goldPool = new ObjectPool<Collectible>(
                new Collectible(collectible, Vector3.Zero, collectibleScale * Vector3.One, Quaternion.Identity), 10);
        }


        public void Update(Vector3 playerPosition, in Queue<GameObject3D> rails, 
            in Queue<Collectible> collectibles, in Queue<Obstacle> obstacles)
        {
            if (playerPosition.Z >= nextRailsGenerationPosition - popupDistance)
            {
                GenerateNextRails(rails);
            }

            if (playerPosition.Z >= nextGenerationPosition - popupDistance)
            {
                GenerateObjects(collectibles, obstacles);
            }

            // TODO: Garbage collect
        }


        private void GenerateNextRails(in Queue<GameObject3D> rails)
        {
            if (rails.Count > 0)
                rails.Dequeue().IsActive = false;

            GameObject3D newRails = railsPool.GetOne();
            newRails.EnableLightingModel();
            newRails.Position = new Vector3(0f, 0f, nextRailsGenerationPosition);
            rails.Enqueue(newRails);
            nextRailsGenerationPosition += railSegmentLength;
        }

        private void GenerateObjects(in Queue<Collectible> collectibles, in Queue<Obstacle> obstacles)
        {
            Collectible newGold = goldPool.GetOne();
            Vector3 offset = Vector3.Zero;
            switch (rand.Next(4))
            {
                case 0: offset = normalCollectibleOffset; break;
                case 1: offset = leftCollectibleOffset; break;
                case 2: offset = rightCollectibleOffset; break;
                case 3: offset = topCollectibleOffset; break;
            }
            newGold.Position = new Vector3(0f, 0f, nextGenerationPosition) + offset;
            collectibles.Enqueue(newGold);
            nextGenerationPosition += 10f;
        }
    }
}