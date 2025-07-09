using BepInEx.Preloader.Core.Patching;
using Mono.Cecil;
using System.Runtime.CompilerServices;

namespace KeyForwarder;

[PatcherPluginInfo(GUID, Name, Version)]
public class Patcher : BasePatcher
{
    public const string GUID = "5acd3c53-d747-4883-95c4-ef4638088732";
    public const string Name = "me.art0007i.key_forwarder";
    public const string Version = "0.1.0";

    [TargetAssembly("FrooxEngine.dll")]
    public bool PatchFrooxEngine(ref AssemblyDefinition assembly, string filename)
    {
        var renderiteShared = Context.AvailableAssemblies["Renderite.Shared.dll"];

        var keyType = renderiteShared.MainModule.GetType("Renderite.Shared.Key");
        var importedKeyType = assembly.MainModule.ImportReference(keyType);

        var typeForwardedToAttrRef = assembly.MainModule.ImportReference(typeof(TypeForwardedToAttribute));
        var attrCtor = typeForwardedToAttrRef.Resolve()
            .Methods.First(m => m.IsConstructor && m.Parameters.Count == 1);
        var importedCtor = assembly.MainModule.ImportReference(attrCtor);

        var typeType = assembly.MainModule.ImportReference(typeof(Type));

        var attr = new CustomAttribute(importedCtor);
        attr.ConstructorArguments.Add(new CustomAttributeArgument(typeType, importedKeyType));

        assembly.CustomAttributes.Add(attr);

        return true;
    }
}