using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Input;
using Rhino.Input.Custom;

using FaroNET;

namespace RhinoFaro
{
    [System.Runtime.InteropServices.Guid("61ed636b-9bf7-4762-88cd-a080ffbc6cc7")]
    public class RFScanSettings : Command
    {
        public RFScanSettings()
        {
            Instance = this;
        }

        public static RFScanSettings Instance
        {
            get; private set;
        }

        public override string EnglishName
        {
            get { return "RFScanSettings"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Rhino.Input.Custom.GetOption go = new GetOption();
            go.SetCommandPrompt("Set Faro scan settings");

            Rhino.Input.Custom.GetString gs = new GetString();
            gs.SetCommandPrompt("Set Faro IP address.");
            gs.SetDefaultString("127.0.0.1");

            gs.AddOption("Scanner IP");

            gs.Get();

            string val = gs.StringResult();
            Rhino.RhinoApp.WriteLine("IP: " + val);

            // set up the options
            Rhino.Input.Custom.OptionInteger intOption = new Rhino.Input.Custom.OptionInteger(1, 1, 99);
            Rhino.Input.Custom.OptionDouble dblOption = new Rhino.Input.Custom.OptionDouble(2.2, 0, 99.9);
            Rhino.Input.Custom.OptionToggle boolOption = new Rhino.Input.Custom.OptionToggle(true, "Off", "On");
            string[] resolutionValues = new string[] { "Full", "Half", "Quarter", "Eighth", "Sixsteenth" };
            string[] measurementValues = new string[] { "Low", "Medium", "High" };
            string[] noiseValues = new string[] { "Low", "Medium", "High" };


            go.AddOptionInteger("Integer", ref intOption);
            go.AddOptionDouble("Double", ref dblOption);
            go.AddOptionToggle("Boolean", ref boolOption);

            int resolutionIndex = 2;
            int measurementIndex = 1;
            int noiseIndex = 1;

            int resolutionList = go.AddOptionList("Resolution", resolutionValues, resolutionIndex);
            int measurementList = go.AddOptionList("MeasurementRate", measurementValues, measurementIndex);
            int noiseList = go.AddOptionList("NoiseCompression", noiseValues, noiseIndex);

            while (true)
            {
                // perform the get operation. This will prompt the user to input a point, but also
                // allow for command line options defined above
                Rhino.Input.GetResult get_rc = go.Get();
                Rhino.RhinoApp.WriteLine(get_rc.ToString());

                if (go.CommandResult() != Rhino.Commands.Result.Success)
                    return go.CommandResult();

                if (get_rc == Rhino.Input.GetResult.Nothing)
                {
                    //doc.Objects.AddPoint(go.Point());
                    doc.Views.Redraw();
                    Rhino.RhinoApp.WriteLine("Command line option values are");
                    Rhino.RhinoApp.WriteLine(" Integer = {0}", intOption.CurrentValue);
                    Rhino.RhinoApp.WriteLine(" Double = {0}", dblOption.CurrentValue);
                    Rhino.RhinoApp.WriteLine(" Boolean = {0}", boolOption.CurrentValue);
                    Rhino.RhinoApp.WriteLine(" Measurement rate = {0}", measurementValues[measurementIndex]);
                    Rhino.RhinoApp.WriteLine(" Resolution = {0}", resolutionValues[resolutionIndex]);
                }
                else if (get_rc == Rhino.Input.GetResult.Option)
                {
                    if (go.OptionIndex() == resolutionList)
                        resolutionIndex = go.Option().CurrentListOptionIndex;
                    else if (go.OptionIndex() == measurementList)
                        measurementIndex = go.Option().CurrentListOptionIndex;

                    continue;
                }
                break;
            }

            return Rhino.Commands.Result.Success;
        }
    }
}
