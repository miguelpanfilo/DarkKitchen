using System.Reflection;

namespace BusinessLogicsAbstractions;

public interface IPluginAssemblyLoader
{
    Assembly Load(string dllPath);
}
