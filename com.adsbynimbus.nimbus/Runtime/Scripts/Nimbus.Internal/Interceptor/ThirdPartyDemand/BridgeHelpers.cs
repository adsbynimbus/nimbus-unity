using System;
using UnityEngine;

namespace Nimbus.Internal.Interceptor.ThirdPartyDemand
{
    public class BridgeHelpers
    {
        public static string GetStringFromJavaFuture(String className, String methodName, object[] methodParams, long timeout)
        {
            AndroidJNI.AttachCurrentThread();
            var timeUnit = new AndroidJavaClass("java.util.concurrent.TimeUnit");
            var timeUnitMillis = timeUnit.CallStatic<AndroidJavaObject>("valueOf", "MILLISECONDS");
            var unityHelper = new AndroidJavaClass(className);
            var future = unityHelper.CallStatic<AndroidJavaObject>(methodName, methodParams);
            return future.Call<String>("get", timeout, timeUnitMillis);
        }
    }
}