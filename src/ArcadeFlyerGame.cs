using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ArcadeFlyer2D
{
    // The Game itself
    class ArcadeFlyerGame : Game
    {
        Random rand = new Random();

        // Graphics Manager
        private GraphicsDeviceManager graphics;

        // Sprite Drawer
        private SpriteBatch spriteBatch;

        // Text Drawer
        private SpriteFont textFont;

        // Keep track of the player's remaining lives
        private int life = 3;

        // Keep track of the player's current score
        private int score = 0;

        private bool gameOver = false;

        private bool checkWin = false;

        private float timespan = 3.0f;

        // The player
        private Player player;

        // A List of enemies
        private List<Enemy> enemies;

        private Portal portal;

        // A timer for enemy generation
        private Timer enemyCreationTimer;

        // List of all projectiles on the screen
        private List<Projectile> projectiles;

        // Projectile image for player
        private Texture2D playerProjectileSprite;

        // Projectile image for enemy
        private Texture2D enemyProjectileSprite;

        private Texture2D prairieBackground;

        private Texture2D desertBackground;

        private Texture2D tundraBackground;
        private Texture2D caveBackground;
        private Texture2D jungleBackground;

        private Texture2D spaceBackground;

        private int level = 1;

        private int levelCheck = 5;

        // Screen width
        private int screenWidth = 1600;
        public int ScreenWidth
        {
            get { return screenWidth; }
            private set { screenWidth = value; }
        }

        // Screen height
        private int screenHeight = 900;
        public int ScreenHeight
        {
            get { return screenHeight; }
            private set { screenHeight = value; }
        }
        
        // Initalized the game
        public ArcadeFlyerGame()
        {
            // Get the graphics
            graphics = new GraphicsDeviceManager(this);

            // Set the height and width
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();

            // Set up the directory containing the assets
            Content.RootDirectory = "Content";

            // Initialize the font
            textFont = Content.Load<SpriteFont>("Text");

            // Make mouse visible
            IsMouseVisible = true;

            // Initialize the player to be in the top left
            player = new Player(this, new Vector2(0.0f, 0.0f));

            portal = new Portal(this, new Vector2(-200.0f, -200.0f)); 

            // Initialize empty list of enemies
            enemies = new List<Enemy>();
            
            // Add an enemy to be on the right side
            enemies.Add(new Enemy(this, new Vector2(screenWidth, 0)));

            // Initialize enemy creation timer
            enemyCreationTimer = new Timer(3.0f);
            enemyCreationTimer.StartTimer();

            // Initialize empty list of projectiles
            projectiles = new List<Projectile>();
        }

        // Initialize
        protected override void Initialize()
        {
            base.Initialize();
        }

        // Load the content for the game, called automatically on start
        protected override void LoadContent()
        {
            // Create the sprite batch
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load in textures
            playerProjectileSprite = Content.Load<Texture2D>("PlayerFire");
            enemyProjectileSprite = Content.Load<Texture2D>("EnemyFire");
            prairieBackground = Content.Load<Texture2D>("PrairieBackground");
            desertBackground = Content.Load<Texture2D>("DesertBackground");
            tundraBackground = Content.Load<Texture2D>("TundraBackground");
            caveBackground = Content.Load<Texture2D>("CaveBackground");
            jungleBackground = Content.Load<Texture2D>("JungleBackground");
            spaceBackground = Content.Load<Texture2D>("SpaceBackground");
        }

        // Called every frame
        protected override void Update(GameTime gameTime)
        {   
            if (gameOver)
            {
                return;
            }
            
            // Update base game
            base.Update(gameTime);

            // Update the player
            player.Update(gameTime);

            // Update each enemy in the list
            foreach (Enemy enemy in enemies)
            {
                enemy.Update(gameTime);
            }

            // Loop through projectiles backwards (in order to remove projectiles as needed)
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                // Update each projectile
                Projectile p = projectiles[i];
                p.Update();

                // Is this a player projectile?
                bool playerProjectile = p.ProjectileType == ProjectileType.Player;

                // Check if the player collides with this non-player projectile
                if (!playerProjectile && player.Overlaps(p))
                {
                    // There is a collision with the player, remove the projectile
                    projectiles.Remove(p);

                    // Decrement life
                    life--;

                    if (life < 1){
                        gameOver = true;
                    }
                }
                else if (playerProjectile)
                {
                    // Loop through enemies backwards (in order to remove them as needed)
                    for (int enemyIdx = enemies.Count - 1; enemyIdx >= 0; enemyIdx--)
                    {
                        // Get the current enemy
                        Enemy enemy = enemies[enemyIdx];

                        // Check if this enemy collides with the player projectile
                        if (enemy.Overlaps(p))
                        {
                            // There is a collision with the enemy, remove the projectile
                            projectiles.Remove(p);

                            // Remove the enemy as well
                            enemies.Remove(enemy);
                            
                            // Increment score
                            score++;
                        }
                    }
                }
            }

            //Determines if it's time to activate the portal
            if (score >= levelCheck)
            {
                portal.Position = new Vector2(1200.0f, 450.0f);
                if (player.Overlaps(portal))
                {
                    levelCheck = level ^ 2 + level + 12 + score; // Keep in mind that this calculation relies on the old level
                    if (level == 6)
                    {
                        gameOver = true;
                        checkWin = true;
                        return;
                    }
                    else
                    {
                        level++;
                    }
                    player.Position = new Vector2(0.0f, 0.0f);
                    portal.Position = new Vector2(-200.0f, -200.0f);
                    if (timespan > 0.2)
                    {
                        timespan = timespan / 2;
                    }
                    enemyCreationTimer = new Timer(timespan);
                    life += level * 6 + 1;
                }
            }
            // Enemy creation timer is up
            else if (!enemyCreationTimer.Active)
            {
                // Add a new enemy to the list
                enemies.Add(new Enemy(this, new Vector2(screenWidth + rand.Next(-100,0), rand.Next(0, screenHeight - 130))));

                // Re-start the timer
                enemyCreationTimer.StartTimer();
            }

            // Update the enemy creation timer
            enemyCreationTimer.Update(gameTime);
        }

        // Draw everything in the game
        protected override void Draw(GameTime gameTime)
        {
            if (gameOver)
            {
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin();
                Vector2 textPosition = new Vector2(screenWidth/2, screenHeight/2);
                if (checkWin)
                {
                    spriteBatch.DrawString(textFont, "Congratulations! You won!\nYour score was " + score + ".", textPosition, Color.White);
                }
                else
                {
                    spriteBatch.DrawString(textFont, "Game Over!\nYour score was " + score + ".", textPosition, Color.White);
                }
                spriteBatch.End();
                return;
            }
            // First clear the screen
            GraphicsDevice.Clear(Color.Black);

            // Start batch draw
            spriteBatch.Begin();

            switch (level)
            {   
                case 1:
                    spriteBatch.Draw(prairieBackground, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    break;
                case 2:
                    spriteBatch.Draw(desertBackground, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    break;
                case 3:
                    spriteBatch.Draw(tundraBackground, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    break;
                case 4:
                    spriteBatch.Draw(jungleBackground, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    break;
                case 5:
                    spriteBatch.Draw(caveBackground, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    break;
                default:
                    spriteBatch.Draw(spaceBackground, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    break;
            }

            // Draw the player
            player.Draw(gameTime, spriteBatch);

            // Draw each enemy
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(gameTime, spriteBatch);
            }

            // Draw all projectiles
            foreach (Projectile p in projectiles)
            {
                p.Draw(gameTime, spriteBatch);
            }

            portal.Draw(gameTime, spriteBatch);
            // Draw Life count and Score count
            spriteBatch.DrawString(textFont, $"Life: {life}\nScore: {score}", Vector2.Zero, Color.White);

            // End batch draw
            spriteBatch.End();
        }

        // Fires a projectile with the given position and velocity
        public void FireProjectile(Vector2 position, Vector2 velocity, ProjectileType projectileType)
        {
            // Create the image for the projectile
            Texture2D projectileImage;
            
            if (projectileType == ProjectileType.Player)
            {
                // This is a projectile sent from the player, set it to the proper sprite
                projectileImage = playerProjectileSprite;
            }
            else
            {
                // This is a projectile sent from the enemy, set it to the proper sprite
                projectileImage = enemyProjectileSprite;
            }

            // Create the new projectile
            Projectile firedProjectile = new Projectile(position, velocity, projectileImage, projectileType);

            // Add the projectile to the list
            projectiles.Add(firedProjectile);
        }
    }
}
