using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TrabalhoPratico_Monogame_2ano.Componentes
{
    internal class ClsTank
    {
        private const float _speed = 3f;
        private Model _tankModel;

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
            _rightFrontWheelBoneTransform;

        private Matrix[] _boneTransforms;
        private float _vel, _yaw, _yaw_cannon, _yaw_tower, _yaw_wheel;

        public Vector3 correctedDirection;
        public Vector3 normal;
        public Vector3 _pos;

        public ClsTank(GraphicsDevice device, Model modelo, Vector3 position)
        {
            _pos = position;
            _tankModel = modelo;

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

            // create array to store final bone transforms
            _boneTransforms = new Matrix[_tankModel.Bones.Count];

            _scale = Matrix.CreateScale(0.01f);
        }

        public void Update(GameTime gameTime, ClsTerrain terrain)
        {
            KeyboardState kb = Keyboard.GetState();
            Matrix steerRotation = Matrix.Identity;
            Vector3 lastPosition = _pos;

            if (kb.IsKeyDown(Keys.LeftShift)) _vel = 15f;
            else _vel = 5f;

            if (kb.IsKeyDown(Keys.A)) //esquerda
                _yaw = _yaw + MathHelper.ToRadians(_speed);
            if (kb.IsKeyDown(Keys.D)) //direita
                _yaw = _yaw - MathHelper.ToRadians(_speed);

            Matrix rotation = Matrix.CreateFromYawPitchRoll(_yaw, 0f, 0f);
            Vector3 direction = Vector3.Transform(-Vector3.UnitZ, rotation);

            //movimento tank
            if (kb.IsKeyDown(Keys.W))
            { //esquerda
                _pos = _pos + direction * _vel * (float)gameTime.ElapsedGameTime.TotalSeconds;
                _yaw_wheel = _yaw_wheel + MathHelper.ToRadians(_vel);
            }
            if (kb.IsKeyDown(Keys.S))
            { //direita
                _pos = _pos - direction * _vel * (float)gameTime.ElapsedGameTime.TotalSeconds;
                _yaw_wheel = _yaw_wheel - MathHelper.ToRadians(_vel);
            }

            //lemitar tank no terreno
            if (_pos.X >= 0 && _pos.X < terrain.w - 1 && _pos.Z >= 0 && _pos.Z < terrain.h - 1)
            {
                _pos.Y = terrain.GetY(_pos.X, _pos.Z);
                normal = terrain.GetNormal(_pos.X, _pos.Z);
            }
            else _pos = lastPosition;

            //movimento da torre
            if (kb.IsKeyDown(Keys.Left)) //up
                _yaw_tower = _yaw_tower + MathHelper.ToRadians(_speed);
            if (kb.IsKeyDown(Keys.Right)) //down
                _yaw_tower = _yaw_tower - MathHelper.ToRadians(_speed);
            //movimento do canhao
            if (kb.IsKeyDown(Keys.Up)) //esquerda
                _yaw_cannon = _yaw_cannon + MathHelper.ToRadians(_speed);
            if (kb.IsKeyDown(Keys.Down)) //direita
                _yaw_cannon = _yaw_cannon - MathHelper.ToRadians(_speed);

            Vector3 right = Vector3.Cross(direction, normal);
            correctedDirection = Vector3.Cross(normal, right);

            normal.Normalize();
            correctedDirection.Normalize();
            right.Normalize();

            rotation.Up = normal;
            rotation.Forward = correctedDirection;
            rotation.Right = right;

            Matrix translation = Matrix.CreateTranslation(_pos);

            //lemitar rotacao canhao
            if (_yaw_cannon > MathHelper.ToRadians(40.0f))
                _yaw_cannon = MathHelper.ToRadians(40.0f);
            if (_yaw_cannon < -MathHelper.ToRadians(40.0f))
                _yaw_cannon = -MathHelper.ToRadians(40.0f);

            //aplicar transformaçoes
            _tankModel.Root.Transform = _scale * Matrix.CreateRotationY(MathHelper.Pi) * rotation * translation;
            _towerBone.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(45f * _yaw_tower)) * _turretTransform;
            _cannonBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(-45f * _yaw_cannon)) * _cannonTransform;
            _leftBackWheelBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(45f * _yaw_wheel)) * _leftBackWheelBoneTransform;
            _rightBackWheelBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(45f * _yaw_wheel)) * _rightBackWheelBoneTransform;
            _leftFrontWheelBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(45f * _yaw_wheel)) * _leftFrontWheelBoneTransform;
            _rightFrontWheelBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(45f * _yaw_wheel)) * _rightFrontWheelBoneTransform;
            _leftSteerBone.Transform = steerRotation * _leftSteerDefaultTransform;
            _rightSteerBone.Transform = steerRotation * _rightSteerDefaultTransform;

            // Appies transforms to bones in a cascade
            _tankModel.CopyAbsoluteBoneTransformsTo(_boneTransforms);
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
        }
    }
}