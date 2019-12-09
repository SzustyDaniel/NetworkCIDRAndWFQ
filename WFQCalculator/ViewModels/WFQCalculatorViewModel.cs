using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using WFQCalculator.Models;
using WFQCalculator.Services;

namespace WFQCalculator.ViewModels
{
    public class WFQCalculatorViewModel: BindableBase
    {
        private DialogService dialogService;

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set { SetProperty(ref _filePath, value); CalculateWFQcmd.RaiseCanExecuteChanged(); }
        }

        private bool _withoutSkew;
        public bool WithoutSkew
        {
            get { return _withoutSkew; }
            set { SetProperty(ref _withoutSkew, value); CalculateWFQcmd.RaiseCanExecuteChanged(); }
        }

        private bool _withSkew;
        public bool WithSkew
        {
            get { return _withSkew; }
            set { SetProperty(ref _withSkew, value); CalculateWFQcmd.RaiseCanExecuteChanged(); }
        }

        private string _calculatedAnswer;
        public string CalculatedAnswer
        {
            get { return _calculatedAnswer; }
            set { SetProperty(ref _calculatedAnswer, value); }
        }

        public WFQCalculatorViewModel()
        {
            dialogService = DialogService.Instance;
        }

        private DelegateCommand _getFileDialogcmd;
        public DelegateCommand GetFileDialogCmd =>
            _getFileDialogcmd ?? (_getFileDialogcmd = new DelegateCommand(ExecuteGetFileDialogCmd));

        void ExecuteGetFileDialogCmd()
        {
            FilePath = "";
            FilePath = dialogService.GetFilePathDialog();
        }

        private DelegateCommand _calculateWFQcmd;
        public DelegateCommand CalculateWFQcmd =>
            _calculateWFQcmd ?? (_calculateWFQcmd = new DelegateCommand(ExecuteCalculateWFQcmd, CanExecuteCalculateWFQcmd));

        void ExecuteCalculateWFQcmd()
        {
            List<TimedPacket> packets = new List<TimedPacket>();
            List<Flow> flows = new List<Flow>();
            List<TimedPacket> result = new List<TimedPacket>();
            string resultText = "";

            try
            {
                string[] fileLines = System.IO.File.ReadAllLines(FilePath);
                flows = WFQService.CreateFlowsFromFileLines(fileLines);
                packets = WFQService.CreatePacketsFromFileLines(fileLines, flows);
                if(WithSkew)
                {
                    result = WFQService.CalculatWFQwithSkew(flows, packets);
                }
                else if(WithoutSkew)
                {
                    result = WFQService.CalculateWFQWithoutSkew(packets);
                }

                foreach (var packet in result)
                {
                    resultText += packet.Number + " ";
                }

                CalculatedAnswer = resultText;
            }
            catch(Exception e)
            {
                dialogService.ShowMessageDialog(e.Message, "Exception", System.Windows.MessageBoxImage.Error);
            }

        }

        bool CanExecuteCalculateWFQcmd()
        {
            return !string.IsNullOrEmpty(FilePath) && (WithSkew || WithoutSkew);
        }

    }



}
