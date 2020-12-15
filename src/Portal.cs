using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ArcadeFlyer2D
{
    class Portal : Sprite
    {
        // A reference to the game that will contain this enemy
        private ArcadeFlyerGame root;

        // Initialize the portal
        public Portal(ArcadeFlyerGame root, Vector2 position) : base(position)
        {
            // Initialize values
            this.root = root;
            this.position = position;
            this.SpriteWidth = 128.0f;

            // Load the content for this enemy
            LoadContent();
        }

        public void LoadContent()
        {
            // Get the Portal image
            this.SpriteImage = root.Content.Load<Texture2D>("Portal"); 
        }

        // Called each frame
        public void Update(GameTime gameTime, bool nextLevel)
        {
            if (nextLevel)
            {
                position.X = 1200.0f;
                position.Y = 450.0f; 
            }
        }
    }
}