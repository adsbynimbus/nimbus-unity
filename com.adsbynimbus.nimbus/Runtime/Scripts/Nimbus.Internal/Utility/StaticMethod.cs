namespace Nimbus.Internal.Utility {
	public static class StaticMethod {
		public static bool InitializeInterceptor() {
#if NIMBUS_ENABLE_APS || NIMBUS_ENABLE_VUNGLE || NIMBUS_ENABLE_META
				return true;
#endif
#pragma warning disable CS0162
			return false;
#pragma warning restore CS0162
		}
	}
}