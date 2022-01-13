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
        int _particlePerSecond;

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
                    Vector3 particlePosition = new Vector3(wheelPosition.X + (_radius * (float)_random.NextDouble() - (_radius / 2)), wheelPosition.Y, wheelPosition.Z + (_radius * (float)_random.NextDouble() - (_radius / 2)));
                    ClsParticleDust particle = new ClsParticleDust(particlePosition, new Vector3(_radius * (float)_random.NextDouble() - (_radius / 2), 6f, _radius * (float)_random.NextDouble() - (_radius / 2)));
                    _dustParticles.Add(particle);
                }
            }

            foreach (ClsParticleDust particulas in _dustParticles)
                particulas.Update(gameTime, gravity);
            

            foreach (ClsParticleDust particulas in _dustParticles.ToArray())
            {
                if (particulas.position.X >= 0 && particulas.position.X < terrain.w - 1 && particulas.position.Z >= 0 && particulas.position.Z < terrain.h - 1)
                    if (particulas.position.Y <= terrain.GetY(particulas.position.X, particulas.position.Z)) _dustParticles.Remove(particulas);
                else if (particulas.position.Y <= 0) _dustParticles.Remove(particulas);
            }
        }

        public void Draw(GraphicsDevice graphicsDevice)
        {
            if (_dustParticles.Count > 0)
            {
                _effect.View = ClsCamera.Instance.view;
                _effect.Projection = ClsCamera.Instance.projection;
                _effect.World = Matrix.Identity;

                VertexPositionColor[] vertices;
                vertices = new VertexPositionColor[2 * _dustParticles.Count];
                
                float size = 0.01f;

                for (int i = 0; i < _dustParticles.Count; i++)
                {
                    vertices[2 * i] = new VertexPositionColor(_dustParticles[i].position, Color.SandyBrown);
                    vertices[2 * i + 1] = new VertexPositionColor(_dustParticles[i].position + Vector3.Normalize(_dustParticles[i].velocity) * size, Color.SandyBrown);
                }

                _effect.CurrentTechnique.Passes[0].Apply();
                graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, _dustParticles.Count);
            }
        }
    }
}
