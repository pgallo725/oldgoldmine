using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using OldGoldMine.Engine;


namespace OldGoldMine.Gameplay
{
    public class Level
    {
        /// <summary>
        /// Represents each possible type of object that can occupy a slot in the generated level.
        /// </summary>
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

        /// <summary>
        /// Represents a pre-defined set of obstacles and collectibles, manually laid out,
        /// which could be randomly selected to be spawned inside the level.
        /// </summary>
        private class Pattern : IEnumerable
        {
            private readonly List<PatternElement> patternObjects = new List<PatternElement>(8);

            public int Length { get { return patternObjects.Count; } }

            public PatternElement this[int key]
            {
                get { return patternObjects[key]; }
                set { patternObjects[key] = value; }
            }

            public void Add(PatternElement item)
            {
                patternObjects.Add(item);
            }

            public bool Contains(PatternElement item)
            {
                return patternObjects.Contains(item);
            }

            public IEnumerator GetEnumerator()
            {
                return patternObjects.GetEnumerator();
            }
        }


        /* PRE-DEFINED PATTERNS, GROUPED BY THEIR EXPECTED DIFFICULTY */

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


        private Random randomizer = new Random();

        private readonly float caveSegmentLength;
        private readonly float popupDistance;

        private float nextCavePosition = 0f;
        private float nextObjectPosition = 80f;

        // Offsets to correctly position collectibles around the level
        private readonly Vector3 normalCollectibleOffset = new Vector3(0f, 1.5f, 0f);
        private readonly Vector3 leftCollectibleOffset = new Vector3(2f, 1.7f, 0f);
        private readonly Vector3 rightCollectibleOffset = new Vector3(-2f, 1.7f, 0f);
        private readonly Vector3 topCollectibleOffset = new Vector3(0f, 4f, 0f);

        // Collections of objects spawned in the level
        private readonly Queue<GameObject3D> caves = new Queue<GameObject3D>();
        private readonly Queue<GameObject3D> props = new Queue<GameObject3D>();
        private readonly Queue<Collectible> collectibles = new Queue<Collectible>();
        private readonly Queue<Obstacle> obstacles = new Queue<Obstacle>();

        // Object pools
        private readonly ObjectPool<GameObject3D> cavePool;
        private readonly ObjectPool<GameObject3D>[] propsPool;
        private readonly ObjectPool<Collectible> goldPool;
        private readonly ObjectPool<Obstacle> obstaclesLowPool;
        private readonly ObjectPool<Obstacle> obstaclesLeftPool;
        private readonly ObjectPool<Obstacle> obstaclesRightPool;
        private readonly ObjectPool<Obstacle> obstaclesHighPool;
        

        /// <summary>
        /// Difficulty setting used for level generation (0 = Easy, 1 = Medium, 2 = Hard)
        /// </summary>
        public int Difficulty 
        {
            get { return difficulty; }
            set { difficulty = MathHelper.Clamp(value, 0, 2); }
        }
        private int difficulty = 1;

        // Additional level generation parameters based on difficulty
        private readonly float[] marginMultiplier    = { 1.4f, 1.2f, 1.1f };     // Extra room for maneuver: Easy 40%, Medium 20%, Hard 10%
        private readonly float[] easyProbability     = { 0.6f, 0.3f, 0.1f };     // Probability of easy patterns: Easy 60%, Medium 30%, Hard 10%
        private readonly float[] mediumProbability   = { 0.3f, 0.4f, 0.3f };     // Probability of medium patterns: Easy 30%, Medium 40%, Hard 30%
        private readonly float[] hardProbability     = { 0.1f, 0.3f, 0.6f };     // Probability of hard patterns: Easy 10%, Medium 30%, Hard 60%


