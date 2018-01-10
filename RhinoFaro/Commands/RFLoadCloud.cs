using System;
using Rhino;
using Rhino.Commands;

using FaroNET;

namespace RhinoFaro
{
    [System.Runtime.InteropServices.Guid("24deffa1-641a-45f7-a1ec-c72b3b157b2b")]
    public class RFLoadCloud : Command
    {
        static RFLoadCloud _instance;
        public RFLoadCloud()
        {
            _instance = this;
        }

        public static RFLoadCloud Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "RFLoadCloud"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Rhino.UI.OpenFileDialog dialog = new Rhino.UI.OpenFileDialog();
            dialog.Title = "Open pointcloud file";
            dialog.ShowOpenDialog();

            Rhino.Input.Custom.OptionInteger optStep = new Rhino.Input.Custom.OptionInteger(4, 1, 1000);
            Rhino.Input.Custom.GetOption getOpt = new Rhino.Input.Custom.GetOption();
            getOpt.AddOptionInteger("Resolution", ref optStep);
            getOpt.SetCommandPrompt("Loading options");

            string path = dialog.FileName;
            if (!System.IO.File.Exists(path))
            {
                Rhino.RhinoApp.WriteLine("Farhino: Failed to find file...");
                return Result.Failure;
            }

            getOpt.Get();

            Rhino.RhinoApp.WriteLine("Farhino: Step value: " + optStep.CurrentValue.ToString());
            RFContext.LoadScan(path, optStep.CurrentValue);

            return Result.Success;
        }
    }
}
