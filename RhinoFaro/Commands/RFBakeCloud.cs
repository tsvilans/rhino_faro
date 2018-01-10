using System;
using Rhino;
using Rhino.Geometry;
using Rhino.Commands;

namespace RhinoFaro
{
    [System.Runtime.InteropServices.Guid("47991a57-d0a6-4a7b-a667-c7fe310de012")]
    public class RFBakeCloud : Command
    {
        static RFBakeCloud _instance;
        public RFBakeCloud()
        {
            _instance = this;
        }

        public static RFBakeCloud Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "RFBakeCloud"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            doc.Objects.AddPointCloud(RFContext.Cloud);
            Polyline poly = new Polyline(new Point3d[] {
                new Point3d(0, 0, 0), new Point3d(120, 0, 0),
                new Point3d(0, 100, 0), new Point3d(0, 0, 0) });
            poly.Transform(RFContext.Xform);
            doc.Objects.AddPolyline(poly);

            RFContext.ClearCloud();
            return Result.Success;
        }
    }
}
