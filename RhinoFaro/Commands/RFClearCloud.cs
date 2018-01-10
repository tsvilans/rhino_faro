using System;
using Rhino;
using Rhino.Commands;

namespace RhinoFaro
{
    [System.Runtime.InteropServices.Guid("8928bc1b-c038-47ad-8d63-9d3e719bba38")]
    public class RFClearCloud : Command
    {
        static RFClearCloud _instance;
        public RFClearCloud()
        {
            _instance = this;
        }

        public static RFClearCloud Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "RFClearCloud"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            RFContext.ClearCloud();
            return Result.Success;
        }
    }
}
