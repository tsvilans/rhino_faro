using System;
using Rhino;
using Rhino.Commands;

namespace RhinoFaro
{
    [System.Runtime.InteropServices.Guid("ec64b091-ed22-48aa-b72c-8deb811d9236")]
    public class RFListAttributes : Command
    {
        static RFListAttributes _instance;
        public RFListAttributes()
        {
            _instance = this;
        }

        public static RFListAttributes Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "RFListAttributes"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            RFContext.ListAllAttributes();
            return Result.Success;
        }
    }
}
