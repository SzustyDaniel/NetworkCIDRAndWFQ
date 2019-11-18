using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using HomeWorkHelper;

namespace CidrCalculator.ViewModels
{
    public class CalculatorViewModel: BindableBase
    {
        private string rangeOrValidtxt;
        public string RangeOrValidTxt
        {
            get { return rangeOrValidtxt; }
            set { SetProperty(ref rangeOrValidtxt, value); RangeOrValidatecommand.RaiseCanExecuteChanged(); }
        }

        private bool checkValidity = false;
        public bool CheckValidity
        {
            get { return checkValidity; }
            set { SetProperty(ref checkValidity, value); }
        }

        private bool printRange = false;
        public bool PrintRange
        {
            get { return printRange; }
            set { SetProperty(ref printRange, value); RangeOrValidatecommand.RaiseCanExecuteChanged(); }
        }

        private string firstResultText;
        public string FirstResultText
        {
            get { return firstResultText; }
            set { SetProperty(ref firstResultText, value); RangeOrValidatecommand.RaiseCanExecuteChanged(); }
        }


        public CalculatorViewModel()
        {
            
        }


        private DelegateCommand cmdRangOrValidateCommand;
        public DelegateCommand RangeOrValidatecommand =>
            cmdRangOrValidateCommand ?? (cmdRangOrValidateCommand = new DelegateCommand(ExecuteRangeOrValidatecommand, CanExecuteRangeOrValidatecommand));

        void ExecuteRangeOrValidatecommand()
        {
            if (PrintRange)
            {
                string answer = CidrHelper.GetAddresssRagngeFormCidrNotation(RangeOrValidTxt);
                if (answer != "BAD RANGE!")
                {
                    var originAddress = RangeOrValidTxt.Split('/');
                    FirstResultText = "Range: " + originAddress[0] + " - " + answer;
                }
                else
                {
                    FirstResultText = answer;
                }
            }
            else
            {
                FirstResultText = "Validity: " + RangeOrValidTxt + " = " + ((CidrHelper.ValidateGivenCidrAddressNotation(RangeOrValidTxt)) ? "Good" : "Bad");
            }
        }

        bool CanExecuteRangeOrValidatecommand()
        {
            return (PrintRange || CheckValidity) && !string.IsNullOrEmpty(RangeOrValidTxt);
        }
    }
}
