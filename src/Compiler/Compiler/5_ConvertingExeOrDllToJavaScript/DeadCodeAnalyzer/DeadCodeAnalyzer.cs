using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;

namespace JSIL.Compiler.Extensibility.DeadCodeAnalyzer
{
    public class DeadCodeAnalyzer
    {//: IAnalyzer {
        private readonly List<AssemblyDefinition> assemblyDefinitions;
        private DeadCodeInfoProvider deadCodeInfo;
        private Stopwatch stopwatchElapsed;
        private DeadCodeAnalyzerConfiguration Configuration;

        // ------ COMMENTED BY USERWARE: ------
        //private Compiler.Configuration compilerConfiguration;

        // ------ ADDED BY USERWARE: ------
        private string pathOfAssemblyThatContainsEntryPoint;

        public DeadCodeAnalyzer(string pathOfAssemblyThatContainsEntryPoint, bool isDisabled, string deadCodeEliminationWhiteList)
        {
            assemblyDefinitions = new List<AssemblyDefinition>();

            // ------ ADDED BY USERWARE: ------
            this.pathOfAssemblyThatContainsEntryPoint = pathOfAssemblyThatContainsEntryPoint;
            Configuration = new DeadCodeAnalyzerConfiguration(new Dictionary<string, object>());
            Configuration.DeadCodeElimination = !isDisabled;
            Configuration.WhiteList = new List<string>((deadCodeEliminationWhiteList ?? "").Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries));
            deadCodeInfo = new DeadCodeInfoProvider(Configuration);
        }

        // ------ COMMENTED BY USERWARE: ------

        //public void SetConfiguration(Compiler.Configuration configuration) {
        //    compilerConfiguration = configuration;

        //    if (configuration.AnalyzerSettings != null && configuration.AnalyzerSettings.ContainsKey("DeadCodeAnalyzer")) {
        //        Configuration = new Configuration((Dictionary<string, object>) configuration.AnalyzerSettings["DeadCodeAnalyzer"]);
        //    }
        //    else
        //    {
        //        Configuration = new Configuration(new Dictionary<string, object>());
        //    }

        //    if (Configuration.DeadCodeElimination.GetValueOrDefault(false)) {
        //        Console.WriteLine("// Using dead code elimination (experimental). Turn " +
        //                          "DeadCodeElimination off and report an issue if you encounter problems!");

        //        deadCodeInfo = new DeadCodeInfoProvider(Configuration);
        //    }
        //}

        public void AddAssemblies(AssemblyDefinition[] assemblies)
        {
            if (!Configuration.DeadCodeElimination.GetValueOrDefault(false))
                return;

            assemblyDefinitions.AddRange(assemblies);
        }

        public void Analyze(TypeInfoProvider typeInfoProvider)
        {
            if (!Configuration.DeadCodeElimination.GetValueOrDefault(false))
                return;

            deadCodeInfo.TypeInfoProvider = typeInfoProvider;

            stopwatchElapsed = new Stopwatch();
            stopwatchElapsed.Start();

            // --------- MODIFIED BY USERWARE ---------
            Mono.Cecil.MethodDefinition outputMethodDefinition;
            DotNetForHtml5.Compiler.ApplicationEntryPointFinderThatReturnsAMethodDefinition.GetEntryPoint(pathOfAssemblyThatContainsEntryPoint, assemblyDefinitions, out outputMethodDefinition);
            var foundEntrypoints = new List<MethodDefinition>();
            if (outputMethodDefinition != null)
                foundEntrypoints.Add(outputMethodDefinition);
            /*
            var foundEntrypoints = from assembly in assemblyDefinitions
                                   from modules in assembly.Modules
                                   where modules.EntryPoint != null
                                   select modules.EntryPoint;
            */

            deadCodeInfo.AddAssemblies(assemblyDefinitions);


            foreach (MethodDefinition method in foundEntrypoints)
            {
                deadCodeInfo.WalkMethod(method);
            }

            deadCodeInfo.ResolveVirtualMethodsCycle();

            stopwatchElapsed.Stop();
            Console.WriteLine("// Dead code analysis took {0} ms", stopwatchElapsed.ElapsedMilliseconds);
        }

        public bool MemberCanBeSkipped(MemberReference member)
        {
            if (!Configuration.DeadCodeElimination.GetValueOrDefault(false))
                return false;

            return !deadCodeInfo.IsUsed(member);
        }
    }
}