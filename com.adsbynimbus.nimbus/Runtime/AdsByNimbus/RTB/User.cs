using System;
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
        [JsonIgnore]
        public int? age
        {
            set
            {
                if (value == null) return; 
                if (data == null || data.Length == 0)
                {
                    data = new[] { new Data("nimbus") };
                }
                var dataObj = data[0];
                var currentLen = dataObj.segment.Length;
                Array.Resize(ref dataObj.segment, currentLen + 1);
                dataObj.segment[currentLen] = new Segment("age", value.ToString());
            }
        }
        
        // Buyer-specific ID for the user as mapped by the exchange for the buyer.
        // Set to Facebook bidder token if integrating Facebook demand
        [CanBeNull] public string buyeruid;

        [JsonIgnore]
        // The gender of the user
        public Gender? gender
        {
            set
            {
                if (value == null) return; 
                if (data == null || data.Length == 0)
                {
                    data = new[] { new Data("nimbus") };
                }
                var dataObj = data[0];
                var currentLen = dataObj.segment.Length;
                Array.Resize(ref dataObj.segment, currentLen + 1);
                dataObj.segment[currentLen] = new Segment("gender", value);
            }
        }
        
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
        
        [CanBeNull] public Data[] data;

        public User(int? age = null, [CanBeNull] string buyeruid = null, 
            Gender? gender = null, [CanBeNull] string keywords = null, [CanBeNull] string customData = null)
        {
            this.age = age;
            this.gender = gender;
            this.buyeruid = buyeruid;
            this.keywords = keywords;
            this.customData = customData;
        }
    }
    
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Gender
    {
        [EnumMember(Value = "M")]
        // Male
        male,
        
        [EnumMember(Value = "F")]
        // Female
        female,
        
        [EnumMember(Value = "O")]
        // Other
        other
    
    }

    public class Data
    {
        public string name;

        public Segment[] segment;

        public Data(string name, Segment[] segment)
        {
            this.name = name;
            this.segment = segment;
        }

        public Data(string name)
        {
            this.name = name;
            this.segment = Array.Empty<Segment>();
        }
    }

    public class Segment
    {
        public string name;
        public string value;
        
        public Segment(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public Segment(string name, Gender? gender)
        {
            this.name = name;
            switch (gender)
            {
                case Gender.male:
                    value = "M";
                    break;
                case Gender.female:
                    value = "F";
                    break;
                case Gender.other:
                    value = "O";
                    break;
            }
        }
        
    }
}

