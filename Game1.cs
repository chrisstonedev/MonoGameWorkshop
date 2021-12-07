﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using tainicom.Aether.Physics2D.Dynamics;

namespace MonoGameWorkshop
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D playerTexture;
        private Texture2D platformTexture;
        private Texture2D backgroundTexture;
        private OrthographicCamera camera;
        private World world;
        private Body playerBody;
        private Vector2 playerPosition = new(width * 0.5f, height * 0.3f);
        private Vector2 playerSize = new(width * 0.06f, height * 0.12f);

        private const int width = 200;
        private const int height = 120;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            world = new World
            {
                Gravity = new Vector2(0, 300)
            };
        }

        protected override void Initialize()
        {
            var viewportAdapter = new ScalingViewportAdapter(GraphicsDevice, width, height);
            camera = new OrthographicCamera(viewportAdapter);

            playerBody = world.CreateBody(playerPosition, 0, BodyType.Dynamic);
            var fixture = playerBody.CreateRectangle(playerSize.X, playerSize.Y, 1f, Vector2.Zero);
            fixture.Restitution = 0.1f;
            fixture.Friction = 0.5f;

            var bottomEdge = world.CreateEdge(new Vector2(0, height * 0.92f), new Vector2(width, height * 0.92f));
            bottomEdge.SetRestitution(0.01f);
            bottomEdge.SetFriction(0.5f);
            var leftEdge = world.CreateEdge(new Vector2(0, 0), new Vector2(0, height));
            var rightEdge = world.CreateEdge(new Vector2(width, 0), new Vector2(width, height));
            var topEdge = world.CreateEdge(new Vector2(0, 0), new Vector2(width, 0));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            playerTexture = Content.Load<Texture2D>("player");
            platformTexture = Content.Load<Texture2D>("platform");
            backgroundTexture = Content.Load<Texture2D>("background");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), null, Color.White);
            spriteBatch.End();

            spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());
            var playerDestination = new Rectangle((int)(playerBody.Position.X - playerSize.X / 2), (int)(playerBody.Position.Y - (playerSize.Y / 2)), (int)playerSize.X, (int)playerSize.Y);
            spriteBatch.Draw(playerTexture, playerDestination, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
