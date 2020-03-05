using System;
using Rhino;
using Rhino.PlugIns;
using FaroNET;
using Rhino.Geometry;
using System.Drawing;
using System.Threading.Tasks;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;

namespace RhinoFaro
{
    public class RFPlugIn : Rhino.PlugIns.PlugIn

    { 
        internal RFContext rf;

        public RFPlugIn()
        {
            Instance = this;
            rf = new RFContext();
        }

        public static RFPlugIn Instance
        {
            get; private set;
        }

        protected override LoadReturnCode OnLoad(ref string errorMessage)
        {
            return base.OnLoad(ref errorMessage);
        }


    }
}