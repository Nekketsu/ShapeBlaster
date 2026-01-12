using BloomPostprocess;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ShapeBlaster;

public class GameRoot : Game
{
    public static GameRoot Instance { get; private set; }
    public static Viewport Viewport => Instance.GraphicsDevice.Viewport;
    public static Vector2 ScreenSize => new Vector2(Viewport.Width, Viewport.Height);
    public static GameTime GameTime { get; private set; }
    public static ParticleManager<ParticleState> ParticleManager { get; private set; }
    public static Grid Grid { get; private set; }

    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private BloomComponent bloom;

    private bool paused = false;
    private bool useBloom = true;

    public GameRoot()
    {
        Instance = this;

        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        graphics.PreferredBackBufferWidth = 1920;
        graphics.PreferredBackBufferHeight = 1080;

        bloom = new BloomComponent(this);
        Components.Add(bloom);
        bloom.Settings = new BloomSettings(null, 0.25f, 4, 2, 1, 1.5f, 1);
    }

    protected override void Initialize()
    {
        base.Initialize();

        ParticleManager = new ParticleManager<ParticleState>(1024 * 20, ParticleState.UpdateParticle);

        const int maxGridPoints = 1600;
        var gridSpacing = new Vector2(float.Sqrt(Viewport.Width * Viewport.Height / maxGridPoints));
        Grid = new Grid(Viewport.Bounds, gridSpacing);

        EntityManager.Add(PlayerShip.Instance);

        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(Sound.Music);
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        Art.Load(Content);
        Sound.Load(Content);
    }

    protected override void Update(GameTime gameTime)
    {
        GameTime = gameTime;
        Input.Update();

        // Allows the game to exit
        if (Input.WasButtonPressed(Buttons.Back) || Input.WasKeyPressed(Keys.Escape))
        {
            Exit();
        }

        if (Input.WasKeyPressed(Keys.P))
        {
            paused = !paused;
        }
        if (Input.WasKeyPressed(Keys.B))
        {
            useBloom = !useBloom;
        }

        if (!paused)
        {
            EntityManager.Update();
            EnemySpawner.Update();
            ParticleManager.Update();
            PlayerStatus.Update();
            Grid.Update();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        bloom.BeginDraw();
        if (!useBloom)
        {
            base.Draw(gameTime);
        }

        GraphicsDevice.Clear(Color.Black);

        // Draw entities. Sort by texture for better batching.
        spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
        EntityManager.Draw(spriteBatch);
        spriteBatch.End();

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        Grid.Draw(spriteBatch);
        ParticleManager.Draw(spriteBatch);
        spriteBatch.End();

        if (useBloom)
        {
            base.Draw(gameTime);
        }

        // Draw user interface
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

        spriteBatch.DrawString(Art.Font, "Lives: " + PlayerStatus.Lives, new Vector2(5), Color.White);
        DrawRightAlignedString("Score: " + PlayerStatus.Score, 5);
        DrawRightAlignedString("Multiplier: " + PlayerStatus.Multiplier, 35);

        // draw the custom mouse cursor
        spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);

        if (PlayerStatus.IsGameOver)
        {
            var text = "Game Over\n" +
                "Your Score: " + PlayerStatus.Score + "\n" +
                "High Score: " + PlayerStatus.HighScore;
            var textSize = Art.Font.MeasureString(text);
            spriteBatch.DrawString(Art.Font, text, ScreenSize / 2 - textSize / 2, Color.White);
        }

        spriteBatch.End();
    }

    private void DrawRightAlignedString(string text, float y)
    {
        var textWidth = Art.Font.MeasureString(text).X;
        spriteBatch.DrawString(Art.Font, text, new Vector2(ScreenSize.X - textWidth - 5, y), Color.White);
    }
}
