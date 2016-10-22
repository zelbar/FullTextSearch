using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FullTextSearch.Models;

namespace FullTextSearch.Tools
{
    public class SearchQueryBuilder
    {
        private readonly string _userQuery;
        private readonly SearchConcatenationOperator _op;

        /// <summary>
        /// Parses a plain user input search query and returns it in TSQuery form for PostgreSQL Full Text Search.
        /// </summary>
        /// <example>
        /// The plain user input <code>"new car"</code> would be parsed into <code>"new & car"</code> (just as plain_totsquery would do).
        /// However, <code>new car</code> would become <code>new | car</code>.
        /// A more complex example: <code>"fast new" car boat</code> becomes <code>(fast & new) | car | boat</code>.
        /// Furthermore, <code>(fast or cheap) and car</code> becomes <code>(fast | cheap) & car</code>.
        /// </example>
        /// <param name="plainQuery">The plain user input search query containing spaces ( ), double quotes (\") and keywords like and, or, not.</param>
        public SearchQueryBuilder(string userQuery, SearchConcatenationOperator op)
        {
            _userQuery = userQuery;
            _op = op;
        }

        /// <summary>
        /// PostgreSQL TSQuery equivalent.
        /// </summary>
        public string TSQuery
        {
            get
            {
                var rv = new StringBuilder();
                var quotes = Regex.Matches(_userQuery, "\"([^\"]*)\"");
                var concatenator = (_op == SearchConcatenationOperator.and) ? '&' : '|';
                var quotedList = new List<string>();
                foreach (var quote in quotes)
                {
                    quotedList.Add((quote.ToString().Replace("\"", "").Replace(' ', '&')));
                }
                rv.Append(string.Join(concatenator.ToString(), quotedList));
                
                var withoutQuotesSb = new StringBuilder();
                for (int i = 0; i < _userQuery.Length; ++i)
                {
                    if (_userQuery[i] == '\"')
                    do { ++i; } while (_userQuery[i] != '\"');
                    else
                        withoutQuotesSb.Append(_userQuery[i]);
                }
                string withoutQuotes = withoutQuotesSb.ToString().Trim();
                if (quotedList.Count > 0 && withoutQuotes.Count() > 0)
                    rv.Append(concatenator);
                rv.Append(withoutQuotes.ToString().Replace(' ', concatenator));
                return rv.ToString();
            }
        }

        public string SqlQuery(SearchType type, int numberOfResults = 10, int page = 1)
        {
            if (page < 1) page = 1;
            var sb = new StringBuilder();
            sb.AppendLine("SELECT 	id, pdfname, eventtype");
            sb.AppendLine("	, ts_headline(result.title, query, \'HighlightAll=true\') AS title");
            sb.AppendLine("	, ts_headline(result.abstract, query, \'MaxFragments=2,MaxWords=25,MinWords=12\') AS abstract");
            sb.AppendLine("	, ts_headline(result.papertext, query, \'MaxFragments=4,MaxWords=50,MinWords=25,FragmentDelimiter=<br/><br/>\') AS papertext");
            sb.AppendLine(@"FROM (
	SELECT	*
	FROM    public.paper AS paper");
            sb.AppendLine("	, to_tsquery('english', '" + TSQuery + "') AS query");
            sb.AppendLine(@"	, ts_rank_cd(tsv, query, 2) AS rank
	WHERE   tsv @@ query
	ORDER BY rank DESC");
            sb.AppendLine("	LIMIT " + numberOfResults.ToString()+" OFFSET ("+page.ToString()+" - 1) * "+numberOfResults.ToString()+"");
            sb.AppendLine(") AS result;");
            return sb.ToString();
        }
    }
}
