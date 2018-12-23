using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using HtmlAgilityPack;
using Microsoft.Scripting.Hosting;
using System.Threading;
using System.Collections;

namespace JobCrawler
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        CompanyList compList;

        public MainWindow()
        {
            InitializeComponent();
            
            compList = Resources["CompanyListData"] as CompanyList;
        }
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            compList.Clear();

            JobKoreaCrawl jobkorea = new JobKoreaCrawl();
            jobkorea.SetAfterWork(AfterWork);
            jobkorea.SetBeforeWork(BeforeWork);
            jobkorea.SetIngWork(IngWork);
            jobkorea.SetGetInfo(GetInfo);

            jobkorea.Run(Convert.ToInt32(intUD_Cur.Value));

        }

        void BeforeWork()
        {
            btn_start.Dispatcher.Invoke((Action)delegate
            {
                btn_start.IsEnabled = false;
            });

            pbar_pro.Dispatcher.Invoke((Action)delegate
            {
                pbar_pro.Value = 1;
            });
        }

        void AfterWork()
        {
            pbar_pro.Dispatcher.Invoke((Action)delegate
            {
                pbar_pro.Value = 100;
            });

            btn_start.Dispatcher.Invoke((Action)delegate
            {
                btn_start.IsEnabled = true;
            });

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                // this.DataContext = compList;
                lv_compList.ItemsSource = (IEnumerable)compList;
            });
        }

        void IngWork(int curPage)
        {
            pbar_pro.Dispatcher.Invoke((Action)delegate
            {
                double tempProVal = (Convert.ToDouble(curPage) / Convert.ToDouble(intUD_Cur.Value)) * 100;
                pbar_pro.Value = Convert.ToInt32(tempProVal);
            });
        }

        void GetInfo(CompanyInfo compInfo)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                compList.Add(compInfo);
            });
        }

    }
}
