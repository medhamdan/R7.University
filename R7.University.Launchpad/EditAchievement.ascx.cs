//
//  EditAchievement.ascx.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2014-2018 Roman M. Yagodin
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
using System.Linq;
using R7.Dnn.Extensions.ControlExtensions;
using R7.Dnn.Extensions.Utilities;
using R7.Dnn.Extensions.ViewModels;
using R7.University.ModelExtensions;
using R7.University.Models;
using R7.University.Modules;
using R7.University.Queries;

namespace R7.University.Launchpad
{
    public partial class EditAchievement : UniversityEditPortalModuleBase<AchievementInfo>
    {
        protected EditAchievement () : base ("achievement_id")
        {
        }

        protected override void InitControls ()
        {
            InitControls (buttonUpdate, buttonDelete, linkCancel);
        }

        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            comboAchievementType.DataSource = new FlatQuery<AchievementTypeInfo> (ModelContext).List ()
                .Select (at => new ListItemViewModel (at.AchievementTypeId, at.Localize (LocalResourceFile)));
            
            comboAchievementType.DataBind ();
            comboAchievementType.InsertDefaultItem (LocalizeString ("NotSelected.Text"));
        }

        protected override void LoadItem (AchievementInfo item)
        {
            textTitle.Text = item.Title;
            textShortTitle.Text = item.ShortTitle;
            comboAchievementType.SelectByValue (item.AchievementTypeId);
        }

        protected override void BeforeUpdateItem (AchievementInfo item)
        {
            item.Title = textTitle.Text.Trim ();
            item.ShortTitle = textShortTitle.Text.Trim ();
            item.AchievementTypeId = TypeUtils.ParseToNullable<int> (comboAchievementType.SelectedValue);
        }

        #region Implemented abstract members of UniversityEditPortalModuleBase

        protected override int GetItemId (AchievementInfo item) => item.AchievementID;

        protected override void AddItem (AchievementInfo item)
        {
            ModelContext.Add (item);
            ModelContext.SaveChanges ();
        }

        protected override void UpdateItem (AchievementInfo item)
        {
            ModelContext.Update (item);
            ModelContext.SaveChanges ();
        }

        protected override void DeleteItem (AchievementInfo item)
        {
            ModelContext.Remove (item);
            ModelContext.SaveChanges ();
        }

        #endregion
    }
}

