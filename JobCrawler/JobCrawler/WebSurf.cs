using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JobCrawler
{
    public static class WebSurf
    {
        public static HtmlDocument retrieveData(string url)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();

                // used to build entire input
                StringBuilder sb = new StringBuilder();

                // used on each read operation
                byte[] buf = new byte[8192];

                // prepare the web page we will be asking for
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 10000; //10 millisecond
                                       // execute the request

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                
                StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
                
                doc.Load(streamReader);
                return doc;
            }
            catch(WebException ex)
            {
                LogHandler.getInstance().AddLog(ex.ToString());
                return null;
            }
        }

        
        /*
        /// <summary>
        /// Append a url parameter to a string builder, url-encodes the value
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected void AppendParameter(StringBuilder sb, string name, string value)
        {
            string encodedValue = HttpUtility.UrlEncode(value);
            sb.AppendFormat("{0}={1}&", name, encodedValue);
        }

        private void SendDataToService()
        {
            StringBuilder sb = new StringBuilder();
            AppendParameter(sb, "email", "hello@example.com");

            byte[] byteArray = Encoding.UTF8.GetBytes(sb.ToString());

            string url = "http://example.com/"; //or: check where the form goes

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            //request.Credentials = CredentialCache.DefaultNetworkCredentials; // ??

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(byteArray, 0, byteArray.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            m_doc.Load(response.GetResponseStream());
            // do something with response
        }
        */

    }
}
