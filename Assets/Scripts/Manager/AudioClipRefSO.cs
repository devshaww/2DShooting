
using UnityEngine;

[CreateAssetMenu()]
public class AudioClipRefSO : ScriptableObject
{
    public AudioClip jump;
    public AudioClip shoot;
    public AudioClip playerHurt;
    public AudioClip stampede;
    public AudioClip[] upgrade;
    public AudioClip[] explosion;
    public AudioClip gameLose;
    public AudioClip gameWin;
}
