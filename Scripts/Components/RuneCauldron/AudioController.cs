using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voltage.Witches.Models;
using Voltage.Witches.Exceptions;

public interface IAudioController
{
    void PlayBGMTrack(string bgmName);
    void StopAndClearTrack();
    string CurrentClip { get; }
}

public class AudioController : MonoBehaviour, IAudioController
{
    public const string DEFAULT_MUSIC = "K+C Happy Theme Looped (96kbps)";
	private IPlayerPreferences _playerPrefs;
	private static AudioController _instance = null;
	private Dictionary<string,AudioClip> _clipLibrary = new Dictionary<string,AudioClip>();
	private Dictionary<string, ClipConfigObject> _clipConfigs = new Dictionary<string,ClipConfigObject>();
	private AudioSource _myAudioSource = null;
	private AudioSource _myFXAudioSource = null;
	private AudioSource _myOneShotAudioSource = null;

	public float _bgmVolumeLimit = 0.65f;
    public float _fadeDuration = 1.0f;

	private float _loopingClipVolumeLimit = 0.75f;
	private List<AudioSource> _addedSources = new List<AudioSource>();
	//TODO Implement an FX volume limit
//	private float _fxVolumeLimit = 1.0f;

    private Coroutine _currentTransition;

	private enum EnabledState
	{
		ENABLED = 0,
		DISABLED = 1
	}

    public string CurrentClip { get; private set; }

	public static AudioController GetAudioController()
	{
		return _instance;
	}

	private void Awake()
	{
		if(_instance != null)
		{
			KillAudioController();
		}
		_instance = this;
		DontDestroyOnLoad(_instance);

		if((_playerPrefs == null) && (PlayerPreferences.GetInstance() != null))
		{
			_playerPrefs = PlayerPreferences.GetInstance();
		}

		_playerPrefs.SetBGMToggledCallback(HandleBGMToggledCallback);
		_playerPrefs.SetSFXToggledCallback (HandleSFXToggledCallback);
		_myAudioSource = gameObject.AddComponent<AudioSource>();
		_myAudioSource.volume = _bgmVolumeLimit;
		_myAudioSource.loop = true;
		_myAudioSource.playOnAwake = false;
		_myAudioSource.priority = 150;
		_myFXAudioSource = gameObject.AddComponent<AudioSource>();
		_myFXAudioSource.playOnAwake = false;
		_myFXAudioSource.volume = _loopingClipVolumeLimit;
		_myFXAudioSource.loop = true;
		_myFXAudioSource.priority = 100;
		_myOneShotAudioSource = gameObject.AddComponent<AudioSource>();
		_myOneShotAudioSource.playOnAwake = false;
		_myOneShotAudioSource.loop = false;
		_myOneShotAudioSource.priority = 0;

		AddAudioToLibrary();
		LoadAudioConfig();
		CheckAndAssignFromPrefs();
	}

	void CheckAndAssignFromPrefs()
	{
		var isSoundMuted = (_playerPrefs.SoundEnabled == false);
		var areSFXMuted = (_playerPrefs.SFXEnabled == false);

		ToggleBGM(isSoundMuted);
		ToggleFX(areSFXMuted);
	}

	public void HandleBGMToggledCallback(bool isEnabled)
	{
		switch(isEnabled)
		{
			case true:
				ToggleBGM(false);
				break;
			case false:
				ToggleBGM(true);
				break;
		}
	}

	public void HandleSFXToggledCallback(bool isEnabled)
	{
		switch(isEnabled)
		{
			case true:
				ToggleFX(false);
				break;
			case false:
				ToggleFX(true);
				break;
		}
	}

	public void ToggleBGM(bool isMuted)
	{
		_myAudioSource.mute = isMuted;
		if((!isMuted) && (_myAudioSource.clip != null))
		{
			_myAudioSource.Play();
		}
	}

	public void ToggleFX(bool isMuted)
	{
		_myFXAudioSource.mute = isMuted;
		_myOneShotAudioSource.mute = isMuted;
		for(int i = 0; i < _addedSources.Count; ++i)
		{
			var source = _addedSources[i];
			source.mute = isMuted;
		}
	}

