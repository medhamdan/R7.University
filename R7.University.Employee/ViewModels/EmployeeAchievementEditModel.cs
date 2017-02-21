//
//  EmployeeAchievementEditModel.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2015-2016 Roman M. Yagodin
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
using System.Xml.Serialization;
using DotNetNuke.Services.Localization;
using R7.University.Components;
using R7.University.Models;
using R7.University.ViewModels;
using R7.University.ModelExtensions;

namespace R7.University.Employee.ViewModels
{
    [Serializable]
    public class EmployeeAchievementEditModel: IEmployeeAchievement
    {
        #region IEmployeeAchievement implementation

        public int EmployeeAchievementID { get; set; }

        public int EmployeeID { get; set; }

        public int? AchievementID { get; set; }

        public int? AchievementTypeId { get; set; }

        public string Title { get; set; }

        public string ShortTitle { get; set; }

        public string Description { get; set; }

        public int? YearBegin { get; set; }

        public int? YearEnd { get; set; }

        public bool IsTitle { get; set; }

        public string DocumentURL { get; set; }

        public string TitleSuffix { get; set; }

        public AchievementTypeInfo AchievementType { get; set; }

        [XmlIgnore]
        public AchievementInfo Achievement { get; set; }

        #endregion

        #region Bindable properties

        public int ItemID { get; set; }

        public string Years_String { get; set; }

        public string AchievementType_String { get; set; }

        public string Title_String
        { 
            get { return Title + " " + TitleSuffix; }
        }

        #endregion

        public void Localize (string resourceFile)
        {
            Years_String = FormatHelper.FormatYears (YearBegin, YearEnd).Replace ("{ATM}", Localization.GetString ("AtTheMoment.Text", resourceFile));

            AchievementType_String = Localization.GetString (
                "SystemAchievementType_" + AchievementType.GetSystemAchievementType () + ".Text",
                resourceFile
            );
        }

        public EmployeeAchievementEditModel ()
        {
            ItemID = ViewNumerator.GetNextItemID ();
        }

        public EmployeeAchievementEditModel (IEmployeeAchievement achievement, string resourceFile) : this ()
        {
            CopyCstor.Copy<IEmployeeAchievement> (achievement, this);

            // use base achievement values
            if (achievement.Achievement != null) {
                Title = achievement.Achievement.Title;
                ShortTitle = achievement.Achievement.ShortTitle;
                AchievementTypeId = achievement.Achievement.AchievementID;
                AchievementType = achievement.Achievement.AchievementType;
            }

            Localize (resourceFile);
        }

        public EmployeeAchievementInfo NewEmployeeAchievementInfo ()
        {
            var achievement = new EmployeeAchievementInfo ();
            CopyCstor.Copy<IEmployeeAchievement> (this, achievement);

            if (achievement.AchievementID != null) {
                achievement.Title = null;
                achievement.ShortTitle = null;
                achievement.AchievementTypeId = null;
                achievement.AchievementType = null;
            }

            return achievement;
        }
    }
}
