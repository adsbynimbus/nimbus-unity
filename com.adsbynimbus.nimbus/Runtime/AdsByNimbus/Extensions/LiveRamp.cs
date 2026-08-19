using System;
using AdsByNimbus.Internal.Extensions;

namespace AdsByNimbus.Extensions
{
#if NIMBUS_ENABLE_LIVERAMP
    public class LiveRamp
    {
        private String _configId;
        private String _email;
        private Boolean _hasConsentForNoLegislation;

        /// <param name="configId">
        ///		Config ID provided by LiveRamp
        /// </param>
        /// <param name="email">
        ///		Email is the preferred method for identifying a user
        /// </param>
        /// <param name="hasConsentForNoLegislation">
        ///		Set to true if the user is not governed by consent laws (i.e CCPA/GDPR)
        ///		Refer to https://developers.liveramp.com/authenticatedtraffic-api/docs/init-best-practices#consent-requirements
        /// </param>
        LiveRamp(String configId, String email, Boolean hasConsentForNoLegislation)
        {
            _configId = configId;
            _email = email;
            _hasConsentForNoLegislation = hasConsentForNoLegislation;
        }

        /// <summary>
        ///     This method will apply LiveRamp to all future Nimbus Requests
        /// </summary>
        public void fetchEnvelopeAndApplyToNimbus()
        {
            NimbusLiveRampHelpers.initializeLiveRamp(_configId, _email, _hasConsentForNoLegislation);
        }
    }
#endif

}

