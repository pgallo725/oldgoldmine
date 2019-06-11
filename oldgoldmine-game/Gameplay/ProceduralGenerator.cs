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


        private readonly Vector3 leftObstacleOffset = new Vector3(1.2f, 1.3f, 0f);
        private readonly Vector3 rightObstacleOffset = new Vector3(-1.2f, 1.3f, 0f);
        private readonly Vector3 bottomObstacleOffset = new Vector3(0f, 0.4f, 0f);
        private readonly Vector3 topObstacleOffset = new Vector3(0f, 3.5f, 0f);

        private readonly Vector3 normalCollectibleOffset = new Vector3(0f, 1.5f, 0f);
        private readonly Vector3 leftCollectibleOffset = new Vector3(2f, 1.7f, 0f);
        private readonly Vector3 rightCollectibleOffset = new Vector3(-2f, 1.7f, 0f);
        private readonly Vector3 topCollectibleOffset = new Vector3(0f, 4f, 0f);

        float nextGenerationPosition = 20f;


        private const float garbageCollectionDistance = 10f;

        ObjectPool<GameObject3D> railsPool;
        ObjectPool<Collectible> goldPool;
        ObjectPool<Obstacle> obstaclesPool;



        public ProceduralGenerator(in Queue<GameObject3D> rails, Model rail, float railLength,
            in Queue<Collectible> collectibles, Model collectible, float collectibleScale,
            in Queue<Obstacle> obstacles, Model obstacle, Vector3 hitboxSize,
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

            this.obstaclesPool = new ObjectPool<Obstacle>(
                new Obstacle(new GameObject3D(obstacle), hitboxSize), 10);
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

            // Garbage collect of items left behind the player
            foreach (Collectible gold in collectibles)
            {
                if (playerPosition.Z >= gold.Position.Z + garbageCollectionDistance)
                {
                    Console.WriteLine("Collected disposable item");
                    gold.IsActive = false;
                }
            }

            foreach (Obstacle obstacle in obstacles)
            {
                if (playerPosition.Z >= obstacle.Position.Z + garbageCollectionDistance)
                {
                    Console.WriteLine("Collected disposable item");
                    obstacle.IsActive = false;
                }
            }
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
            int val = rand.Next(5);
            if (val > 1)
            {
                if (val == 2 || val == 3)   // collectible
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
                }
                else if (val == 4)  // obstacle
                {
                    Obstacle newObstacle = obstaclesPool.GetOne();
                    Vector3 offset = Vector3.Zero;
                    switch (rand.Next(4))
                    {
                        case 0: offset = leftObstacleOffset; break;
                        case 1: offset = rightObstacleOffset; break;
                        case 2: offset = bottomObstacleOffset; break;
                        case 3: offset = topObstacleOffset; break;
                    }
                    newObstacle.Position = new Vector3(0f, 0f, nextGenerationPosition) + offset;
                    obstacles.Enqueue(newObstacle);
                }
                
            }
            
            nextGenerationPosition += 10f;
        }
    }
}