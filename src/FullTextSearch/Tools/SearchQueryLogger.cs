using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FullTextSearch.Models;
using Dapper;

namespace FullTextSearch.Tools
{
    public class SearchQueryLogger
    {
        public async Task Log(IDbConnection connection, Search search)
        {
            await connection.ExecuteAsync(
                @"INSERT INTO public.search(
            query, type, operator, numberofresults, page, querytime, timestamp)
    VALUES('@Query', '@Type', '@Operator', @Numberofresults, @Page, @QTime, '@Timestamp')
            ".Replace("@Query", search.Query)
            .Replace("@Operator", search.Operator.ToString())
            .Replace("@Type", search.Type.ToString())
            .Replace("@Numberofresults", search.NumberOfResults.ToString())
            .Replace("@Page", search.Page.ToString())
            .Replace("@QTime", search.QueryTime.ToString())
            .Replace("@Timestamp", DateTime.UtcNow.ToString())
            );
        }

    }
}
