
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    public AudioClip background;
    public AudioClip jump;
    public AudioClip summon;
    public AudioClip coin;
    public AudioClip regainhealth;
    public AudioClip battery;
    public AudioClip orcDeath;
    public AudioClip dmgOrbshoot;
    public AudioClip damage;
    public AudioClip upgrade;
    public AudioClip tooPoor;
    public AudioClip Bossdeath;



    private void Start(){
        musicSource.clip = background;  //play background music
        musicSource.Play();

    }



    public void PlaySFX(AudioClip clip){


        SFXSource.PlayOneShot(clip);

    }
}
