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
                sb.AppendLine("     SELECT  EXTRACT(MONTH FROM \"timestamp\")||''-''||EXTRACT(DAY FROM \"timestamp\")||''-''||EXTRACT(YEAR FROM \"timestamp\") AS day");
                sb.AppendLine("             , EXTRACT(HOUR FROM \"timestamp\")::INT AS hour");
                sb.AppendLine("             , COUNT(*)");
            }
            else if (_analysis.Type == AnalysisType.YearMonth)
            {
                sb.AppendLine("		    SELECT	EXTRACT(YEAR FROM \"timestamp\")::INT AS year");
                sb.AppendLine("		        , EXTRACT(MONTH FROM \"timestamp\")::INT AS month");
                sb.AppendLine("             , COUNT(*) AS queries");
            }

            sb.AppendLine(@"
        FROM    public.search");
            sb.AppendLine("         WHERE 	\"timestamp\" >= ''" + _analysis.From + "'' AND");
            sb.AppendLine("                 \"timestamp\" <= ''" + _analysis.To + "'' AND");
            sb.AppendLine("			        query = ''" + _analysis.Term + "''");

            if (_analysis.Type == AnalysisType.DayHour)
            {
                sb.AppendLine(@"        GROUP BY day, hour
        ORDER BY day, hour',
	'SELECT number FROM hour;') AS pt(date text, ""0 AM"" INT, ""1 AM"" INT, ""2 AM"" INT, ""3 AM"" INT, ""4 AM"" INT, ""5 AM"" INT, ""6 AM"" INT, ""7 AM"" INT, ""8 AM"" INT, ""9 AM"" INT, ""10 AM"" INT, ""11 AM"" INT, ""12 PM"" INT, ""1 PM"" INT, ""2 PM"" INT, ""3 PM"" INT, ""4 PM"" INT, ""5 PM"" INT, ""6 PM"" INT, ""7 PM"" INT, ""8 PM"" INT, ""9 PM"" INT, ""10 PM"" INT, ""11 PM"" INT)
ORDER BY date;");
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
