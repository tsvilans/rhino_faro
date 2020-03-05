using FaroNET;

using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RhinoFaro
{
    public class RFContext
    {
        internal static PointCloud Cloud;
        internal static Box ClippingBox;
        internal static bool Clip = false;
        internal static Transform Xform;
        internal static double PointSize = 1.0;

        internal static bool Abort = false;

        public RFContext()
        {
            Cloud = new Rhino.Geometry.PointCloud();
            Xform = Transform.Identity;

            Rhino.Display.DisplayPipeline.DrawForeground += DisplayPipeline_DrawPointcloud;
            Rhino.Display.DisplayPipeline.CalculateBoundingBox += DisplayPipeline_CloudBoundingBox;

            LoadSettings();

            Clip = false;
        }

        private static void DisplayPipeline_DrawPointcloud(object sender, Rhino.Display.DrawEventArgs e)
        {
            if (ClippingBox.IsValid && Clip)
            {
                Point3d max = new Point3d(
                        ClippingBox.X.Max,
                        ClippingBox.Y.Max,
                        ClippingBox.Z.Max
                        );

                Point3d min = new Point3d(
                        ClippingBox.X.Min,
                        ClippingBox.Y.Min,
                        ClippingBox.Z.Min
                        );

                int[] clips = new int[6];

                clips[0] = e.Display.AddClippingPlane(max, -Vector3d.XAxis);
                clips[1] = e.Display.AddClippingPlane(min, Vector3d.XAxis);

                clips[2] = e.Display.AddClippingPlane(max, -Vector3d.YAxis);
                clips[3] = e.Display.AddClippingPlane(min, Vector3d.YAxis);

                clips[4] = e.Display.AddClippingPlane(max, -Vector3d.ZAxis);
                clips[5] = e.Display.AddClippingPlane(min, Vector3d.ZAxis);


                e.Display.EnableClippingPlanes(true);

                e.Display.ZBiasMode = Rhino.Display.ZBiasMode.TowardsCamera;

                e.Display.DrawPointCloud(Cloud, (float)PointSize);

                e.Display.EnableClippingPlanes(false);

                for (int i = 0; i < 6; ++i)
                    e.Display.RemoveClippingPlane(clips[i]);
            }
            else
                e.Display.DrawPointCloud(Cloud, 1);


        }

        private static void DisplayPipeline_CloudBoundingBox(object sender, Rhino.Display.CalculateBoundingBoxEventArgs e)
        {
            BoundingBox bb = Cloud.GetBoundingBox(false);
            e.IncludeBoundingBox(bb);
        }

        internal static bool ListAllAttributes()
        {
            Rhino.RhinoApp.WriteLine(string.Format("{0} : {1}", "Test", Workspace.GetAttribute("dummy")));
            return true;
            foreach (var attrName in Workspace.attribute_names)
            {
                Rhino.RhinoApp.WriteLine(string.Format("{0} : {1}", attrName, Workspace.GetAttribute(attrName)));
            }

            return true;
        }

        internal static bool LoadScan(string path, int step)
        {
            LoadSettings();

            //Task.Run(() => ProcessDataAsync(path));
            Task.Run(() =>
            {
                PointCloud temp = null;
                if (path.EndsWith("fls"))
                {
                    Workspace.Initialize();
                    Rhino.RhinoApp.WriteLine("FaroScan: Initialized. Loading scan...");

                    Workspace.ReflectionMode(2);


                    FNResult res = Workspace.Load(path);
                    if (res != FNResult.Success)
                    {
                        Rhino.RhinoApp.WriteLine("FaroScan: Failed to load scan: " + res.ToString());
                        return;
                    }

                    double[] points_raw;
                    int[] color_raw;

                    Workspace.GetXYZPoints(0, out points_raw, out color_raw, step);

                    foreach (var attrName in Workspace.attribute_names)
                    {
                        Rhino.RhinoApp.WriteLine(string.Format("{0} : {1}", attrName, Workspace.GetAttribute(attrName)));
                    }

                    var children = Workspace.GetChildren();
                    foreach (string str in children)
                        RhinoApp.WriteLine(str);

                    Workspace.Unload(0);
                    Workspace.Uninitialize();

                    Rhino.RhinoApp.WriteLine("FaroScan: Faro done.");

                    int N = points_raw.Length / 3;
                    Point3d[] points = new Point3d[N];
                    Color[] colors = new Color[N];

                    double factor = RhinoMath.UnitScale(UnitSystem.Meters, RhinoDoc.ActiveDoc.ModelUnitSystem);

                    for (int i = 0; i < N; ++i)
                    {
                        if (RFContext.Abort)
                        {
                            Abort = false;
                            break;
                        }

                        if (N % 1000000 == 0)
                            RhinoApp.WriteLine("Still working: {0}", N);

                        points[i] = new Point3d(points_raw[i * 3] * factor, points_raw[i * 3 + 1] * factor, points_raw[i * 3 + 2] * factor);

                        colors[i] = Color.FromArgb(color_raw[i]);
                    }

                    temp = new PointCloud(points);
                    for (int i = 0; i < temp.Count; ++i)
                    {
                        temp[i].Color = colors[i];
                    }

                    Rhino.RhinoApp.WriteLine("Farhino: Loaded " + path + " finally.");
                }
                /*
                else if (path.EndsWith("pcd"))
                {
                    tasTools.IO.PCD_Importer pcd = new tasTools.IO.PCD_Importer();
                    pcd.use_transform = true;
                    pcd.Import(path, out temp, true);

                    Rhino.RhinoApp.WriteLine("PCD: Loaded " + path + " finally.");
                    Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
                }
                */

                temp.Transform(Xform);

                if (Clip)
                {
                    Rhino.RhinoApp.WriteLine("Using clipping box.");
                    List<int> indices = new List<int>();
                    for (int i = 0; i < temp.Count; ++i)
                    {
                        if (ClippingBox.Contains(temp[i].Location))
                            indices.Add(i);
                    }

                    PointCloud cliptemp = new PointCloud();
                    List<Color> colors = new List<Color>();

                    for (int i = 0; i < indices.Count; ++i)
                    {
                        cliptemp.Add(temp[indices[i]].Location);
                        colors.Add(temp[indices[i]].Color);
                    }

                    for (int i = 0; i < colors.Count; ++i)
                    {
                        cliptemp[i].Color = colors[i];
                    }

                    Cloud = cliptemp;
                }
                else
                {
                    Cloud = temp;
                }

                Rhino.RhinoDoc.ActiveDoc.Views.Redraw();


            });

            Rhino.RhinoApp.WriteLine("Farhino: Loading " + path + " asynchronously. Keep working, it will appear soon.");
            return true;
        }
        /*
        static void ProcessDataAsync(string path)
        {
            Rhino.RhinoApp.WriteLine("Loading " + path);

            FaroScan.Initialize();
            Rhino.RhinoApp.WriteLine("Faro initialized. Loading scan...");


            FNResult res = FaroScan.Load(path);
            if (res != FNResult.Success)
            {
                Rhino.RhinoApp.WriteLine("Failed to load scan: " + res.ToString());
                return;
            }

            double[] points_raw;
            int[] color_raw;

            FaroScan.GetXYZPoints(0, out points_raw, out color_raw, 10);
            FaroScan.Unload(0);
            FaroScan.Uninitialize();

            Rhino.RhinoApp.WriteLine("Faro done.");


            Cloud = new PointCloud();

            int N = points_raw.Length / 3;
            Point3d[] points = new Point3d[N];
            Color[] colors = new Color[N];

            double factor = RhinoMath.UnitScale(UnitSystem.Meters, RhinoDoc.ActiveDoc.ModelUnitSystem);

            for (int i = 0; i < N; ++i)
            {
                points[i] = new Point3d(points_raw[i * 3] * factor, points_raw[i * 3 + 1] * factor, points_raw[i * 3 + 2] * factor);
                colors[i] = Color.FromArgb(color_raw[i], color_raw[i], color_raw[i]);
            }

            Cloud.AddRange(points);
            for (int i = 0; i < Cloud.Count; ++i)
            {
                Cloud[i].Color = colors[i];
            }

            Rhino.RhinoApp.WriteLine("Loaded " + path + " finally.");

        }
        */

        internal static void ClearCloud()
        {
            Cloud = new PointCloud();
            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
        }

        internal static void SaveSettings()
        {
            Plane p = Plane.WorldXY;
            p.Transform(Xform);
            Quaternion quat = Quaternion.Rotation(Plane.WorldXY, p);


            RFPlugIn.Instance.Settings.SetPoint3d("xform_origin", p.Origin);

            RFPlugIn.Instance.Settings.SetDouble("quat_a", quat.A);
            RFPlugIn.Instance.Settings.SetDouble("quat_b", quat.B);
            RFPlugIn.Instance.Settings.SetDouble("quat_c", quat.C);
            RFPlugIn.Instance.Settings.SetDouble("quat_d", quat.D);

            RFPlugIn.Instance.Settings.SetBool("clip", Clip);

            Point3d min = new Point3d(
                ClippingBox.X.Min,
                ClippingBox.Y.Min,
                ClippingBox.Z.Min
             );

            Point3d max = new Point3d(
                ClippingBox.X.Max,
                ClippingBox.Y.Max,
                ClippingBox.Z.Max
            );

            RFPlugIn.Instance.Settings.SetPoint3d("clip_min", min);
            RFPlugIn.Instance.Settings.SetPoint3d("clip_max", max);

            RFPlugIn.Instance.SaveSettings();

            return;
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            string settings_path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location), "settings.xml");
            RhinoApp.WriteLine(settings_path);


            var root = new XElement("rhino_faro");

            var xform_elem = new XElement("scanner_transform");



            var loc_elem = new XElement("loc");
            loc_elem.Value = string.Format("{0} {1} {2}", p.Origin.X, p.Origin.Y, p.Origin.Z);

            var rot_elem = new XElement("rot");
            rot_elem.Value = string.Format("{0} {1} {2} {3}", quat.A, quat.B, quat.C, quat.D);

            xform_elem.Add(loc_elem);
            xform_elem.Add(rot_elem);

            var clipping_elem = new XElement("clipping");
            var clip_elem = new XElement("clip");
            clip_elem.Value = Clip ? "1" : "0";
            var min_elem = new XElement("min");
            min_elem.Value = string.Format("{0} {1} {2}", ClippingBox.X.Min, ClippingBox.Y.Min, ClippingBox.Z.Min);
            var max_elem = new XElement("max");
            max_elem.Value = string.Format("{0} {1} {2}", ClippingBox.X.Max, ClippingBox.Y.Max, ClippingBox.Z.Max);

            clipping_elem.Add(clip_elem);
            clipping_elem.Add(min_elem);
            clipping_elem.Add(max_elem);


            root.Add(xform_elem);
            root.Add(clipping_elem);

            XDocument doc = new XDocument();
            doc.Add(root);

            doc.Save(settings_path);
        }

        internal static void LoadSettings()
        {
            Plane p = Plane.WorldXY;
            p.Transform(Xform);

            Quaternion quat = new Quaternion(
                RFPlugIn.Instance.Settings.GetDouble("quat_a", 1.0),
                RFPlugIn.Instance.Settings.GetDouble("quat_b", 0.0),
                RFPlugIn.Instance.Settings.GetDouble("quat_c", 0.0),
                RFPlugIn.Instance.Settings.GetDouble("quat_d", 0.0)
                );

            Point3d origin = RFPlugIn.Instance.Settings.GetPoint3d("xform_origin", Point3d.Origin);

            if (quat.IsValid)
                quat.GetRotation(out p);

            p.Origin = origin;

            Xform = Transform.PlaneToPlane(Plane.WorldXY, p);

            Clip = RFPlugIn.Instance.Settings.GetBool("clip", false);

            Point3d min = RFPlugIn.Instance.Settings.GetPoint3d("clip_min", Point3d.Origin);
            Point3d max = RFPlugIn.Instance.Settings.GetPoint3d("clip_max", Point3d.Origin);

            ClippingBox = new Box(Plane.WorldXY, new Point3d[] { min, max });

            return;
        }

            /*
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            string settings_path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location), "settings.xml");
            RhinoApp.WriteLine(settings_path);

            if (!System.IO.File.Exists(settings_path))
            {
                RhinoApp.WriteLine("No settings file found...");
                return;
            }

            var root = XElement.Load(settings_path);
            if (root == null)
            {
                RhinoApp.WriteLine("Could not open " + settings_path + "...");
                return;
            }
            if (root.Name.LocalName != "rhino_faro")
            {
                RhinoApp.WriteLine("Invalid settings file.");
                return;
            }

            #region Load Transform
            var temp = root.Elements(XName.Get("scanner_transform")).First();
            var sub = temp.Elements(XName.Get("loc")).First();
            Point3d loc = StringToPoint(sub.Value);

            sub = temp.Elements(XName.Get("rot")).First();
            Quaternion rot = StringToQuaternion(sub.Value);

            Plane p;
            rot.GetRotation(out p);
            p.Origin = loc;

            Xform = Transform.PlaneToPlane(Plane.WorldXY, p);
            RhinoApp.WriteLine("XForm: " + Xform.ToString());
            #endregion

            #region Load Clipping box

            temp = root.Elements(XName.Get("clipping")).First();
            sub = temp.Elements(XName.Get("clip")).First();
            Clip = int.Parse(sub.Value) > 0;

            if (Clip)
            {
                sub = temp.Elements(XName.Get("min")).First();
                Point3d min = StringToPoint(sub.Value);
                sub = temp.Elements(XName.Get("max")).First();
                Point3d max = StringToPoint(sub.Value);

                ClippingBox = new Box(Plane.WorldXY, new Point3d[] { min, max });
                RhinoApp.WriteLine("Clipping enabled.");
            }

            #endregion

            RhinoApp.WriteLine("Settings loaded.");

        }

        
        internal static Quaternion StringToQuaternion(string s)
        {
            string[] bits = s.Split();
            if (bits.Length != 4) return Quaternion.Identity;

            Quaternion q = new Quaternion();
            q.A = double.Parse(bits[0]);
            q.B = double.Parse(bits[1]);
            q.C = double.Parse(bits[2]);
            q.D = double.Parse(bits[3]);

            return q;
        }

        internal static Point3d StringToPoint(string s)
        {
            string[] bits = s.Split();
            if (bits.Length != 3) return Point3d.Origin;

            double x = double.Parse(bits[0]);
            double y = double.Parse(bits[1]);
            double z = double.Parse(bits[2]);

            return new Point3d(x, y, z);
        }
         */
    }
}
