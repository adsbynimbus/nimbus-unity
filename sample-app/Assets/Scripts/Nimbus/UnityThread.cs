#define ENABLE_UPDATE_FUNCTION_CALLBACK
#define ENABLE_LATEUPDATE_FUNCTION_CALLBACK
#define ENABLE_FIXEDUPDATE_FUNCTION_CALLBACK

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable FieldCanBeMadeReadOnly.Local
// https://stackoverflow.com/questions/41330771/use-unity-api-from-another-thread-or-call-a-function-in-the-main-thread
namespace Nimbus {
    [DisallowMultipleComponent]
    public class UnityThread : MonoBehaviour
    {
        private static UnityThread _instance;
        private static List<Action> _actionQueuesUpdateFunc = new List<Action>();
        private List<Action> _actionCopiedQueueUpdateFunc = new List<Action>();
        private static List<Action> _actionQueuesLateUpdateFunc = new List<Action>();
        private List<Action> _actionCopiedQueueLateUpdateFunc = new List<System.Action>();
        private static volatile bool _noActionQueueToExecuteUpdateFunc = true;
        private static volatile bool _noActionQueueToExecuteLateUpdateFunc = true;
        private static List<Action> _actionQueuesFixedUpdateFunc = new List<Action>();
        private List<Action> _actionCopiedQueueFixedUpdateFunc = new List<System.Action>();
        private static volatile bool _noActionQueueToExecuteFixedUpdateFunc = true;
    
        public static void InitUnityThread(bool visible = false)
        {
            if (_instance != null) {
                return;
            }
            if (!Application.isPlaying) return;
            var obj = new GameObject("MainThreadExecuter");
            if (!visible) {
                obj.hideFlags = HideFlags.HideAndDontSave;
            }
            DontDestroyOnLoad(obj);
            _instance = obj.AddComponent<UnityThread>();
        }

        public void Awake() {
            DontDestroyOnLoad(gameObject);
        }
    
#if (ENABLE_UPDATE_FUNCTION_CALLBACK)
        public static void ExecuteCoroutine(IEnumerator action)
        {
            if (_instance != null)
            {
                ExecuteInUpdate(() => _instance.StartCoroutine(action));
            }
        }
        public static void ExecuteInUpdate(Action action)
        {
            if (action == null) {
                throw new ArgumentNullException(nameof(action));
            }
            lock (_actionQueuesUpdateFunc) {
                _actionQueuesUpdateFunc.Add(action);
                _noActionQueueToExecuteUpdateFunc = false;
            }
        }

        public void Update()
        {
            if (_noActionQueueToExecuteUpdateFunc)
            {
                return;
            }
        
            _actionCopiedQueueUpdateFunc.Clear();
            lock (_actionQueuesUpdateFunc)
            {
                _actionCopiedQueueUpdateFunc.AddRange(_actionQueuesUpdateFunc);
                _actionQueuesUpdateFunc.Clear();
                _noActionQueueToExecuteUpdateFunc = true;
            }
            foreach (var t in _actionCopiedQueueUpdateFunc) {
                t.Invoke();
            }
        }
#endif
    
#if (ENABLE_LATEUPDATE_FUNCTION_CALLBACK)
        public static void ExecuteInLateUpdate(System.Action action)
        {
            if (action == null) {
                throw new ArgumentNullException(nameof(action));
            }
            lock (_actionQueuesLateUpdateFunc) {
                _actionQueuesLateUpdateFunc.Add(action);
                _noActionQueueToExecuteLateUpdateFunc = false;
            }
        }
    
        public void LateUpdate()
        {
            if (_noActionQueueToExecuteLateUpdateFunc) {
                return;
            }
        
            _actionCopiedQueueLateUpdateFunc.Clear();
            lock (_actionQueuesLateUpdateFunc) {
                _actionCopiedQueueLateUpdateFunc.AddRange(_actionQueuesLateUpdateFunc);
                _actionQueuesLateUpdateFunc.Clear();
                _noActionQueueToExecuteLateUpdateFunc = true;
            }
            foreach (var t in _actionCopiedQueueLateUpdateFunc) {
                t.Invoke();
            }
        }
#endif
    
#if (ENABLE_FIXEDUPDATE_FUNCTION_CALLBACK)
        public static void ExecuteInFixedUpdate(System.Action action)
        {
            if (action == null) {
                throw new ArgumentNullException(nameof(action));
            }
            lock (_actionQueuesFixedUpdateFunc) {
                _actionQueuesFixedUpdateFunc.Add(action);
                _noActionQueueToExecuteFixedUpdateFunc = false;
            }
        }

        public void FixedUpdate()
        {
            if (_noActionQueueToExecuteFixedUpdateFunc) {
                return;
            }
            _actionCopiedQueueFixedUpdateFunc.Clear();
            lock (_actionQueuesFixedUpdateFunc) {
                _actionCopiedQueueFixedUpdateFunc.AddRange(_actionQueuesFixedUpdateFunc);
                _actionQueuesFixedUpdateFunc.Clear();
                _noActionQueueToExecuteFixedUpdateFunc = true;
            }
        
            foreach (var t in _actionCopiedQueueFixedUpdateFunc) {
                t.Invoke();
            }
        }
#endif
        public void OnDisable() {
            if (_instance == this) {
                _instance = null;
            }
        }
    }
}