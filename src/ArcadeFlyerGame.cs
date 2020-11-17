using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ArcadeFlyer2D
{
    // The Game itself
    class ArcadeFlyerGame : Game
    {
        // Graphics Manager
        private GraphicsDeviceManager graphics;

        // Sprite Drawer
        private SpriteBatch spriteBatch;

        // The player
        private Player player;

        private List<Enemy> enemies;

        // An enemy
        private Enemy enemy;
        private Timer enemyCreationTimer;

        private List<Projectile> projectiles;

        private Texture2D playerProjectileSprite;

        private Texture2D enemyProjectileSprite;

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

            // Make mouse visible
            IsMouseVisible = true;

            // Initialize the player to be in the top left
            player = new Player(this, new Vector2(0.0f, 0.0f));
            
            enemies = new List<Enemy>();
            // Initialize an enemy to be on the right side
            enemies.Add(new Enemy(this, new Vector2(screenWidth, 0)));

            enemyCreationTimer = new Timer(0.5f);
            enemyCreationTimer.StartTimer();
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
            playerProjectileSprite = Content.Load<Texture2D>("Projectile");   
            enemyProjectileSprite = Content.Load<Texture2D>("EnemyFire");     
        }

        // Called every frame
        protected override void Update(GameTime gameTime)
        {   
            // Update base game
            base.Update(gameTime);

            // Update the components
            player.Update(gameTime);

            foreach(Enemy enemy in enemies)
            {
                enemy.Update(gameTime);
            }
            
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                Projectile p = projectiles[i];
                p.Update();

                bool isPlayerProjectile = p.ProjectileType == ProjectileType.Player;

                //Is this an enemy projectile? And, if it is, did it hit my player?
                if (!isPlayerProjectile && player.Overlaps(p))
                {
                    projectiles.Remove(p);
                }
                //Is this a player projectile? And, if it is, did it hit my enemy?
                else if (isPlayerProjectile)
                {
                    //projectiles.Remove(p);
                    for (int j = enemies.Count  - 1; j >= 0; j--)
                    {
                        Enemy e = enemies[j];
                        if (e.Overlaps(p))
                        {
                            projectiles.Remove(p);
                            enemies.Remove(e);
                        }
                    }
                }
            }
            if (!enemyCreationTimer.Active)
            {
                enemies.Add(new Enemy(this, new Vector2(screenWidth, 0)));

                enemyCreationTimer.StartTimer();
            }

            enemyCreationTimer.Update(gameTime);
        }

        // Draw everything in the game
        protected override void Draw(GameTime gameTime)
        {
            // First clear the screen
            GraphicsDevice.Clear(Color.White);

            // Start batch draw
            spriteBatch.Begin();

            // Draw the components
            player.Draw(gameTime, spriteBatch);
            foreach(Enemy enemy in enemies)
            {
                enemy.Draw(gameTime, spriteBatch);
            }
            foreach(Projectile p in projectiles)
            {
                p.Draw(gameTime, spriteBatch);
            }

            // End batch draw
            spriteBatch.End();
        }

        public void FireProjectile(Vector2 position, Vector2 velocity, ProjectileType projectileType)
        {
            Texture2D projectileTexture;
            if (projectileType == ProjectileType.Player)
            {
                projectileTexture = playerProjectileSprite;
            }
            else
            {
                projectileTexture = enemyProjectileSprite;
            }
            Projectile firedProjectile = new Projectile(position, velocity, projectileTexture, projectileType);
            projectiles.Add(firedProjectile);
        }
    }
}
