﻿//
// ViewDivisionDirectory.ascx.cs
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
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Icons;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using R7.DotNetNuke.Extensions.ControlExtensions;
using R7.DotNetNuke.Extensions.ModuleExtensions;
using R7.DotNetNuke.Extensions.Modules;
using R7.DotNetNuke.Extensions.Utilities;
using R7.DotNetNuke.Extensions.ViewModels;
using R7.University.ControlExtensions;
using R7.University.Data;
using R7.University.DivisionDirectory.Components;
using R7.University.ModelExtensions;

namespace R7.University.DivisionDirectory
{
    // TODO: Make module instances co-exist on same page

    public partial class ViewDivisionDirectory : PortalModuleBase<DivisionDirectorySettings>
    {
        #region Session properties

        protected string SearchText
        {
            get { 
                var objSearchText = Session ["DivisionDirectory.SearchText." + TabModuleId];
                return (string) objSearchText ?? string.Empty;
            }
            set { Session ["DivisionDirectory.SearchText." + TabModuleId] = value; }
        }

        protected int SearchDivision
        {
            get { 
                var objSearchDivision = Session ["DivisionDirectory.SearchDivision." + TabModuleId];
                return objSearchDivision != null ? (int) objSearchDivision : Null.NullInteger;

            }
            set { Session ["DivisionDirectory.SearchDivision." + TabModuleId] = value; }
        }

        protected bool SearchIncludeSubdivisions
        {
            get { 
                var objSearchIncludeSubdivisions = Session ["DivisionDirectory.SearchIncludeSubdivisions." + TabModuleId];
                return objSearchIncludeSubdivisions != null ? (bool) objSearchIncludeSubdivisions : true;

            }
            set { Session ["DivisionDirectory.SearchIncludeSubdivisions." + TabModuleId] = value; }
        }

        #endregion

        private ViewModelContext viewModelContext;
        protected ViewModelContext ViewModelContext
        {
            get { 
                if (viewModelContext == null)
                    viewModelContext = new ViewModelContext (this);

                return viewModelContext;
            }
        }

        #region Handlers

        /// <summary>
        /// Handles Init event for a control
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            mviewDivisionDirectory.ActiveViewIndex = R7.University.Utilities.Utils.GetViewIndexByID (
                mviewDivisionDirectory,
                "view" + Settings.Mode);

            if (Settings.Mode == DivisionDirectoryMode.Search) {
                // display search hint
                this.Message ("SearchHint.Info", MessageType.Info, true); 

                var divisions = UniversityRepository.Instance.DataProvider.GetObjects <DivisionInfo> ()
                    .Where (d => d.IsPublished || IsEditable)
                    .OrderBy (d => d.Title).ToList ();
                
                treeDivisions.DataSource = divisions;
                treeDivisions.DataBind ();

                // select first node
                if (treeDivisions.Nodes.Count > 0) {
                    treeDivisions.Nodes [0].Selected = true;
                }

                // REVIEW: Level should be set in settings?
                R7.University.Utilities.Utils.ExpandToLevel (treeDivisions, 2);

                gridDivisions.LocalizeColumns (LocalResourceFile);
            }
            else if (Settings.Mode == DivisionDirectoryMode.ObrnadzorDivisions) {
                gridObrnadzorDivisions.LocalizeColumns (LocalResourceFile);
            }
        }

        /// <summary>
        /// Handles Load event for a control
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnLoad (EventArgs e)
        {
            base.OnLoad (e);
            
            try {
                if (!IsPostBack) {
                    if (Settings.Mode == DivisionDirectoryMode.Search) {
                        if (!string.IsNullOrWhiteSpace (SearchText) || !Null.IsNull (SearchDivision)) {

                            // restore current search
                            textSearch.Text = SearchText;

                            if (Null.IsNull (SearchDivision)) {
                                // select first node
                                if (treeDivisions.Nodes.Count > 0) {
                                    treeDivisions.Nodes [0].Selected = true;
                                }
                            }
                            else {
                                treeDivisions.SelectAndExpandByValue (SearchDivision.ToString ());
                            }

                            // perform search
                            if (SearchParamsOK (SearchText, SearchDivision, false)) {
                                DoSearch (SearchText, SearchDivision);
                            }
                        }
                    }
                    else if (Settings.Mode == DivisionDirectoryMode.ObrnadzorDivisions) {
                        // getting all root divisions
                        var rootDivisions = DivisionRepository.Instance.GetRootDivisions ().OrderBy (d => d.Title);

                        if (rootDivisions.Any ()) {
                            var divisions = new List<DivisionInfo> ();

                            foreach (var rootDivision in rootDivisions) {
                                divisions.AddRange (DivisionRepository.Instance.GetSubDivisions (rootDivision.DivisionID));
                            }

                            // bind divisions to the grid
                            var divisionViewModels = DivisionObrnadzorViewModel.Create (divisions, ViewModelContext);
                            gridObrnadzorDivisions.DataSource = divisionViewModels;
                            gridObrnadzorDivisions.DataBind ();
                        }
                    }
                }
            }
            catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }

