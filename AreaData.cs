using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace Worksets
{
  internal class AreaSet
  {
    public Document doc;
    public Dictionary<double, List<AreaData>> data;
    public AreaSet(Document d)
    {
      this.doc = d;
      this.data = null;
    }
    public AreaSet()
    {
      this.doc = null;
      this.data = null;
    }
    public void UpdateAreas()
    {
      var areas = this.doc.getAllAreas();
      this.data = new Dictionary<double, List<AreaData>>();
      foreach(var area in areas)
      {
        var areaData = new AreaData(area);
        if(this.data.ContainsKey(areaData.Z)) this.data[areaData.Z].Add(areaData);
        else this.data.Add(areaData.Z, new List<AreaData>() { areaData });
      }
    }
    public bool UpdateDoc(Document doc)
    {
      this.doc = Document;
      var areas = this.doc.getAllAreas();
      this.data = new Dictionary<double, List<AreaData>>();
      if(areas == null) return false;
      if(!areas.Any()) return false;
      foreach(var area in areas)
      {
        var areaData = new AreaData(area);
        if(this.data.ContainsKey(areaData.Z)) this.data[areaData.Z].Add(areaData);
        else this.data.Add(areaData.Z, new List<AreaData>() { areaData });
      }
      return true;
    }
  }
  public class AreaData
  {
    public string Name { get; set; }
    public double[] Maxima { get; set; }
    public double Z { get; set; }
    public List<double[]> Edges { get; set; }
    public AreaData(Area a)
    {
      this.Name = a.LookupParameter("Name").AsString();
      this.Z = a.Level.ProjectElevation;
      this.Maxima = new double[4] { double.MaxValue, double.MinValue, double.MaxValue, double.MinValue };
      this.Edges = new List<double[]>();

      SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();
      options.SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Center;

      var boundaries = a.GetBoundarySegments(options);
      Parallel.For(0, boundaries.Count(), j => this.Edges.AddRange(boundaries[j].Select(x => getEndPoints(x.GetCurve()))));
    }
    private double[] getEndPoints(Curve c)
    {
      var ep1 = c.GetEndPoint(0);
      var ep2 = c.GetEndPoint(1);
      double[] edge = new double[4] { ep1.X, ep1.Y, ep2.X, ep2.Y };
      //update X values
      this.Maxima[0] = edge[0] < this.Maxima[0] ? this.edge[0] : this.Maxima[0];
      this.Maxima[0] = edge[2] < this.Maxima[0] ? this.edge[2] : this.Maxima[0];
      this.Maxima[0] = edge[0] > this.Maxima[2] ? this.edge[0] : this.Maxima[2];
      this.Maxima[0] = edge[2] > this.Maxima[2] ? this.edge[2] : this.Maxima[2];

      //update Y values
      this.Maxima[1] = edge[1] < this.Maxima[1] ? this.edge[1] : this.Maxima[1];
      this.Maxima[1] = edge[3] < this.Maxima[1] ? this.edge[3] : this.Maxima[1];
      this.Maxima[3] = edge[1] > this.Maxima[3] ? this.edge[1] : this.Maxima[3];
      this.Maxima[3] = edge[3] > this.Maxima[3] ? this.edge[3] : this.Maxima[3];

      return edge;
    }
  }
}
