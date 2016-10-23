using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullTextSearch.Models;
using System.Text;

namespace FullTextSearch.Tools
{
    public class AnalysisQueryBuilder
    {
        private readonly Analysis _analysis;

        public AnalysisQueryBuilder(Analysis analysis)
        {
            _analysis = analysis;
        }

        public string Build()
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"SELECT 	*
FROM    crosstab('");
            sb.AppendLine();

            if (_analysis.Type == AnalysisType.DayHour)
            {

            }
            else if (_analysis.Type == AnalysisType.YearMonth)
            {
                sb.AppendLine("		SELECT	EXTRACT(YEAR FROM \"timestamp\")::INT AS year");
                sb.AppendLine("		, EXTRACT(MONTH FROM \"timestamp\")::INT AS month");
                sb.AppendLine("     , COUNT(*) AS queries");

            
            }

            sb.AppendLine(@"
        FROM    public.search");
            sb.AppendLine("     WHERE 	\"timestamp\" >= ''" + _analysis.From + "'' AND");
            sb.AppendLine("         \"timestamp\" <= ''" + _analysis.To + "'' AND");
            sb.AppendLine("			query = ''" + _analysis.Term + "''");

            if (_analysis.Type == AnalysisType.DayHour)
            {

            }
            else if (_analysis.Type == AnalysisType.YearMonth)
            {
                sb.AppendLine(@"		GROUP BY year, month
        ORDER BY year, month',
	'SELECT number FROM month;') AS pt(YEAR INT, jan INT, feb INT, mar INT, apr INT, may INT, jun INT, jul INT, aug INT, sep INT, oct INT, nov INT, dec INT)
ORDER BY year;");
            }

            return sb.ToString();
        }
    }
}