        /// <summary>
        /// Create a new Level, feeding it with the parameters and assets for the procedural generation.
        /// </summary>
        /// <param name="cave">GameObject3D to be used as the cave environment for the level.</param>
        /// <param name="caveLength">Length (in units) of the provided cave segment.</param>
        /// <param name="caveProps">Array of 3D objects that will be used to populate the cave environment.</param>
        /// <param name="gold">Object to be used as the archetype for gold Collectibles.</param>
        /// <param name="lower">Archetype for the lower obstacles in the level.</param>
        /// <param name="left">Archetype for the left obstacles in the level.</param>
        /// <param name="right">Archetype for the right obstacles in the level.</param>
        /// <param name="upper">Archetype for the upper obstacles in the level.</param>
        /// <param name="popupDistance">Distance (in units) from the camera at which new objects are spawned.</param>
        public Level(GameObject3D cave, float caveLength, GameObject3D[] caveProps, Collectible gold,
            Obstacle lower, Obstacle left, Obstacle right, Obstacle upper, float popupDistance)
        {
            // Set-up the distance fog effect for all level objects, based on the popup distance
            this.popupDistance = popupDistance;
            cave.SetFogEffect(true, Color.Black, popupDistance - 20f, popupDistance);
            foreach (GameObject3D prop in caveProps)
                prop.SetFogEffect(true, Color.Black, popupDistance - 20f, popupDistance);
            gold.SetFogEffect(true, Color.Black, popupDistance - 20f, popupDistance);
            lower.SetFogEffect(true, Color.Black, popupDistance - 20f, popupDistance);
            left.SetFogEffect(true, Color.Black, popupDistance - 20f, popupDistance);
            right.SetFogEffect(true, Color.Black, popupDistance - 20f, popupDistance);
            upper.SetFogEffect(true, Color.Black, popupDistance - 20f, popupDistance);

            // Initializing object pools for the procedural generator (Cave, Props, Collectibles and Obstacles)
            this.cavePool = new ObjectPool<GameObject3D>(cave, 4);
            this.propsPool = new ObjectPool<GameObject3D>[caveProps.Length];
            for (int i = 0; i < caveProps.Length; i++)
                this.propsPool[i] = new ObjectPool<GameObject3D>(caveProps[i], 2);
            this.goldPool = new ObjectPool<Collectible>(gold, 10);
            this.obstaclesLowPool = new ObjectPool<Obstacle>(lower, 8);
            this.obstaclesLeftPool = new ObjectPool<Obstacle>(left, 8);
            this.obstaclesRightPool = new ObjectPool<Obstacle>(right, 8);
            this.obstaclesHighPool = new ObjectPool<Obstacle>(upper, 8);

            // First cave segment is spawned behind the player to avoid starting outside the cave
            this.caveSegmentLength = caveLength;
            this.nextCavePosition = -caveLength;
            this.nextObjectPosition = 80f;
        }


        /// <summary>
        /// Begin the generation of the procedural level based on the provided seed.
        /// </summary>
        /// <param name="seed">64-bit integer seed.</param>
        public void Initialize(long seed, float startSpeed)
        {
            randomizer = new Random((int)seed);

            // Generate the first few cave segments and patterns
            while (nextCavePosition < popupDistance)
                GenerateCave();

            while (nextObjectPosition < popupDistance)
                GenerateObjects(startSpeed);
        }


        /// <summary>
        /// Reset the level to its original state, removing all generated objects.
        /// </summary>
        public void Reset()
        {
            while (caves.Count > 0)
                caves.Dequeue().IsActive = false;

            while (props.Count > 0)
                props.Dequeue().IsActive = false;

            while (collectibles.Count > 0)
                collectibles.Dequeue().IsActive = false;

            while (obstacles.Count > 0)
                obstacles.Dequeue().IsActive = false;

            this.nextCavePosition = -caveSegmentLength;
            this.nextObjectPosition = 80f;
        }


