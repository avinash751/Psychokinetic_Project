using System.Collections.Generic;
using UnityEngine;
using VInspector;

[CreateAssetMenu(fileName = "Sound Library", menuName = "Audio/Sound Library", order = 2)]
public class SoundLibrary : ScriptableObject
{
    public SerializedDictionary<string, SoundOptions> soundOptionsDictionary;
}
