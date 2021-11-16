using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TrabalhoPratico_Monogame_2ano.Componentes
{
    class ClsTank
    {
        private const float speed = 5f;

        private Model _tankModel;
        private ModelBone _torreBone,
            _canhaoBone,
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
            _rightSteerDefaultTransform;
        private Matrix[] _boneTransforms;
        private float _stearAngle;
        public Vector3 _pos;
        private float _yaw;


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
            _torreBone = _tankModel.Bones["turret_geo"];
            _canhaoBone = _tankModel.Bones["canon_geo"];
            _hatchBone = _tankModel.Bones["hatch_geo"];

            // Read bone default transforms
            _leftSteerDefaultTransform = _leftSteerBone.Transform;
            _rightSteerDefaultTransform = _rightSteerBone.Transform;
            _turretTransform = _torreBone.Transform;
            _cannonTransform = _canhaoBone.Transform;

            // create array to store final bone transforms
            _boneTransforms = new Matrix[_tankModel.Bones.Count];

            _scale = Matrix.CreateScale(0.01f);
        }

        public void Update(GameTime gameTime, ClsTerreno terreno)
        {
            KeyboardState kb = Keyboard.GetState();
            Matrix steerRotation = Matrix.Identity;

            //limitar camara para nao inverter
            if (_stearAngle > MathHelper.ToRadians(45f))
                _stearAngle = MathHelper.ToRadians(45f);
            if (_stearAngle < MathHelper.ToRadians(-45f))
                _stearAngle = MathHelper.ToRadians(-45f);

            if (kb.IsKeyDown(Keys.A)) //esquerda
                _yaw = _yaw + MathHelper.ToRadians(speed);
            if (kb.IsKeyDown(Keys.D)) //direita
                _yaw = _yaw - MathHelper.ToRadians(speed);


            Matrix rotacao = Matrix.CreateFromYawPitchRoll(_yaw, 0f, 0f);
            Vector3 direction = Vector3.Transform(-Vector3.UnitZ, rotacao);


            if (kb.IsKeyDown(Keys.W)) //esquerda
                this._pos = this._pos + direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (kb.IsKeyDown(Keys.S)) //direita
                this._pos = this._pos - direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector3 normal;
            if (this._pos.X >= 0 && this._pos.X < terreno.w - 1 && this._pos.Z >= 0 && this._pos.Z < terreno.h - 1){
                this._pos.Y = terreno.GetY(this._pos.X, this._pos.Z);
                normal = terreno.GetNormal(this._pos.X, this._pos.Z);
            }else
            {
                this._pos.Y = 0.0f;
                normal = Vector3.Up;
            }

            Vector3 right = Vector3.Cross(direction, normal);
            Vector3 direcaoCorrigida = Vector3.Cross(normal, right);


            normal.Normalize();
            direcaoCorrigida.Normalize();
            right.Normalize();

            rotacao.Up = normal;
            rotacao.Forward = direcaoCorrigida;
            rotacao.Right = right;


            Matrix translacao = Matrix.CreateTranslation(this._pos);

            _tankModel.Root.Transform = _scale * Matrix.CreateRotationY(MathHelper.Pi) * rotacao * translacao;
            _torreBone.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(45f)) * _turretTransform;
            _canhaoBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(-45f)) * _cannonTransform;
            _leftSteerBone.Transform = steerRotation * _leftSteerDefaultTransform;
            _rightSteerBone.Transform = steerRotation * _rightSteerDefaultTransform;
            // Appies transforms to bones in a cascade
            _tankModel.CopyAbsoluteBoneTransformsTo(_boneTransforms);

        }

        public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in _tankModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
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
