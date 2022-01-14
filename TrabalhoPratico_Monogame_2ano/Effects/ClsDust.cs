using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrabalhoPratico_Monogame_2ano.Components;

namespace TrabalhoPratico_Monogame_2ano.Effects
{
    class ClsDust
    {
        private BasicEffect _effect;
        private Random _random;
        private List<ClsParticleDust> _dustParticles;
        private float _radius;
        private int _particlePerSecond;

        public ClsDust(GraphicsDevice device)
        {
            _effect = new BasicEffect(device);
            _effect.LightingEnabled = false;
            _effect.VertexColorEnabled = true;
            _particlePerSecond = 3;
            _radius = 2;
            _random = new Random();
            _dustParticles = new List<ClsParticleDust>();
        }

        public void Update(Vector3 wheelPosition, GameTime gameTime, Vector3 gravity, ClsTerrain terrain, bool createParticles = false)
        {
            int pariclesToGenerate = (int)(Math.Round(_particlePerSecond * (float)gameTime.ElapsedGameTime.TotalMilliseconds));

            if (createParticles)
            {
                for (int i = 0; i < pariclesToGenerate; i++)
                {
                    float posX = wheelPosition.X + (_radius * (float)_random.NextDouble() - (_radius / 2));
                    float posZ = wheelPosition.Z + (_radius * (float)_random.NextDouble() - (_radius / 2));
                    Vector3 particlePosition = new Vector3(posX, wheelPosition.Y, posZ);
                    posX = _radius * (float)_random.NextDouble() - (_radius / 2);
                    posZ = _radius * (float)_random.NextDouble() - (_radius / 2);
                    ClsParticleDust particle = new ClsParticleDust(particlePosition, new Vector3(posX, 6f, posZ));
                    _dustParticles.Add(particle);
                }
            }

            foreach (ClsParticleDust particle in _dustParticles)
                particle.Update(gameTime, gravity);
            

            foreach (ClsParticleDust particle in _dustParticles.ToArray())
            {
                if (particle.Position.X >= 0 && particle.Position.X < terrain.W - 1 && particle.Position.Z >= 0 && particle.Position.Z < terrain.H - 1)
                    if (particle.Position.Y <= terrain.GetY(particle.Position.X, particle.Position.Z)) _dustParticles.Remove(particle);
                else if (particle.Position.Y <= 0) _dustParticles.Remove(particle);
            }
        }

        public void Draw(GraphicsDevice graphicsDevice)
        {
            if (_dustParticles.Count > 0)
            {
                _effect.View = ClsCamera.Instance.View;
                _effect.Projection = ClsCamera.Instance.Projection;
                _effect.World = Matrix.Identity;

                VertexPositionColor[] vertices;
                vertices = new VertexPositionColor[2 * _dustParticles.Count];
                
                float size = 0.01f;

                for (int i = 0; i < _dustParticles.Count; i++)
                {
                    vertices[2 * i] = new VertexPositionColor(_dustParticles[i].Position, Color.SandyBrown);
                    vertices[2 * i + 1] = new VertexPositionColor(_dustParticles[i].Position + Vector3.Normalize(_dustParticles[i].Velocity) * size, Color.SandyBrown);
                }

                _effect.CurrentTechnique.Passes[0].Apply();
                graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, _dustParticles.Count);
            }
        }
    }
}
