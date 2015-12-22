﻿//
// SettingsEduProgramProfileDirectory.ascx.cs
//
// Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
// Copyright (c) 2015 Roman M. Yagodin
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.UserControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Web.UI.WebControls;
using DotNetNuke.R7;
using R7.University;

namespace R7.University.EduProgramProfileDirectory
{
    public partial class SettingsEduProgramProfileDirectory 
        : ExtendedModuleSettingsBase<EduProgramProfileDirectoryController, EduProgramProfileDirectorySettings>
    {
        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            // fill edulevels list
            var eduLevels = Controller.GetObjects<EduLevelInfo> ().OrderBy (el => el.SortIndex);

            foreach (var eduLevel in eduLevels)
            {
                listEduLevels.Items.Add (new DnnListBoxItem { 
                    Text = eduLevel.DisplayShortTitle, 
                    Value = eduLevel.EduLevelID.ToString ()
                });
            }
        }

        /// <summary>
        /// Handles the loading of the module setting for this control
        /// </summary>
        public override void LoadSettings ()
        {
            try
            {
                if (!IsPostBack)
                {
                    // check edulevels list items
                    foreach (var eduLevelId in Settings.EduLevels)
                    {
                        var item = listEduLevels.FindItemByValue (eduLevelId.ToString ());
                        if (item != null)
                        {
                            item.Checked = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }

        /// <summary>
        /// handles updating the module settings for this control
        /// </summary>
        public override void UpdateSettings ()
        {
            try
            {
                Settings.EduLevels = listEduLevels.CheckedItems.Select (i => int.Parse (i.Value)).ToList ();

                ModuleController.SynchronizeModule (ModuleId);
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }
    }
}
