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
        GetPlanetInfo m_getPlanetInfo = null;

        HtmlDocument m_doc;

        bool isStop = false;
        string m_mainUrl = "http://www.jobkorea.co.kr";
        string m_searchUrl = "/recruit/joblist?menucode=duty&dutyCtgr=10016";
        int m_curPage = 1;
        int m_curProgress = 1;

        public bool IsStop { get => isStop; set => isStop = value; }

        public JobKoreaCrawl()
        {
            m_doc = WebSurf.retrieveData(m_mainUrl + m_searchUrl);
        }

        public JobKoreaCrawl(string url)
        {
            url = url.Replace("jobkorea.co.kr", "");
            url = url.Replace("http://", "");
            url = url.Replace("www.", "");
            m_searchUrl = url;
            m_doc = WebSurf.retrieveData(m_mainUrl + m_searchUrl);
        }

        public JobKoreaCrawl(HtmlDocument doc)
        {
            m_doc = doc;
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

        public void SetPlanetInfo(GetPlanetInfo getInfo)
        {
            m_getPlanetInfo += getInfo;
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

            while ((int)maxVal >= m_curPage)
            {
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
            string nextUrl = m_mainUrl + m_searchUrl + "#anchorGICnt_" + m_curPage.ToString();
            m_doc = WebSurf.retrieveData(nextUrl);
        }

        private bool GetCompanyInfo()
        {
            try
            {
                string expression = "//div[contains(@class, 'tplList')]//table//tbody//tr[contains(@class, 'devloopArea')]";
                HtmlNodeCollection nodes = m_doc.DocumentNode.SelectNodes(expression);

                foreach (HtmlNode node in nodes)
                {
                    //회사명 찾기
                    HtmlNode targetNode = node.SelectSingleNode(".//td[contains(@class, 'tplCo')]//a");
                    if (targetNode == null)
                        return false;
                    CompanyInfo tempCompany = new CompanyInfo(targetNode.InnerHtml);

                    //채용일정 찾기
                    targetNode = node.SelectSingleNode(".//td[contains(@class, 'odd')]//span[contains(@class, 'date dotum')]//span[contains(@class, 'tahoma')]");
                    if (targetNode != null)
                        tempCompany.Period = targetNode.InnerHtml;

                    //채용 공고URL 찾기
                    targetNode = node.SelectSingleNode(".//td[contains(@class, 'tplTit')]//a");
                    if (targetNode != null)
                        tempCompany.Jobkorea_link = m_mainUrl + targetNode.Attributes["href"].Value;

                    //회사 주소 찾기
                    HtmlNodeCollection targetNodes = node.SelectNodes(".//td[contains(@class, 'tplTit')]//span");
                    if (targetNodes[3] != null)
                        tempCompany.Address = targetNodes[3].InnerHtml;

                    //잡플래닛 검색
                    if (m_getInfo != null)
                    {
                        m_getInfo(tempCompany);

                        if (m_getPlanetInfo != null)
                            new JobPlanetCrawl().Run(tempCompany, m_getPlanetInfo);
                    }

                    m_curProgress++;

                    if (m_ingWork != null)
                        m_ingWork((int)m_curProgress);

                    if (isStop)
                        return false;
                }
            }
            catch (NullReferenceException ex)
            {
                LogHandler.getInstance().AddLog(ex.ToString());
                return true;
            }
            return true;
        }
    }
}
