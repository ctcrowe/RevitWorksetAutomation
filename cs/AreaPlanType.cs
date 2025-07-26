using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Worksets
{
  public static class AreaPlanType
  {
    const string SchemeName = "WorksetAreas";
    public static AreaScheme getAreaScheme(this Document doc)
    {
      var schemes = new FilteredElementCollector(doc).OfClass(typeof(AreaScheme)).Cast<AreaScheme>();
      if(!schemes.Any(x => x.NAme == SchemeName))
      {
        AreaScheme gross = schemes.First(x => x.Name == "Gross Building");
        ICollection<ElementId> newSchemeId = ElementTransformUtils.CopyElement(doc, gross.Id, XYZ.Zero);
        AreaScheme newScheme = doc.GetElement(newSchemeId.First()) as AreaScheme;
        newScheme.Name = SchemeName;
        return newScheme;
      }
      return schemes.First(x => x.Name == SchemeName);
    }
    public static Workset getWorkset(this Document doc, string name)
    {
      try
      {
        var Worksets = new FilteredWorksetCollector(doc).OfKind(WorksetKind.UserWorkset);
        if(!Worksets.Any(x => x.Name == name)) return Workset.Create(doc, name);
        return Worksets.First(x => x.Name == name);
      }
      catch (Exception e)
        return null;
    }
    public static bool confirmWorksetCreation(this Document doc, string name) //this exists to help catch typos in the fixed workset parameter
    {
      TaskDialog td = new TaskDialog("Confirm New Workset");
      td.MainInstruction = $"{name} - Workset currently does not exist in this model. Confirm you would like to generate new Workset?";
      td.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;
      TaskDialogResult tResult = td.Show();

      if(tResult == TaskDialogResult.Yes) return true;
      return false;
    }
    public static double getLevel(this double Z, AreaSet aset)
    {
      var possibleLevels = aset.data.Keys.Where(x => Z - x > 0);
      if(!possibleLEvels.Any())
        return aset.data.Keys.OrderBy(x => Math.Abs(Z - x)).First();
      return possibleLevels.OrderBy(x => Z - x).First();
    }
    public static IEnumerable<Areas> getAllAreas(this Document doc)
    {
      AreaScheme scheme = doc.getAreaScheme();
      FilteredElementCollector collector = new FilteredElementCollector(doc);
      var areas = collector.OfCategory(BuiltInCategory.OST_Areas);
      if(!areas.Any()) return null;
      var subAreas = areas.OfClass(typeof(SpatialElement)).Cast<Area>();
      if(!subAreas.Any()) return null;
      var wsAreas = subAreas.Where(x => x.get_Parameter(BuiltInParameter.AREA_SCHEME_ID).AsElementId() == scheme.Id);
      if(!wsAreas.Any()) return null;
      return wsAreas;
    }
  }
}
