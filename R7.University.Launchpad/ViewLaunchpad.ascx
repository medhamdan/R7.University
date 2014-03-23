﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewLaunchpad.ascx.cs" Inherits="R7.University.Launchpad.ViewLaunchpad" %>

<div class="dnnForm">
	<asp:Repeater id="repeatTabs" runat="server" OnItemDataBound="repeatTabs_ItemDataBound">
		<HeaderTemplate>
			<ul class="dnnAdminTabNav dnnClear">
		</HeaderTemplate>
		<ItemTemplate>
			<li id="liTab" runat="server"><asp:LinkButton id="linkTab" runat="server" OnClick="linkTab_Clicked" /></li>
		</ItemTemplate>
		<FooterTemplate>
			</ul>
		</FooterTemplate>
	</asp:Repeater>
	<asp:MultiView id="multiView" runat="server" OnActiveViewChanged="multiView_ActiveViewChanged">
		<asp:View id="viewPositions" runat="server">
			<div id="positions" style="overflow:auto">
					<asp:HyperLink runat="server" id="buttonAddPosition" CssClass="dnnPrimaryAction">Add position</asp:HyperLink><br />
					<asp:GridView id="gridPositions" runat="server" AutoGenerateColumns="true"
					PageSize="15" AllowPaging="true" AllowSorting="true" GridLines="None"
					OnSorting="gridView_Sorting" 
					OnPageIndexChanging="gridView_PageIndexChanging"
					OnRowDataBound="gridView_RowDataBound" CssClass="dnnGrid">
						<HeaderStyle CssClass="dnnGridHeader" horizontalalign="Left" />
				        <RowStyle CssClass="dnnGridItem" horizontalalign="Left" />
				        <AlternatingRowStyle CssClass="dnnGridAltItem" />
				        <EditRowStyle CssClass="dnnFormInput" />
				        <SelectedRowStyle CssClass="dnnFormError" />
				        <FooterStyle CssClass="dnnGridFooter" />
				        <PagerStyle CssClass="dnnGridPager" />
						<Columns>
							<asp:TemplateField>
								<ItemTemplate>
					                	<asp:HyperLink id="linkEdit" runat="server">
					                		<asp:Image runat="server" ImageUrl="~/images/edit.gif" />
					                	</asp:HyperLink>
					            </ItemTemplate>
					        </asp:TemplateField>
						</Columns>
						<PagerSettings Mode="Numeric" Visible="true" />
			        </asp:GridView>
			        <br />
				</div>
		</asp:View>
		<asp:View id="viewDivisions" runat="server"> 
			<div id="divisions" style="overflow:auto">
				<asp:HyperLink runat="server" id="buttonAddDivision" CssClass="dnnPrimaryAction">Add division</asp:HyperLink><br />
				<asp:GridView id="gridDivisions" runat="server" AutoGenerateColumns="true" 
				PageSize="15" AllowPaging="true" AllowSorting="true" GridLines="None"
				OnSorting="gridView_Sorting" 
				OnPageIndexChanging="gridView_PageIndexChanging"
				OnRowDataBound="gridView_RowDataBound">
					<HeaderStyle CssClass="dnnGridHeader" horizontalalign="Left" />
			        <RowStyle CssClass="dnnGridItem" horizontalalign="Left" />
			        <AlternatingRowStyle CssClass="dnnGridAltItem" />
			        <SelectedRowStyle CssClass="dnnFormError" />
			        <EditRowStyle CssClass="dnnFormInput" />
			        <FooterStyle CssClass="dnnGridFooter" />
			        <PagerStyle CssClass="dnnGridPager" />
					<Columns>
						<asp:TemplateField>
			               <ItemTemplate>
			                	<asp:HyperLink id="linkEdit" runat="server">
			                		<asp:Image runat="server" ImageUrl="~/images/edit.gif" />
			                	</asp:HyperLink>
			               </ItemTemplate>
			        	</asp:TemplateField>
					</Columns>
					<PagerSettings Mode="Numeric" Visible="true" />
		        </asp:GridView>
		        <br />
		    </div>
		</asp:View>
		<asp:View id="viewEmployees" runat="server">
			<div id="employees" style="overflow:auto">
				<asp:HyperLink runat="server" id="buttonAddEmployee" CssClass="dnnPrimaryAction">Add employee</asp:HyperLink><br />
				<asp:GridView id="gridEmployees" runat="server" AutoGenerateColumns="true" 
				PageSize="15" AllowPaging="true" AllowSorting="true" GridLines="None" 
				OnSorting="gridView_Sorting" 
				OnPageIndexChanging="gridView_PageIndexChanging"
				OnRowDataBound="gridView_RowDataBound">
					<HeaderStyle CssClass="dnnGridHeader" horizontalalign="Left" />
			        <RowStyle CssClass="dnnGridItem" horizontalalign="Left" />
			        <AlternatingRowStyle CssClass="dnnGridAltItem" />
			        <SelectedRowStyle CssClass="dnnFormError" />
			        <EditRowStyle CssClass="dnnFormInput" />
			        <FooterStyle CssClass="dnnGridFooter" />
			        <PagerStyle CssClass="dnnGridPager" />
			        <Columns>
						<asp:TemplateField>
			               <ItemTemplate>
			                	<asp:HyperLink id="linkEdit" runat="server">
			                		<asp:Image runat="server" ImageUrl="~/images/edit.gif" />
			                	</asp:HyperLink>
			               </ItemTemplate>
			        	</asp:TemplateField>
					</Columns>
					<PagerSettings Mode="Numeric" Visible="true" />
		        </asp:GridView>
		        <br />
		    </div>
		</asp:View>
	</asp:MultiView>
</div>