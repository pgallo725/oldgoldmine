using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.Engine;


namespace oldgoldmine_game.Gameplay
{
    public class Collectible : GameObject3D
    {

        private BoundingBox hitbox;


        public Collectible()
            : base()
        {
            this.hitbox = new BoundingBox(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.5f, 0.5f, -0.5f));
        }

        public Collectible(Model model, BoundingBox hitbox)
            : base(model)
        {
            this.hitbox = hitbox;
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

            base.Update();
        }


        bool CheckPlayerCollision(Player player)
        {
            if (this.hitbox.Intersects(player.hitbox))
                return true;
            return false;
        }

    }
}