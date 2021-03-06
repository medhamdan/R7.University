//
//  YearMapping.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2017 Roman M. Yagodin
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

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using R7.University.Models;

namespace R7.University.Data.Mappings
{
    public class YearMapping: EntityTypeConfiguration<YearInfo>
    {
        public YearMapping ()
        {
            ToTable (UniversityMappingHelper.GetTableName<YearInfo> ());
            HasKey (m => m.YearId);
            Property (m => m.YearId).HasDatabaseGeneratedOption (DatabaseGeneratedOption.Identity);

            Property (m => m.Year).IsRequired ();
            Property (m => m.AdmissionIsOpen).IsRequired ();
        }
    }
}

