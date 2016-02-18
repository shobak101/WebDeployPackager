using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.Deployment;

namespace WebDeployablePackage
{
    class Program
    {
        static void Main(string[] args)
        {
            SyncDirsExample(args[0], args[1]);
        }
        public static void SyncDirsExample(string source, string dest)
        {
            DeploymentSyncOptions syncOptions = new DeploymentSyncOptions();
            DeploymentBaseOptions sourceBaseOptions = new DeploymentBaseOptions();
            sourceBaseOptions.Trace += TraceEventHandler;
            sourceBaseOptions.TraceLevel = TraceLevel.Verbose;

            DeploymentBaseOptions destBaseOptions = new DeploymentBaseOptions();
            destBaseOptions.Trace += TraceEventHandler;
            destBaseOptions.TraceLevel = TraceLevel.Verbose;

            DeploymentProviderOptions destProviderOptions =
                new DeploymentProviderOptions(DeploymentWellKnownProvider.Package);

            using (DeploymentObject sourceObj =
                DeploymentManager.CreateObject(
                    DeploymentWellKnownProvider.IisApp, source, sourceBaseOptions))
            {
                DeploymentSyncParameter parameter = new DeploymentSyncParameter(
                    "AppPath",              // Name
                    "The path to the application.",  // Description
                    "Default Web Site/DemoSite",                // Default Value to set
                    "iisapp");                    // Tag

                DeploymentSyncParameterEntry entry = new DeploymentSyncParameterEntry(
                    DeploymentSyncParameterEntryKind.ProviderPath,      // Kind
                    DeploymentWellKnownProvider.IisApp.ToString(), // Scope
                    ".*",                                               // Match
                    null);                                              // Tag

                parameter.Add(entry);
                syncOptions.DeclaredParameters.Add(parameter);
                destProviderOptions.Path = dest;
                sourceObj.SyncTo(destProviderOptions, destBaseOptions, syncOptions);
            }
        }

        static void TraceEventHandler(object sender, DeploymentTraceEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        public static void SyncWithAddedParameters()
        {
            DeploymentSyncOptions syncOptions = new DeploymentSyncOptions();
            DeploymentBaseOptions sourceBaseOptions = new DeploymentBaseOptions();
            DeploymentBaseOptions destBaseOptions = new DeploymentBaseOptions();
            DeploymentProviderOptions destProviderOptions =
                new DeploymentProviderOptions(DeploymentWellKnownProvider.Auto);

            using (DeploymentObject sourceObj =
                DeploymentManager.CreateObject(
                    DeploymentWellKnownProvider.Manifest, @"C:\SourceManifest.xml", sourceBaseOptions))
            {
                DeploymentSyncParameter parameter = new DeploymentSyncParameter(
                    "TestParam",              // Name
                    "TestParam Description",  // Description
                    "destApp",                // Default Value to set
                    null);                    // Tag

                DeploymentSyncParameterEntry entry = new DeploymentSyncParameterEntry(
                    DeploymentSyncParameterEntryKind.ProviderPath,      // Kind
                    DeploymentWellKnownProvider.ContentPath.ToString(), // Scope
                    ".*",                                               // Match
                    null);                                              // Tag

                parameter.Add(entry);
                sourceObj.SyncParameters.Add(parameter);

                sourceObj.SyncTo(destProviderOptions, destBaseOptions, syncOptions);
            }

        }

    }
}
