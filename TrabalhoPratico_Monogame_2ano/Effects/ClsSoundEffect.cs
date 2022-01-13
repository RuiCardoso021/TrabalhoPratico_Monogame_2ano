using Microsoft.Xna.Framework.Audio;

namespace TrabalhoPratico_Monogame_2ano.Effects
{
    internal class ClsSoundEffect
    {
        private SoundEffectInstance _soundEffectInstance;
        private bool _loop;

        public ClsSoundEffect(SoundEffect sound, float volume)
        {
            _soundEffectInstance = sound.CreateInstance();
            _soundEffectInstance.Volume = volume;
            _loop = true;
        }

        public void Play()
        {
            if (_loop)
            {
                _soundEffectInstance.Play();
                _loop = false;
            }
        }

        public void PlayWithLoop()
        {
            _soundEffectInstance.Play();
        }
    }
}