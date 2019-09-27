using System;
using Rhino;
using Rhino.Commands;

namespace RhinoFaro
{
    [System.Runtime.InteropServices.Guid("3892824e-2ea7-4c86-b2f8-691833eb15b8")]
    public class RFAbort : Command
    {
        static RFAbort _instance;
        public RFAbort()
        {
            _instance = this;
        }

        public static RFAbort Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "RFAbort"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            RFContext.Abort = true;
            return Result.Success;
        }
    }
}
