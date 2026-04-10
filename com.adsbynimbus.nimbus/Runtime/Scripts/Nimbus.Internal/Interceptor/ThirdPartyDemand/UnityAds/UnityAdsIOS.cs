using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.UnityAds {
	#if UNITY_IOS && NIMBUS_ENABLE_UNITY_ADS
	internal class UnityAdsIOS : IInterceptor, IProvider {
		private readonly string _gameID;

		public UnityAdsIOS(string gameID) {
			_gameID = gameID;
		}
		
		public JObject GetConfigObject()
		{
			var jObject = new JObject();
			jObject["demand"] = "Unity";
			jObject["gameId"] = _gameID;
			return jObject;
		}
	}
#endif
}