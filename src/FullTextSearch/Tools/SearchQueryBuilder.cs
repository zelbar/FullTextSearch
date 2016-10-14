using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullTextSearch.Tools
{
    public class SearchQueryBuilder
    {
        private readonly string _plainQuery;

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
        public SearchQueryBuilder(string plainQuery)
        {
            _plainQuery = plainQuery;
        }

        /// <summary>
        /// PostgreSQL TSQuery equivalent.
        /// </summary>
        public string TSQuery
        {
            get
            {
                return _plainQuery.Replace(' ', '|');
            }
        }

        public string SqlQuery(bool morphologyAndSemantics, bool fuzzyMatching, int results = 10, int page = 1)
        {
            return @"SELECT id, pdfname, abstract, 
	similarity(title, '@Term') sim,
	ts_rank_cd(to_tsvector(title), '@Term'::TSQuery, 2) rank,
	ts_headline(title, '@Term'::TSQuery) AS title,
	ts_headline(papertext, '@Term'::TSQuery) AS papertext
  FROM public.paper
  WHERE papertext @@ '@Term'::TSQuery
  ORDER BY rank DESC, sim DESC
  LIMIT @Results OFFSET @Page;"
            .Replace("@Term", this.TSQuery)
            .Replace("@Results", results.ToString())
            .Replace("@Page", page.ToString());
        }
    }
}
