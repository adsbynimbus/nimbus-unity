using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Nimbus.Internal.Utility;
using UnityEngine;

[assembly:InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Extensions.AdMob {
   internal class AdMobAndroid
   {
      private const string NimbusAdMobPackage = "com.adsbynimbus.request.internal.NimbusRequestsAdMobInternal";

      private readonly bool _testMode;
      private readonly AdMobAdUnit[] _adUnitIds;
      private AdType _adUnitType;


      public AdMobAndroid(AdMobAdUnit[] adUnitIds)
      {
         _adUnitIds = adUnitIds;
      }
      

      public void InitializeNativeSDK()
      {
         //do nothing
      }

      private string GetAdUnitId(AdType type)
      {
         foreach (AdMobAdUnit adUnit in _adUnitIds)
         {
            if (adUnit.AdUnitType == type)
            {
               _adUnitType = type;
               return adUnit.AdUnitId;
            }
         }

         return "";
      }
   }

}