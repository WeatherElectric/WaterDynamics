using Il2CppInterop.Runtime.Injection;

namespace WeatherElectric.WaterDynamics;

internal static class FieldInjection
{
    private static bool _injected;
    
    public static void Inject()
    {
        if (_injected) return;
        ClassInjector.RegisterTypeInIl2Cpp<FloatingObject>();
        ClassInjector.RegisterTypeInIl2Cpp<HandSwimmingController>();
        ClassInjector.RegisterTypeInIl2Cpp<WaterZone>();
        _injected = true;
    }
}