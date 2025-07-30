# RevitWorksetAutomation
Tool to automatically control object worksets inside of Revit based on location with overrides built into object parameters.
Worksets in Revit are used for 2 things
  1) graphical control of a drawing set
  2) Model speed optimization

The first and the second can be best managed by controling worksets by building boundaries. These boundaries could be wings or portions of the building, or other defining features.
This tool takes boundaries that are defined in an Area Plan and uses them automatically as a background process in Revit to prescribe worksets to elements while working.

The tool uses a series of Revit API Events: IUpdaters and View Changed Events, to track what areas to update based upon and what elements to update.
There is also a Workset parameter to override location based worksettting. This can be used for things like core and shell anad fire and life safety equipment, which should show up in all drawings regardless of location in the building.
