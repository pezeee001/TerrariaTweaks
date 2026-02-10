using System;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Tweaks
{
    public class TweakLoader
    {
        static void Main()
        {
            try
            {
                Patch();
            }
            catch (Exception ex) {
                Console.WriteLine($"Error during patching: {ex.ToString()}");
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void Patch()
        {
            //todo exclude unused mono.cecil.* dlls
            var executable = @"Terraria.exe";
            var patched = @"Terraria-patched.exe";
            if (!File.Exists(executable))
            {
                throw new Exception($"Failed to read executable: {executable}");
            }

            using (var module = ModuleDefinition.ReadModule(executable))
            {
                var method = module.GetType("Terraria.Program").Methods.First((MethodDefinition def) => def.Name == "StartForceLoad");

                var loadMethod = typeof(TweakLoader).GetMethod("Load");

                var firstInstruction = method.Body.Instructions[0];
                if (firstInstruction.OpCode == OpCodes.Call)
                {
                    Console.WriteLine($"Already patched: {executable}");
                    return;
                }

                File.Copy(executable, $"{executable}.bk", true);

                var il = method.Body.GetILProcessor();

                var call = il.Create(
                    OpCodes.Call,
                    module.ImportReference(loadMethod)
                );

                il.InsertBefore(firstInstruction, call);

                module.Write(patched);
            }

            File.Delete(executable);
            File.Move(patched, executable);

            Console.WriteLine($"Succesfully patched {executable}");
        }

        public static void Load()
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
                    MethodInfo execute = type.GetMethod("Execute");

                    execute.Invoke(null, null);
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