	void LoadAudioConfig()
	{
		UnityEngine.Object configJsonRaw = Resources.Load<UnityEngine.Object>("Audio/Minigame_Audio_Config");
		JObject configJson = JObject.Parse (configJsonRaw.ToString());

		foreach(var clipJson in configJson["audioclips"])
		{
			ClipConfigObject config = JsonConvert.DeserializeObject<ClipConfigObject>(clipJson.ToString());
			if((!_clipConfigs.ContainsKey(config.clipname)) && (!_clipConfigs.ContainsValue(config)))
			{
				_clipConfigs[config.clipname] = config;
			}
		}
	}

	public void KillCurrentBGM()
	{
		_myAudioSource.Stop();
		_myAudioSource.clip = null;
	}

	public void PlayMinigameMusic()
	{
		if(_myAudioSource.clip != null)
		{
			_myAudioSource.Stop();
			_myAudioSource.clip = _clipLibrary["Cooking Minigame [96k MP3]"];
			_myAudioSource.Play();
		}
		else
		{
			_myAudioSource.clip = _clipLibrary["Cooking Minigame [96k MP3]"];
			_myAudioSource.Play();
			if(!_myAudioSource.loop)
			{
				_myAudioSource.loop = true;
			}
		}
	}

	public void SetBGMTrack(string bgmName)
	{
        if (!_clipLibrary.ContainsKey(bgmName))
        {
            throw new WitchesException(bgmName + "is not in the loaded audio clips");
        }

        _myAudioSource.clip = _clipLibrary[bgmName];
        CurrentClip = bgmName;
	}

    public void PlayBGMTrack(string bgmName)
	{
		if ((_myAudioSource.clip != null) && (CurrentClip != bgmName))
		{
            iTween.Stop(gameObject);
            if (_currentTransition != null)
            {
                StopCoroutine(_currentTransition);
                _currentTransition = null;
            }

            _currentTransition = StartCoroutine(FadeOutFadeIn(bgmName));
		}
        else if ((_myAudioSource.clip != null) && (CurrentClip == bgmName))
		{
            SetBGMTrack(bgmName);
			_myAudioSource.Play();
		}
		else
		{
            PlayNewTrack(bgmName);
		}

        CurrentClip = bgmName;
	}

    public void StopAndClearTrack()
    {
        if ((_myAudioSource.clip != null))
        {
            if (_currentTransition != null)
            {
                StopCoroutine(_currentTransition);
                _currentTransition = null;
            }

            _currentTransition = StartCoroutine(FadeOutAndRemove());
        }

        CurrentClip = string.Empty;
    }

	public void BGMVolumeAdjustment(float volume)
	{
		if(_myAudioSource != null)
		{
			if((_myAudioSource.volume + volume) < _bgmVolumeLimit)
			{
				_myAudioSource.volume += volume;
			}
			else
			{
				_myAudioSource.volume = _bgmVolumeLimit;
			}
		}
	}

	public void FXVolumeAdjustment(float volume)
	{
		if(_myFXAudioSource != null)
		{
			if((_myFXAudioSource.volume + volume) < _loopingClipVolumeLimit)
			{
				_myFXAudioSource.volume += volume;
			}
			else
			{
				_myFXAudioSource.volume = _loopingClipVolumeLimit;
			}
		}
	}

    private void PlayNewTrack(string newTrack)
    {
        if (!_clipLibrary.ContainsKey(newTrack))
        {
            throw new WitchesException("No track found in library for: " + newTrack);
        }

        _myAudioSource.clip = _clipLibrary[newTrack];
        if (_clipConfigs.ContainsKey(newTrack))
        {
            _bgmVolumeLimit = _clipConfigs[newTrack].volumeLimit;
        }
        _myAudioSource.loop = true;
        _myAudioSource.Play();
    }

