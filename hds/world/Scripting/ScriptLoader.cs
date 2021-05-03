namespace hds.world.scripting
{
    public class ScriptLoader
    {
        public void LoadScripts()
        {
            //
            // CSharpCodeProvider prov = new CSharpCompilation(); ();
            // ICodeCompiler compiler = prov.CreateCompiler ();
            // CompilerParameters cp = new CompilerParameters (){
            // 	GenerateExecutable = false, // This would generate a dll with the assembly too
            // 	GenerateInMemory = true
            // };
            //
            // cp.ReferencedAssemblies.Add("system.dll");
            // cp.ReferencedAssemblies.Add("hds.exe");
            //
            // var list = Directory.GetFiles("./data/resources");
            //
            // Console.WriteLine("Found " + list.Length + " scripts");
            //
            // foreach (var filename in list)
            // {
            //     try
            //     {
            //         var classname = System.IO.Path.GetFileNameWithoutExtension(filename);
            //         var source = File.ReadAllText(filename);
            //         var cr = compiler.CompileAssemblyFromSource(cp, source);
            //
            //         if (cr.Errors.Count == 0)
            //         {
            //             // All seems ok
            //             Assembly a = cr.CompiledAssembly;
            //
            //             // Cheating here, as we use the interfacing way :P
            //             var myClass = a.CreateInstance(classname);
            //
            //             var healthCheck = myClass.GetType().GetMethod("Test").Invoke(myClass, null);
            //             Console.WriteLine("Script autotest result was:" + healthCheck);
            //
            //             Console.WriteLine("Script autoregistering: ");
            //             Dictionary<int, string> registeredMethods =
            //                 (Dictionary<int, string>) myClass.GetType().GetMethod("Register").Invoke(myClass, null);
            //
            //             foreach (var res in registeredMethods)
            //             {
            //                 Store.rpcScriptManager.AddEntryPoint(res.Key, classname, res.Value, myClass);
            //                 Console.WriteLine(String.Format("\t{0} --> {1}", res.Key, res.Value));
            //             }
            //         }
            //         else
            //         {
            //             foreach (var e in cr.Errors)
            //             {
            //                 Console.WriteLine("Error found:" + e); // Express mistakes to user
            //             }
            //         }
            //     }
            //     catch (Exception ex)
            //     {
            //         Console.WriteLine("Exception during loading... " + ex.Message);
            //         return;
            //     }
            // }
        }
    }
}