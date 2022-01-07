using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using TrabalhoPratico_Monogame_2ano.Collider;
using TrabalhoPratico_Monogame_2ano.KeyBoard;

namespace TrabalhoPratico_Monogame_2ano.Components
{
    public class ClsTank
    {
        private const float _speed = 3f;
        private Model _tankModel;
        private ClsKeyboardManager _kb;
        private Keys[] _movTank;
        private List<ClsBullet> _bulletList;
        private ClsBullet _bullet;

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

        private Matrix[] _boneTransforms;
        private float _vel, _yaw, _yaw_cannon, _yaw_tower, _yaw_wheel, _yaw_hatch, _yaw_steer;

        public Vector3 direction;
        
        public Vector3 normal;
        public Vector3 _pos;

        ClsColliderBullet _colliderBullet;

        public ClsTank(GraphicsDevice device, Model modelo, Vector3 position, Keys[] movTank)
        {
            _pos = position;
            _tankModel = modelo;
            _kb = new ClsKeyboardManager();
            _movTank = movTank;
            _bulletList = new List<ClsBullet>();
            _colliderBullet = new ClsColliderBullet(4f);

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

        public void Update(GameTime gameTime, ClsTerrain terrain, Game1 game)
        {
            KeyboardState kb = Keyboard.GetState();
            Vector3 lastPosition = _pos;

            //aumentar velucidade com shift
            if (kb.IsKeyDown(Keys.LeftShift)) _vel = 15f;
            else _vel = 5f;

            _yaw_wheel = _kb.Left_and_Right(_yaw_wheel, _vel, _movTank[1], _movTank[3]);                      //movimenta as rodas
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
            _pos = _kb.MovimentWithPosition(_pos, direction, _vel, _movTank[1], _movTank[3], gameTime);       //movimento tank frente e traz

            //limitar tank no terreno
            if (_pos.X >= 2 && _pos.X < terrain.w - 2 && _pos.Z >= 2 && _pos.Z < terrain.h - 2)
            {
                _pos.Y = terrain.GetY(_pos.X, _pos.Z);
                normal = terrain.GetNormal(_pos.X, _pos.Z);
            }
            else _pos = lastPosition;

            //shoot bullet to cannon
            ShootBullet(game, gameTime, kb, terrain);

            Vector3 right = Vector3.Cross(direction, normal);
            Vector3 correctedDirection = Vector3.Cross(normal, right);

            normal.Normalize();
            correctedDirection.Normalize();
            right.Normalize();

            rotation.Up = normal;
            rotation.Forward = correctedDirection;
            rotation.Right = right;

            Matrix translation = Matrix.CreateTranslation(_pos);

            //aplicar transformaçoes
            _tankModel.Root.Transform = _scale * Matrix.CreateRotationY(MathHelper.Pi) * rotation * translation;
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

        public void ShootBullet(Game1 game, GameTime gameTime, KeyboardState kb, ClsTerrain terrain )
        {
            if (kb.IsKeyDown(Keys.Space))
            {
                Vector3 dirCanhao = _boneTransforms[10].Backward;
                dirCanhao.Normalize();
                Vector3 posCanhao = _boneTransforms[10].Translation;

                for (int i = 0; i < 1; i++)
                {
                    _bullet = new ClsBullet(game.Content.Load<Model>("Sphere"), posCanhao, dirCanhao);
                    _bulletList.Add(_bullet);
                }
            }

            //update bullet
            foreach (ClsBullet bullet in _bulletList)
                bullet.Update(gameTime);


            //remove bullet
            foreach (ClsBullet bullet in _bulletList.ToArray())
                if (bullet.posicao.Y <= terrain.GetY(bullet.posicao.X, bullet.posicao.Z) || bullet.posicao.Y < 0)
                    _bulletList.Remove(bullet);


            foreach (var bullet in _bulletList.ToArray())
            {

                if (_colliderBullet.CollidedTank(bullet.posicao, bullet.posicaoAntiga, game._tankEnemy._pos))
                {
                    _bulletList.Remove(_bullet);
                    game._tankEnemy._pos = new Vector3(58.0f, 0f, 58.0f);
                    //game.tank2.boidActive = true;
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

                // Draw each mesh of the model
                mesh.Draw();
            }

            if (_bulletList.Count > 0)
            {
                foreach (ClsBullet bullet in _bulletList)
                {
                    // Draw the model
                    bullet.Draw(device);
                }
            }
        }
    }
}