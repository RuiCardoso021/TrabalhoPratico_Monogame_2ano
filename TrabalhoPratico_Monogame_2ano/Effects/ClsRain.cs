using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrabalhoPratico_Monogame_2ano.Components;

namespace TrabalhoPratico_Monogame_2ano.Effects
{
    class ClsRain
    {
        private List<ClsParticleRain> _particles;
        private Random _random;
        private BasicEffect _effect;
        private GraphicsDevice _device;
        private float _heigth;
        private float _radius;
        private int _particlePerSecond;

        public ClsRain(GraphicsDevice device)
        {
            _random = new Random();
            _particles = new List<ClsParticleRain>();
            _device = device;
            _heigth = 100;
            _radius = 230;
            _particlePerSecond = 5;

            _effect = new BasicEffect(device);
            _effect.VertexColorEnabled = true;
        }


        private ClsParticleRain Generate()
        {
            //generate position
            float angle = (float)_random.NextDouble() * 2 * MathF.PI;
            float d = (float)_random.NextDouble() * _radius;
            Vector3 pos = new Vector3(d * MathF.Cos(angle), _heigth, d * MathF.Sin(angle));

            //random number * normal to calculate vel
            Vector3 vel = (float)_random.NextDouble() * Vector3.Down;
            vel.X = (float)_random.NextDouble();
            vel.Z = (float)_random.NextDouble();

            return new ClsParticleRain(vel, pos);
        }


        public void Update(GameTime gameTime, ClsTerrain terrain)
        {

            int pariclesToGenerate = (int)(Math.Round(_particlePerSecond * (float)gameTime.ElapsedGameTime.TotalMilliseconds));

            //create particula
            for (int i = 0; i < pariclesToGenerate; i++)
            {
                ClsParticleRain newParticle = Generate();
                if (terrain.TerrainLimit(newParticle.Position.X, newParticle.Position.Z))
                    _particles.Add(newParticle);
            }

            //remove particula if position y = 0
            for (int i = _particles.Count - 1; i >= 0; i--)
                if (_particles[i].Position.Y < 0 || !(terrain.TerrainLimit(_particles[i].Position.X, _particles[i].Position.Z)))
                    _particles.RemoveAt(i);

            //update to particulas
            foreach (ClsParticleRain particle in _particles)
                particle.Update(gameTime);

        }

        public void Draw(Matrix view, Matrix projection)
        {
            _effect.View = view;
            _effect.Projection = projection;

            VertexPositionColor[] vertices = new VertexPositionColor[2 * _particles.Count];

            float size = 0.5f;

            for (int i = 0; i < _particles.Count; i++)
            {
                vertices[2 * i] = new VertexPositionColor(_particles[i].Position, Color.LightBlue);
                vertices[2 * i + 1] = new VertexPositionColor(_particles[i].Position + Vector3.Normalize(_particles[i].Velocity) * size, Color.LightBlue);
            }
            _effect.CurrentTechnique.Passes[0].Apply();

            _device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, _particles.Count);
        }
    }
}
