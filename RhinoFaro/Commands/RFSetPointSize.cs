using System;
using Rhino;
using Rhino.Input;
using Rhino.Commands;

namespace RhinoFaro
{
    [System.Runtime.InteropServices.Guid("02919f42-3b6f-4f30-8b04-f4f4c297de57")]
    public class RFSetPointSize : Command
    {
        static RFSetPointSize _instance;
        public RFSetPointSize()
        {
            _instance = this;
        }

        public static RFSetPointSize Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "RFSetPointSize"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            double psize = doc.Views.ActiveView.DisplayPipeline.DisplayPipelineAttributes.PointRadius;

            Result res = RhinoGet.GetNumber("Set point size", true, ref psize, 0.1, 10);
            //if (res != Result.Success)
            //{
            //    return res;
            //}

            RhinoApp.WriteLine(string.Format("New point size: {0}", psize));

            doc.Views.ActiveView.DisplayPipeline.DisplayPipelineAttributes.PointRadius = (float)psize;
            doc.Views.ActiveView.ActiveViewport.DisplayMode.DisplayAttributes.PointRadius = (float)psize;
            RFContext.PointSize = psize;

            return Result.Success;
        }
    }
}
