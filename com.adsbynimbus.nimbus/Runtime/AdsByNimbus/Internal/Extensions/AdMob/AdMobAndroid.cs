using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AdsByNimbus;

[assembly:InternalsVisibleTo("nimbus.test")]
namespace Internal.Extensions.AdMob {
   internal class AdMobAndroid
   {
      private readonly AdMobAdUnit[] _adUnitIds;
      
      public AdMobAndroid(AdMobAdUnit[] adUnitIds)
      {
         _adUnitIds = adUnitIds;
      }

      public string[] GetAdUnitId(AdType type)
      {
         var ids = new List<string>();
         foreach (AdMobAdUnit adUnit in _adUnitIds)
         {
            if (adUnit.AdUnitType == type)
            {
               ids.Add(adUnit.AdUnitId);
            }
         }
         return ids.ToArray();
      }
   }

}