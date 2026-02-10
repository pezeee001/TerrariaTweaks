using System;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace Tweaks
{
    public class TweakLoader
    {
        public static void Inject()
        {
            string tweaksPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tweaks");

            if (!Directory.Exists(tweaksPath))
            {
                FileLog.Debug($"Tweaks folder not found: {tweaksPath}");
                return;
            }

            string[] dllFiles = Directory.GetFiles(tweaksPath, "*.dll");

            foreach (string dllFile in dllFiles)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(dllFile);
                    FileLog.Debug($"Loaded: {assembly.FullName}");
                    Type type = assembly.GetType("Tweaks.Patch");
                    object instance = Activator.CreateInstance(type);
                    MethodInfo execute = type.GetMethod("Execute");

                    execute.Invoke(instance, null);
                    FileLog.Debug($"Executed: {assembly.FullName}");
                }
                catch (Exception ex)
                {
                    var indent = FileLog.indentLevel;
                    while (ex != null)
                    {
                        FileLog.Debug($"Failed to load {Path.GetFileName(dllFile)}: {ex.Message}");
                        FileLog.Debug($"Stack trace: {ex.StackTrace}");
                        FileLog.ChangeIndent(2);
                        ex = ex.InnerException;
                    }
                    FileLog.indentLevel = indent;
                }
            }
        }
    }
}
