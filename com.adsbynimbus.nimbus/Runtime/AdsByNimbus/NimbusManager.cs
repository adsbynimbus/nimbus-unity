using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdsByNimbus.Internal;
using AdsByNimbus.Internal.Extensions;
using Newtonsoft.Json.Linq;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class NimbusManager : MonoBehaviour
{
	[field: SerializeField] internal NimbusSDKConfiguration _configuration;

	private bool _isTheApplicationBackgrounded;
	public NimbusAPI NimbusPlatformAPI;
	private CancellationTokenSource _ctx;
	public AdEvents NimbusEvents;
	public static NimbusManager Instance;
	private bool _coppa;

	private void Awake()
	{
		if (_configuration == null) throw new Exception("The configuration object cannot be null");

		if (Instance == null)
		{
			Debug.unityLogger.logEnabled = _configuration.enableUnityLogs;
			NimbusPlatformAPI = NimbusPlatformAPI ?? new
#if UNITY_EDITOR
				Editor
#elif UNITY_ANDROID
			Android
#else
			IOS
#endif
				();
			NimbusEvents = new AdEvents();
			_ctx = new CancellationTokenSource();
			Instance = this;
			if (!_configuration.enableManualInitialization)
			{
				Nimbus.initialize(_configuration.publisherKey, _configuration.apiKey);
			}

			DontDestroyOnLoad(gameObject);
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}

	}

	private IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();
		AutoUnsubscribe();
		AutoSubscribe();
		SceneManager.sceneLoaded -= OnSceneLoaded;

		// SceneLoaded gets called BEFORE Start on app/game start
		SceneManager.sceneLoaded += OnSceneLoaded;
		yield return null;
	}

	// Listener for sceneLoaded
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		AutoUnsubscribe();
		AutoSubscribe();
	}

	private void OnDisable()
	{
		_ctx?.Cancel();
		AutoUnsubscribe();
	}

	private void OnApplicationPause(bool isPaused)
	{
		_isTheApplicationBackgrounded = isPaused;
	}

	[SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement")]
	[SuppressMessage("ReSharper", "InvertIf")]
	private static void AutoSubscribe()
	{
		if (Instance == null) return;
		var iAdEvents = FindObjectsOfType<MonoBehaviour>().OfType<IAdEvents>();
		foreach (var iAdEvent in iAdEvents)
		{
			Instance.NimbusEvents.OnAdLoaded += iAdEvent.OnAdLoaded;
			Instance.NimbusEvents.OnAdRendered += iAdEvent.OnAdWasRendered;
			Instance.NimbusEvents.OnAdError += iAdEvent.OnAdError;
			Instance.NimbusEvents.OnAdClicked += iAdEvent.OnAdClicked;
			Instance.NimbusEvents.OnAdCompleted += iAdEvent.OnAdCompleted;

			if (iAdEvent is IAdEventsExtended iAdEventExt)
			{
				Instance.NimbusEvents.OnAdImpression += iAdEventExt.OnAdImpression;
				Instance.NimbusEvents.OnAdDestroyed += iAdEventExt.OnAdDestroyed;
				Instance.NimbusEvents.OnAdRewardEarned += iAdEventExt.OnAdRewardEarned;
			}

			if (iAdEvent is IAdEventsVideoExtended iAdEventVideoExt)
			{
				Instance.NimbusEvents.OnVideoAdPaused += iAdEventVideoExt.OnVideoAdPaused;
				Instance.NimbusEvents.OnVideoAdResume += iAdEventVideoExt.OnVideoAdResume;
			}
		}
	}

	[SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement")]
	[SuppressMessage("ReSharper", "InvertIf")]
	private static void AutoUnsubscribe()
	{
		if (Instance == null) return;
		var iAdEvents = FindObjectsOfType<MonoBehaviour>().OfType<IAdEvents>();
		foreach (var iAdEvent in iAdEvents)
		{
			Instance.NimbusEvents.OnAdLoaded -= iAdEvent.OnAdLoaded;
			Instance.NimbusEvents.OnAdRendered -= iAdEvent.OnAdWasRendered;
			Instance.NimbusEvents.OnAdError -= iAdEvent.OnAdError;
			Instance.NimbusEvents.OnAdClicked -= iAdEvent.OnAdClicked;
			Instance.NimbusEvents.OnAdCompleted -= iAdEvent.OnAdCompleted;

			if (iAdEvent is IAdEventsExtended iAdEventExt)
			{
				Instance.NimbusEvents.OnAdImpression -= iAdEventExt.OnAdImpression;
				Instance.NimbusEvents.OnAdDestroyed -= iAdEventExt.OnAdDestroyed;
			}

			if (iAdEvent is IAdEventsVideoExtended iAdEventVideoExt)
			{
				Instance.NimbusEvents.OnVideoAdPaused -= iAdEventVideoExt.OnVideoAdPaused;
				Instance.NimbusEvents.OnVideoAdResume -= iAdEventVideoExt.OnVideoAdResume;
			}
		}
	}

	internal void InitializeNimbusSDK(string publisherKey, string apiKey)
	{
		if (!_configuration.sdkInitialized)
		{
			_configuration.sdkInitialized = true;
			_configuration.publisherKey = publisherKey;
			_configuration.apiKey = apiKey;
			NimbusPlatformAPI.InitializeSDK(_configuration);
		}
	}

	public void SetNimbusSDKConfiguration(NimbusSDKConfiguration configuration)
	{
		_configuration = configuration;
	}

	public NimbusSDKConfiguration GetNimbusConfiguration()
	{
		return _configuration;
	}

}