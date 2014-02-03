using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameSound
{
	public string Name;
	public AudioClip Clip;
}

public class Audio : MonoBehaviour 
{
	public static Audio Instance = null;
	public static AudioSource Music = null;
	public static AudioSource Sound = null;
	public GameSound[] Tracks;
	public GameSound[] Sounds;
	Dictionary<string, AudioClip> SoundClips = new Dictionary<string, AudioClip>();
	Dictionary<string, AudioClip> MusicClips = new Dictionary<string, AudioClip>();
	
	bool Paused = false;
	
	public int _CurrentTrack = 0;
	public int CurrentTrack
	{
		get { return _CurrentTrack; }
		set
		{
			_CurrentTrack = value;
			//Game.Instance.Data.Settings.CurrentTrack = value;
		}
	}
	
	public static bool _SoundMute = false;
	public static bool SoundMute
	{
		get { return _SoundMute; }
		set
		{
			_SoundMute = value;
			Sound.mute = value;
			//Game.Instance.Data.Settings.SoundMute = value;
		}
	}
	
	public static bool _MusicMute = false;
	public static bool MusicMute
	{
		get { return _MusicMute; }
		set
		{
			_MusicMute = value;
			Music.mute = value;
			//Game.Instance.Data.Settings.MusicMute = value;
		}
	}
	
	//============================================================================================================================================//
	void Awake()
	{
		if (Instance != null && Music != this)
		{           
			Destroy(this.gameObject);
		}
		else
		{
			var sources = GetComponents<AudioSource>();
			Music = sources[0];
			Sound = sources[1];
			Instance = this;
			
			DontDestroyOnLoad(this.gameObject);
			
			// Mute Music if Ipod is playing already //
			//if (InhumanIOS.IsMusicPlaying())
			//    Game.Instance.Data.Settings.MusicMute = true;
			
			//CurrentTrack = Game.Instance.Data.Settings.CurrentTrack; 
			//MusicMute = Game.Instance.Data.Settings.MusicMute;
			//SoundMute = Game.Instance.Data.Settings.SoundMute; 
			
			// Sound  Dictionaries //
			for (int i = 0; i < Sounds.Length; i++)
			{
				SoundClips.Add(Sounds[i].Name, Sounds[i].Clip);
			}
			
			for (int i = 0; i < Tracks.Length; i++)
			{
				MusicClips.Add(Tracks[i].Name, Tracks[i].Clip);
			}
		}
	}
	
	//============================================================================================================================================//
	public static void PlaySound(AudioClip clip)
	{
		if (Sound != null)
		{
			Sound.PlayOneShot(clip);
		}       
	}
	
	//============================================================================================================================================//
	public static void PlaySound(string sound)
	{
		if (Sound != null)
		{
			//print("Sound: " + sound);               
			Sound.PlayOneShot(Audio.Instance.SoundClips[sound]);
		}
	}
	
	//============================================================================================================================================//
	public static void PlayMusic(string song, bool restart)
	{
		if (Music != null)
		{
			if (song == "Next" || song == "Random" || Music.clip != Audio.Instance.MusicClips[song] || restart)
			{
                if (song == "Random")
                {
                    List<string> clips = new List<string>(Audio.Instance.MusicClips.Keys);
                    int index = Random.Range(0, clips.Count);
                    Music.clip = Audio.Instance.MusicClips[clips[index]];
                }
                else if (song == "Next")
                {
                    List<string> clips = new List<string>(Audio.Instance.MusicClips.Keys);               
                    Music.clip = Audio.Instance.MusicClips[clips[Audio.Instance.CurrentTrack]];
                    Audio.Instance.CurrentTrack++;
                    if (Audio.Instance.CurrentTrack >= clips.Count)
                        Audio.Instance.CurrentTrack = 0;
                }
                else
                {
                    Music.clip = Audio.Instance.MusicClips[song];          
                }
                
				print("Music: " + song);
				Music.Stop();
				Music.Play();
			}            
		}
	}
	
	//============================================================================================================================================//
	public void Destroy()
	{
		Music = null;
		Destroy(this.gameObject);
	}
	
	//============================================================================================================================================//
	void OnApplicationPause(bool paused)
	{
		Paused = paused;
	}
}