using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArcadeFlyer2D
{
    class Timer
    {
        private float totalTime;
        private float timer;

        public bool Active
        {
            get;
            private set;
        }

        public Timer(float totalTime)
        {
            this.endTIme = endTime;
            this.currentTime = 0.0f;
            this.Active = false;
        }

        public void StartTimer()
        {
            Active = true;
            currentTime = 0.0f;
        }

        public void Update(GameTime gameTime)
        {
            if (Active)
            {
                currentTime = currentTime + (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (currentTime >= endTime)
                {
                    Active = false;
                }
            }
        }
    }
}