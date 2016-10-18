using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FullTextSearch.Models
{
    public class Search
    {
        public string Query { get; set; }
        public string SqlQuery { get; set; }
        public SearchType Type { get; set; }
        public SearchConcatenationOperator Operator { get; set; }
        public int NumberOfResults { get; set; } = 20;
        public int Page { get; set; } = 1;
        public long QueryTime { get; set; }

        public IEnumerable<Paper> Results { get; set; }
    }
}
