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
                PatternElement.Empty,
                PatternElement.ObstacleBelow,
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


        private readonly float railSegmentLength;
        private readonly float popupDistance;

        internal float nextRailsGenerationPosition = 0f;


        private readonly Vector3 leftObstacleOffset = new Vector3(1.2f, 1.3f, 0f);
        private readonly Vector3 rightObstacleOffset = new Vector3(-1.2f, 1.3f, 0f);
        private readonly Vector3 bottomObstacleOffset = new Vector3(0f, 0.4f, 0f);
        private readonly Vector3 topObstacleOffset = new Vector3(0f, 3.5f, 0f);

        private readonly Vector3 normalCollectibleOffset = new Vector3(0f, 1.5f, 0f);
        private readonly Vector3 leftCollectibleOffset = new Vector3(2f, 1.7f, 0f);
        private readonly Vector3 rightCollectibleOffset = new Vector3(-2f, 1.7f, 0f);
        private readonly Vector3 topCollectibleOffset = new Vector3(0f, 4f, 0f);


        float nextGenerationPosition = 80f;

        Queue<GameObject3D> rails;
        Queue<Collectible> collectibles;
        Queue<Obstacle> obstacles;


        private const float garbageCollectionDistance = 10f;

        ObjectPool<GameObject3D> railsPool;
        ObjectPool<Collectible> goldPool;
        ObjectPool<Obstacle> obstaclesPool;
        ObjectPool<Obstacle> obstaclesLowPool;


        private int difficulty = 1;
        public int Difficulty {
            get { return difficulty; }
            set { difficulty = MathHelper.Clamp(value, 0, 3); }
        }

        float[] marginMultiplier    = { 1.4f, 1.2f, 1.1f };     // Extra room for maneuver: Easy 40%, Medium 20%, Hard 10%
        float[] easyProbability     = { 0.6f, 0.3f, 0.1f };     // Probability of easy patterns: Easy 60%, Medium 30%, Hard 10%
        float[] mediumProbability   = { 0.3f, 0.4f, 0.3f };     // Probability of medium patterns: Easy 30%, Medium 40%, Hard 30%
        float[] hardProbability     = { 0.1f, 0.3f, 0.6f };     // Probability of hard patterns: Easy 10%, Medium 30%, Hard 60%



        public ProceduralGenerator(Model rail, float railLength, Model collectible, float collectibleScale,
            Model obstacleLow, Model obstacle, Vector3 hitboxSize, float popupDistance = 100f)
        {
            this.rails = new Queue<GameObject3D>();
            this.collectibles = new Queue<Collectible>();
            this.obstacles = new Queue<Obstacle>();

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

            this.obstaclesLowPool = new ObjectPool<Obstacle>(
                new Obstacle(new GameObject3D(obstacleLow), hitboxSize), 10);
        }


        public void InitializeSeed(long seed)
        {
            if (seed != 0)
                rand = new Random((int)seed);
        }


        public void Reset()
        {
            foreach (GameObject3D rail in rails)
                rail.IsActive = false;

            foreach (Collectible gold in collectibles)
                gold.IsActive = false;

            foreach (Obstacle obstacle in obstacles)
                obstacle.IsActive = false;

            rails.Clear();
            collectibles.Clear();
            obstacles.Clear();

            nextRailsGenerationPosition = 0f;
            nextGenerationPosition = 80f;

            for (int i = 0; i <= (int)(popupDistance / railSegmentLength); i++)
            {
                GameObject3D newRails = railsPool.GetOne();
                newRails.EnableLightingModel();
                newRails.Position = new Vector3(0f, 0f, nextRailsGenerationPosition);
                rails.Enqueue(newRails);
                nextRailsGenerationPosition += railSegmentLength;
            }
        }


        public void Update(Vector3 playerPosition)
        {
            if (playerPosition.Z >= nextRailsGenerationPosition - popupDistance)
            {
                if (rails.Count > 0)
                    rails.Dequeue().IsActive = false;

                GameObject3D newRails = railsPool.GetOne();
                newRails.EnableLightingModel();
                newRails.Position = new Vector3(0f, 0f, nextRailsGenerationPosition);
                rails.Enqueue(newRails);
                nextRailsGenerationPosition += railSegmentLength;
            }

            if (playerPosition.Z >= nextGenerationPosition - popupDistance)
            {
                GenerateObjects();
            }

            // Garbage collect the items behind the player, update all active items

            foreach (Collectible gold in collectibles)
            {
                if (playerPosition.Z >= gold.Position.Z + garbageCollectionDistance)
                {
                    gold.IsActive = false;
                }

                gold.Update();
            }

            foreach (Obstacle obstacle in obstacles)
            {
                if (playerPosition.Z >= obstacle.Position.Z + garbageCollectionDistance)
                {
                    obstacle.IsActive = false;
                }

                obstacle.Update();
            }
        }


        private void GenerateObjects()
        {
            int val = rand.Next(100);
            Pattern pattern;
            int index;

            // Choose the type of pattern to generate, 
            // according to their probabilities (determined by difficulty)

            if (val <= easyProbability[difficulty] * 100)   // easy pattern
            {
                index = rand.Next(easyPatterns.Length);
                Console.WriteLine("Generated EASY pattern #" + index);
                pattern = easyPatterns[index];
            }
            else if (val <= (easyProbability[difficulty] + mediumProbability[difficulty]) * 100)    // medium pattern
            {
                index = rand.Next(mediumPatterns.Length);
                Console.WriteLine("Generated MEDIUM pattern #" + index);
                pattern = mediumPatterns[index];
            }
            else    // hard pattern
            {
                index = rand.Next(hardPatterns.Length);
                Console.WriteLine("Generated HARD pattern #" + index);
                pattern = hardPatterns[index];
            }

            nextGenerationPosition = GeneratePatternAtPosition(pattern, nextGenerationPosition);
        }


        private float GeneratePatternAtPosition(Pattern pattern, float position)
        {
            Vector3 offset = Vector3.Zero;
            float speed = OldGoldMineGame.Application.Speed;
            float distance = MathHelper.Clamp(0.5f * speed, 12f, 1000f);

            foreach (PatternElement p in pattern)
            {
                if (p == PatternElement.Empty)
                {
                    // empty space
                }
                else if (p <= PatternElement.GoldAbove)     // collectible
                {
                    Collectible newGold = goldPool.GetOne();
                    switch (p)
                    {
                        case PatternElement.GoldCenter: offset = normalCollectibleOffset; break;
                        case PatternElement.GoldLeft: offset = leftCollectibleOffset; break;
                        case PatternElement.GoldRight: offset = rightCollectibleOffset; break;
                        case PatternElement.GoldAbove: offset = topCollectibleOffset; break;
                        case PatternElement.GoldBothSides:
                            Collectible extraGold = goldPool.GetOne();
                            extraGold.Position = new Vector3(0f, 0f, position) + leftCollectibleOffset;
                            collectibles.Enqueue(extraGold);
                            offset = rightCollectibleOffset;
                            break;
                    }
                    newGold.Position = new Vector3(0f, 0f, position) + offset;
                    collectibles.Enqueue(newGold);
                }
                else if (p <= PatternElement.ObstacleAbove)  // obstacle
                {
                    Obstacle newObstacle = null;
                    switch (p)
                    {
                        case PatternElement.ObstacleLeft:
                            newObstacle = obstaclesPool.GetOne();
                            offset = leftObstacleOffset;
                            break;
                        case PatternElement.ObstacleRight:
                            newObstacle = obstaclesPool.GetOne();
                            offset = rightObstacleOffset;
                            break;
                        case PatternElement.ObstacleBelow:
                            newObstacle = obstaclesLowPool.GetOne();
                            offset = bottomObstacleOffset;
                            break;
                        case PatternElement.ObstacleAbove:
                            newObstacle = obstaclesPool.GetOne();
                            offset = topObstacleOffset;
                            break;
                    }
                    newObstacle.Position = new Vector3(0f, 0f, position) + offset;
                    obstacles.Enqueue(newObstacle);
                }

                position += distance * marginMultiplier[difficulty];
            }

            return position + distance;
        }


        // Draw the level objects according to the player's POV
        public void Draw (GameCamera camera)
        {
            foreach (GameObject3D rail in rails)
                rail.Draw(camera);

            foreach (Collectible gold in collectibles)
                gold.Draw(camera);

            foreach (Obstacle obstacle in obstacles)
                obstacle.Draw(camera);
        }
    }
}