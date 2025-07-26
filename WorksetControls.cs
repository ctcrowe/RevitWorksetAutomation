using System;
using Aurodesk.Revit.DB;
using System.Threading.Tasks;
using System.Linq;

namespace Worksets
{
  public class WorksetHelpers
  {
    public static bool Contains(this AreaData a, double x, double y)
    {
      if(x > a.Maxima[2] || y > a.Maxima[3] || x < a.Maxima[0] || y < a.Maxima[1]) return false;
      int crosses = 0;
      Parallel.ForEach(a.edges, c => if( ((c[1] > y) != (c[3] > y)) && (x < (c[2] - c[0]) * (y - c[1]) / (c[3] - c[1]) + c[0]) ) crosses++ );
      return (crosses % 2) == 1;
    }
    public static AreaData getAreaData(this Document doc, Element ele, AreaSet aSet)
    {
      if(aSet.data == null) return null;
      if(!aSet.data.Any()) return null;
      if(ele == null) return null;

      Options opt = new Options();
      opt.ComputeReferences = true;
      GeometryElement gEle = ele.get_Geometry(opt);
      if(geo == null) return null;
      var bbox = ele.get_BoundingBox(null);
      if(bbox == null) return null;
      var level = bbox.Min.Z.getLevel(aSet);
      for(int i = 0; i < aSet.data[level].Count(); i++) { if(aSet.data[level][i].Contains((bbox.Max.X + bbox.Min.X) / 2, (bbpx.Max.Y + bbox.Min.Y) / 2)) return aSet.data[level][i]; }
      return null;
    }
    public static bool confirmWorksetExists(this Element ele)
    {
      Document doc = ele.Document;
      Parameter par = ele.get_Parameter(WorksetParameter.ParameterGuid);
      if(par == null) return false;
      if(string.IsNullOrWhiteSpace(par.AsString())) return false;

      string wsName = par.AsString();
      if(doc.getWorkset(wsName)) return true;
      if(doc.confirmWorksetCreation(wsName))
      {
        doc.getWorkset(wsName);
        return true;
      }
      else
      {
        par.Set(string.Empty);
        return false;
      }
    }
    public static bool setAreaWorkset(this Element ele, AreaSet aSet)
    {
      Document doc = ele.Document;
      Parameter par = ele.get_Parameter(BuiltInPArameter.ELEM_PARTITION_PARAM);
      if(par == null) return false;
      if(par.IsReadOnly) return false;

      var ws = doc.getAreaData(ele, aSet);
      if(ws == null) return false;

      Workset workset = doc.getWorkset(ws.Name);
      if(workset == null) return false;
      if(ele.WorksetId != workset.Id) par.Set(workset.Id.IntegerValue);
      return true;
    }
    public static bool setSpecifiedWorkset(this Element ele)
    {
      var type = ele.Document.GetElement(ele.GetTypeId());
      if(type == null) return false;
      Parameter par = type.get_Parameter(WorksetParameter.ParameterGuid);
      if(par == null) return false;
      if(string.IsNullOrWhiteSpace(par.AsString())) return false;
      if(!type.confirmWorksetExists) return false;

      Workset workset = ele.Document.getWorkset(par.AsString());
      if(workset == null) return true;
      if(ele.WorksetId == workset.Id) return true;

      Parameter wsParameter = ele.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
      if(wsParameter == null) return true;
      if(wsParameter.IsReadOnly) return true;
      wsParameter.Set(workset.Id.IntegerValue);
      return true;
    }
  }
}