    private IEnumerator FadeOutAndRemove()
    {
        float fadeLength = _fadeDuration / 2.0f;

        object[] fadeOutArgs = new object[]
        {
            "name", "transition",
            "from", _myAudioSource.volume,
            "to", 0.0f,
            "time", fadeLength,
            "easetype", iTween.EaseType.linear,
            "onupdate", (Action<object>)(value => _myAudioSource.volume = (float)value)
        };

        iTween.ValueTo(gameObject, iTween.Hash(fadeOutArgs));
        yield return new WaitForSeconds(fadeLength);

        _myAudioSource.Stop();
        _myAudioSource.clip = null;
    }

    private IEnumerator FadeOutFadeIn(string newTrack)
    {
        float fadeLength = _fadeDuration / 2.0f; // half per part

        object[] fadeOutArgs = new object[]
        {
            "name", "transition",
            "from", _myAudioSource.volume,
            "to", 0.0f,
            "time", fadeLength,
            "easetype", iTween.EaseType.linear,
            "onupdate", (Action<object>)(value => _myAudioSource.volume = (float)value)
        };
        
        iTween.ValueTo(gameObject, iTween.Hash(fadeOutArgs));
        yield return new WaitForSeconds(fadeLength);

        PlayNewTrack(newTrack);

        object[] fadeInArgs = new object[]
        {
            "name", "transition",
            "from", _myAudioSource.volume,
            "to", _bgmVolumeLimit,
            "time", fadeLength,
            "easetype", iTween.EaseType.linear,
            "onupdate", (Action<object>)(value => _myAudioSource.volume = (float)value)
        };
        iTween.ValueTo(gameObject, iTween.Hash(fadeInArgs));
        yield return new WaitForSeconds(fadeLength);
    }

	IEnumerator FadeOverToNewClip(string clipName, float volume)
	{
		while(_myFXAudioSource.clip != _clipLibrary[clipName])
		{
			_myFXAudioSource.volume -= 0.05f;
			yield return new WaitForSeconds(0.1f);
			if(_myFXAudioSource.volume <= 0.0f)
			{
				_myFXAudioSource.Stop();
				_myFXAudioSource.clip = _clipLibrary[clipName];
			}

		}

		if(volume > _loopingClipVolumeLimit)
		{
			volume = _loopingClipVolumeLimit;
		}

		_myFXAudioSource.volume = volume;
		_myFXAudioSource.Play();
	}

	public void PlayStaticLoopedClip(string clipName)
	{
		AudioSource newStaticClipSource = gameObject.AddComponent<AudioSource>();
		newStaticClipSource.loop = true;
		newStaticClipSource.playOnAwake = false;
		if(_clipConfigs.ContainsKey(clipName))
		{
			newStaticClipSource.volume = _clipConfigs[clipName].volumeLimit;
		}
		newStaticClipSource.clip = _clipLibrary[clipName];
		newStaticClipSource.mute = _myFXAudioSource.mute;
		newStaticClipSource.Play();
		_addedSources.Add(newStaticClipSource);
	}

