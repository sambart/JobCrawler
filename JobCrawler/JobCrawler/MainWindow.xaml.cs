using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace JobCrawler
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScriptEngine engine = Python.CreateEngine();

            var scope = engine.CreateScope();
            var libs = new[] {
    "C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\Common7\\IDE\\Extensions\\Microsoft\\Python Tools for Visual Studio\\2.2",
    "C:\\Program Files (x86)\\IronPython 2.7\\Lib",
    "C:\\Program Files (x86)\\IronPython 2.7\\DLLs",
    "C:\\Program Files (x86)\\IronPython 2.7",
    "C:\\Program Files (x86)\\IronPython 2.7\\lib\\site-packages"
};

            engine.SetSearchPaths(libs);

            engine.ExecuteFile(@"SoapTest.py", scope);
        }
    }
}
