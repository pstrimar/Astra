using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Sound 
{
    public string name;
    public AudioClip clip;

    [Range(0f,1f)]
    public float volume = 0.7f;
    [Range(0.5f,1.5f)]
    public float pitch = 1f;

    [Range(0f, 0.5f)]
    public float randomVolume = 0.1f;
    [Range(0f, 0.5f)]
    public float randomPitch = 0.1f;

    public bool loop = false;

    public AudioSource source;

    public void SetSource (AudioSource _source) 
    {
        source = _source;
        source.clip = clip;
        source.loop = loop;
    }

    public void Play()
    {
        source.volume = volume * (1 + Random.Range(-randomVolume/2f, randomVolume/2f));
        source.pitch = pitch * (1 + Random.Range(-randomPitch/2f, randomPitch/2f));
        source.Play();
    }

    public void Pause()
    {
        source.Pause();
    }

    public void PlayOneShot()
    {
        source.volume = volume * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f));
        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));
        source.PlayOneShot(source.clip);
    }

    public void Stop()
    {
        source.Stop();
    }
}

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] Sound[] sounds;

    public override void Awake() 
    {
        base.Awake();

        // Add all sounds as child gameobjects to audiomanager
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _go.transform.SetParent(this.transform);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
        }
        PlaySound("MenuMusic");
    }

    private void OnEnable()
    {
        Portal.onSceneLoaded += PlaySceneMusic;
        // Play level sound
        PlaySceneMusic(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDisable()
    {
        Portal.onSceneLoaded -= PlaySceneMusic;
        StopAllSounds();
    }

    public void PlaySound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Play();
                return;
            }
        }

        // no sound with _name
        Debug.LogWarning("AudioManage: Sound not found in list: " + _name);
    }

    public void PauseSound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Pause();
                return;
            }
        }

        // no sound with _name
        Debug.LogWarning("AudioManage: Sound not found in list: " + _name);
    }

    public void PlaySoundOnce(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                if (!sounds[i].source.isPlaying)
                {
                    sounds[i].PlayOneShot();
                }
                return;
            }
        }

        // no sound with _name
        Debug.LogWarning("AudioManage: Sound not found in list: " + _name);
    }

    public void StopSound(string _name) 
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Stop();
                return;
            }
        }
    }

    public void StopAllSounds()
    {
        foreach (Sound sound in sounds)
        {
            sound.Stop();
        }
    }

    private void PlaySceneMusic(int scene)
    {
        switch (scene)
        {
            case 0:
                StopAllSounds();
                PlaySound("MenuMusic");
                break;
            case 1:
                StopAllSounds();
                PlaySound("AboveGround");
                break;
            case 2:
                StopAllSounds();
                PlaySound("BasicCave");
                break;
            case 3:
                StopAllSounds();
                PlaySound("PoisonousCave");
                break;
            case 4:
                StopAllSounds();
                PlaySound("CrystalCave");
                break;
            default:
                StopAllSounds();
                PlaySound("MenuMusic");
                break;
        }
    }
}