	public void PlayClipFromString(string clipName, float volume, bool isLoop)
	{
		if(isLoop)
		{
			if((_myFXAudioSource.clip != _clipLibrary[clipName]) && (!_myFXAudioSource.isPlaying))
			{
				if(_clipConfigs.ContainsKey(clipName))
				{
					_loopingClipVolumeLimit = _clipConfigs[clipName].volumeLimit;
				}
				if((volume > _loopingClipVolumeLimit) || (volume < _loopingClipVolumeLimit))
				{
					volume = _loopingClipVolumeLimit;
				}
				_myFXAudioSource.volume = volume;
				_myFXAudioSource.clip = _clipLibrary[clipName];
				_myFXAudioSource.loop = isLoop;
				_myFXAudioSource.Play();
			}
			else if((_myFXAudioSource.clip != _clipLibrary[clipName]) && (_myFXAudioSource.isPlaying))
			{
				if(_clipConfigs.ContainsKey(clipName))
				{
					_loopingClipVolumeLimit = _clipConfigs[clipName].volumeLimit;
				}
				if((volume > _loopingClipVolumeLimit) || (volume < _loopingClipVolumeLimit))
				{
					volume = _loopingClipVolumeLimit;
				}
				StartCoroutine(FadeOverToNewClip(clipName,volume));
			}
			else
			{
				if(_clipConfigs.ContainsKey(clipName))
				{
					_loopingClipVolumeLimit = _clipConfigs[clipName].volumeLimit;
				}
				if((volume > _loopingClipVolumeLimit) || (volume < _loopingClipVolumeLimit))
				{
					volume = _loopingClipVolumeLimit;
				}
				_myFXAudioSource.volume = volume;
			}
		}
		else
		{
			if(_myOneShotAudioSource.clip == null)
			{
				_myOneShotAudioSource.clip = _clipLibrary[clipName];
				if(!_clipConfigs.ContainsKey(clipName))
				{
					_myOneShotAudioSource.volume = volume;
				}
				else
				{
					_myOneShotAudioSource.volume = _clipConfigs[clipName].volumeLimit;
				}
				_myOneShotAudioSource.Play();
				StartCoroutine(WaitToClearOneShot());
			}
			else if(_myOneShotAudioSource.clip != _clipLibrary[clipName])
			{
				float newVolume = volume;
				if(_clipConfigs.ContainsKey(clipName))
				{
					newVolume = _clipConfigs[clipName].volumeLimit;
				}
				StartCoroutine(AddNewSourceAndPlay(clipName,newVolume));
			}
			else
			{
				_myOneShotAudioSource.Play();
			}
		}
	}

	IEnumerator WaitToClearOneShot()
	{
		while(_myOneShotAudioSource.isPlaying)
		{
			yield return new WaitForSeconds(0.1f);
		}

		_myOneShotAudioSource.clip = null;
	}

	IEnumerator AddNewSourceAndPlay(string clipName, float volume)
	{
		AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
		newAudioSource.loop = false;
		newAudioSource.mute = _myOneShotAudioSource.mute;
		newAudioSource.volume = volume;
		newAudioSource.playOnAwake = false;
		newAudioSource.priority = 1;
		newAudioSource.clip = _clipLibrary[clipName];
		newAudioSource.Play();
		_addedSources.Add(newAudioSource);
		while(newAudioSource.isPlaying)
		{
			yield return new WaitForSeconds(0.1f);
		}

		newAudioSource.clip = null;
		_addedSources.Remove(newAudioSource);
		Destroy(newAudioSource);
	}

	IEnumerator FadeOutFXTracks()
	{
		if(_myOneShotAudioSource.isPlaying)
		{
			_myOneShotAudioSource.Stop();
			_myOneShotAudioSource.clip = null;
		}

		if(_addedSources.Count > 0)
		{
			for(int i = 0; i < _addedSources.Count; ++i)
			{
				if(_addedSources[i] != null)
				{
					_addedSources[i].Stop();
				}
			}
		}

		while(_myFXAudioSource.volume > 0.0f)
		{
			_myFXAudioSource.volume -= 0.05f;
			yield return new WaitForSeconds(0.1f);
			if(_myFXAudioSource.volume <= 0.0f)
			{
				_myFXAudioSource.Stop();
			}
		}

		_myFXAudioSource.clip = null;
		_myFXAudioSource.volume = 1.0f;
		
	}

	public void KillFXTracks()
	{
		StartCoroutine(FadeOutFXTracks());
	}

	void AddAudioToLibrary ()
	{
//        // TODO: Move this over to on-demand loading
		AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio");
		for(int i = clips.Length - 1; i > -1; --i)
		{
			if((!_clipLibrary.ContainsKey(clips[i].name)) && (!_clipLibrary.ContainsValue(clips[i])))
			{
				_clipLibrary[clips[i].name] = clips[i] as AudioClip;
			}
		}
	}

	public void KillAudioController()
	{
		Destroy(gameObject);
		_instance = null;
	}
}
