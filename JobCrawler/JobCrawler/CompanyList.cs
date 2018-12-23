using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCrawler
{
    public class CompanyList : ObservableCollection<CompanyInfo>
    {
        private int curIndex = 1;

        protected override void InsertItem(int index, CompanyInfo item)
        {
            base.InsertItem(index, item);
            item.Index = curIndex;
            curIndex++;
        }
    }


    public class CompanyInfo
    {
        private int index;
        private string name;
        private int rating;
        private string period;
        private string address;

        private string jobkorea_link;
        private string jobplanet_link;

        public CompanyInfo(string name)
        {
            this.name = name;
        }

        public string Name { get => name; set => name = value; }
        public int Rating { get => rating; set => rating = value; }
        public string Period { get => period; set => period = value; }
        public string Address { get => address; set => address = value; }
        public string Jobkorea_link { get => jobkorea_link; set => jobkorea_link = value; }
        public string Jobplanet_link { get => jobplanet_link; set => jobplanet_link = value; }
        public int Index { get => index; set => index = value; }
    }
}
