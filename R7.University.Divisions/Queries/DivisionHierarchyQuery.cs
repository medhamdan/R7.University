//
//  DivisionHierarchyQuery.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016 Roman M. Yagodin
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Affero General Public License for more details.
//
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Linq;
using R7.University.ModelExtensions;
using R7.University.Models;
using R7.University.Queries;

namespace R7.University.Divisions.Queries
{
    class DivisionHierarchyQuery: QueryBase
    {
        public DivisionHierarchyQuery (IModelContext modelContext): base (modelContext)
        {
        }

        public IEnumerable<DivisionInfo> ListHierarchy ()
        {
            return ModelContext.Query<DivisionInfo> ()
                               .IncludeOccupiedPositions ()
                               .ToList ()
                               .CalculateLevelAndPath ()
                               .OrderBy (d => d.Path)
                               .ThenBy (d => d.Title);
        }

        public IEnumerable<DivisionInfo> ListGoverningHierarchy ()
        {
            return ModelContext.Query<DivisionInfo> ()
                               .Where (d => d.IsGoverning)
                               .IncludeOccupiedPositions ()
                               .ToList ()
                               .CalculateLevelAndPath ()
                               .OrderBy (d => d.Path)
                               .ThenBy (d => d.Title);
        }
    }

    static class DivisionQueryExtensions
    {
        internal static IQueryable<DivisionInfo> IncludeOccupiedPositions (this IQueryable<DivisionInfo> divisions)
        {
            return divisions.Include (d => d.OccupiedPositions)
                            .Include (d => d.OccupiedPositions.Select (op => op.Position))
                            .Include (d => d.OccupiedPositions.Select (op => op.Employee));
        }
    }
}

