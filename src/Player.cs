using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ArcadeFlyer2D
{
    // The player, controlled by the keyboard
    class Player : Sprite
    {
        // A reference to the game that will contain the player
        private ArcadeFlyerGame root;

        // The speed at which the player can move
        private float movementSpeed = 4.0f;

        private Timer cooldownTimer;

        // Initialize a player
        public Player(ArcadeFlyerGame root, Vector2 Position) : base(Position)
        {
            // Initialize values
            this.root = root;
            this.Position = Position;
            this.SpriteWidth = 128.0f;

            cooldownTimer = new Timer(0.5f);

            // Load the content for the player
            LoadContent();
        }

        // Loads all the assets for the player
        public void LoadContent()
        {
            // Get the MainChar image
            this.SpriteImage = root.Content.Load<Texture2D>("MainChar");
        }

        // Update Position based on input
        private void HandleInput(KeyboardState currentKeyboardState)
        {
            // Get all the key states
            bool upKeyPressed = currentKeyboardState.IsKeyDown(Keys.W);
            bool downKeyPressed = currentKeyboardState.IsKeyDown(Keys.S);
            bool leftKeyPressed = currentKeyboardState.IsKeyDown(Keys.A);
            bool rightKeyPressed = currentKeyboardState.IsKeyDown(Keys.D);
            bool spaceKeyPressed = currentKeyboardState.IsKeyDown(Keys.Space);

            // If Up is pressed, decrease Position Y
            if (upKeyPressed)
            {
                position.Y -= movementSpeed;
            }
            
            // If Down is pressed, increase Position Y
            if (downKeyPressed)
            {
                position.Y += movementSpeed;
            }
            
            // If Left is pressed, decrease Position X
            if (leftKeyPressed)
            {
                position.X -= movementSpeed;
            }
            
            // If Right is pressed, increase Position X
            if (rightKeyPressed)
            {
                position.X += movementSpeed;
            }

            if (spaceKeyPressed && !cooldownTimer.Active)
            {
                Vector2 projectilePosition;
                Vector2 projectileVelocity;

                projectilePosition = new Vector2(position.X + (SpriteWidth / 2), position.Y + (SpriteHeight / 2));
                projectileVelocity = new Vector2(10.0f, 0f);
                root.FireProjectile(projectilePosition, projectileVelocity, "Projectile");
                cooldownTimer.StartTimer();

            }
        }

        // Called each frame
        public void Update(GameTime gameTime)
        {   
            // Get current keyboard state
            KeyboardState currentKeyboardState = Keyboard.GetState();

            // Handle any movement input
            HandleInput(currentKeyboardState);

            cooldownTimer.Update(gameTime);

        }
    }
}
