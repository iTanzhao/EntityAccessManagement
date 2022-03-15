using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace EntityAccessManagement
{
    // Do not forget to update version number and author (company attribute) in AssemblyInfo.cs class
    // To generate Base64 string for Images below, you can use https://www.base64-image.de/
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "Entity Access Management"),
        ExportMetadata("Description", "Manage role permissions in the dimension of entity"),
        // Please specify the base64 content of a 32x32 pixels image
        ExportMetadata("SmallImageBase64", "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"),
        // Please specify the base64 content of a 80x80 pixels image
        ExportMetadata("BigImageBase64", "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"),
        ExportMetadata("BackgroundColor", "Lavender"),
        ExportMetadata("PrimaryFontColor", "Black"),
        ExportMetadata("SecondaryFontColor", "Gray")]
    public class MyPlugin : PluginBase
    {
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new MyPluginControl();
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        public MyPlugin()
        {
            // If you have external assemblies that you need to load, uncomment the following to 
            // hook into the event that will fire when an Assembly fails to resolve
            // AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolveEventHandler);
        }

        /// <summary>
        /// Event fired by CLR when an assembly reference fails to load
        /// Assumes that related assemblies will be loaded from a subfolder named the same as the Plugin
        /// For example, a folder named Sample.XrmToolBox.MyPlugin 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            Assembly loadAssembly = null;
            Assembly currAssembly = Assembly.GetExecutingAssembly();

            // base name of the assembly that failed to resolve
            var argName = args.Name.Substring(0, args.Name.IndexOf(","));

            // check to see if the failing assembly is one that we reference.
            List<AssemblyName> refAssemblies = currAssembly.GetReferencedAssemblies().ToList();
            var refAssembly = refAssemblies.Where(a => a.Name == argName).FirstOrDefault();

            // if the current unresolved assembly is referenced by our plugin, attempt to load
            if (refAssembly != null)
            {
                // load from the path to this plugin assembly, not host executable
                string dir = Path.GetDirectoryName(currAssembly.Location).ToLower();
                string folder = Path.GetFileNameWithoutExtension(currAssembly.Location);
                dir = Path.Combine(dir, folder);

                var assmbPath = Path.Combine(dir, $"{argName}.dll");

                if (File.Exists(assmbPath))
                {
                    loadAssembly = Assembly.LoadFrom(assmbPath);
                }
                else
                {
                    throw new FileNotFoundException($"Unable to locate dependency: {assmbPath}");
                }
            }

            return loadAssembly;
        }
    }
}