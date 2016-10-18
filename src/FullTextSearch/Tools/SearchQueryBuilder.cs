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
            return
    @"SELECT 	id, pdfname, eventtype
	, ts_headline(result.title, query) AS title
	, ts_headline(result.abstract, query) AS abstract
	, ts_headline(result.papertext, query) AS papertext
FROM (
	SELECT	*
	FROM    public.paper AS paper
		, to_tsquery('english', '@TSQuery') AS query
		, ts_rank_cd(tsv, query, 2) AS rank
	WHERE   tsv @@ query
	ORDER BY rank DESC
	LIMIT @NumberOfResults OFFSET @Page
) AS result;"
            .Replace("@TSQuery", TSQuery)
            .Replace("@NumberOfResults", numberOfResults.ToString())
            .Replace("@Page", page.ToString());
        }
    }
}
