﻿//
//  DivisionRepository.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016 Roman M. Yagodin
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using R7.DotNetNuke.Extensions.Data;
using R7.DotNetNuke.Extensions.Utilities;
using R7.University.Models;

namespace R7.University.Data
{
    [Obsolete]
    public class DivisionRepository
    {
        protected Dal2DataProvider DataProvider;

        public DivisionRepository (Dal2DataProvider dataProvider)
        {
            DataProvider = dataProvider;
        }

        #region Singleton implementation

        private static readonly Lazy<DivisionRepository> instance = new Lazy<DivisionRepository> (
            () => new DivisionRepository (UniversityDataProvider.Instance)
        );

        public static DivisionRepository Instance
        {
            get { return instance.Value; }
        }

        #endregion

        public IEnumerable<DivisionInfo> FindDivisions (string searchText, int divisionId)
        {
            // TODO: Remove @includeSubdivision argument from sp
            return DataProvider.GetObjectsFromSp<DivisionInfo> ("{databaseOwner}[{objectQualifier}University_FindDivisions]", 
                searchText, true, divisionId);
        }
    }
}
