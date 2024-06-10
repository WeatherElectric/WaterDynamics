using System.Reflection;

[assembly: AssemblyTitle(WeatherElectric.WaterDynamics.Main.Description)]
[assembly: AssemblyDescription(WeatherElectric.WaterDynamics.Main.Description)]
[assembly: AssemblyCompany(WeatherElectric.WaterDynamics.Main.Company)]
[assembly: AssemblyProduct(WeatherElectric.WaterDynamics.Main.Name)]
[assembly: AssemblyCopyright("Developed by " + WeatherElectric.WaterDynamics.Main.Author)]
[assembly: AssemblyTrademark(WeatherElectric.WaterDynamics.Main.Company)]
[assembly: AssemblyVersion(WeatherElectric.WaterDynamics.Main.Version)]
[assembly: AssemblyFileVersion(WeatherElectric.WaterDynamics.Main.Version)]
[assembly:
    MelonInfo(typeof(WeatherElectric.WaterDynamics.Main), WeatherElectric.WaterDynamics.Main.Name,
        WeatherElectric.WaterDynamics.Main.Version,
        WeatherElectric.WaterDynamics.Main.Author, WeatherElectric.WaterDynamics.Main.DownloadLink)]
[assembly: MelonColor(255, 30, 152, 255)]

// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("Stress Level Zero", "BONELAB")]