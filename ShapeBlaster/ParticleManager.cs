using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ShapeBlaster;

public class ParticleManager<T>
{
    public class Particle
    {
        public Texture2D Texture;
        public Vector2 Position;
        public float Orientation;
        public Vector2 Scale = Vector2.One;
        public Color Color;
        public float Duration;
        public float PercentLife = 1f;
        public T State;
    }

    private class CircularParticleArray
    {
        public int Start
        {
            get;
            set => field = value % list.Length;
        }

        public int Count { get; set; }
        public int Capacity => list.Length;
        private Particle[] list;

        public CircularParticleArray(int capacity)
        {
            list = new Particle[capacity];
        }

        public Particle this[int i]
        {
            get => list[(Start + i) % list.Length];
            set => list[(Start + i) % list.Length] = value;
        }
    }

    // This delegate will be called for each particle. 
    private Action<Particle> updateParticle;
    private CircularParticleArray particleList;

    public ParticleManager(int capacity, Action<Particle> updateParticle)
    {
        this.updateParticle = updateParticle;
        particleList = new CircularParticleArray(capacity);

        // Populate the list with empty particle objects, for reuse. 
        for (var i = 0; i < capacity; i++)
        {
            particleList[i] = new Particle();
        }
    }

    public void CreateParticle(Texture2D texture, Vector2 position, Color tint, float duration, float scale, T state, float theta = 0) =>
        CreateParticle(texture, position, tint, duration, new Vector2(scale), state, theta);

    public void CreateParticle(Texture2D texture, Vector2 position, Color tint, float duration, Vector2 scale, T state, float theta = 0)
    {
        Particle particle;
        if (particleList.Count == particleList.Capacity)
        {
            // if the list is full, overwrite the oldest particle, and rotate the circular list 
            particle = particleList[0];
            particleList.Start++;
        }
        else
        {
            particle = particleList[particleList.Count];
            particleList.Count++;
        }

        // Create the particle 
        particle.Texture = texture;
        particle.Position = position;
        particle.Color = tint;

        particle.Duration = duration;
        particle.PercentLife = 1f;
        particle.Scale = scale;
        particle.Orientation = theta;
        particle.State = state;
    }

    public void Update()
    {
        var removalCount = 0;
        for (var i = 0; i < particleList.Count; i++)
        {
            var particle = particleList[i];
            updateParticle(particle);
            particle.PercentLife -= 1f / particle.Duration;

            // sift deleted particles to the end of the list 
            Swap(particleList, i - removalCount, i);

            // if the particle has expired, delete this particle 
            if (particle.PercentLife < 0)
            {
                removalCount++;
            }
        }
        particleList.Count -= removalCount;
    }

    private static void Swap(CircularParticleArray list, int index1, int index2)
    {
        (list[index2], list[index1]) = (list[index1], list[index2]);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for (var i = 0; i < particleList.Count; i++)
        {
            var particle = particleList[i];

            var origin = new Vector2(particle.Texture.Width / 2, particle.Texture.Height / 2);
            spriteBatch.Draw(particle.Texture, particle.Position, null, particle.Color, particle.Orientation, origin, particle.Scale, 0, 0);
        }
    }
}