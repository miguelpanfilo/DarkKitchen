using System.Reflection;
using BusinessLogicsAbstractions;

namespace BusinessLogics;

public class PluginAssemblyLoader : IPluginAssemblyLoader
{
    public Assembly Load(string dllPath)
    {
        return Assembly.LoadFrom(dllPath);
    }
}