        #endregion

        protected bool SearchParamsOK (
            string searchText,
            int searchDivision,
            bool showMessages = true)
        {
            var divisionNotSpecified = Null.IsNull (searchDivision);
            var searchTextIsEmpty = string.IsNullOrWhiteSpace (searchText);

            // no search params - shouldn't perform search
            if (searchTextIsEmpty && divisionNotSpecified) {
                if (showMessages) {
                    this.Message ("SearchParams.Warning", MessageType.Warning, true);
                }

                gridDivisions.Visible = false;
                return false;
            }

            return true;
        }

        protected void DoSearch (string searchText, int searchDivision)
        {
            // REVIEW: If division is not published, it's child divisions also should not
            var divisions = DivisionRepository.Instance
                .FindDivisions (searchText, searchDivision)
                .Where (d => d.IsPublished || IsEditable); 

            if (!divisions.Any ()) {
                this.Message ("NoDivisionsFound.Warning", MessageType.Warning, true);
            }

            gridDivisions.DataSource = divisions;
            gridDivisions.DataBind ();

            // make divisions grid visible anyway
            gridDivisions.Visible = true;
        }

        protected void linkSearch_Click (object sender, EventArgs e)
        {
            var searchText = textSearch.Text.Trim ();
            var searchDivision = (treeDivisions.SelectedNode != null) ? 
                int.Parse (treeDivisions.SelectedNode.Value) : Null.NullInteger;
           
            if (SearchParamsOK (searchText, searchDivision)) {
                // save current search
                SearchText = searchText;
                SearchDivision = searchDivision;
               
                // perform search
                DoSearch (SearchText, SearchDivision);
            }
        }

        protected void grid_RowCreated (object sender, GridViewRowEventArgs e)
        {
            // table header row should be inside <thead> tag
            if (e.Row.RowType == DataControlRowType.Header) {
                e.Row.TableSection = TableRowSection.TableHeader;
            }
        }

        protected void gridDivisions_RowDataBound (object sender, GridViewRowEventArgs e)
        {
            // show / hide edit column
            e.Row.Cells [0].Visible = IsEditable;

            if (e.Row.RowType == DataControlRowType.DataRow) {
                var division = (DivisionInfo) e.Row.DataItem;

                if (IsEditable) {
                    // get edit link controls
                    var linkEdit = (HyperLink) e.Row.FindControl ("linkEdit");
                    var iconEdit = (Image) e.Row.FindControl ("iconEdit");

                    // fill edit link controls
                    linkEdit.NavigateUrl = EditUrl ("division_id", division.DivisionID.ToString (), "EditDivision");
                    iconEdit.ImageUrl = IconController.IconURL ("Edit");
                }

                if (!division.IsPublished) {
                    e.Row.CssClass = "not-published";
                }

                var labelTitle = (Label) e.Row.FindControl ("labelTitle");
                var linkTitle = (HyperLink) e.Row.FindControl ("linkTitle");
                var literalPhone = (Literal) e.Row.FindControl ("literalPhone");
                var linkEmail = (HyperLink) e.Row.FindControl ("linkEmail");
                var literalLocation = (Literal) e.Row.FindControl ("literalLocation");
                var linkDocument = (HyperLink) e.Row.FindControl ("linkDocument");
                var linkContactPerson = (HyperLink) e.Row.FindControl ("linkContactPerson");

                // division label / link
                var divisionTitle = division.Title + ((division.HasUniqueShortTitle) ? string.Format (
                                        " ({0})",
                                        division.ShortTitle) : string.Empty);
                if (!string.IsNullOrWhiteSpace (division.HomePage)) {
                    linkTitle.NavigateUrl = R7.University.Utilities.Utils.FormatURL (this, division.HomePage, false);
                    linkTitle.Text = divisionTitle;
                    labelTitle.Visible = false;
                }
                else {
                    labelTitle.Text = divisionTitle;
                    linkTitle.Visible = false;
                }

                literalPhone.Text = division.Phone;
                literalLocation.Text = division.Location;

                // email
                if (!string.IsNullOrWhiteSpace (division.Email)) {
                    linkEmail.Text = division.Email;
                    linkEmail.NavigateUrl = division.FormatEmailUrl;
                }
                else
                    linkEmail.Visible = false;

                // (main) document
                if (!string.IsNullOrWhiteSpace (division.DocumentUrl)) {
                    linkDocument.Text = LocalizeString ("Regulations.Text");
                    linkDocument.NavigateUrl = Globals.LinkClick (division.DocumentUrl, TabId, ModuleId);

                    // REVIEW: Add GetUrlCssClass() method to the utils or use IconController.GetFileIconUrl () method
                    // set link CSS class according to file extension
                    if (Globals.GetURLType (division.DocumentUrl) == TabType.File) {
                        var fileId = int.Parse (division.DocumentUrl.Remove (0, "FileId=".Length));
                        var file = FileManager.Instance.GetFile (fileId);
                        if (file != null)
                            linkDocument.CssClass = file.Extension.ToLowerInvariant ();
                    }
                }
                else
                    linkDocument.Visible = false;

                // contact person (head employee)
                var contactPerson = DivisionRepository.Instance.GetHeadEmployee (
                                        division.DivisionID,
                                        division.HeadPositionID);
                
                if (contactPerson != null && contactPerson.IsPublished ()) {
                    linkContactPerson.Text = contactPerson.AbbrName;
                    linkContactPerson.ToolTip = contactPerson.FullName;
                    linkContactPerson.NavigateUrl = EditUrl (
                        "employee_id",
                        contactPerson.EmployeeID.ToString (),
                        "EmployeeDetails");
                }
            }
        }

