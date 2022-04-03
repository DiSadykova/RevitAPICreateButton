using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPICreateButton
{
    internal class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public DelegateCommand OutputForPipesCommand { get; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            OutputForPipesCommand = new DelegateCommand(OnOutputForPipesCommand);
        }

        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }

        private void OnOutputForPipesCommand()
        {
            RaiseCloseRequest();
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var pipes = new FilteredElementCollector(doc)
                .OfClass(typeof(Pipe))
                .Cast<Pipe>()
                .ToList();

            TaskDialog.Show("Количество труб в модели", pipes.Count.ToString());

        }
        private void OnOutputForWallsCommand()
        {
            RaiseCloseRequest();
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;


            var walls = new FilteredElementCollector(doc)
               .OfClass(typeof(Wall))
               .Cast<Wall>()
               .ToList();

            double value = 0;
            double sum = 0;
            foreach (var wall in walls)
            {
                Parameter volumeParameter = wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                if (volumeParameter.StorageType == StorageType.Double)
                {
                    value = UnitUtils.ConvertFromInternalUnits(volumeParameter.AsDouble(), UnitTypeId.CubicMeters);
                }
                sum += value;
            }
            TaskDialog.Show("Сумма объемов всех стен в модели", sum.ToString());
        }

        private void OnOutputForDoorsCommand()
        {
            RaiseCloseRequest();
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<FamilyInstance> fInstances = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();

            TaskDialog.Show("Количество дверей в модели", fInstances.Count.ToString());
        }
    }
}
