namespace WeatherElectric.WaterDynamics;

public class Main : MelonMod
{
    internal const string Name = "WaterDynamics";
    internal const string Description = "Water Physics for BONELAB";
    internal const string Author = "SoulWithMae";
    internal const string Company = "Weather Electric";
#if DEBUG
    internal const string Version = "1.0.1-DEBUG";
#else
    internal const string Version = "1.0.1";
#endif
    internal const string DownloadLink = "https://thunderstore.io/c/bonelab/p/SoulWithMae/WaterDynamics/";

    public override void OnInitializeMelon()
    {
        ModConsole.Setup(LoggerInstance);
        Preferences.Setup();
        Assets.LoadAssets();
        
#if DEBUG
        ModConsole.Warning("This is a debug build!");
#endif

        FieldInjection.Inject();
    }
}