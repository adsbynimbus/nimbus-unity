namespace Nimbus.Internal.Utility {
	public static class StaticMethod {
		public static bool InitializeInterceptor() {
#if NIMBUS_ENABLE_APS || NIMBUS_ENABLE_VUNGLE
				return true;
#endif
#pragma warning disable CS0162
			return false;
#pragma warning restore CS0162
		}
	}
}