using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.IO;
using System.Linq;

namespace Tweaks
{
    public class Program
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
            var executable = @"Terraria.exe";
            var patched = @"Terraria-patched.exe";
            if (!File.Exists(executable))
            {
                throw new Exception($"Failed to read executable: {executable}");
            }

            File.Copy(executable, $"{executable}.bk", true);

            using (var module = ModuleDefinition.ReadModule(executable))
            {
                var method = module.GetType("Terraria.Program").Methods.First((MethodDefinition def) => def.Name == "StartForceLoad");

                var patchedInMethod = typeof(TweakLoader).GetMethod("Inject");

                var firstInstruction = method.Body.Instructions[0];
                if (firstInstruction.OpCode == OpCodes.Call)
                {
                    Console.WriteLine($"Already patched: {executable}");
                    return;
                }

                var il = method.Body.GetILProcessor();

                var call = il.Create(
                    OpCodes.Call,
                    module.ImportReference(typeof(TweakLoader).GetMethod("Inject"))
                );

                il.InsertBefore(firstInstruction, call);

                module.Write(patched);
            }

            File.Delete(executable);
            File.Move(patched, executable);

            Console.WriteLine($"Succesfully patched {executable}");
        }
    }
}