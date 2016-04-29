﻿//
// EditDivision.ascx.cs
//
// Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
// Copyright (c) 2014-2016 Roman M. Yagodin
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
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Services.Localization;
using R7.DotNetNuke.Extensions.ControlExtensions;
using R7.DotNetNuke.Extensions.Modules;
using R7.DotNetNuke.Extensions.Utilities;
using R7.University;
using R7.University.ControlExtensions;
using R7.University.Data;

namespace R7.University.Division
{
    public partial class EditDivision: EditPortalModuleBase<DivisionInfo,int>
	{
        private int? itemId;

        #region Types

        public enum EditDivisionTab { Common, Contacts, Documents, Bindings };

        #endregion

        #region Properties

        private DivisionSettings settings;
        protected new DivisionSettings Settings
        {
            get { return settings ?? (settings = new DivisionSettings (this)); }
        }

        protected EditDivisionTab SelectedTab
        {
            get 
            {
                // get postback initiator
                var eventTarget = Request.Form ["__EVENTTARGET"];

                if (!string.IsNullOrEmpty (eventTarget) && eventTarget.Contains ("$" + urlHomePage.ID +"$"))
                {
                    ViewState ["SelectedTab"] = EditDivisionTab.Bindings;
                    return EditDivisionTab.Bindings;
                }

                if (!string.IsNullOrEmpty (eventTarget) && eventTarget.Contains ("$" + urlDocumentUrl.ID +"$"))
                {
                    ViewState ["SelectedTab"] = EditDivisionTab.Documents;
                    return EditDivisionTab.Documents;
                }

                // otherwise, get current tab from viewstate
                var obj = ViewState ["SelectedTab"];
                return (obj != null) ? (EditDivisionTab)obj : EditDivisionTab.Common;
            }
            set { ViewState ["SelectedTab"] = value; }
        }

        #endregion

        protected EditDivision (): base ("division_id")
        {}

        /// <summary>
        /// Handles Init event for a control.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            // parse QueryString
            itemId = TypeUtils.ParseToNullable<int> (Request.QueryString ["division_id"]);

            // fill divisions dropdown
            var divisions = UniversityRepository.Instance.DataProvider.GetObjects<DivisionInfo> ()
                // exclude current division
                .Where (d => (itemId == null || itemId != d.DivisionID)).OrderBy (dd => dd.Title).ToList ();

            // insert default item
            divisions.Insert (0, DivisionInfo.DefaultItem (LocalizeString ("NotSelected.Text")));

            // bind divisions to the tree
            treeParentDivisions.DataSource = divisions;
            treeParentDivisions.DataBind ();

            // init working hours
            WorkingHoursLogic.Init (this, comboWorkingHours);

            // Fill terms list
            // REVIEW: Org. structure vocabulary name must be set in settings?
            var termCtrl = new TermController ();
            var terms = termCtrl.GetTermsByVocabulary ("University_Structure").ToList (); 

            // add default term, 
            // TermId = Null.NullInteger is set in cstor
            terms.Insert (0, new Term (Localization.GetString ("NotSelected.Text", LocalResourceFile)));

            // bind terms to the tree
            treeDivisionTerms.DataSource = terms;
            treeDivisionTerms.DataBind ();

            // bind positions
            var positions = UniversityRepository.Instance.DataProvider.GetObjects<PositionInfo> ().OrderBy (p => p.Title).ToList ();
            positions.Insert (0, new PositionInfo { ShortTitle = LocalizeString ("NotSelected.Text"), PositionID = Null.NullInteger });
            comboHeadPosition.DataSource = positions;
            comboHeadPosition.DataBind ();
            comboHeadPosition.SelectedIndex = 0;
        }

        protected override void InitControls ()
        {
            InitControls (buttonUpdate, buttonDelete, linkCancel);
        }

