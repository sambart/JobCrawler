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
using System.Windows.Shapes;

namespace JobCrawler
{
    /// <summary>
    /// WebFInd.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WebFInd : Window
    {
        TextBox m_mainUrlTb;
        public string m_html;
        public WebFInd(TextBox a_tb)
        {
            InitializeComponent();

            m_mainUrlTb = a_tb;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            wb_main.Navigate(m_mainUrlTb.Text);
        }

        private void WebBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            m_mainUrlTb.Text = wb_main.Source.AbsoluteUri;
            dynamic doc = wb_main.Document;
            m_html = doc.documentElement.InnerHtml;
        }

    }
}
