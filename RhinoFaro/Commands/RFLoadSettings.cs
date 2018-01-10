using System;
using Rhino;
using Rhino.Commands;

namespace RhinoFaro.Commands
{
    [System.Runtime.InteropServices.Guid("ef486ddb-a177-480b-bbd6-7450b1be2d0e")]
    public class RFLoadSettings : Command
    {
        static RFLoadSettings _instance;
        public RFLoadSettings()
        {
            _instance = this;
        }

        public static RFLoadSettings Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "RFLoadSettings"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            RFContext.LoadSettings();
            return Result.Success;
        }
    }
}
