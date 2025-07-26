using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Worksets
{
  internal class WorksetUpdater : IUpdater
  {
    public void Execute(UpdaterData data)
    {
      if(handler.CheckTransactions()) return; //this is used to stop the updated from running in certain situatuions.
      var added = data.GetAddedElementIds();
      var modified = data.GetModifiedElementIds();
      Document doc = data.GetDocument();
      if(!doc.IsWorkshared) return;
      if(areaSet.doc != doc) { areaSet.UpdateDoc(doc); }
        // the fastest way to make this run is to store all of the areas in memory and then access them as needed rather than collect them every time it runs.
        // this if statement updates if it runs and you have switched documents in your session.
      foreach(var eid in modified) SetWorkset(doc, eid);
      foreach(var  in added) SetWorkset(doc, eid);
    }
    internal void SetWorkset(Document doc, ElementId eid)
    {
      Element ele = doc.GetElement(eid);
      if(ele == null) return;
      if(ele.ViewSpecific) return;
      if(ele.SetSpecifiedWorkset()) return;
      ele.SetOtherWorkset(this.areaSet);
    }
    public void OnStartup()
    {
      UpdaterRegistry.RegisterUpdater(this, true);
      ElementIsElementTypeFilter filter = new ElementIsElementTypeFilter(true);
      UpdaterRegistry.AddTrigger(this.GetUpdaterId(), filter, Element.GetChangeTypeAny());
      UpdaterRegistry.AddTrigger(this.GetUpdaterId(), filter, Element.GetChangeTypeElementAddition());
    }
    public void OnShutdown() { UpdaterRegistry.UnregisterUpdater(this.GetUpdaterId()); }
    public WorksetUpdater(UIControlledApplication app, TransactionHandler handler, AreaSet areas)
    {
      this.handler = handler;
      this.areaSet = areas;
      appId = app.ActiveAddinId;
      updaterId = new UpdaterId(appId, guid);
    }
    TransactionHandler handler;
    AreaSet areaSet {get; set;}
    private static Guid guid = new Guid();

    private static AddInId appId;
    private static UpdaterId updaterId;
    public string GetAdditionalInformation() { return ""; }
    public ChangePrioriy GetChangePriority() { return ChangePriority.Annotations; }
    public UpdaterId GetUpdaterId() { return updaterId; }
    public string GetUpdaterName() { return "Workset Updater"; }
  }
}
