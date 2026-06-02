using System.Reflection;
using BusinessLogicsAbstractions;

namespace BusinessLogics;

public class PluginAssemblyLoader : IPluginAssemblyLoader
{
    public Assembly Load(string dllPath)
    {
        var shadowFolder =
            Path.Combine(
                Path.GetDirectoryName(dllPath)!,
                "plugins_shadow");

        Directory.CreateDirectory(shadowFolder);

        var shadowPath =
            Path.Combine(
                shadowFolder,
                $"{Guid.NewGuid()}_{Path.GetFileName(dllPath)}");

        File.Copy(
            dllPath,
            shadowPath,
            overwrite: true);

        return Assembly.LoadFrom(shadowPath);
    }
}
