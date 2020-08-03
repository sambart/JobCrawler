using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace JobCrawler
{
    delegate void GetPlanetInfo(CompanyInfo compInfo, string rating, string planetLink);

    class JobPlanetCrawl
    {
        CompanyInfo m_compInfo;
        GetPlanetInfo m_getInfo;
        HtmlDocument m_doc = new HtmlDocument();
        string m_mainUrl = "https://www.jobplanet.co.kr";
        //string m_searchUrl = "/search?category=&query=SearchingString&_rs_con=welcome&_rs_act=index&_rs_element=main_search_bar";
        string m_searchUrl = "/search?query=SearchingString&category=&_rs_con=companies&_rs_act=cover&_rs_element=main_search_bar";
        //string m_searchUrl = "/job/search?q=SearchingString";
        string m_nextUrl = "";

        public void Run(CompanyInfo compInfo, GetPlanetInfo getInfo)
        {
            m_compInfo = compInfo;
            m_searchUrl = m_searchUrl.Replace("SearchingString", compInfo.Name);
            m_getInfo = getInfo;

            MainWork();

          //  ThreadStart mainWork = new ThreadStart(MainWork);
            // ParameterizedThreadStart mainWork = new ParameterizedThreadStart(MainWork);

         //   Thread task = new Thread(mainWork);
          //  task.Start();
        }

        private void MainWork()
        {
            //m_webget = new HtmlWeb();
            //m_doc = m_webget.Load(m_mainUrl + m_searchUrl);
            m_doc = WebSurf.retrieveData(m_mainUrl + m_searchUrl);

            if (m_doc == null)
                return;
            if (!GetCompanyId() || m_nextUrl == "")
                return;
            string tempUrl = m_mainUrl + m_nextUrl;
            //m_doc = m_webget.Load(tempUrl);
            m_doc = WebSurf.retrieveData(tempUrl);

            if (m_doc == null)
                return;

            GetCompanyInfo();
        }

        private bool GetCompanyId()
        {
            try
            {
                string expression = "//div[contains(@class, 'is_company_card')]//a";
                HtmlNode targetNode = m_doc.DocumentNode.SelectSingleNode(expression);
                //HtmlNodeCollection nodes = m_doc.DocumentNode.SelectNodes(expression);
                m_nextUrl = targetNode.Attributes["href"].Value;
            }
            catch (NullReferenceException ex)
            {
                LogHandler.getInstance().AddLog(m_compInfo.Name);
                LogHandler.getInstance().AddLog("잡플래닛 검색 실패");
                m_getInfo(m_compInfo, "검색실패", "");

                return false;
            }
            catch (WebException ex)
            {
                LogHandler.getInstance().AddLog(m_compInfo.Name);
                LogHandler.getInstance().AddLog(ex.Message);
                // Logic to retry (maybe in 10 minutes) goes here
            }
            return true;
        }

        private bool GetCompanyInfo()
        {
            try
            {
                //string expression = "//span[contains(@class, 'rate_point')]";
                //string expression = "//span[@class='rate_point']";
                string expression = "//head/title";
                //string expression = "//*[@id='premiumReviewStatistics']/div/div/div/div[2]/div[1]/span";
                HtmlNode targetNode = m_doc.DocumentNode.SelectSingleNode(expression);
                //HtmlNodeCollection nodes = m_doc.DocumentNode.SelectNodes(expression);

                string rating = targetNode.InnerHtml;
                string url = m_mainUrl + m_nextUrl;

                m_getInfo(m_compInfo, rating, url);
            }
            catch (NullReferenceException ex)
            {
                LogHandler.getInstance().AddLog(m_compInfo.Name);
                LogHandler.getInstance().AddLog(ex.Message);
                return false;
            }

            catch (FormatException ex)
            {
                LogHandler.getInstance().AddLog(m_compInfo.Name);
                LogHandler.getInstance().AddLog(ex.Message);
                return false;
            }
            return true;
        }


    }
}
