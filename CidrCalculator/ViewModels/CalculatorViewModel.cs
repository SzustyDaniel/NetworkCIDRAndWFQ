using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using CidrCalculator.Services;

namespace CidrCalculator.ViewModels
{
    public class CalculatorViewModel: BindableBase
    {
        #region Validity or Range Properties

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

        #endregion

        private string overlapFirst;
        public string OverlapFirst
        {
            get { return overlapFirst; }
            set { SetProperty(ref overlapFirst, value);  CheckOverlapCommand.RaiseCanExecuteChanged(); }
        }

        private string overlapSecond;
        public string OverlapSecond
        {
            get { return overlapSecond; }
            set { SetProperty(ref overlapSecond, value); CheckOverlapCommand.RaiseCanExecuteChanged(); }
        }

        private string overlapResult;
        public string OverlapResult
        {
            get { return overlapResult; }
            set { SetProperty(ref overlapResult, value); }
        }

        public CalculatorViewModel()
        {
            CheckValidity = true;
        }


        private DelegateCommand cmdRangOrValidateCommand;
        public DelegateCommand RangeOrValidatecommand =>
            cmdRangOrValidateCommand ?? (cmdRangOrValidateCommand = new DelegateCommand(ExecuteRangeOrValidatecommand, CanExecuteRangeOrValidatecommand));

        void ExecuteRangeOrValidatecommand()
        {
            if (PrintRange)
            {
                string answer = CidrHelper.GetAddresssRagngeFormCidrNotation(RangeOrValidTxt.Trim());
                if (answer != "BAD RANGE!")
                {
                    var originAddress = RangeOrValidTxt.Split('/');
                    FirstResultText = "Range: " + originAddress[0].Trim() + " - " + answer;
                }
                else
                {
                    FirstResultText = answer;
                }
            }
            else
            {
                FirstResultText = "Validity: " + RangeOrValidTxt.Trim() + " = " + ((CidrHelper.ValidateGivenCidrAddressNotation(RangeOrValidTxt.Trim())) ? "Good" : "Bad");
            }
        }

        bool CanExecuteRangeOrValidatecommand()
        {
            return (PrintRange || CheckValidity) && !string.IsNullOrEmpty(RangeOrValidTxt);
        }


        private DelegateCommand cmdCheckOverlap;
        public DelegateCommand CheckOverlapCommand =>
            cmdCheckOverlap ?? (cmdCheckOverlap = new DelegateCommand(ExecuteCheckOverlapCommand, CanExecuteCheckOverlapCommand));

        void ExecuteCheckOverlapCommand()
        {
            OverlapResult = OverlapFirst.Trim() + " And " + OverlapSecond.Trim() + " = " + ((CidrHelper.CheckCidrIpAddressOverlap(OverlapFirst.Trim(), OverlapSecond.Trim()))? "Overlaping": "Not overlaping");
        }

        bool CanExecuteCheckOverlapCommand()
        {
            return !string.IsNullOrEmpty(OverlapFirst) && !string.IsNullOrEmpty(OverlapSecond);
        }
    }
}
