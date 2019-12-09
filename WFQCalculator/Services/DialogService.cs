using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WFQCalculator.Services
{
    public class DialogService
    {
        private static DialogService _instace;
        public static DialogService Instance 
        {
            get 
            {
                if (_instace == null)
                    _instace = new DialogService();

                return _instace;
            } 
        }

        private DialogService()
        {

        }

        public void ShowMessageDialog(string message, string title ,MessageBoxImage image)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, image);
        }

        public string GetFilePathDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text files(*.txt) | *.txt"
            };

            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileName;

            return "";
        }

    }
}
