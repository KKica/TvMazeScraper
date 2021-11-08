using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TvMazeScraper.Controllers.Paging
{
    public static class PagingUtils
    {
        private static int MaxPageSize = 250;
        public static Task<List<TDestionation>> Page<TSource, TDestionation>(
            this IQueryable<TSource> queryable,
            IMapper mapper,
            PageRequest pageRequest,
            CancellationToken cancellationToken
        )
        {
            int skip = Math.Max(pageRequest.Skip, 0);
            int take = Math.Min(Math.Max(pageRequest.Take, 1), MaxPageSize);

            return mapper.ProjectTo<TDestionation>(queryable.Skip(skip).Take(take))
                .ToListAsync(cancellationToken);
        }
    }
}
