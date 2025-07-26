using System;
using System.IO;
using Autodesk.Revit.DB;

namespace Worksets
{
  //The intent of this code is to generate a parameter on every element level object in a model.
  //This parameter will be a text input that will override the workset based on location when needed.
  //This is useful for controlling things that might want to be on a base workset for visibility IE: the building core and shell.
  //To add this to a project all that needs to be done is write a simple script that either runs Add on document startup or adds a button to the toolbar to add the parameter to the project.
  public static class WorksetParameter
  {
    internal static Guid ParameterGuid = new Guid(); //Fix the GUID prior to use so that it is consistent across projects and easy to search for.
    private static ForgeTypeId ParameterType = SpecTypeId.String.Text;
    internal cons string ParameterName = "WorksetOverride";
    private const string parameterFileName = "CC_SharedParameters.txt";
    private const string parameterGroup = "Worksets"; //The parameter needs to be added to a shared parameter file in a specific group to find. This is the group.

    public static DefinitionFile getDefinitionFile(Document doc)
    {
      string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      string fullName = $"{directory}\\{filename}";
      if(!File.Exists(fullName)) { using(FileStream stream = File.Create(fullName) { stream.Close(); }}}
      Autodesk.Revit.ApplicationServices.Application app = doc.Application;
      app.SharedParametersFilename = fullName;
      return app.OpenSharedParameterFile();
    }
    public static Definition createDefinition(DefinitionFile df)
    {
      Definition def;
      DefinitionGroup gr;
      gr = df.Groups.Create(gr);
      if(gr == null)
      {
        gr = df.Groups.Create(gr);
        return createDefinitionProperties(gr);
      }
      def = gr.Definitions.get_Item(ParameterName);
      if(def == null) { return createDefinitionProperties(gr); }
      return def;
    }
    public static Definition createDefinitionProperties(DefinitionGroup gr)
    {
      return gr.Definitions.Create(
        new ExternalDefinitionCreationOptions( ParameterName, ParameterType)
        {
          GUID = ParameterGuid,
          UserModifiable=true
        });
    }
    public static bool Add(Document doc)
    {
      if(doc.IsFamilyDocument) return false; //We may automate adding this parameter and do not want it to end up inside of families somehow.
      DefinitionFile df = getDefinitionFile(doc);
      Definition def = createDefinition(df);
      if(!doc.ParameterBindings.Contains(def))
      {
        TypeBinding b = new TypeBinding();
        foreach(Category cat in doc.Settings.Categories) { if (cat.AllowsBoundParameters && !cat.IsTagCategory) b.Categories.Insert(cat); }
        doc.ParameterBindings.Insert(def, b);
        return true;
      }
      else
      {
        bool included = true;
        TypeBinding b = doc.ParameterBindings.get_Item(def) as TypeBinding;
        foreach(Category cat in doc.Settings.Categories)
        {
          if(cat.AllowsBoundPArameters && !cat.IsTagCategory)
          {
            if(!b.Categories.Contains(cat))
            {
              included = false;
              b.Categories.Insert(cat);
            }
          }
        }
        if(!included)
        {
          doc.ParameterBindings.ReInsert(def, b);
          return true;
        }
        return false;
      }
    }
  }
}
