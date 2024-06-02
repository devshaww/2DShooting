
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioClipRefSO audioClipRefSO;

    public static SoundManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void PlaySound(AudioClip[] audioClips, Vector3 position, float volume = 1f) {
        PlaySound(audioClips[Random.Range(0, audioClips.Length)], position, volume);
    }

    public void PlayJumpSound(Vector3 position)
    {
        PlaySound(audioClipRefSO.jump, position);
    }

    public void PlayStampedeSound(Vector3 position)
    {
        PlaySound(audioClipRefSO.stampede, position);
    }

    public void PlayPlayerHurtSound(Vector3 position)
    {
        PlaySound(audioClipRefSO.playerHurt, position);
    }

    public void PlayGameLoseSound(Vector3 position) {
        PlaySound(audioClipRefSO.gameLose, position);
    }

    public void PlayGameWinSound(Vector3 position) {
        PlaySound(audioClipRefSO.gameWin, position);
    }

    public void PlayExplosionSound(Vector3 position)
    {
        PlaySound(audioClipRefSO.explosion, position);
    }

    public void PlayUpgradeSound(Vector3 position)
    {
        PlaySound(audioClipRefSO.upgrade, position);
    }

    public void PlayShootSound(Vector3 position)
    {
        PlaySound(audioClipRefSO.shoot, position, 0.5f);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f) {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }

}
