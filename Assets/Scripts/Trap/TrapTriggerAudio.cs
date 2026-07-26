using UnityEngine;

namespace CountdownTraps
{
    [System.Serializable]
    public sealed class TrapTriggerAudio
    {
        [Tooltip("Optional override. Leave empty to use the AudioSource on this trap object.")]
        [SerializeField] private AudioSource audioSource;

        public void Play(AudioSource fallbackAudioSource)
        {
            AudioSource source = audioSource != null ? audioSource : fallbackAudioSource;
            if (source == null || source.clip == null)
            {
                return;
            }

            source.PlayOneShot(source.clip);
        }
    }
}
