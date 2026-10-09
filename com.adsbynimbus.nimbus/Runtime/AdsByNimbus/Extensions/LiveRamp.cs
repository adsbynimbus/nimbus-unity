using System;
using AdsByNimbus.Internal.Extensions;

namespace AdsByNimbus.Extensions
{
#if NIMBUS_ENABLE_LIVERAMP
    public class LiveRamp
    {

        ///<summary>
        ///     Fetches an identity envelope and applies it to the Nimbus identity
        ///     configuration.
        ///
        ///     The stored envelope is reused where possible: one that is fresh is applied
        ///     directly, one that is stale but within its time-to-live is refreshed, and
        ///     anything older is replaced.  
        /// </summary>
        /// <param name="placementId">
        ///		The ATS placement ID from LiveRamp Console.
        /// </param>
        /// <param name="identifiers">
        ///		One or more user identifiers.
        /// </param>
        /// <param name="appId">
        ///		The bundle ID registered on the ATS placement. Defaults to
        ///     `Bundle.main.bundleIdentifier` on iOS and INCLUDE_BIT_ABOUT_ANDROID.
        ///     Pass this explicitly when the runtime bundle ID differs from the registered one,
        ///     as in build configurations that append a suffix, or in app extensions.
        /// </param>
        public static void initialize(String placementId, Identifier[] identifiers, String appId)
        {
            NimbusLiveRampHelpers.InitializeLiveRamp(placementId, identifiers, appId);
        }

        /// <summary>
        ///     Removes the stored envelope.
        ///
        ///     Call this method on logout.
        /// </summary>
        public static void clear()
        {
            NimbusLiveRampHelpers.Clear();
        }


        /// <summary>
        ///     A user identifier to exchange for an identity envelope.
        /// 
        ///     Pass raw user input; values are normalized and hashed before transmission.
        ///     Custom identifiers are the exception and are sent unhashed, as LiveRamp
        ///     expects.
        /// </summary>
        public class Identifier
        {
            public IdentifierType type;
            
            public enum IdentifierType
            {
                Email,
                Phone,
                Custom
            }
        }

        /// An email address, sent as SHA-256, SHA-1, and MD5.
        public class Email: Identifier
        {
            public string email;
            
            public Email(string email)
            {
                this.email = email;
                type = IdentifierType.Email;
            }
        }

        /// A phone number, sent as SHA-1. ATS supports phone-based matching for US numbers only.
        public class Phone: Identifier
        {
            public string phoneNumber;
            
            public Phone(string phoneNumber)
            {
                this.phoneNumber = phoneNumber;
                type = IdentifierType.Phone;
            }
        }
        
        /// A publisher-issued identifier, sent unhashed as `accountId:id`.
        public class Custom: Identifier
        {
            public string accountId;
            public string id;
            
            public Custom(string accountId, string id)
            {
                this.accountId = accountId;
                this.id = id;
                type = IdentifierType.Custom;
            }
        }
    }
#endif

}

