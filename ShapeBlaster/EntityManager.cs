using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace ShapeBlaster;

static class EntityManager
{
    static List<Entity> entities = [];

    static bool isUpdating;
    static List<Entity> addedEntities = [];

    public static int Count => entities.Count;

    public static void Add(Entity entity)
    {
        if (!isUpdating)
        {
            entities.Add(entity);
        }
        else
        {
            addedEntities.Add(entity);
        }
    }

    public static void Update()
    {
        isUpdating = true;

        foreach (var entity in entities)
        {
            entity.Update();
        }

        isUpdating = false;

        foreach (var entity in addedEntities)
        {
            entities.Add(entity);
        }

        addedEntities.Clear();

        // remove any expired entities. 
        entities = [.. entities.Where(x => !x.IsExpired)];
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        foreach (var entity in entities)
        {
            entity.Draw(spriteBatch);
        }
    }
}