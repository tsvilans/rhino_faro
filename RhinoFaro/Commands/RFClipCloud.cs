using System;
using System.Linq;

using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;

using Rhino.Geometry;

using Rhino.Input;
using Rhino.Input.Custom;

namespace RhinoFaro.Commands
{
    [System.Runtime.InteropServices.Guid("5bade122-e484-40da-a793-06585316df0b")]
    public class RFClippingBounds : Command
    {
        static RFClippingBounds _instance;
        public RFClippingBounds()
        {
            _instance = this;
        }

        public static RFClippingBounds Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "RFClippingBounds"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            GetObject go = new GetObject();
            go.SetCommandPrompt("Select surfaces, polysurfaces, or meshes");
            go.GroupSelect = true;
            go.SubObjectSelect = false;
            go.EnableClearObjectsOnEntry(false);
            go.EnableUnselectObjectsOnExit(false);
            go.DeselectAllBeforePostSelect = false;
            bool bHavePreselectedObjects = false;

            for (; ; )
            {
                GetResult res = go.GetMultiple(1, 0);

                if (res == GetResult.Option)
                {
                    go.EnablePreSelect(false, true);
                    continue;
                }

                else if (res != GetResult.Object)
                    return Result.Cancel;

                if (go.ObjectsWerePreselected)
                {
                    bHavePreselectedObjects = true;
                    go.EnablePreSelect(false, true);
                    continue;
                }

                break;
            }


            BoundingBox box = BoundingBox.Empty;

            for (int i = 0; i < go.ObjectCount; i++)
            {
                RhinoObject rhinoObject = go.Object(i).Object();
                if (null != rhinoObject)
                    box.Union(rhinoObject.Geometry.GetBoundingBox(true));
                rhinoObject.Select(false);
            }

            if (box.IsValid)
            {
                RFContext.ClippingBox = new Box(box);
                //RFContext.Clip = true;

                return Result.Success;
            }

            return Result.Nothing;
        }
    }
}
