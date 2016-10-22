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
                var query = _userQuery;
                var quotes = Regex.Matches(query, "\"([^\"]*)\"");
                var concatenator = (_op == SearchConcatenationOperator.and) ? '&' : '|';
                var quotedList = new List<string>();
                foreach (var quote in quotes)
                {
                    quotedList.Add((quote.ToString().Replace("\"", "").Replace(' ', '&')));
                }
                var rv = new StringBuilder();
                rv.Append(string.Join(concatenator.ToString(), quotedList));
                
                var withoutQuotesSb = new StringBuilder();
                for (int i = 0; i < query.Length; ++i)
                {
                    if (query[i] == '\"')
                        do { ++i; } while (query[i] != '\"');
                    else
                        withoutQuotesSb.Append(query[i]);
                }
                string withoutQuotes = withoutQuotesSb.ToString().Trim();
                if (quotedList.Count > 0 && withoutQuotes.Count() > 0)
                    rv.Append(concatenator);
                rv.Append(withoutQuotes.ToString().Replace(' ', concatenator));
                return rv.ToString();
            }
        }

        /// <summary>
        /// Query user for similarity check (Trigram) in PostgreSQL
        /// </summary>
        public string SimilarityQuery
        {
            get
            {
                return _userQuery.Trim();
            }
        }

        public string SqlQuery(SearchType type, int numberOfResults = 10, int page = 1)
        {
            if (page < 1) page = 1;
            var sb = new StringBuilder();
            sb.AppendLine("SELECT 	id, pdfname, eventtype");
            sb.AppendLine("	, ts_headline(result.title, tsquery, \'HighlightAll=true\') AS title");
            sb.AppendLine("	, ts_headline(result.abstract, tsquery, \'MaxFragments=2,MaxWords=50,MinWords=12\') AS abstract");
            sb.AppendLine("	, ts_headline(result.papertext, tsquery, \'MaxFragments=4,MaxWords=75,MinWords=25,FragmentDelimiter=<br/><br/>\') AS papertext");

            if (type == SearchType.MorphologyAndSemantics) sb.AppendLine("	, rank");
            else if (type == SearchType.FuzzyMatching) sb.AppendLine("	, titlesim + abstractsim AS rank");

            sb.AppendLine(@"FROM (
	SELECT	*
	FROM    public.paper AS paper");
            sb.AppendLine("		, to_tsquery('english', '" + TSQuery + "') AS tsquery");

            if (type == SearchType.MorphologyAndSemantics)
            {
                sb.AppendLine("		, ts_rank_cd(tsv, tsquery, 2) AS rank");
                sb.AppendLine("	WHERE   tsv @@ tsquery");
                sb.AppendLine("	ORDER BY rank DESC");
            }
            else if (type == SearchType.FuzzyMatching)
            {
                sb.AppendLine(@"		, similarity(lower(paper.title), lower('"+SimilarityQuery+"')) AS titlesim");
                sb.AppendLine("		, similarity(lower(paper.abstract), lower('"+SimilarityQuery+"')) AS abstractsim");
                sb.AppendLine("	WHERE   titlesim + abstractsim >= 0.05");
                sb.AppendLine("	ORDER BY titlesim + abstractsim DESC");
            }
            
            sb.AppendLine("	LIMIT " + numberOfResults.ToString()+" OFFSET ("+page.ToString()+" - 1) * "+numberOfResults.ToString()+"");
            sb.AppendLine(") AS result;");
            return sb.ToString();
        }
    }
}