        /// <summary>
        /// Update the level's status in the current frame.
        /// </summary>
        /// <param name="gameTime">Time signature of the current frame.</param>
        /// <param name="playerPosition">The current Player object navigating the level.</param>
        public void Update(in GameTime gameTime, in Player player)
        {
            // Generate new cave segments when needed
            if (player.Position.Z >= nextCavePosition - popupDistance)
                GenerateCave();

            // Generate new level objects when needed
            if (player.Position.Z >= nextObjectPosition - popupDistance)
                GenerateObjects(player.Speed);

            while (IsRemovable(caves.Peek(), player))
            {
                // Remove previous cave segments and props from the queue
                caves.Dequeue().IsActive = false;
                props.Dequeue().IsActive = false;
            }

            while (IsRemovable(obstacles.Peek(), player))
            {
                // Remove surpassed obstacles from the queue (ordered by distance)
                obstacles.Dequeue().IsActive = false;
            }

            // Update all active collectibles in the current frame
            foreach (Collectible gold in collectibles)
                gold.Update(gameTime, player);

            // Check collisions for all obstacles in the level
            foreach (Obstacle obstacle in obstacles)
                obstacle.Update(gameTime, player);

            // Remove all inactive or surpassed collectibles from the queue
            int N = collectibles.Count;
            for (int i = 0; i < N; i++)
            {
                Collectible gold = collectibles.Dequeue();
                if (IsRemovable(gold, player))
                    gold.IsActive = false;

                // Re-insert only active collectibles in the queue (preserving order)
                if (gold.IsActive)
                    collectibles.Enqueue(gold);
            }
        }


        // Check if the provided GameObject has been surpassed by the player
        private bool IsRemovable(GameObject3D gameObject, Player player)
        {
            return gameObject.Position.Z < player.Position.Z - caveSegmentLength;
        }


        private void GenerateCave()
        {
            // Generate new cave segment
            GameObject3D newCave = cavePool.GetOne();
            newCave.Position = new Vector3(0f, 0f, nextCavePosition);
            caves.Enqueue(newCave);

            // Populate the cave segment with a random set of props
            int random = randomizer.Next(propsPool.Length);
            GameObject3D newProps = propsPool[random].GetOne();
            newProps.Position = newCave.Position;
            props.Enqueue(newProps);

            nextCavePosition += caveSegmentLength;
        }

        private void GenerateObjects(float speed)
        {
            int val = randomizer.Next(100);
            float distance = MathHelper.Clamp(speed/2, 12f, 1000f);

            // Choose the pattern to be generated, based on probabilities
            Pattern pattern;
            if (val <= easyProbability[difficulty] * 100)
            {
                // Easy pattern
                pattern = easyPatterns[randomizer.Next(easyPatterns.Length)];
            }
            else if (val <= (easyProbability[difficulty] + mediumProbability[difficulty]) * 100)
            {
                // Medium pattern
                pattern = mediumPatterns[randomizer.Next(mediumPatterns.Length)];
            }
            else
            {
                // Hard pattern
                pattern = hardPatterns[randomizer.Next(hardPatterns.Length)];
            }

            // Spawn the objects in the selected pattern
            foreach (PatternElement p in pattern)
            {
                if (p == PatternElement.Empty)
                {
                    // Empty space
                }
                else if (p <= PatternElement.GoldAbove)     // Collectible
                {
                    Vector3 offset = Vector3.Zero;
                    Collectible newGold = goldPool.GetOne();
                    switch (p)
                    {
                        case PatternElement.GoldCenter: offset = normalCollectibleOffset; break;
                        case PatternElement.GoldLeft: offset = leftCollectibleOffset; break;
                        case PatternElement.GoldRight: offset = rightCollectibleOffset; break;
                        case PatternElement.GoldAbove: offset = topCollectibleOffset; break;
                        case PatternElement.GoldBothSides: offset = rightCollectibleOffset;
                            Collectible extraGold = goldPool.GetOne();
                            extraGold.Position = new Vector3(0f, 0f, nextObjectPosition) + leftCollectibleOffset;
                            collectibles.Enqueue(extraGold);
                            break;
                    }
                    newGold.Position = new Vector3(0f, 0f, nextObjectPosition) + offset;
                    collectibles.Enqueue(newGold);
                }
                else if (p <= PatternElement.ObstacleAbove)     // Obstacle
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
            }

            nextObjectPosition += distance * marginMultiplier[difficulty];
        }


        /// <summary>
        /// Draw the level environment and all gameplay objects (obstacles and collectibles)
        /// </summary>
        /// <param name="camera">The camera used to render the scene.</param>
        public void Draw (Camera camera)
        {
            foreach (GameObject3D cave in caves)
                cave.Draw(camera);

            foreach (GameObject3D prop in props)
                prop.Draw(camera);

            foreach (Collectible gold in collectibles)
                gold.Draw(camera);

            foreach (Obstacle obstacle in obstacles)
                obstacle.Draw(camera);
        }

    }
}