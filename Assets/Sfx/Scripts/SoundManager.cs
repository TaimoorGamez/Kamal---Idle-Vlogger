using UnityEngine;
using Core.Events;
using Core.DB.Variables;

namespace Core.Sfx
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] AudioClip BgMusic, BtnClick;
        [SerializeField] AudioClip[] EffectClips;

        AudioSource _bgSource = null, _btnSource = null, _effectSource = null;
        float _bgVolume = 0.35f;

        private void OnEnable()
        {
            SimpleEventsHolder.UpdateMusicStateEvent += UpdateBGMusicState;
            SimpleEventsHolder.UpdateSoundStateEvent += UpdateSoundState;
            SimpleEventsHolder.BtnPressSfxEvent += PlayBtnSound;
            SingleIntegerEventsHolder.SoundEffectEvent += PlaySoundEffect;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.UpdateMusicStateEvent -= UpdateBGMusicState;
            SimpleEventsHolder.UpdateSoundStateEvent -= UpdateSoundState;
            SimpleEventsHolder.BtnPressSfxEvent -= PlayBtnSound;
            SingleIntegerEventsHolder.SoundEffectEvent -= PlaySoundEffect;
        }

        private void Start()
        {
            UpdateBGMusicState();
            UpdateSoundState();
        }

        void UpdateBGMusicState()
        {
            if (DBVariablesHolder.Music.Value == 1)
            {
                _bgSource = gameObject.AddComponent<AudioSource>();
                PlayBGMusic();
            }
            else
            {
                if (_bgSource != null)
                {
                    Destroy(_bgSource);
                }
            }
        }

        void PlayBGMusic()
        {
            if (DBVariablesHolder.Music.Value == 1)
            {
                _bgSource.Stop();
                _bgSource.volume = _bgVolume;
                _bgSource.clip = BgMusic;
                _bgSource.loop = true;
                _bgSource.Play();
            }
        }

        void UpdateSoundState()
        {
            if (DBVariablesHolder.Sound.Value == 1)
            {
                CreateSoundSources();
            }
            else
            {
                if (_btnSource != null)
                {
                    Destroy(_btnSource);
                }

                if (_effectSource != null)
                {
                    Destroy(_effectSource);
                }
            }
        }
        void CreateSoundSources()
        {
            if (_btnSource == null)
            {
                _btnSource = gameObject.AddComponent<AudioSource>();
                _btnSource.clip = BtnClick;
            }

            if (_effectSource == null)
            {
                _effectSource = gameObject.AddComponent<AudioSource>();
            }
        }

        void PlayBtnSound()
        {
            if (DBVariablesHolder.Sound.Value == 1 && _btnSource != null)
            {
                _btnSource.Play();
            }
        }
    
        void PlaySoundEffect(int effectNum)
        {
            if (DBVariablesHolder.Sound.Value == 1)
            {
                if (_effectSource!= null && _effectSource.isPlaying)
                {
                    AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();
                    tempAudioSource.clip = EffectClips[effectNum];

                    // Play the sound
                    tempAudioSource.Play();

                    // Destroy the AudioSource after the sound has finished playing
                    Destroy(tempAudioSource, EffectClips[effectNum].length);
                }
                else if (_effectSource != null)
                {
                    _effectSource.PlayOneShot(EffectClips[effectNum]);
                }
            }
        }
    }
}
