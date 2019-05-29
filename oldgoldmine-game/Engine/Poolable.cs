namespace oldgoldmine_game.Engine
{
    public abstract class Poolable
    {
        bool active = true;
        public bool IsActive { get { return active; } set { active = value; } }

    }
}