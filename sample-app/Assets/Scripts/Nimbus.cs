using System;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable once HeapView.ObjectAllocation
public class Nimbus : MonoBehaviour {
    public string publisherSubDomain ="dev-sdk";
    public string publisherAPIKey = "d352cac1-cae2-4774-97ba-4e15c6276be0";
    public TextMeshProUGUI debugText;

    private AndroidJavaClass _nimbus;
    private AndroidJavaClass _helper;
    private const string NimbusPackage = "com.adsbynimbus.Nimbus";
    private static bool _inEditor;
    
    public static Nimbus Instance;

    private void Awake() {
#if UNITY_EDITOR
        _inEditor = true;
#endif
    }

    private void Start()
    {
        if (Instance == null) {
            InitNimbusSDK();
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    private void InitNimbusSDK() {
        Log("Calling Nimbus Behavior");
        var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        Debug.unityLogger.Log(player);
        var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
        _nimbus = new AndroidJavaClass(NimbusPackage);
        _helper = new AndroidJavaClass("com.nimbus.demo.UnityHelper");
            
        var logger = new AndroidJavaObject("com.adsbynimbus.Nimbus$Logger$Default", 0);
        _nimbus.CallStatic("addLogger", logger);
        _nimbus.CallStatic("initialize", activity, publisherSubDomain, publisherAPIKey);
        _nimbus.CallStatic("setTestMode", true);
        _helper.CallStatic("setApp", Application.identifier, "Nimbus Demo", "https://www.adsbynimbus.com");
        Instance = this;
    }


    private int _counter = 0;
    public void LogPublisherKey() {
       var key = _nimbus.CallStatic<string>("getPublisherKey");
       _counter++;
       debugText.text = $"{key} number of clicks {_counter}";
       Log($"Test Mode: Publisher Key {key}");
    }

    public void showBanner() {
        var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
        _helper.CallStatic("showBannerAd", activity);
    }

    public void showInterstitial() {
        var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
        _helper.CallStatic("showInterstitialAd", activity);
    }

    public void showRewardedVideoAd() {
        var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
        _helper.CallStatic("showRewardedVideoAd", activity);
    }

    public void LogApplicationIdentifier() {
        // pull in the RTB application bundle 
        var appBundle = $"application bundle {Application.identifier}";
        Debug.unityLogger.Log(appBundle);
        debugText.text = appBundle;
    }

    // ReSharper disable MemberCanBeMadeStatic.Local
    private void Log(Object message) {
        if (IsDebugOrEditor()) {
            Debug.Log(message);
        }
    }
    private void Log(string message) {
        if (IsDebugOrEditor()) {
            Debug.Log(message);
        }
    }
    private void Log(string message, Object obj) {
        if (IsDebugOrEditor()) {
            Debug.Log(message, obj);
        }
    }

    private static bool IsDebugOrEditor() {
        return Debug.isDebugBuild || _inEditor;
    }
    
}