        private int prevLevel = -1;

        protected void gridObrnadzorDivisions_RowDataBound (object sender, GridViewRowEventArgs e)
        {
            // show / hide edit column
            e.Row.Cells [0].Visible = IsEditable;

            if (e.Row.RowType == DataControlRowType.DataRow) {
                var division = (DivisionObrnadzorViewModel) e.Row.DataItem;

                if (IsEditable) {
                    // get edit link controls
                    var linkEdit = (HyperLink) e.Row.FindControl ("linkEdit");
                    var iconEdit = (Image) e.Row.FindControl ("iconEdit");

                    // fill edit link controls
                    linkEdit.NavigateUrl = EditUrl ("division_id", division.DivisionID.ToString (), "EditDivision");
                    iconEdit.ImageUrl = IconController.IconURL ("Edit");
                }

                #region Contact person

                // REVIEW: Should not access database here, maybe in the model? 

                var literalContactPerson = (Literal) e.Row.FindControl ("literalContactPerson");

                // contact person (head employee)
                var contactPerson = DivisionRepository.Instance.GetHeadEmployee (
                                        division.DivisionID,
                                        division.HeadPositionID);
                
                if (contactPerson != null && contactPerson.IsPublished ()) {
                    var headPosition = UniversityRepository.Instance.DataProvider.GetObjects<OccupiedPositionInfoEx> (
                                           "WHERE [EmployeeID] = @0 AND [PositionID] = @1", 
                                           contactPerson.EmployeeID, division.HeadPositionID).FirstOrDefault ();

                    var positionTitle = (!string.IsNullOrWhiteSpace (headPosition.PositionShortTitle)) ?
                        headPosition.PositionShortTitle : headPosition.PositionTitle;

                    literalContactPerson.Text = "<strong><a href=\""
                    + EditUrl ("employee_id", contactPerson.EmployeeID.ToString (), "EmployeeDetails")
                    + "\" itemprop=\"Fio\">" + contactPerson.FullName + "</a></strong><br />"
                    + TextUtils.FormatList (" ", positionTitle, headPosition.TitleSuffix);
                }

                #endregion

                if (!division.IsPublished) {
                    e.Row.CssClass = "not-published";
                }

                #region Beautify (Bootstrap-specific)

                if (division.Level > 0) {
                    e.Row.Cells [2].CssClass = "level-" + division.Level;
                }

                if (prevLevel >= 0) {
                    if (division.Level < prevLevel) {
                        e.Row.CssClass = "return return-" + (prevLevel - division.Level);
                    }
                }

                prevLevel = division.Level;

                #endregion
            }
        }
    }
}

