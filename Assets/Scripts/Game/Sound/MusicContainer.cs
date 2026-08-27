using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization;

namespace Main.Sound
{
    [CreateAssetMenu(fileName = "MusicContainer", menuName = "Scriptable Objects/MusicContainer")]
    public class MusicContainer : ScriptableObject
    {
        public AudioClip audioClip;
        public string musicAuthor;
        [Space(10f)]
        public LocalizedString musicName;
        public LocalizedString musicDescription;
    }
}
