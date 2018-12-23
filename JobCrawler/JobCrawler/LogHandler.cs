using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JobCrawler
{
    //싱글톤
    class LogHandler
    {
        private static LogHandler instance = null;
        private RichTextBox m_rb_log;

        private LogHandler(RichTextBox rb_log)
        {
            m_rb_log = rb_log;
            m_rb_log.AppendText("\n");
        }

        public static LogHandler getInstance(RichTextBox rb_log)
        {
            if (instance != null)
            {
                throw new InvalidOperationException("Singleton already created - use getinstance()");
            }
            instance = new LogHandler(rb_log);
            return instance;
        }

        public static LogHandler getInstance()
        {
            if (instance == null)
                throw new InvalidOperationException("Singleton not created - use GetInstance(arg1, arg2)");
            return instance;
        }

        public void ClearLog()
        {
            m_rb_log.Dispatcher.Invoke((Action)delegate
            {
                m_rb_log.Document.Blocks.Clear();
            });
        }

        public void AddLog(string str)
        {
            m_rb_log.Dispatcher.Invoke((Action)delegate
            {
                m_rb_log.AppendText(DateTime.Now.ToString());
                m_rb_log.AppendText(" - ");
                m_rb_log.AppendText(str);
                m_rb_log.AppendText("\n");
            });
        }
    }
}
