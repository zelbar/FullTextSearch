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
        public int Page { get; set; }
        public bool MorphologyAndSemantics { get; set; }
        public bool FuzzyMatching { get; set; }

        public IEnumerable<Paper> Results { get; set; }
    }
}
