using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace ShapeBlaster;

static class EntityManager
{
    static List<Entity> entities = [];
    static List<Enemy> enemies = [];
    static List<Bullet> bullets = [];
    static List<BlackHole> blackHoles = [];

    public static IEnumerable<BlackHole> BlackHoles => blackHoles;

    static bool isUpdating;
    static List<Entity> addedEntities = [];

    public static int Count => entities.Count;
    public static int BlackHoleCount => blackHoles.Count;

    public static void Add(Entity entity)
    {
        if (!isUpdating)
        {
            AddEntity(entity);
        }
        else
        {
            addedEntities.Add(entity);
        }
    }

    private static void AddEntity(Entity entity)
    {
        entities.Add(entity);
        if (entity is Bullet)
        {
            bullets.Add(entity as Bullet);
        }
        else if (entity is Enemy)
        {
            enemies.Add(entity as Enemy);
        }
        else if (entity is BlackHole)
        {
            blackHoles.Add(entity as BlackHole);
        }
    }

    public static void Update()
    {
        isUpdating = true;
        HandleCollisions();

        foreach (var entity in entities)
        {
            entity.Update();
        }

        isUpdating = false;

        foreach (var entity in addedEntities)
        {
            AddEntity(entity);
        }

        addedEntities.Clear();

        // remove any expired entities. 
        entities = [.. entities.Where(x => !x.IsExpired)];
        bullets = [.. bullets.Where(x => !x.IsExpired)];
        enemies = [.. enemies.Where(x => !x.IsExpired)];
        blackHoles = [.. blackHoles.Where(x => !x.IsExpired)];
    }

    static void HandleCollisions()
    {
        // handle collisions between enemies 
        for (var i = 0; i < enemies.Count; i++)
        {
            for (var j = i + 1; j < enemies.Count; j++)
            {
                if (IsColliding(enemies[i], enemies[j]))
                {
                    enemies[i].HandleCollision(enemies[j]);
                    enemies[j].HandleCollision(enemies[i]);
                }
            }
        }

        // handle collisions between bullets and enemies 
        for (var i = 0; i < enemies.Count; i++)
        {
            for (var j = 0; j < bullets.Count; j++)
            {
                if (IsColliding(enemies[i], bullets[j]))
                {
                    enemies[i].WasShot();
                    bullets[j].IsExpired = true;
                }
            }
        }

        // handle collisions between the player and enemies 
        for (var i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].IsActive && IsColliding(PlayerShip.Instance, enemies[i]))
            {
                KillPlayer();
                break;
            }
        }

        // handle collisions with black holes 
        for (var i = 0; i < blackHoles.Count; i++)
        {
            for (var j = 0; j < enemies.Count; j++)
            {
                if (enemies[j].IsActive && IsColliding(blackHoles[i], enemies[j]))
                {
                    enemies[j].WasShot();
                }
            }
            for (var j = 0; j < bullets.Count; j++)
            {
                if (IsColliding(blackHoles[i], bullets[j]))
                {
                    bullets[j].IsExpired = true;
                    blackHoles[i].WasShot();
                }
            }
            if (IsColliding(PlayerShip.Instance, blackHoles[i]))
            {
                KillPlayer();
                break;
            }
        }
    }

    private static void KillPlayer()
    {
        PlayerShip.Instance.Kill();
        enemies.ForEach(x => x.WasShot());
        blackHoles.ForEach(x => x.Kill());
        EnemySpawner.Reset();
    }

    private static bool IsColliding(Entity a, Entity b)
    {
        var radius = a.Radius + b.Radius;
        return !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < radius * radius;
    }

    public static IEnumerable<Entity> GetNearbyEntities(Vector2 position, float radius)
    {
        return entities.Where(x => Vector2.DistanceSquared(position, x.Position) < radius * radius);
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        foreach (var entity in entities)
        {
            entity.Draw(spriteBatch);
        }
    }
}