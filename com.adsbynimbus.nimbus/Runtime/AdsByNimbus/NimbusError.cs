using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdsByNimbus
{
    public class NimbusError
    {
        public int adUnitInstanceID;
        /// Broad, stable classification of what went wrong.
        ///
        /// `Reason` is intentionally small and stage-agnostic so callers can reliably switch on it.
        /// For additional context, use `stage`, `domain`, and (optionally) `detail`.
        [JsonConverter(typeof(StringEnumConverter))]
        public Reason reason;
        [JsonConverter(typeof(StringEnumConverter))]
        public Stage stage;
        public string domain;
        /// More details about error
        [CanBeNull] public string detail;
        /// A localized message describing what error occurred.
        [CanBeNull] public string errorDescription;
    }

    public enum Reason
    {
        /// No ad was available to serve.
        noFill,
        /// A requested operation is not supported in this context,
        /// e.g. rendering a native ad in a Unity Ads extension.
        unsupported,
        /// Required configuration was not provided by the publisher.
        configuration,
        /// The API was invoked in an invalid lifecycle state.
        invalidState,
        /// A failure occurred but could not be classified more specifically.
        failure
    }

    public enum Stage
    {
        /// The error occurred while building or validating the ad request.
        request,
        /// The error occurred while rendering the ad.
        render,
    }
}