﻿//
// ViewVCard.ascx.cs
//
// Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
// Copyright (c) 2014-2015 Roman M. Yagodin
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
using System.Text;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using R7.University;
using R7.University.Data;

namespace R7.University.Employee
{
	public partial class ViewVCard : PortalModuleBase
	{
		#region Handlers

		/// <summary>
		/// Handles Load event for a control
		/// </summary>
		/// <param name="e">Event args.</param>
		protected override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);

			try
			{
				if (!IsPostBack)
				{
					var employee_id = Request.QueryString ["employee_id"];
					if (!string.IsNullOrWhiteSpace (employee_id))
					{
                        var employee = UniversityRepository.Instance.DataProvider.Get<EmployeeInfo> (int.Parse (employee_id));
						if (employee != null)
						{
							var vcard = employee.VCard;

							Response.Clear ();
							Response.ContentType = "text/x-vcard";
							Response.AddHeader ("content-disposition", 
                                string.Format ("attachment; filename=\"{0}.vcf\"", employee.FileName));

							if (Request.Browser.Platform.ToUpperInvariant ().StartsWith ("WIN"))
							{
								// HACK: Windows russian version hack
								// TODO: Need a way to determine language / locale for employee description
								Response.ContentEncoding = Encoding.GetEncoding (1251);
								vcard.Encoding = Response.ContentEncoding;
							}
							else
								Response.ContentEncoding = Encoding.UTF8;

							Response.Write (vcard.ToString ());
							Response.Flush ();
							Response.Close ();
						}
						else
							throw new Exception ("No employee found with EmployeeID=" + employee_id);
					}
					else
						throw new Exception ("\"employee_id\" query parameter should not be empty");
				} 
			}
			catch (Exception ex)
			{
				Exceptions.ProcessModuleLoadException (this, ex);
			}
		}

		#endregion

	}
	// class
}
 // namespace

