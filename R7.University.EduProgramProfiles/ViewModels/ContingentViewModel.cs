﻿//
//  ContingentViewModel.cs
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

using System;
using System.Web;
using R7.Dnn.Extensions.ViewModels;
using R7.University.EduProgramProfiles.Models;
using R7.University.ModelExtensions;
using R7.University.Models;

namespace R7.University.EduProgramProfiles.ViewModels
{
    public class ContingentViewModel: IEduProgramProfileFormYear
    {
        protected readonly IEduProgramProfileFormYear FormYear;

        protected readonly ViewModelContext<ContingentDirectorySettings> Context;

        public readonly ContingentDirectoryViewModel RootViewModel;

        public ContingentViewModel (IEduProgramProfileFormYear formYear,
                                    ViewModelContext<ContingentDirectorySettings> context,
                                    ContingentDirectoryViewModel rootViewModel)
        {
            FormYear = formYear;
            Context = context;
            RootViewModel = rootViewModel;
        }

        #region IEduProgramProfileFormYear implementation

        public int EduProgramProfileFormYearId => FormYear.EduProgramProfileFormYearId;

        public int EduProgramProfileId => FormYear.EduProgramProfileId;

        public int EduFormId => FormYear.EduFormId;

        public int YearId => FormYear.YearId;

        public IEduForm EduForm => FormYear.EduForm;

        public IYear Year => FormYear.Year;

        public IEduVolume EduVolume => FormYear.EduVolume;

        public IContingent Contingent => FormYear.Contingent;

        public IEduProgramProfile EduProgramProfile => FormYear.EduProgramProfile;

        public DateTime? StartDate => FormYear.StartDate;

        public DateTime? EndDate => FormYear.EndDate;

        #endregion

        #region Bindable properties

        public string EditUrl =>
            FormYear.Contingent != null
                    ? Context.Module.EditUrl ("contingent_id", FormYear.Contingent.ContingentId.ToString (), "EditContingent")
                    : Context.Module.EditUrl ("eduprogramprofileformyear_id", FormYear.EduProgramProfileFormYearId.ToString (), "EditContingent");

        public string CssClass =>
            FormYear.IsPublished (HttpContext.Current.Timestamp) ? string.Empty : "u8y-not-published";

        public string EduProgramProfileTitle => FormYear.EduProgramProfile.FormatTitle (withEduProgramCode: false);
         
        public string EduFormTitle {
            get {
                var sysEduForm = FormYear.EduForm.GetSystemEduForm ();
                if (sysEduForm != SystemEduForm.Custom) {
                    return Context.LocalizeString ($"EduForm_{sysEduForm}.Text");
                }
                return FormYear.EduForm.Title;
            }
        }

        public string VacantFB => FormatValue (() => FormYear.Contingent?.VacantFB);

        public string VacantRB => FormatValue (() => FormYear.Contingent?.VacantRB);

        public string VacantMB => FormatValue (() => FormYear.Contingent?.VacantMB);

        public string VacantBC => FormatValue (() => FormYear.Contingent?.VacantBC);

        #endregion

        string FormatValue (Func<int?> getValue)
        {
            var value = getValue ();
            return value != null ? value.ToString () : "-";
        }
    }
}
