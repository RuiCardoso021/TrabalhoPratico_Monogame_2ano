using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using TrabalhoPratico_Monogame_2ano.Collider;
using TrabalhoPratico_Monogame_2ano.Effects;
using TrabalhoPratico_Monogame_2ano.KeyBoard;

namespace TrabalhoPratico_Monogame_2ano.Components
{
    public class ClsTank
    {
        public Vector3 direction;
        public Vector3 normal;
        public Vector3 position;

        private const float _speed = 3f;
        private Model _tankModel;
        private ClsKeyboardManager _kb;
        private Keys[] _movTank;
        private List<ClsBullet> _bulletList;
        private ClsBullet _bullet;
        private Matrix[] _boneTransforms;
        private float _vel, _yaw, _yaw_cannon, _yaw_tower, _yaw_wheel, _yaw_hatch, _yaw_steer;
        private bool _allowShoot = true;
        private bool _moveTank;
        private ClsDust _dust;
        private ClsColliderBullet _colliderBullet;
        private ClsColliderTanks _colliderTank;
        private ModelBone _towerBone,
            _cannonBone,
            _leftBackWheelBone,
            _rightBackWheelBone,
            _leftFrontWheelBone,
            _rightFrontWheelBone,
            _leftSteerBone,
            _rightSteerBone,
            _hatchBone;
        private Matrix _turretTransform,
            _cannonTransform,
            _scale,
            _leftSteerDefaultTransform,
            _rightSteerDefaultTransform,
            _leftBackWheelBoneTransform,
            _rightBackWheelBoneTransform,
            _leftFrontWheelBoneTransform,
            _rightFrontWheelBoneTransform,
            _hatchBoneTransform;

        public ClsTank(GraphicsDevice device, Model modelo, Vector3 position, bool moveTank, Keys[] movTank)
        {
            this.position = position;
            _tankModel = modelo;
            _moveTank = moveTank;
            _kb = new ClsKeyboardManager();
            _movTank = movTank;
            _bulletList = new List<ClsBullet>();
            _colliderBullet = new ClsColliderBullet(4f);
            _colliderTank = new ClsColliderTanks(4f);
            _dust = new ClsDust(device);

            _leftBackWheelBone = _tankModel.Bones["l_back_wheel_geo"];
            _rightBackWheelBone = _tankModel.Bones["r_back_wheel_geo"];
            _leftFrontWheelBone = _tankModel.Bones["l_front_wheel_geo"];
            _rightFrontWheelBone = _tankModel.Bones["r_front_wheel_geo"];
            _leftSteerBone = _tankModel.Bones["l_steer_geo"];
            _rightSteerBone = _tankModel.Bones["r_steer_geo"];
            _towerBone = _tankModel.Bones["turret_geo"];
            _cannonBone = _tankModel.Bones["canon_geo"];
            _hatchBone = _tankModel.Bones["hatch_geo"];

            // Read bone default transforms
            _leftBackWheelBoneTransform = _leftBackWheelBone.Transform;
            _rightBackWheelBoneTransform = _rightBackWheelBone.Transform;
            _leftFrontWheelBoneTransform = _leftFrontWheelBone.Transform;
            _rightFrontWheelBoneTransform = _rightFrontWheelBone.Transform;
            _leftSteerDefaultTransform = _leftSteerBone.Transform;
            _rightSteerDefaultTransform = _rightSteerBone.Transform;
            _turretTransform = _towerBone.Transform;
            _cannonTransform = _cannonBone.Transform;
            _hatchBoneTransform = _hatchBone.Transform;

            // create array to store final bone transforms
            _boneTransforms = new Matrix[_tankModel.Bones.Count];

            _scale = Matrix.CreateScale(0.01f);
        }

