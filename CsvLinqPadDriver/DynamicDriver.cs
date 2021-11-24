using LINQPad.Extensibility.DataContext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace CsvLinqPadDriver
{
    internal class DynamicDriver : DynamicDataContextDriver
    {
#if DEBUG
        static DynamicDriver()
        {
            AppDomain.CurrentDomain.FirstChanceException += (sender, args) =>
            {
                if (args.Exception.StackTrace?.Contains("CsvLinqPadDriver") is true)
                    Debugger.Launch();
            };
        }
#endif

        public override string Name => "CSV to LINQ driver";

        public override string Author => "Drakir";

        public override string GetConnectionDescription(IConnectionInfo cxInfo)
            => "Transforming CSV to queryable objects using LINQ";

        /// <summary>
        /// The entry point which shows the connection dialog so that the user can select the desired folder containing
        /// the CSV files.
        /// </summary>
        public override bool ShowConnectionDialog(IConnectionInfo cxInfo, ConnectionDialogOptions dialogOptions)
        {
            var dialog = new FolderBrowserDialog
            {
                Description = "Select a folder containing the desired CSV files",
                UseDescriptionForTitle = true
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                cxInfo.DisplayName = dialog.SelectedPath;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads model from the provided path, then generates schema and emits the generated code using the model.
        /// </summary>
        /// <param name="cxInfo">Provides the path the model is read from</param>
        /// <param name="assemblyToBuild">The assembly that will contain the generated code</param>
        /// <param name="nameSpace">Namespace of the generated code</param>
        /// <param name="typeName">Name of the class containing the generated data context</param>
        /// <returns>Generated schema of build from the model</returns>
        public override List<ExplorerItem> GetSchemaAndBuildAssembly(
            IConnectionInfo cxInfo, AssemblyName assemblyToBuild, ref string nameSpace, ref string typeName)
        {
            string csvFilesPath = cxInfo.DisplayName;

            return new DriverResultBuilder(ref nameSpace, ref typeName)
                .ApplyModelFrom(csvFilesPath)
                .EmitInto(assemblyToBuild)
                .BuildSchema();
        }

        public static void WriteToLog(string message) => WriteToLog(message, "SunBlast.CsvLinqPadDriver.log");

        public override IEnumerable<string> GetNamespacesToAdd(IConnectionInfo cxInfo) =>
            new[] { "CsvLinqPadDriver.UserExtensions.Static" };
    }
}