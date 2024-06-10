using System.Reflection;

namespace WeatherElectric.WaterDynamics.Resources;

internal static class Assets
{
    private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
    private static AssetBundle _assetBundle;
    public static GameObject DefaultSplashParticles { get; private set; }
    
    public static void LoadAssets()
    {
        _assetBundle = AssetLoader.LoadEmbeddedAssetBundle(Assembly, "WeatherElectric.WaterDynamics.Resources.SplashPrefab.Windows.pack");
        DefaultSplashParticles = _assetBundle.LoadPersistentAsset<GameObject>("Assets/Splash.prefab");
    }
}

internal static class AssetLoader
{
    public static AssetBundle LoadEmbeddedAssetBundle(Assembly assembly, string name)
    {
        string[] manifestResources = assembly.GetManifestResourceNames();
        AssetBundle bundle = null;
        if (manifestResources.Contains(name))
        {
            ModConsole.Msg($"Loading embedded resource data {name}...", 1);
            using Stream str = assembly.GetManifestResourceStream(name);
            using MemoryStream memoryStream = new MemoryStream();

            str.CopyTo(memoryStream);
            ModConsole.Msg("Done!", 1);
            byte[] resource = memoryStream.ToArray();

            ModConsole.Msg($"Loading assetBundle from data {name}, please be patient...", 1);
            bundle = AssetBundle.LoadFromMemory(resource);
            ModConsole.Msg("Done!", 1);
        }
        return bundle;
    }
    
    public static T LoadPersistentAsset<T>(this AssetBundle assetBundle, string name) where T : UnityEngine.Object
    {
        UnityEngine.Object asset = assetBundle.LoadAsset(name);

        if (asset != null)
        {
            asset.hideFlags = HideFlags.DontUnloadUnusedAsset;
            return asset.TryCast<T>();
        }

        return null;
    }
}