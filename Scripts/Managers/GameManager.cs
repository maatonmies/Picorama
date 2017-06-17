using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager manager;

	public GameObject transitionScreen;
	public Image whiteBG;
	public Image logo;
	public GameObject loadingBar;
	public Slider slider;

	private AudioSource[] music;
	private AudioSource mainTheme;
	private AudioSource levelMusic;
	private AudioSource bgWhirlIntroSound;
	private AsyncOperation async;

	private List<int>_unlockedLevels;
	private bool _allLevelsUnlocked = false;

	void  OnEnable ()
	{
		LevelManager.OnNoEnemiesLeft += StopBgMusic;
		CameraScriptedAnimations.OnCameraOutroAnimComplete += PlayMainTheme;
	}

	void  OnDisable ()
	{
		LevelManager.OnNoEnemiesLeft -= StopBgMusic;
		CameraScriptedAnimations.OnCameraOutroAnimComplete -= PlayMainTheme;
	}

	void Awake ()
	{
		manager = this;
		music = transform.GetChild (0).GetComponents<AudioSource> ();
		mainTheme = music [0];
		levelMusic = music [1];
		bgWhirlIntroSound = music [2];
		_unlockedLevels = new List<int> ();

		LoadGameProgress ();

		//if (! _unlockedLevels.Contains (3))
		//_unlockedLevels.Add (3);
			
		for (int i=3; i<33; i++) {
			
			_unlockedLevels.Add (i);
		}

		DontDestroyOnLoad (gameObject);

		LoadSceneWithTransition (1);
	}

	void OnLevelWasLoaded (int level)
	{
		if (level == 1) {

			if (!mainTheme.isPlaying) {
				bgWhirlIntroSound.Play ();
				mainTheme.volume = 1;
				mainTheme.Play ();
			}

		} else if (level == 2) {

			if (levelMusic.isPlaying)
				StartCoroutine (FadeOutMusic (levelMusic));
			
			if (!mainTheme.isPlaying) {
				mainTheme.volume = 1;
				mainTheme.Play ();
			}

		} else if (level < 31) {

			if (mainTheme.isPlaying)
				StartCoroutine (FadeOutMusic (mainTheme));

			if (!levelMusic.isPlaying) {
				bgWhirlIntroSound.Play ();
				levelMusic.pitch = 1;
				levelMusic.volume = 1;
				levelMusic.Play ();
			}

		} else {

			if (mainTheme.isPlaying) {
				StartCoroutine (FadeOutMusic (mainTheme));
				bgWhirlIntroSound.Play ();
			}
		}
	}

	private void StopBgMusic ()
	{
		levelMusic.Stop ();
	}

	private void PlayMainTheme ()
	{
		mainTheme.volume = 1;
		mainTheme.Play ();
	}

	private void LoadSceneWithTransition (int scene)
	{
		transitionScreen.SetActive (true);
		
		whiteBG.CrossFadeAlpha (0f, 0f, false);
		logo.CrossFadeAlpha (0f, 0f, false);
		
		whiteBG.CrossFadeAlpha (1f, 0.7f, false);
		logo.CrossFadeAlpha (1f, 0.7f, false);

		StartCoroutine (LoadSceneAsync (scene));
	}

	private IEnumerator LoadSceneAsync (int scene)
	{
		yield return new WaitForSeconds (0.7f);
		loadingBar.SetActive (true);

		async = Application.LoadLevelAsync (scene);

		while (!async.isDone) {
			slider.value = async.progress;
			yield return null;
		}

		loadingBar.SetActive (false);

		whiteBG.CrossFadeAlpha (0f, 0.7f, false);
		logo.CrossFadeAlpha (0f, 0.7f, false);
		yield return new WaitForSeconds (0.7f);

		transitionScreen.SetActive (false);
	}

	private IEnumerator FadeOutMusic (AudioSource sound)
	{
		for (int i= 10; i > 0; i --) {
			
			sound.volume -= 0.25f;
			yield return new WaitForSeconds (0.001f);
			
		}
		sound.Stop ();
	}

	public int GetLevelButtonInfo (int levelId)
	{
		if (_allLevelsUnlocked || _unlockedLevels.Contains (levelId + 1))
			return 2;
		else if (_unlockedLevels.Contains (levelId))
			return 1;
		else
			return 0;
	}

	public void UnlockLevel (int levelId)
	{
		if (levelId > 32)
			_allLevelsUnlocked = true;
		else if (!_unlockedLevels.Contains (levelId))
			_unlockedLevels.Add (levelId);

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/gameProgressData.dat");
		GameProgessData data = new GameProgessData ();
		data.allLevelsUnlocked = _allLevelsUnlocked;
		data.unlockedLevels = _unlockedLevels.ToArray ();
		bf.Serialize (file, data);
		file.Close ();
	}

	public void LoadGameProgress ()
	{
		if (File.Exists (Application.persistentDataPath + "/gameProgressData.dat")) {

			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/gameProgressData.dat", FileMode.Open);
			GameProgessData data = (GameProgessData)bf.Deserialize (file);
			file.Close ();

			_allLevelsUnlocked = data.allLevelsUnlocked;

			for (int i=0; i < data.unlockedLevels.Length; i++) {

				_unlockedLevels.Add (data.unlockedLevels [i]);
			}
		} 
	}

	[Serializable]
	class GameProgessData
	{
		public int[] unlockedLevels;
		public bool allLevelsUnlocked;
	}
}