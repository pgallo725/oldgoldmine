using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.Engine;


namespace oldgoldmine_game.Gameplay
{
    public class ProceduralGenerator
    {

        private enum PatternElement
        {
            GoldLeft,
            GoldRight,
            GoldCenter,
            GoldBothSides,
            GoldAbove,
            ObstacleLeft,
            ObstacleRight,
            ObstacleBelow,
            ObstacleAbove,
            Empty
        };


        private class Pattern : IEnumerable
        {
            public List<PatternElement> patternObjects = new List<PatternElement>();

            public PatternElement this[int key]
            {
                get { return patternObjects[key]; }
                set { patternObjects[key] = value; }
            }


            public void Add(PatternElement item)
            {
                patternObjects.Add(item);
            }

            public IEnumerator GetEnumerator()
            {
                return new PatternEnumerator(patternObjects);
            }
        }


        private class PatternEnumerator : IEnumerator
        {
            private List<PatternElement> _objects;
            int position = -1;

            public PatternEnumerator(List<PatternElement> list)
            {
                _objects = list;
            }

            object IEnumerator.Current { get { return Current; } }

            public PatternElement Current
            {
                get
                {
                    try
                    {
                        return _objects[position];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            public bool MoveNext()
            {
                position++;
                return (position < _objects.Count);
            }

            public void Reset()
            {
                position = -1;
            }
        }


        private readonly Pattern[] easyPatterns = 
        {
            new Pattern {
                PatternElement.Empty,
                PatternElement.Empty
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.Empty,
                PatternElement.Empty
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.ObstacleBelow,
                PatternElement.Empty,
                PatternElement.GoldCenter,
                PatternElement.GoldCenter,
                PatternElement.Empty
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.ObstacleLeft,
                PatternElement.GoldRight,
                PatternElement.Empty,
                PatternElement.GoldLeft,
                PatternElement.GoldLeft
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.ObstacleAbove,
                PatternElement.GoldLeft,
                PatternElement.GoldLeft,
                PatternElement.Empty,
                PatternElement.ObstacleLeft,
                PatternElement.Empty
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.GoldCenter,
                PatternElement.GoldAbove,
                PatternElement.Empty,
                PatternElement.ObstacleLeft,
                PatternElement.ObstacleLeft,
                PatternElement.Empty
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.ObstacleBelow,
                PatternElement.Empty,
                PatternElement.Empty,
                PatternElement.ObstacleAbove,
                PatternElement.Empty
            },

            new Pattern {
                PatternElement.ObstacleBelow,
                PatternElement.Empty,
                PatternElement.Empty,
                PatternElement.GoldRight,
                PatternElement.ObstacleLeft,
                PatternElement.ObstacleLeft,
                PatternElement.Empty
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.ObstacleRight,
                PatternElement.Empty,
                PatternElement.Empty,
                PatternElement.ObstacleLeft,
                PatternElement.GoldCenter,
                PatternElement.GoldCenter
            },

            new Pattern {

                PatternElement.Empty,
                PatternElement.Empty,
                PatternElement.ObstacleRight,
                PatternElement.Empty,
                PatternElement.Empty
            },

            new Pattern {
                PatternElement.ObstacleLeft,
                PatternElement.Empty,
                PatternElement.ObstacleRight,
                PatternElement.GoldCenter,
                PatternElement.GoldCenter,
                PatternElement.GoldCenter,
                PatternElement.Empty
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.GoldAbove,
                PatternElement.GoldCenter,
                PatternElement.Empty,
                PatternElement.ObstacleAbove,
                PatternElement.Empty
            }
        };


        private readonly Pattern[] mediumPatterns =
        {
            new Pattern { PatternElement.Empty },

            new Pattern {
                PatternElement.Empty,
                PatternElement.Empty
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.ObstacleLeft,
                PatternElement.GoldLeft,
                PatternElement.GoldLeft,
                PatternElement.ObstacleLeft
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.ObstacleRight,
                PatternElement.GoldRight,
                PatternElement.GoldRight,
                PatternElement.ObstacleRight
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.ObstacleBelow,
                PatternElement.Empty,
                PatternElement.GoldBothSides,
                PatternElement.ObstacleAbove,
                PatternElement.Empty
            },

            new Pattern {
                PatternElement.GoldCenter,
                PatternElement.GoldCenter,
                PatternElement.ObstacleAbove,
                PatternElement.Empty,
                PatternElement.GoldRight,
                PatternElement.ObstacleRight
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.GoldLeft,
                PatternElement.ObstacleLeft,
                PatternElement.Empty,
                PatternElement.ObstacleAbove,
                PatternElement.GoldRight,
                PatternElement.GoldRight
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.ObstacleBelow,
                PatternElement.GoldLeft,
                PatternElement.GoldLeft,
                PatternElement.ObstacleBelow
            },

            new Pattern {
                PatternElement.ObstacleBelow,
                PatternElement.Empty,
                PatternElement.ObstacleAbove,
                PatternElement.Empty,
                PatternElement.GoldLeft,
                PatternElement.GoldRight
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.ObstacleAbove,
                PatternElement.Empty,
                PatternElement.GoldRight,
                PatternElement.GoldLeft,
                PatternElement.ObstacleLeft,
                PatternElement.ObstacleRight,
                PatternElement.Empty
            },

            new Pattern {
                PatternElement.GoldLeft,
                PatternElement.GoldLeft,
                PatternElement.ObstacleLeft,
                PatternElement.Empty,
                PatternElement.ObstacleRight,
                PatternElement.ObstacleLeft,
                PatternElement.Empty
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.GoldCenter,
                PatternElement.ObstacleBelow,
                PatternElement.GoldAbove,
                PatternElement.ObstacleAbove,
                PatternElement.Empty
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.ObstacleBelow,
                PatternElement.GoldBothSides,
                PatternElement.GoldBothSides,
                PatternElement.ObstacleBelow
            }
        };


        private readonly Pattern[] hardPatterns =
        {
            new Pattern { PatternElement.Empty },

            new Pattern {
                PatternElement.Empty,
                PatternElement.ObstacleLeft,
                PatternElement.ObstacleRight,
                PatternElement.Empty,
                PatternElement.ObstacleLeft, 
                PatternElement.GoldRight,
                PatternElement.GoldRight
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.ObstacleRight,
                PatternElement.GoldBothSides,
                PatternElement.GoldBothSides,
                PatternElement.ObstacleRight,
                PatternElement.ObstacleLeft,
                PatternElement.Empty
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.ObstacleBelow,
                PatternElement.ObstacleAbove,
                PatternElement.Empty,
                PatternElement.GoldRight,
                PatternElement.GoldRight
            },

            new Pattern {
                PatternElement.ObstacleBelow,
                PatternElement.Empty,
                PatternElement.ObstacleBelow,
                PatternElement.GoldBothSides,
                PatternElement.GoldBothSides,
                PatternElement.ObstacleRight
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.ObstacleAbove,
                PatternElement.GoldCenter,
                PatternElement.ObstacleAbove,
                PatternElement.Empty
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.ObstacleRight,
                PatternElement.ObstacleLeft,
                PatternElement.GoldRight,
                PatternElement.GoldLeft,
                PatternElement.ObstacleLeft
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.ObstacleAbove,
                PatternElement.Empty,
                PatternElement.ObstacleBelow,
                PatternElement.GoldLeft,
                PatternElement.GoldLeft
            },

            new Pattern {
                PatternElement.ObstacleBelow,
                PatternElement.Empty,
                PatternElement.ObstacleRight,
                PatternElement.ObstacleRight,
                PatternElement.GoldRight,
                PatternElement.GoldRight
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.Empty,
                PatternElement.ObstacleRight,
                PatternElement.ObstacleLeft,
                PatternElement.GoldBothSides,
                PatternElement.GoldBothSides,
                PatternElement.Empty
            },

            new Pattern {
                PatternElement.Empty,
                PatternElement.GoldAbove,
                PatternElement.GoldCenter,
                PatternElement.GoldCenter,
                PatternElement.ObstacleBelow,
                PatternElement.Empty,
                PatternElement.ObstacleAbove
            }
        };


        Random rand = new Random();


        private readonly float caveSegmentLength;
        private readonly float popupDistance;

        private const float garbageCollectionDistance = 10f;

        float nextCavePosition = 0f;
        float nextObjectPosition = 80f;

        private Pattern activePattern = null;
        private int curPatternElementIndex = 0;


        private readonly Vector3 normalCollectibleOffset = new Vector3(0f, 1.5f, 0f);
        private readonly Vector3 leftCollectibleOffset = new Vector3(2f, 1.7f, 0f);
        private readonly Vector3 rightCollectibleOffset = new Vector3(-2f, 1.7f, 0f);
        private readonly Vector3 topCollectibleOffset = new Vector3(0f, 4f, 0f);
        

        Queue<GameObject3D> caves;
        Queue<Collectible> collectibles;
        Queue<Obstacle> obstacles;


        ObjectPool<GameObject3D> cavePool;
        ObjectPool<Collectible> goldPool;
        ObjectPool<Obstacle> obstaclesLowPool;
        ObjectPool<Obstacle> obstaclesLeftPool;
        ObjectPool<Obstacle> obstaclesRightPool;
        ObjectPool<Obstacle> obstaclesHighPool;


        private int difficulty = 1;
        public int Difficulty {
            get { return difficulty; }
            set { difficulty = MathHelper.Clamp(value, 0, 3); }
        }

        float[] marginMultiplier    = { 1.4f, 1.2f, 1.1f };     // Extra room for maneuver: Easy 40%, Medium 20%, Hard 10%
        float[] easyProbability     = { 0.6f, 0.3f, 0.1f };     // Probability of easy patterns: Easy 60%, Medium 30%, Hard 10%
        float[] mediumProbability   = { 0.3f, 0.4f, 0.3f };     // Probability of medium patterns: Easy 30%, Medium 40%, Hard 30%
        float[] hardProbability     = { 0.1f, 0.3f, 0.6f };     // Probability of hard patterns: Easy 10%, Medium 30%, Hard 60%



        public ProceduralGenerator(GameObject3D cave, float caveLength, Collectible gold,
            Obstacle lower, Obstacle left, Obstacle right, Obstacle upper, float popupDistance = 100f)
        {
            this.caves = new Queue<GameObject3D>();
            this.collectibles = new Queue<Collectible>();
            this.obstacles = new Queue<Obstacle>();

            this.caveSegmentLength = caveLength;
            this.nextCavePosition = -caveLength;        // First cave segment is spawned behind the player to avoid starting outside the cave
            this.popupDistance = popupDistance;

            this.cavePool = new ObjectPool<GameObject3D>(cave, 4);

            // Generate the first few cave segments

            for (int i = 0; i <= (int)((popupDistance / caveSegmentLength)  + 1.5f); i++)
            {
                GenerateCave(nextCavePosition);
                nextCavePosition += caveSegmentLength;
            }

            // Initializing GameObjects that will be spawned by the procedural generator (Collectibles and Obstacles)

            this.goldPool = new ObjectPool<Collectible>(gold, 10);
            this.obstaclesLowPool = new ObjectPool<Obstacle>(lower, 8);
            this.obstaclesLeftPool = new ObjectPool<Obstacle>(left, 8);
            this.obstaclesRightPool = new ObjectPool<Obstacle>(right, 8);
            this.obstaclesHighPool = new ObjectPool<Obstacle>(upper, 8);
        }


        public void InitializeSeed(long seed)
        {
            if (seed != 0)
                rand = new Random((int)seed);
        }


        // Reset the level to its original state, removing all generated items
        public void Reset()
        {
            while (caves.Count > 0)
                caves.Dequeue().IsActive = false;

            while (collectibles.Count > 0)
                collectibles.Dequeue().IsActive = false;

            while (obstacles.Count > 0)
                obstacles.Dequeue().IsActive = false;

            nextCavePosition = -caveSegmentLength;
            nextObjectPosition = 80f;

            for (int i = 0; i <= (int)((popupDistance / caveSegmentLength) + 1.5f); i++)
            {
                GenerateCave(nextCavePosition);
                nextCavePosition += caveSegmentLength;
            }
        }


        public void Update(Vector3 playerPosition)
        {
            if (playerPosition.Z >= nextCavePosition - popupDistance)
            {
                GenerateCave(nextCavePosition);
                nextCavePosition += caveSegmentLength;
            }
                
            if (playerPosition.Z >= nextObjectPosition - popupDistance)
            {
                GenerateObjects();
            }


            // Garbage collect the items behind the player, update all active items

            foreach (GameObject3D cave in caves.ToArray())
            {
                if (playerPosition.Z >= cave.Position.Z + caveSegmentLength + garbageCollectionDistance)
                {
                    caves.Dequeue().IsActive = false;     // Remove previous cave segments from the active queue
                }
            }

            foreach (Collectible gold in collectibles.ToArray())
            {
                if (playerPosition.Z >= gold.Position.Z + garbageCollectionDistance)
                {
                    gold.IsActive = false;
                    collectibles.Dequeue();     // Remove element from the queue (knowing that collectibles are sorted in order of encounter)
                }
                else gold.Update();
            }

            foreach (Obstacle obstacle in obstacles.ToArray())
            {
                if (playerPosition.Z >= obstacle.Position.Z + garbageCollectionDistance)
                {
                    obstacle.IsActive = false;
                    obstacles.Dequeue();        // Remove element from the queue (knowing that obstacles are sorted in order of encounter)
                }
                else obstacle.Update();
            }
        }


        private void GenerateCave(float position)
        {
            GameObject3D newCave = cavePool.GetOne();
            newCave.EnableDefaultLighting();
            newCave.Position = new Vector3(0f, 0f, position);
            caves.Enqueue(newCave);
        }


        private void GenerateObjects()
        {
            float speed = OldGoldMineGame.Application.Speed;
            float distance = MathHelper.Clamp(0.5f * speed, 12f, 1000f);

            if (activePattern == null)
            {
                int val = rand.Next(100);
                int index;

                // Choose the type of pattern to generate, 
                // according to their probabilities (determined by difficulty)

                if (val <= easyProbability[difficulty] * 100)   // easy pattern
                {
                    index = rand.Next(easyPatterns.Length);
                    activePattern = easyPatterns[index];
                }
                else if (val <= (easyProbability[difficulty] + mediumProbability[difficulty]) * 100)    // medium pattern
                {
                    index = rand.Next(mediumPatterns.Length);
                    activePattern = mediumPatterns[index];
                }
                else    // hard pattern
                {
                    index = rand.Next(hardPatterns.Length);
                    activePattern = hardPatterns[index];
                }

                curPatternElementIndex = 0;
            }


            // Spawn the next element in the currently active pattern
            PatternElement p = activePattern[curPatternElementIndex++];

            if (p == PatternElement.Empty)
            {
                // empty space
            }
            else if (p <= PatternElement.GoldAbove)     // collectible
            {
                Vector3 offset = Vector3.Zero;
                Collectible newGold = goldPool.GetOne();
                switch (p)
                {
                    case PatternElement.GoldCenter: offset = normalCollectibleOffset; break;
                    case PatternElement.GoldLeft: offset = leftCollectibleOffset; break;
                    case PatternElement.GoldRight: offset = rightCollectibleOffset; break;
                    case PatternElement.GoldAbove: offset = topCollectibleOffset; break;
                    case PatternElement.GoldBothSides:
                        Collectible extraGold = goldPool.GetOne();
                        extraGold.Position = new Vector3(0f, 0f, nextObjectPosition) + leftCollectibleOffset;
                        collectibles.Enqueue(extraGold);
                        offset = rightCollectibleOffset;
                        break;
                }
                newGold.Position = new Vector3(0f, 0f, nextObjectPosition) + offset;
                collectibles.Enqueue(newGold);
            }
            else if (p <= PatternElement.ObstacleAbove)  // obstacle
            {
                Obstacle newObstacle = null;
                switch (p)
                {
                    case PatternElement.ObstacleLeft:
                        newObstacle = obstaclesLeftPool.GetOne();
                        break;
                    case PatternElement.ObstacleRight:
                        newObstacle = obstaclesRightPool.GetOne();
                        break;
                    case PatternElement.ObstacleBelow:
                        newObstacle = obstaclesLowPool.GetOne();
                        break;
                    case PatternElement.ObstacleAbove:
                        newObstacle = obstaclesHighPool.GetOne();
                        break;
                }
                newObstacle.Position = new Vector3(newObstacle.Position.X, newObstacle.Position.Y, nextObjectPosition);
                obstacles.Enqueue(newObstacle);
            }

            nextObjectPosition += distance * marginMultiplier[difficulty];


            // If reached the end of the pattern, reset the variable so at the next 
            // generation a new one will be randomly picked

            if (curPatternElementIndex == activePattern.patternObjects.Count)
            {
                activePattern = null;
                curPatternElementIndex = 0;
                nextObjectPosition += distance;
            }
        }



        // Draw the level objects according to the player's POV
        public void Draw (GameCamera camera)
        {
            foreach (GameObject3D cave in caves)
                cave.Draw(camera);

            foreach (Collectible gold in collectibles)
                gold.Draw(camera);

            foreach (Obstacle obstacle in obstacles)
                obstacle.Draw(camera);
        }
    }
}