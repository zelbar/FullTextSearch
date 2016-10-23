using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FullTextSearch.Models
{
    public enum SearchConcatenationOperator { and, or };
    public enum SearchType { MorphologyAndSemantics, FuzzyMatching };
    public enum AnalysisType { YearMonth, DayHour };
}