        protected override void LoadItem (DivisionInfo item)
        {
            // FIXME: Need support in EditModuleBase to drop this on top of OnLoad method
            // if (DotNetNuke.Framework.AJAX.IsInstalled ())
            //    DotNetNuke.Framework.AJAX.RegisterScriptManager ();
            
            txtTitle.Text = item.Title;
            txtShortTitle.Text = item.ShortTitle;
            txtWebSite.Text = item.WebSite;
            textWebSiteLabel.Text = item.WebSiteLabel;
            txtEmail.Text = item.Email;
            txtSecondaryEmail.Text = item.SecondaryEmail;
            txtLocation.Text = item.Location;
            txtPhone.Text = item.Phone;
            txtFax.Text = item.Fax;
            datetimeStartDate.SelectedDate = item.StartDate;
            datetimeEndDate.SelectedDate = item.EndDate;
            checkIsVirtual.Checked = item.IsVirtual;
            comboHeadPosition.SelectByValue (item.HeadPositionID);

            // load working hours
            WorkingHoursLogic.Load (comboWorkingHours, textWorkingHours, item.WorkingHours);

            // select parent division
            Utils.SelectAndExpandByValue (treeParentDivisions, item.ParentDivisionID.ToString ());

            // select taxonomy term
            var treeNode = treeDivisionTerms.FindNodeByValue (item.DivisionTermID.ToString ());
            if (treeNode != null)
            {
                treeNode.Selected = true;

                // expand all parent nodes
                treeNode = treeNode.ParentNode;
                while (treeNode != null)
                {
                    treeNode.Expanded = true;
                    treeNode = treeNode.ParentNode;
                } 
            }
            else
                treeDivisionTerms.Nodes [0].Selected = true;

            // set HomePage url
            if (!string.IsNullOrWhiteSpace (item.HomePage))
                urlHomePage.Url = item.HomePage;
            else
                // or set to "None", if Url is empty
                urlHomePage.UrlType = "N";

            // set Document url
            if (!string.IsNullOrWhiteSpace (item.DocumentUrl))
                urlDocumentUrl.Url = item.DocumentUrl;
            else
                // or set to "None", if url is empty
                urlDocumentUrl.UrlType = "N";

            ctlAudit.Bind (item);
        }

        protected override void BeforeUpdateItem (DivisionInfo item)
        {
            // fill the object
            item.Title = txtTitle.Text.Trim ();
            item.ShortTitle = txtShortTitle.Text.Trim ();
            item.Email = txtEmail.Text.Trim ().ToLowerInvariant ();
            item.SecondaryEmail = txtSecondaryEmail.Text.Trim ().ToLowerInvariant ();
            item.Phone = txtPhone.Text.Trim ();
            item.Fax = txtFax.Text.Trim ();
            item.Location = txtLocation.Text.Trim ();
            item.WebSite = txtWebSite.Text.Trim ();
            item.WebSiteLabel = textWebSiteLabel.Text.Trim ();
            item.ParentDivisionID = TypeUtils.ParseToNullable<int> (treeParentDivisions.SelectedValue);
            item.DivisionTermID = TypeUtils.ParseToNullable<int> (treeDivisionTerms.SelectedValue);
            item.HomePage = urlHomePage.Url;
            item.DocumentUrl = urlDocumentUrl.Url;
            item.StartDate = datetimeStartDate.SelectedDate;
            item.EndDate = datetimeEndDate.SelectedDate;
            item.IsVirtual = checkIsVirtual.Checked;
            item.HeadPositionID = TypeUtils.ParseToNullable<int> (comboHeadPosition.SelectedValue);

            // update working hours
            item.WorkingHours = WorkingHoursLogic.Update (comboWorkingHours, textWorkingHours.Text, checkAddToVocabulary.Checked);
        }

        #region implemented abstract members of EditPortalModuleBase

        protected override DivisionInfo GetItem (int itemId)
        {
            return UniversityRepository.Instance.DataProvider.Get<DivisionInfo> (itemId);
        }

        protected override int AddItem (DivisionInfo item)
        {
            // update audit info
            item.CreatedByUserID = item.LastModifiedByUserID = UserId;
            item.CreatedOnDate = item.LastModifiedOnDate = DateTime.Now;

            UniversityRepository.Instance.DataProvider.Add<DivisionInfo> (item);

            // then adding new division from Division module, 
            // set calling module to display new division info
            if (ModuleConfiguration.ModuleDefinition.DefinitionName == "R7.University.Division")
            {
                Settings.DivisionID = item.DivisionID;
            }

            return item.DivisionID;
        }

        protected override void UpdateItem (DivisionInfo item)
        {
            // update audit info
            item.LastModifiedByUserID = UserId;
            item.LastModifiedOnDate = DateTime.Now;

            UniversityRepository.Instance.DataProvider.Update<DivisionInfo> (item);
        }

        protected override void DeleteItem (DivisionInfo item)
        {
            UniversityRepository.Instance.DataProvider.Delete<DivisionInfo> (item);
        }

        #endregion
	}
}

