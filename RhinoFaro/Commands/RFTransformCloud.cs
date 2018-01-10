using System;
using Rhino;
using Rhino.Commands;

namespace RhinoFaro.Commands
{
    [System.Runtime.InteropServices.Guid("df938c1d-a62b-4866-8a29-a2887126b558")]
    public class RFTransformCloud : Command
    {
        static RFTransformCloud _instance;
        public RFTransformCloud()
        {
            _instance = this;
        }

        public static RFTransformCloud Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "RFTransformCloud"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            RFContext.Cloud.Transform(RFContext.Xform);
            return Result.Success;
        }
    }
}
