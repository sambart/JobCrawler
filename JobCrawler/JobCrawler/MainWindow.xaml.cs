﻿using System;
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
using System.ComponentModel;
namespace JobCrawler
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        CompanyList compList;
        JobKoreaCrawl jobkorea;
        HtmlDocument m_doc = new HtmlDocument();
        WebFInd m_webFind;
        public MainWindow()
        {
            InitializeComponent();
            // IWebElement q = driver.FindElement(By.Name("q"));
            // q.Clear();
            // q.SendKeys("HttpWebRequest");
            // driver.FindElement(By.Name("sa")).Click();

            //  Thread.Sleep(5000);
            Init();
        }

        void Init()
        {
            compList = Resources["CompanyListData"] as CompanyList;
            LogHandler.getInstance(rb_log);

            compList.Clear();

            /*
            jobkorea = new JobKoreaCrawl();
            jobkorea.SetAfterWork(AfterWork);
            jobkorea.SetBeforeWork(BeforeWork);
            jobkorea.SetIngWork(IngWork);
            jobkorea.SetGetInfo(GetInfo);
            jobkorea.SetPlanetInfo(JobPlanetGetInfo);
            */
        }

        private void Btn_test_Click(object sender, RoutedEventArgs e)
        {
            if (m_webFind == null)
            {
                m_webFind = new WebFInd(tb_searchUrl);
                m_webFind.Show();
            }

            if (!m_webFind.IsLoaded)
            {
                m_webFind = new WebFInd(tb_searchUrl);
                m_webFind.Show();
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                m_doc.LoadHtml(m_webFind.m_html);
                jobkorea = new JobKoreaCrawl(m_doc);
                jobkorea.SetAfterWork(AfterWork);
                jobkorea.SetBeforeWork(BeforeWork);
                jobkorea.SetIngWork(IngWork);
                jobkorea.SetGetInfo(GetInfo);
                jobkorea.SetPlanetInfo(JobPlanetGetInfo);

                jobkorea.Run(Convert.ToInt32(intUD_Cur.Value));
            }
            catch(ArgumentNullException ex)
            {
                MessageBox.Show("작업을 수행 할 웹 페이지를 선택해주세요");
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("작업을 수행 할 웹 페이지를 선택해주세요");
            }
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
                //lv_compList.ItemsSource = (IEnumerable)compList;
            });
        }

        void IngWork(int curPage)
        {
            pbar_pro.Dispatcher.Invoke((Action)delegate
            {
                double tempProVal = (Convert.ToDouble(curPage) / (Convert.ToDouble(intUD_Cur.Value) * 40)) * 100;
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

        void JobPlanetGetInfo(CompanyInfo compInfo, string rating, string url)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                compInfo.Rating = rating;
                compInfo.Jobplanet_link = url;

                ICollectionView view = CollectionViewSource.GetDefaultView(lv_compList.ItemsSource);
                view.Refresh();
            });
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var urlPart = ((Hyperlink)sender).NavigateUri;
            var fullUrl = string.Format("{0}", urlPart);
            Process.Start(new ProcessStartInfo(fullUrl));
            e.Handled = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if(jobkorea != null)
                jobkorea.IsStop = true;
        }
    }

}
