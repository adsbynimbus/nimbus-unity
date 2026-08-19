using System;
using AdsByNimbus.RTB;
using AdsByNimbus.RTB.Request;
using JetBrains.Annotations;
using UnityEditor;

namespace AdsByNimbus.Internal.Utility
{
    /// <summary>
    ///     Modifiers Added to an Ad on a per-request basis
    /// </summary>
    internal struct RequestModifiers
    {
        // Adds per-request app categories to the RTB request.
        public app? app;
        // A banner creative to be attached to the ad request.
        public banner? banner;
        // content url for single ad
        [CanBeNull] public content content;
        // Overrides the environment for a single ad.
        public environment? environment;
        // Adds device geolocation to the RTB request.
        public location? location;
        // Adds per-request user keywords to the RTB request.
        [CanBeNull] public user user;
        // Attaches a video creative to the ad request.
        public video? video;
        // Adds viewability information to the RTB request.
        public viewability? viewability;
        
    }
}