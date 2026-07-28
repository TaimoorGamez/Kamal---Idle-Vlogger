using UnityEngine;
using Core.DB.Variables;

namespace Core.Sfx
{
    [RequireComponent (typeof (AudioSource))]
    public class SoundClipPlayer : MonoBehaviour
    {
        [SerializeField] AudioSource AudioSourceToUse;
        [SerializeField] AudioClip ClipToPlay;
        [SerializeField] bool PlayOnEnable;

        private void OnEnable()
        {
            if (PlayOnEnable)
            {
                PlaySoundClip();
            }
        }

        public void PlaySoundClip()
        {
            if (DBVariablesHolder.Sound.Value == 1)
            {
                AudioSourceToUse.PlayOneShot(ClipToPlay);
            }
        }
    }
}
