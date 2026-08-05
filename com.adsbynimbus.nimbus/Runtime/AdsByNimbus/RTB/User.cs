using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdsByNimbus.RTB
{
    /// <summary>
    /// This object contains information known or derived about the human user of the device (i.e., the audience for advertising).
    /// The user id is an exchange artifact and may be subject to rotation or other privacy policies.
    /// However, this user ID must be stable long enough to serve reasonably as the basis for frequency capping and retargeting.
    /// OpenRTB Section 3.2.20
    /// </summary>
    public class User
    {
        // The age of the user
        public int? age;
        
        // Buyer-specific ID for the user as mapped by the exchange for the buyer.
        // Set to Facebook bidder token if integrating Facebook demand
        [CanBeNull] public string buyeruid;
        
        
        // The gender of the user
        public Gender? gender;
        
        // Comma separated list of keywords, interests, or intent
        [CanBeNull] public string keywords;
        
        #if UNITY_ANDROID
            [JsonProperty("custom_data")]
        #endif
        /*
         * Optional feature to pass bidder data that was set in the exchange’s cookie.
         * The string must be in base85 cookie safe characters and be in any format.
         * Proper JSON encoding must be used to include “escaped” quotation marks
         */
        [CanBeNull] public string customData; 

        public User(int? age = null, [CanBeNull] string buyeruid = null, 
            Gender? gender = null, [CanBeNull] string keywords = null, [CanBeNull] string customData = null)
        {
            this.age = age;
            this.customData = customData;
            this.gender = gender;
            this.keywords = keywords;
        }
    }
    
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Gender
    {
        #if UNITY_IOS
            [EnumMember(Value = "male")]
        #else
            [EnumMember(Value = "M")]
        #endif
        // Male
        male,
        #if UNITY_IOS
            [EnumMember(Value = "female")]
        #else
            [EnumMember(Value = "F")]
        #endif
        // Female
        female,
        #if UNITY_IOS
            [EnumMember(Value = "other")]
        #else
            [EnumMember(Value = "O")]
        #endif
        // Other
        other
    
    }
}

