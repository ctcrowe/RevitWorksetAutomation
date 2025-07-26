using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Events;

namespace Worksets
{
  //This class helps to catch any transactions that we wouldnt want to run our updater on.
  //Most of the transactions listed in the method CheckTransactions run on synchronize, which would create conflicts and errors dealing with ownership between users.
  public class TransactionHandler
  {
    public bool hideTransaction {get; set;}
    public string transactionName {get; set;}
    public TransactionHandler()
    {
      this.transactionName = "NONE";
      this.hideTransaction = false;
    }
    public void OnStartup(UIControlledApplication app) { app.ControlledApplication.FailuresProcessing += FailuresProcessing; }
    public void OnShutdown(UIControlledApplication app) { app.ControlledApplication.FailuresProcessing -= FailuresProcessing; }
    private void FailuresProcessing(object sender, FailuresProcessingEventArgs e)
    {
      var accessor = e.GetFailuresAccessor();
      transactionName = accessor.GetTransactionName();
    }
    public bool CheckTransactions()
    {
      hideTransaction = transactionName.Equals("Reload Latest");
      hideTransaction = hideTransaction || transactionName.Equals("Synchronize with Central");
      hideTransaction = hideTransaction || transactionName.Equals("Update project to latest changes.");
      hideTransaction = hideTransaction || transactionName.Equals("Reload Linked Instances");
      hideTransaction = hideTransaction || transactionName.Equals("Fix bad fabrication part guids");
      hideTransaction = hideTransaction || transactionName.Equals("Snaps");
      hideTransaction = hideTransaction || transactionName.Equals("Keynote Settings");
      hideTransaction = hideTransaction || transactionName.Equals("Assembly Code Settings");
      hideTransaction = hideTransaction || transactionName.Equals("Fix Content Doc Tree");
      hideTransaction = hideTransaction || transactionName.Equals("Upgrade for external parameters");
      hideTransaction = hideTransaction || transactionName.Equals("Resolve Missing Servers");
      hideTransaction = hideTransaction || transactionName.Equals("Save macro elements");
      hideTransaction = hideTransaction || transactionName.Equals("Notify AppInfo internal");
      hideTransaction = hideTransaction || transactionName.Equals("Preview");
      hideTransaction = hideTransaction || transactionName.Equals("Stop sharing");
      hideTransaction = hideTransaction || transactionName.Equals("Save Modified Doc");
      hideTransaction = hideTransaction || transactionName.Equals("Relative links");
      hideTransaction = hideTransaction || transactionName.Equals("Precast Update Configuration");
      hideTransaction = hideTransaction || transactionName.Equals("Modify type attributes");
      hideTransaction = hideTransaction || transactionName.Equals("ContentADoc__UpdateReplicasInSmallDocStep");
      hideTransaction = hideTransaction || transactionName.Equals("Same Place");
      return hideTransaction;
    }
  }
}
