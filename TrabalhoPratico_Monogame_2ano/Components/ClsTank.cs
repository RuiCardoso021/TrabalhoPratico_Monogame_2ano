using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        public Vector3 Direction;
        public Vector3 Normal;
        public Vector3 Position;
        public Vector3 CannonDirection;
        public Vector3 CannonPosition;
        public Matrix Rotation;

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
        private bool _autoMove = true;
        private Game1 game;
        private ClsSoundEffect _soundHeart;

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

        public ClsTank(GraphicsDevice device, Game1 game1, Model modelo, Vector3 position, bool moveTank, Keys[] movTank)
        {
            game = game1;
            this.Position = position;
            _tankModel = modelo;
            _moveTank = moveTank;
            _kb = new ClsKeyboardManager();
            _movTank = movTank;
            _bulletList = new List<ClsBullet>();
            _colliderBullet = new ClsColliderBullet(4f);
            _colliderTank = new ClsColliderTanks(4f);
            _dust = new ClsDust(device);
            _vel = 5f;
            _yaw = 0;
            _soundHeart = new ClsSoundEffect(game.Content.Load<SoundEffect>("SoundEffect/heart"), 1f);

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

            Rotation = Matrix.CreateFromYawPitchRoll(_yaw, 0f, 0f);
            Direction = Vector3.Transform(-Vector3.UnitZ, Rotation);

            _scale = Matrix.CreateScale(0.01f);
        }

        public void Update(GameTime gameTime, ClsTerrain terrain, ClsTank otherTank)
        {
            KeyboardState kb = Keyboard.GetState();
            Vector3 lastPosition = Position;

            //funcionalidade extra para tank inimigo (para defenir se e autonomo)
            if (_moveTank)
            {
                if (kb.IsKeyDown(Keys.O))
                    _autoMove = true;
                if (kb.IsKeyDown(Keys.P))
                    _autoMove = false;

                if (_autoMove) ChaseEnemy(otherTank, gameTime, terrain);
                else KeyboardMove(gameTime, kb, terrain, otherTank);
            }
            else KeyboardMove(gameTime, kb, terrain, otherTank);

            //limitar tank no terreno
            if (Position.X >= 2 && Position.X < terrain.W - 2 && Position.Z >= 2 && Position.Z < terrain.H - 2)
            {
                Position.Y = terrain.GetY(Position.X, Position.Z);
                Normal = terrain.GetNormal(Position.X, Position.Z);
            }
            else Position = lastPosition;

            if (!_colliderTank.Collide(Position, otherTank.Position))
                Position = lastPosition;

            CannonDirection = _boneTransforms[10].Backward;
            CannonDirection.Normalize();
            CannonPosition = _boneTransforms[10].Translation;
            //shoot bullet to cannon
            Shoot(gameTime, kb, terrain, otherTank);

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

        //metodo para movimento por teclado
        public void KeyboardMove(GameTime gameTime, KeyboardState kb, ClsTerrain terrain, ClsTank otherTank)
        {
            //efeito som se estiver a ser perseguido
            if (!LimitRadius(Position, otherTank.Position, 15f) && otherTank._autoMove && otherTank._moveTank)
                _soundHeart.PlayWithLoop();

            //aumentar velucidade com shift
            if (kb.IsKeyDown(_movTank[10])) _vel = 15f;
            else _vel = 5f;

            Vector3 posicaoRodaEsq = _boneTransforms[6].Translation;
            Vector3 posicaoRodaDir = _boneTransforms[2].Translation;

            if (kb.IsKeyDown(_movTank[1]))
            {
                _yaw_wheel = _yaw_wheel + MathHelper.ToRadians(_vel);
                _dust.Update(posicaoRodaEsq, gameTime, new Vector3(0.0f, -9.6f, 0.0f), terrain, true);
                _dust.Update(posicaoRodaDir, gameTime, new Vector3(0.0f, -9.6f, 0.0f), terrain, true);
            }
            else
            {
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
            Direction = Vector3.Transform(-Vector3.UnitZ, rotation);
            Position = _kb.MovimentWithPosition(Position, Direction, _vel, _movTank[1], _movTank[3], gameTime);       //movimento tank frente e traz

            Vector3 right = Vector3.Cross(Direction, Normal);
            Vector3 correctedDirection = Vector3.Cross(Normal, right);

            Normal.Normalize();
            correctedDirection.Normalize();
            right.Normalize();

            rotation.Up = Normal;
            rotation.Forward = correctedDirection;
            rotation.Right = right;

            Matrix translation = Matrix.CreateTranslation(Position);
            _tankModel.Root.Transform = _scale * Matrix.CreateRotationY(MathHelper.Pi) * rotation * translation;
        }

        private void Pursuite(ClsTank tank, GameTime gametime)
        {
            float aMax = 5f, velMax = 5f;

            Vector3 Vseek = tank.Position - Position;
            Vseek.Normalize();
            Vseek *= velMax;
            Direction.Normalize();

            Vector3 v = Direction * _vel;

            Vector3 a = (Vseek - v);
            a.Normalize();
            a *= aMax;

            v = v + a * (float)gametime.ElapsedGameTime.TotalSeconds;

            _vel = v.Length();
            Direction = v;
            Direction.Normalize();
        }

        //shoot bullet to cannon
        public void Shoot(GameTime gameTime, KeyboardState kb, ClsTerrain terrain, ClsTank otherTank)
        {
            //valida se está em automático
            if (_moveTank && _autoMove)
            {
                //valida o raio para começar a disparar e se tem permição de disparo
                if (!LimitRadius(otherTank.Position, Position, 10f) && _allowShoot)
                {
                    for (int i = 0; i < 1; i++)
                    {
                        _bullet = new ClsBullet(game.Content.Load<Model>("pokeball"), CannonPosition, CannonDirection);
                        _bulletList.Add(_bullet);
                        new ClsSoundEffect(game.Content.Load<SoundEffect>("SoundEffect/shot"), 0.3f).PlayWithLoop();
                    }
                    _allowShoot = false;
                }
            }
            else
            {
                if (kb.IsKeyUp(_movTank[11]) && !_allowShoot)
                {
                    _allowShoot = true;
                }

                if (kb.IsKeyDown(_movTank[11]) && _allowShoot)
                {
                    _allowShoot = false;

                    for (int i = 0; i < 1; i++)
                    {
                        _bullet = new ClsBullet(game.Content.Load<Model>("pokeball"), CannonPosition, CannonDirection);
                        _bulletList.Add(_bullet);
                        new ClsSoundEffect(game.Content.Load<SoundEffect>("SoundEffect/shot"), 0.3f).PlayWithLoop();
                    }
                }
            }

            foreach (ClsBullet bullet in _bulletList)
                bullet.Update(gameTime);

            foreach (ClsBullet bullet in _bulletList.ToArray())
            {
                if (bullet.Position.X >= 0 && bullet.Position.X < terrain.W - 1 && bullet.Position.Z >= 0 && bullet.Position.Z < terrain.H - 1)  //valida se a bola esta dentro terreno
                {
                    if (bullet.Position.Y <= terrain.GetY(bullet.Position.X, bullet.Position.Z) || bullet.Position.Y < 0)   //valida quando a bola bate no terreno
                    {
                        _bulletList.Remove(bullet);
                        if (_moveTank && _autoMove && !_allowShoot) _allowShoot = true; //permite disparar novamente apos remover bala se estiver modo automatico
                    }
                }
                else if (bullet.Position.Y < 0) //valida quando a bola obtem y menor que 0 (e se nao esta no terreno)
                {
                    _bulletList.Remove(bullet);
                    if (_moveTank && _autoMove && !_allowShoot) _allowShoot = true;
                }
            }

            foreach (var bullet in _bulletList.ToArray())
            {
                if (_colliderBullet.Collide(bullet.Position, bullet.LastPosition, otherTank.Position))
                {
                    _bulletList.Remove(_bullet);
                    if (_moveTank && _autoMove && !_allowShoot) _allowShoot = true; //permite disparar novamente apos remover bala se estiver modo automatico
                    otherTank.Position = ChangeNewPosition();
                    new ClsSoundEffect(game.Content.Load<SoundEffect>("SoundEffect/win"), 0.03f).PlayWithLoop();
                }
            }
        }

        //funcao recursiva para obter uma posicao fora de um raio.
        public Vector3 ChangeNewPosition()
        {
            Random random = new Random();
            Vector3 newPosition = new Vector3(random.Next(2, 50), 0, random.Next(2, 50));
            if (LimitRadius(newPosition, Position, 15f)) return newPosition;
            else return ChangeNewPosition();
        }

        //movimento autonomo
        public void ChaseEnemy(ClsTank otherTank, GameTime gameTime, ClsTerrain terrain)
        {
            //effect dust in wheels
            _dust.Update(_boneTransforms[6].Translation, gameTime, new Vector3(0.0f, -9.6f, 0.0f), terrain, true);
            _dust.Update(_boneTransforms[2].Translation, gameTime, new Vector3(0.0f, -9.6f, 0.0f), terrain, true);

            //rotation wheels
            _yaw_wheel += MathHelper.ToRadians(_vel);

            // valida o raio para começar a perseguição ou se chegou ao limite do terreno
            if (!LimitRadius(otherTank.Position, Position, 15f) ||
                !(Position.X >= 3 && Position.X < terrain.W - 3 && Position.Z >= 3 && Position.Z < terrain.H - 3))
                Pursuite(otherTank, gameTime);

            Position = Position + Direction * _vel * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector3 right = Vector3.Cross(Direction, Normal);
            Vector3 correctedDirection = Vector3.Cross(Normal, right);

            Normal.Normalize();
            correctedDirection.Normalize();
            right.Normalize();

            Rotation.Up = Normal;
            Rotation.Forward = correctedDirection;
            Rotation.Right = right;

            Matrix translation = Matrix.CreateTranslation(Position);
            _tankModel.Root.Transform = _scale * Matrix.CreateRotationY(MathHelper.Pi) * Rotation * translation;
        }

        //desenhar tank
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

        //limitar um raio sobre uma posição
        public bool LimitRadius(Vector3 position, Vector3 enimyPosition, float radius)
        {
            //calcular o ponto medio entre os dois tanks
            float x = position.X - enimyPosition.X;
            float y = position.Y - enimyPosition.Y;
            float z = position.Z - enimyPosition.Z;

            // calcular a distancia entre os dois tanks
            float distance = (float)Math.Sqrt(x * x + y * y + z * z);

            if (distance <= radius * 2)
                return false;
            else
                return true;
        }
    }
}