        public void Update(GameTime gameTime, ClsTerrain terrain, Game1 game, ClsTank otherTank)
        {
            KeyboardState kb = Keyboard.GetState();
            Vector3 lastPosition = position;

            //movimento tank
            if (_moveTank) KeyboardMove(gameTime, kb, terrain, game);
            else ChaseEnemy();

            //limitar tank no terreno
            if (position.X >= 2 && position.X < terrain.w - 2 && position.Z >= 2 && position.Z < terrain.h - 2)
            {
                position.Y = terrain.GetY(position.X, position.Z);
                normal = terrain.GetNormal(position.X, position.Z);
            }
            else position = lastPosition;

            if (!_colliderTank.CollidedTank(position, otherTank.position))
                position = lastPosition;

            //shoot bullet to cannon
            ShootBullet(game, gameTime, kb, terrain, otherTank);

            //aplicar transformaçoes
            _towerBone.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(45f * _yaw_tower)) * _turretTransform;
            _cannonBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(-45f * _yaw_cannon)) * _cannonTransform;
            _leftBackWheelBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(45f * _yaw_wheel)) * _leftBackWheelBoneTransform;
            _rightBackWheelBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(45f * _yaw_wheel)) * _rightBackWheelBoneTransform;
            _leftFrontWheelBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(45f * _yaw_wheel)) * _leftFrontWheelBoneTransform;
            _rightFrontWheelBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(45f * _yaw_wheel)) * _rightFrontWheelBoneTransform;
            _hatchBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(-45f * _yaw_hatch)) * _hatchBoneTransform;
            _leftSteerBone.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(45f * _yaw_steer)) * _leftSteerDefaultTransform;
            _rightSteerBone.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(45f * _yaw_steer)) * _rightSteerDefaultTransform;

            // Appies transforms to bones in a cascade
            _tankModel.CopyAbsoluteBoneTransformsTo(_boneTransforms);
        }

        public void KeyboardMove(GameTime gameTime, KeyboardState kb, ClsTerrain terrain, Game1 game)
        {
            //aumentar velucidade com shift
            if (kb.IsKeyDown(_movTank[10])) _vel = 15f;
            else _vel = 5f;

            Vector3 posicaoRodaEsq = _boneTransforms[6].Translation;
            Vector3 posicaoRodaDir = _boneTransforms[2].Translation;

            if (kb.IsKeyDown(_movTank[1]))
            {
                _yaw_wheel = _yaw_wheel + MathHelper.ToRadians(_vel);
                _dust.Update(posicaoRodaEsq, gameTime, new Vector3(0.0f, -9.6f, 0.0f), terrain);
                _dust.Update(posicaoRodaDir, gameTime, new Vector3(0.0f, -9.6f, 0.0f), terrain);
            }
            if (kb.IsKeyDown(_movTank[3]))
            {
                _yaw_wheel = _yaw_wheel - MathHelper.ToRadians(_vel);
            }

            _yaw_hatch = _kb.Left_and_Right(_yaw_hatch, _speed, _movTank[4], _movTank[5]);                    //abre e fecha escutilha
            _yaw_tower = _kb.Left_and_Right(_yaw_tower, _speed, _movTank[6], _movTank[7]);                    //movimento da torre
            _yaw_cannon = _kb.Left_and_Right(_yaw_cannon, _speed, _movTank[8], _movTank[9]);                  //movimento do canhao
            //_yaw_steer = _kb.Left_and_Right(_yaw_steer, _speed, Keys.A, Keys.D);                            //movimento da direcao

            _yaw_cannon = _kb.LimitAngle(_yaw_cannon, 45f, 0f);                                     //lemitar rotacao canhao
            _yaw_hatch = _kb.LimitAngle(_yaw_hatch, 90f, 0f);                                       //lemitar rotacao escutilha
            _yaw_steer = 0f;
            //_yaw_steer = _kb.LimitAngle(_yaw_steer, 90f, 90f);                                       //lemitar rotacao direcao

            //movimentos do tanque
            _yaw = _kb.Left_and_Right(_yaw, _speed, _movTank[0], _movTank[2]);                                //movimento tank, esq, dir
            Matrix rotation = Matrix.CreateFromYawPitchRoll(_yaw, 0f, 0f);
            direction = Vector3.Transform(-Vector3.UnitZ, rotation);
            position = _kb.MovimentWithPosition(position, direction, _vel, _movTank[1], _movTank[3], gameTime);       //movimento tank frente e traz

            Vector3 right = Vector3.Cross(direction, normal);
            Vector3 correctedDirection = Vector3.Cross(normal, right);

            normal.Normalize();
            correctedDirection.Normalize();
            right.Normalize();

            rotation.Up = normal;
            rotation.Forward = correctedDirection;
            rotation.Right = right;

            Matrix translation = Matrix.CreateTranslation(position);
            _tankModel.Root.Transform = _scale * Matrix.CreateRotationY(MathHelper.Pi) * rotation * translation;
        }

        //shoot bullet to cannon
        public void ShootBullet(Game1 game, GameTime gameTime, KeyboardState kb, ClsTerrain terrain, ClsTank otherTank)
        {
            if (kb.IsKeyUp(_movTank[11]) && !_allowShoot)
            {
                _allowShoot = true;
            }

            if (kb.IsKeyDown(_movTank[11]) && _allowShoot)
            {
                _allowShoot = false;
                Vector3 cannonDirection = _boneTransforms[10].Backward;
                cannonDirection.Normalize();
                Vector3 cannonPosition = _boneTransforms[10].Translation;

                for (int i = 0; i < 1; i++)
                {
                    _bullet = new ClsBullet(game.Content.Load<Model>("Sphere"), cannonPosition, cannonDirection);
                    _bulletList.Add(_bullet);
                }
            }

            foreach (ClsBullet bullet in _bulletList)
                bullet.Update(gameTime);

            foreach (ClsBullet bullet in _bulletList.ToArray())
                if (bullet.Position.Y <= terrain.GetY(bullet.Position.X, bullet.Position.Z) || bullet.Position.Y < 0)
                    _bulletList.Remove(bullet);

            foreach (var bullet in _bulletList.ToArray())
            {
                if (_colliderBullet.Collide(bullet.Position, bullet.LastPosition, otherTank.position))
                {
                    _bulletList.Remove(_bullet);
                    Random random = new Random();
                    otherTank.position = new Vector3(random.Next(1, 60), 0, random.Next(1, 60));
                }
            }
        }

        public void ChaseEnemy()
        {
            float angle = 30f;
            Vector3 pos = new Vector3(30 * MathF.Cos(angle), 0, 30 * MathF.Sin(angle));
        }

        public void Draw(GraphicsDevice device, Matrix view, Matrix projection, Vector3 emissiveColor)
        {
            foreach (ModelMesh mesh in _tankModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true; // turn on the lighting subsystem.
                    effect.EmissiveColor = emissiveColor;
                    effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
                    effect.DirectionalLight0.Enabled = true;
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
                    effect.DirectionalLight0.SpecularColor = new Vector3(1.0f, 1.0f, 1.0f);
                    effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
                    effect.SpecularColor = new Vector3(0.0f, 0.0f, 0.0f);
                    effect.SpecularPower = 127;

                    Vector3 lightDirection = new Vector3(1.0f, -1f, 1f);
                    lightDirection.Normalize();
                    effect.DirectionalLight0.Direction = lightDirection;
                    effect.EnableDefaultLighting();
                    effect.World = _boneTransforms[mesh.ParentBone.Index];
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                }

                mesh.Draw();
            }

            if (_bulletList.Count > 0)
                foreach (ClsBullet bullet in _bulletList)
                    bullet.Draw();
          

            _dust.Draw(device);
        }
    }
}