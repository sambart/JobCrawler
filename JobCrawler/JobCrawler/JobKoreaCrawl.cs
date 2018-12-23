using System;
using System.Collections.Generic;
using System.Threading;
using HtmlAgilityPack;

namespace JobCrawler
{
    delegate void BeforeWork();
    delegate void AfterWork();
    delegate void IngWork(int value);
    delegate void GetInfo(CompanyInfo info);

    class JobKoreaCrawl
    {
        BeforeWork m_beforeWork = null;
        AfterWork m_afterWork = null;
        IngWork m_ingWork = null;
        GetInfo m_getInfo = null;

        HtmlWeb m_webget;
        HtmlDocument m_doc;
        string m_mainUrl = "http://www.jobkorea.co.kr/recruit/joblist?menucode=duty&dutyCtgr=10016";
        int m_curPage = 1;

        public JobKoreaCrawl()
        {
            Init();
        }

        public JobKoreaCrawl(string url)
        {
            m_mainUrl = url;
            Init();
        }

        private void Init()
        {
            m_webget = new HtmlWeb();
            m_doc = m_webget.Load(m_mainUrl);

        }

        public void SetBeforeWork(BeforeWork beforeWork)
        {
            m_beforeWork += beforeWork;
        }

        public void SetIngWork(IngWork ingWork)
        {
            m_ingWork += ingWork;
        }

        public void SetAfterWork(AfterWork afterWork)
        {
            m_afterWork += afterWork;
        }

        public void SetGetInfo(GetInfo getInfo)
        {
            m_getInfo += getInfo;
        }

        public void Run(int maxVal)
        {
            ParameterizedThreadStart mainWork = new ParameterizedThreadStart(MainWork);

            Thread task = new Thread(mainWork);
            task.Start(maxVal);
        }

        private void MainWork(object maxVal)
        {
            if(m_beforeWork != null)
               m_beforeWork();

            m_curPage = 1;

            while ((int)maxVal > m_curPage)
            {
                if (m_ingWork != null)
                    m_ingWork((int)m_curPage);

                if (!GetCompanyInfo())
                    break;

                GoToNextPage();
            }
            if (m_afterWork != null)
                m_afterWork();
        }
        

        private void GoToNextPage()
        {
            m_curPage++;
            string nextUrl = m_mainUrl + "#anchorGICnt_" + m_curPage.ToString();
            m_doc = m_webget.Load(nextUrl);
        }

        private bool GetCompanyInfo()
        {
            try
            {
                string expression = "//div[contains(@class, 'tplList')]//table//tbody//tr[contains(@class, 'devloopArea')]//td[contains(@class, 'tplCo')]";
                HtmlNodeCollection nodes = m_doc.DocumentNode.SelectNodes(expression);

                foreach (HtmlNode node in nodes)
                {
                    HtmlNode targetNode = node.SelectSingleNode(".//a");
                    CompanyInfo tempCompany = new CompanyInfo(targetNode.InnerHtml);
                    tempCompany.Jobkorea_link = targetNode.Attributes["href"].Value;
                    if (m_getInfo != null)
                        m_getInfo(tempCompany);
                }
            }
            catch (NullReferenceException ex)
            {
                return false;
            }
            return true;
        }
    }
}
