namespace Nimbus.Internal.Utility {
	public static class StaticMethod {
		public static bool InitializeInterceptor() {
#if NIMBUS_ENABLE_APS || NIMBUS_ENABLE_VUNGLE || NIMBUS_ENABLE_META || NIMBUS_ENABLE_ADMOB || NIMBUS_ENABLE_MINTEGRAL || NIMBUS_ENABLE_UNITY_ADS || NIMBUS_ENABLE_MOBILEFUSE || NIMBUS_ENABLE_MOLOCO
				return true;
#endif
#pragma warning disable CS0162
			return false;
#pragma warning restore CS0162
		}
	}
}