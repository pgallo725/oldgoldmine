using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.Engine;


namespace oldgoldmine_game.Gameplay
{
    public class Collectible : GameObject3D
    {
        private static readonly Vector3 cornerOffset = new Vector3(0.5f, 0.5f, -0.5f);

        private BoundingBox hitbox;

        public override Vector3 Position {
            get { return base.Position; }
            set {
                base.Position = value;
                hitbox.Min = value - cornerOffset;
                hitbox.Max = value + cornerOffset;
            }
        }

        public Collectible()
            : base()
        {
            this.hitbox = new BoundingBox(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.5f, 0.5f, -0.5f));
        }

        public Collectible(Model model/*, BoundingBox hitbox*/)
            : base(model)
        {
            // Automatically create the bounding box based on the model meshes
            this.hitbox = BoundingBox.CreateFromSphere(model.Meshes[0].BoundingSphere);
            for (int meshIndex = 1; meshIndex < model.Meshes.Count; meshIndex++)
            {
                BoundingBox meshBox = BoundingBox.CreateFromSphere(model.Meshes[meshIndex].BoundingSphere);
                this.hitbox = BoundingBox.CreateMerged(this.hitbox, meshBox);
            }
        }

        public Collectible(Model model, Vector3 position, Vector3 scale, Quaternion rotation, BoundingBox hitbox)
            : base(model, position, scale, rotation)
        {
            this.hitbox = hitbox;
        }

        public Collectible(GameObject3D collectibleObj, BoundingBox hitbox)
            : base(collectibleObj)
        {
            this.hitbox = hitbox;
        }


        public override void Update()
        {
            if (!active)
                return;

            if (CheckPlayerCollision(OldGoldMineGame.player))
            {
                this.IsActive = false;
                OldGoldMineGame.Score += 100;
            }
        }


        bool CheckPlayerCollision(Player player)
        {
            if (this.hitbox.Intersects(player.hitbox))
                return true;
            return false;
        }

    }
}