using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusiBlocks
{
    public class Help
    {
        public string Page { get; set; }
        public string Title { get; set; }
        public string Purpose { get; set; }
        public string Works { get; set; }
        public string Use { get; set; }

        public Help()
        {
            Page = string.Empty;
            Title = string.Empty;
            Purpose = string.Empty;
            Works = string.Empty;
            Use = string.Empty;
        }

        public Help(string page)
            : this()
        {
            Page = page;
        }
    }
}
