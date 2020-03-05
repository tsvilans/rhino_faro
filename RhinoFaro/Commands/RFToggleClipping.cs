using System;
using Rhino;
using Rhino.Commands;

namespace RhinoFaro
{
    [System.Runtime.InteropServices.Guid("966c1e74-d8d9-4feb-83ef-84c631df3108")]
    public class RFToggleClipping : Command
    {
        static RFToggleClipping _instance;
        public RFToggleClipping()
        {
            _instance = this;
        }

        public static RFToggleClipping Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "RFToggleClipping"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            RFContext.Clip = !RFContext.Clip;
            return Result.Success;
        }
    }
}
