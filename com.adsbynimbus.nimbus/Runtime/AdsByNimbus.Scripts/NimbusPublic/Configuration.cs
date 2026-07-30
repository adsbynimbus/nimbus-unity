using System.Collections.Generic;
using Internal;
using Internal.Extensions;

namespace NimbusPublic
{
	public class Configuration
	{
			
		/// <summary>
		///		Unique session id for the current app session
		/// </summary>
		/// <param name="sessionId">
		///		string for the preferred session Id
		/// </param>

		public string SessionId
		{
			set => ConfigHelpers.SetSessionId(value);
		}

		/// <summary>
		///     If this inventory is subject to COPPA restrictions use this function to get the passed in RTB COPPA information for all Nimbus requests
		/// </summary>
		/// <param name="coppa">
		///		boolean depending on whether coppa restrictions are in place
		/// </param>
		public bool Coppa
		{
			set => ConfigHelpers.SetCoppa(value);
		}

		/// <summary>
		///		Details about the human user of the device; the advertising audience
		/// </summary>
		public User User
		{
			set => ConfigHelpers.SetUser(value);
		}

		/// <summary>
		///		Identifies the app to buyers (e.g., bundle ID, store URL, name, categories, publisher, privacy flags)
		/// </summary>
		public App App
		{
			set => ConfigHelpers.SetApp(value);
		}

		/// <summary>
		///		Block list of advertisers by their domains (e.g., “ford.com”)
		/// </summary>
		public string[] BlockedAdvertisingDomains
		{
			set => ConfigHelpers.SetBlockedAdvertisingDomains(value);
		}

		/// <summary>
		///		Set Request URL for bid requests
		/// </summary>
		public string RequestUrl
		{
			set => ConfigHelpers.SetRequestUrl(value);
		}

		/// <summary>
		///		Set additional request headers
		/// </summary>
		public Dictionary<string, string> AdditionalRequestHeaders
		{
			set => ConfigHelpers.SetAdditionalRequestHeaders(value);
		}

		/// <summary>
		///		Maximum time (in milliseconds) interceptors have to modify the request before it fires. Default is 500 milliseconds.
		/// </summary>
		public int InterceptorTimeout
		{
			set => ConfigHelpers.SetInterceptorTimeout(value);
		}

		/// <summary>
		///		Whether the video player should show the mute button. True by default
		/// </summary>
		public bool ShowMuteButton
		{
			set => ConfigHelpers.ShowMuteButton(value);
		}

		/// <summary>
		///		If enabled, only tap gestures are allowed for inline ads. Default is false
		///		(iOS only setting)
		/// </summary>
		public bool EnableSwipeProtection
		{
			set => ConfigHelpers.EnableSwipeProtection(value);
		}

		/// <summary>
		///		Sets if SKOverlay is enabled for all ad units (iOS only setting)
		/// </summary>
		public bool IsSkOverlayEnabledForAllUnits
		{
			set => ConfigHelpers.SetIsSkOverlayEnabledForAllUnits(value);
		}

		/// <summary>
		///		Set Verification Providers for Ad Viewability Tracking (OM SDK)
		/// </summary>
		/*
		 * /// Example of the methods in the Native Nimbus iOS SDK
		   public protocol VerificationProvider : Sendable {

		       func verificationMarkup(response: NimbusKit.NimbusResponse) -> String

		       func verificationResource(response: NimbusKit.NimbusResponse) -> NimbusKit.VerificationScriptResource?
		   }

		   public struct VerificationScriptResource {

		       public init?(url: URL, vendorKey: String?, parameters: String?)
		   }
		 */
		public VerificationProvider[] VerificationProviders
		{
			set => ConfigHelpers.SetVerificationProviders(value);
		}
	}
}