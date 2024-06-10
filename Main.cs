namespace WeatherElectric.WaterDynamics;

public class Main : MelonMod
{
    internal const string Name = "WaterDynamics";
    internal const string Description = "Water Physics for BONELAB";
    internal const string Author = "SoulWithMae";
    internal const string Company = "Weather Electric";
#if DEBUG
    internal const string Version = "0.0.1";
#else
    internal const string Version = "1.0.0";
#endif
    internal const string DownloadLink = null;

    public override void OnInitializeMelon()
    {
        ModConsole.Setup(LoggerInstance);
        Preferences.Setup();
        Assets.LoadAssets();
        
        FieldInjection.Inject();
    }
}