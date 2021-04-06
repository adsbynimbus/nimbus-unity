using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nimbus.Internal {
	/// <summary>
	/// NimbusLogger wraps the unity logger and turns on logging for development builds and the unity editor
	/// </summary>
	public class NimbusLogger : ILogger {
		private readonly bool _inEditor;

		public NimbusLogger() {
#if UNITY_EDITOR
			_inEditor = true;
#endif
		}
		
		public void LogFormat(LogType logType, Object context, string format, params object[] args) {
			if (!IsDebugOrEditor()) return;
			Debug.unityLogger.LogFormat(logType, context, format, args);
		}

		public void LogException(Exception exception, Object context) {
			if (!IsDebugOrEditor()) return;
			Debug.unityLogger.LogException(exception, context);
		}

		public bool IsLogTypeAllowed(LogType logType) {
			return Debug.unityLogger.IsLogTypeAllowed(logType);
		}

		public void Log(LogType logType, object message) {
			if (!IsDebugOrEditor()) return;
			Debug.unityLogger.Log(logType, message);
		}

		public void Log(LogType logType, object message, Object context) {
			if (!IsDebugOrEditor()) return;
			Debug.unityLogger.Log(logType, message, context);
		}

		public void Log(LogType logType, string tag, object message) {
			if (!IsDebugOrEditor()) return;
			Debug.unityLogger.Log(logType, tag, message);
		}

		public void Log(LogType logType, string tag, object message, Object context) {
			if (!IsDebugOrEditor()) return;
			Debug.unityLogger.Log(logType, tag, message, context);
		}

		public void Log(object message) {
			if (!IsDebugOrEditor()) return;
			Debug.unityLogger.Log(message);
		}

		public void Log(string tag, object message) {
			if (!IsDebugOrEditor()) return;
			Debug.unityLogger.Log(tag, message);
		}

		public void Log(string tag, object message, Object context) {
			if (!IsDebugOrEditor()) return;
			Debug.unityLogger.Log(tag, message,context);
		}

		public void LogWarning(string tag, object message) {
			if (!IsDebugOrEditor()) return;
			Debug.unityLogger.LogWarning(tag, message);
		}

		public void LogWarning(string tag, object message, Object context) {
			if (!IsDebugOrEditor()) return;
			Debug.unityLogger.LogWarning(tag, message, context);
		}

		public void LogError(string tag, object message) {
			if (!IsDebugOrEditor()) return;
			Debug.unityLogger.LogError(tag, message);
		}

		public void LogError(string tag, object message, Object context) {
			if (!IsDebugOrEditor()) return;
			Debug.unityLogger.LogError(tag, message, context);
		}

		public void LogFormat(LogType logType, string format, params object[] args) {
			if (!IsDebugOrEditor()) return;
			Debug.unityLogger.LogFormat(logType, format, args);
		}

		public void LogException(Exception exception) {
			if (!IsDebugOrEditor()) return;
			Debug.unityLogger.LogException(exception);
		}

		public ILogHandler logHandler
		{
			get => Debug.unityLogger.logHandler;
			set => Debug.unityLogger.logHandler = value;
		}
		
		public bool logEnabled {
			get => Debug.unityLogger.logEnabled;
			set => Debug.unityLogger.logEnabled = value;
		}

		public LogType filterLogType {
			get => Debug.unityLogger.filterLogType;
			set => Debug.unityLogger.filterLogType = value;
		}
		
		private bool IsDebugOrEditor() {
			return Debug.isDebugBuild || _inEditor;
		}
	}
}