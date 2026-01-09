using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ShapeBlaster;

public class GameRoot : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;

    public static GameRoot Instance { get; private set; }
    public static Viewport Viewport => Instance.GraphicsDevice.Viewport;
    public static Vector2 ScreenSize => new Vector2(Viewport.Width, Viewport.Height);

    public GameRoot()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        Instance = this;
    }

    protected override void Initialize()
    {
        base.Initialize();

        EntityManager.Add(PlayerShip.Instance);
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        Art.Load(Content);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        Input.Update();
        EntityManager.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
        EntityManager.Draw(spriteBatch);

        // draw the custom mouse cursor 
        spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);
        spriteBatch.End();

        base.Draw(gameTime);
    }
}
