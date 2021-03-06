﻿//$Header:$
//
// U.S. Department of Energy under contract number DE-AC02-76SF00515
// DOE O 241.1B, SCIENTIFIC AND TECHNICAL INFORMATION MANAGEMENT In the performance of Department of Energy(DOE) contracted obligations, each contractor is required to manage scientific and technical information(STI) produced under the contract as a direct and integral part of the work and ensure its broad availability to all customer segments by making STI available to DOE's central STI coordinating office, the Office of Scientific and Technical Information (OSTI).
//  ClauseList.ascx.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2013 SLAC. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OracleClient;
using System.Text;
using System.IO;

namespace ContractManagement.User_Controls
{
    public partial class ClauseList : System.Web.UI.UserControl
    {
        # region "Object Instances"
        Data.CMS_DMLUtil objDml = new Data.CMS_DMLUtil();
        Data.CMS_DataUtil objData = new Data.CMS_DataUtil();
        Business.CMS_Common_Util objCommon = new Business.CMS_Common_Util();
        # endregion

        # region "Page Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            TxtClauseName.Attributes.Add("onkeydown", "return onKeypress('cmdFind');");
            
            if (!Page.IsPostBack)
            {
                if (!IsSpecialForReport())
                {
                   Lblhead.Text = Lblhead.Text.Replace("all", "your");
                   TblSearch.Visible = false;
                }
                BindClause();
               
            }
        }
        # endregion

        #region "User Functions"

       
        private bool IsSpecial()
        {
            if ( ViewState["isspecial"] != null)
            {
                return (bool)ViewState["isspecial"];
            }
            else
            {
                bool _isspecial = false;
                (this.Page as BasePage).CheckIfManager();
                if ((!(this.Page as BasePage)._admin) && (!(this.Page as BasePage)._cma) && (!(this.Page as BasePage)._ald) && (!(this.Page as BasePage)._diradmin))
                    _isspecial = false;
                else _isspecial = true;
                ViewState["isspecial"] = _isspecial;
                return _isspecial;
            }
        }

        private bool IsSpecialForReport()
        {
            if (ViewState["isspecialRep"] != null)
            {
                return (bool)ViewState["isspecialRep"];
            }
            else
            {
                bool _isspecialForRep = false;
                (this.Page as BasePage).CheckIfManager();
                if ((!(this.Page as BasePage)._admin) && (!(this.Page as BasePage)._cma) && (!(this.Page as BasePage)._ald) && (!(this.Page as BasePage)._diradmin)
                    && (!(this.Page as BasePage)._sso) && (! (this.Page as BasePage)._ssosuper))
                    _isspecialForRep = false;
                else _isspecialForRep = true;
                ViewState["isspecialRep"] = _isspecialForRep;
                return _isspecialForRep;
            }
        }

        private void BindClause()
        {
            
            StringBuilder _sbFilter = new StringBuilder();
         
            _sbFilter.Append(" WHERE LOWER(CL.CLAUSETYPE) = 'clause' ");
            using (OracleCommand _cmdList = new OracleCommand())
            {
                if (TblSearch.Visible)
                {
                    if (TxtClauseName.Text != "")
                    {
                        _sbFilter = objCommon.SetSBFilter(_sbFilter);
                        if (!(IsSpecial())) _sbFilter.Append(" (");
                        _sbFilter.Append(" ( LOWER(CL.CLAUSE_NAME) LIKE :Clause OR LOWER(CL.CLAUSE_NUMBER) LIKE :Clause OR LOWER(CL.SUBCLAUSENAME) LIKE :Clause");
                        _sbFilter.Append(" OR LOWER(CL.SUBCLAUSENUM) LIKE :Clause  OR LOWER(CL.CONTRACT_NAME) LIKE :Clause OR LOWER(CL.CLAUSETYPE) LIKE :Clause");
                        //if (!(IsSpecial()))
                        //{
                        //    _sbFilter.Append(" ) AND");
                        //    _sbFilter.Append(" LOWER(CL.OWNERNAME) LIKE :Empname ) ");
                        //    _cmdList.Parameters.Add(":Empname", OracleType.VarChar).Value = "%" + objCommon.GetEmpname(objCommon.GetUserID()).ToLower () + "%"; ;
                        //}
                        //else
                        //{
                        _sbFilter.Append(" OR LOWER(CL.OWNERNAME) LIKE :Clause ) ");
                        //}

                        _cmdList.Parameters.Add(":Clause", OracleType.VarChar).Value = "%" + Server.HtmlEncode(TxtClauseName.Text.ToLower()) + "%";
                    }
                }
                else
                {
                    if (!(IsSpecialForReport()))
                    {
                        _sbFilter = objCommon.SetSBFilter(_sbFilter);
                        _sbFilter.Append(" LOWER(CL.OWNERNAME) LIKE :Empname");
                        _cmdList.Parameters.Add(":Empname", OracleType.VarChar).Value = "%" + objCommon.GetEmpname(objCommon.GetUserID()).ToLower() + "%";
                    }
                }
               
              
                FillClauseDetails(_sbFilter.ToString(), _cmdList);
           }
        }

        protected void FillClauseDetails(string filter, OracleCommand cmdList)
        {
            DataSet _dsClause = new DataSet();
            filter += " ORDER BY " + SortExpression + " " +  SortDirect;

            _dsClause = objDml.GetClauseInfo(filter, cmdList);
            ViewState["clausetable"] = _dsClause.Tables["clause"];

            if (_dsClause.Tables["clause"].Rows.Count > 0)
            {
                GvClause.DataSource = _dsClause.Tables["clause"];
                GvClause.DataBind();
            }
            else
            {
                GvClause.DataSource = null;
                GvClause.DataBind();
            }
        }

        protected void Toggle_ReqsGrid(object sender, EventArgs e)
        {
            ImageButton imgShowHide = (sender as ImageButton);
            GridViewRow row = (imgShowHide.NamingContainer as GridViewRow);
            if (imgShowHide.CommandArgument == "Show")
            {
                row.FindControl("PnlRequirement").Visible = true;
                imgShowHide.CommandArgument = "Hide";
                imgShowHide.ImageUrl = "~/Images/collapsebig.gif";
                string clauseId = GvClause.DataKeys[row.RowIndex].Value.ToString();
                GridView GvRequirement = row.FindControl("GvRequirement") as GridView;
                BindRequirements(clauseId, GvRequirement);

            }
            else
            {
                row.FindControl("PnlRequirement").Visible = false;
                imgShowHide.CommandArgument = "Show";
                imgShowHide.ImageUrl = "~/Images/expandbig.gif";
            }


        }

        private void BindRequirements(string clauseId, GridView GvRequirement)
        {
            GvRequirement.ToolTip = clauseId;
            GvRequirement.DataSource = GetGridRequirementDetails(clauseId);
            GvRequirement.DataBind();
        }

        private DataSet GetGridRequirementDetails(string clauseId)
        {
            string _sqlReq = "";
            StringBuilder _sbReq = new StringBuilder();
            DataSet dsReq = null;

            _sbReq.Append(" SELECT REQ.REQUIREMENT_ID,REQ.REQUIREMENT,REQ.OWNERNAME,REQ.CLAUSE_NUMBER,REQ.FREQUENCY,");
            _sbReq.Append(" (SELECT COUNT(*) FROM  VW_CMS_DELIVERABLE_DETAILS WHERE REQUIREMENT_ID = REQ.REQUIREMENT_ID ");
            if (!(IsSpecialForReport()))
            {
                //Add subowner check also
                _sbReq.Append(" AND OWNERID = ");
                _sbReq.Append(objCommon.GetUserID());

            }
            _sbReq.Append(" ) AS DELCOUNT FROM VW_CMS_REQUIREMENT_DETAILS REQ WHERE ((REQ.CLAUSE_ID = :ClauseID) OR (REQ.CLAUSE_PARENTID = :ClauseId))");
            _sbReq.Append(" ORDER BY ");
            _sbReq.Append(SortExpressionReq);
            _sbReq.Append(" ");
            _sbReq.Append(SortDirectReq);
            _sqlReq = _sbReq.ToString();
            using (OracleCommand _cmdReq = new OracleCommand())
            {
                _cmdReq.Parameters.Add(":ClauseID", OracleType.VarChar).Value = clauseId;
                dsReq = objData.ReturnDataset(_sqlReq, "requirement", _cmdReq);
                return dsReq;
            }

        }

        protected void Toggle_DeliGrid(object sender, EventArgs e)
        {
            ImageButton imgShowHide = (sender as ImageButton);
            GridViewRow row = imgShowHide.NamingContainer as GridViewRow;
            if (imgShowHide.CommandArgument == "Show")
            {
                row.FindControl("PnlDeli").Visible = true;
                imgShowHide.CommandArgument = "Hide";
                imgShowHide.ImageUrl = "~/Images/collapsebig.gif";
                string reqId = (row.NamingContainer as GridView).DataKeys[row.RowIndex].Value.ToString();
                GridView GvDeli = row.FindControl("GvDeli") as GridView;
                BindDeliverables(reqId, GvDeli);
            }
            else
            {
                row.FindControl("PnlDeli").Visible = false;
                imgShowHide.CommandArgument = "Show";
                imgShowHide.ImageUrl = "~/Images/expandbig.gif";
            }

        }

        private void BindDeliverables(string reqId, GridView GvDeli)
        {
            GvDeli.ToolTip = reqId;
            GvDeli.DataSource = GetGridDeliverableDetails(reqId);
            GvDeli.DataBind();
        }

        private DataSet GetGridDeliverableDetails(string reqId)
        {
            string _sqlDeli = "";
            StringBuilder _sbDel = new StringBuilder();
            DataSet dsDeli = null;


            _sbDel.Append("SELECT DELIVERABLE_ID,COMPOSITE_KEY, DUE_DATE, OWNER,DESCRIPTION, STATUS_DESC FROM VW_CMS_DELIVERABLE_DETAILS WHERE REQUIREMENT_ID= :ReqID ");
            if (!(IsSpecialForReport()))
            {
                _sbDel.Append(" AND ( OWNERID = ");
                _sbDel.Append(objCommon.GetUserID());
                string _deliIdforUser = (this.Page as BasePage).GetDeliverablesAsList(objCommon.GetUserID(), userType: "so");
                if (_deliIdforUser != "")
                {
                    _sbDel.Append(" OR DELIVERABLE_ID IN (" + _deliIdforUser + ")");
                }
                _sbDel.Append(" ) ");
            }
            _sbDel.Append(" ORDER BY ");
            _sbDel.Append(this.SortExpressionDeli);
            _sbDel.Append(" ");
            _sbDel.Append(this.SortDirectDeli);
            _sqlDeli = _sbDel.ToString();

            using (OracleCommand _cmdDeli = new OracleCommand())
            {
                _cmdDeli.Parameters.Add(":ReqID", OracleType.VarChar).Value = reqId;
                dsDeli = objData.ReturnDataset(_sqlDeli, "del", _cmdDeli);
                return dsDeli;

            }

        }

        #endregion

        #region "Controls' events"
        protected void cmdFind_Click(object sender, EventArgs e)
        {
            BindClause();
        }
        #endregion

        #region "Properties"
        public string SortExpression
        {
            get
            {
                 ViewState["sortcl"] = (null == ViewState["sortcl"])?"CLAUSE_NUMBER": ViewState["sortcl"];
                 return ViewState["sortcl"].ToString();
            }
            set { ViewState["sortcl"] = value; }
        }

        public string SortExpressionReq
        {
            get
            {
                ViewState["sortreq"] = (null == ViewState["sortreq"]) ? "REQUIREMENT" : ViewState["sortreq"];
                return ViewState["sortreq"].ToString();
            }
            set {
                ViewState["sortreq"] = value;
            }
        }

        public string SortExpressionDeli
        {
            get { 
                ViewState["sortdeli"] = (null == ViewState["sortdeli"]) ? "COMPOSITE_KEY" : ViewState["sortdeli"];
                return ViewState["sortdeli"].ToString();
            }
            set { ViewState["sortdeli"] = value; }
        }

        public string SortDirect
        {
            get
            {
               ViewState["sortdirectioncl"] = (null == ViewState["sortdirectioncl"])?"ASC": ViewState["sortdirectioncl"];
                return ViewState["sortdirectioncl"].ToString();
            }
            set
            {
                ViewState["sortdirectioncl"] = value;
            }

        }


        public string SortDirectReq
        {
            get
            {
                ViewState["sortdirectreq"] = (null == ViewState["sortdirectreq"])?"ASC": ViewState["sortdirectreq"];
                return ViewState["sortdirectreq"].ToString();
            }
            set
            {
                ViewState["sortdirectreq"] = value;
            }
        }

        public string SortDirectDeli
        {
            get {
                ViewState["sortdirectdeli"] = (null == ViewState["sortdirectdeli"]) ? "ASC" : ViewState["sortdirectdeli"];
                return ViewState["sortdirectdeli"].ToString();
            }
            set { ViewState["sortdirectdeli"] = value; }

        }

        #endregion

        # region "GridView Events"

        protected void GvClause_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView GvClause = sender as GridView;
            GvClause.PageIndex = e.NewPageIndex;
            BindClause();
        }

        protected void GvClause_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView rowview = (DataRowView)e.Row.DataItem;
                int _reqCount = 0;
                _reqCount = Convert.ToInt32(rowview["REQCOUNT"]);

                if (_reqCount <= 0)
                {
                    ImageButton ImgReqShow = (ImageButton)e.Row.FindControl("ImgReqShow");
                    ImgReqShow.Visible = false;
                }

            }
            //if ((IsSpecial()))
            //{
            //    e.Row.Cells[4].Visible = true;
            //}


        }

        protected void GvClause_Sorting(object sender, GridViewSortEventArgs e)
        {
            this.SortExpression = e.SortExpression;
            if (ViewState["sortdirectioncl"].ToString() == "ASC")
            {
                this.SortDirect = "DESC";
            }
            else
            {
                this.SortDirect = "ASC";
            }
            BindClause();
        }

        protected void GvRequirement_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView GvRequirement = sender as GridView;
            GvRequirement.PageIndex = e.NewPageIndex;
            BindRequirements(GvRequirement.ToolTip, GvRequirement);
        }

        protected void GvRequirement_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView rowview = (DataRowView)e.Row.DataItem;
                int _delicount = 0;
                _delicount = Convert.ToInt32(rowview["DELCOUNT"]);

                if (_delicount <= 0)
                {
                    ImageButton imgDeliShow = (ImageButton)e.Row.FindControl("ImgDeliShow");
                    imgDeliShow.Visible = false;
                }
            }
            //if ((IsSpecial()))
            //{
            //    e.Row.Cells[4].Visible = true;
            //}

        }

        protected void GvRequirement_Sorting(object sender, GridViewSortEventArgs e)
        {
            GridView GvRequirement = sender as GridView;
            this.SortExpressionReq = e.SortExpression;
            if (ViewState["sortdirectreq"].ToString() == "ASC")
            {
                this.SortDirectReq = "DESC";
            }
            else
            {
                this.SortDirectReq = "ASC";
            }
            BindRequirements(GvRequirement.ToolTip, GvRequirement);

        }

        protected void GvDeli_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string _deliId = "";

                DataRowView rv = (DataRowView)e.Row.DataItem;
                _deliId = rv["DELIVERABLE_ID"].ToString();

                Label LblSO = new Label();
                LblSO = (Label)e.Row.FindControl("LblSO");
                LblSO.Text = (this.Page as BasePage).ConvertListToStrArr("so", _deliId);
            }
        }

        protected void GvDeli_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView GvDeli = sender as GridView;
            GvDeli.PageIndex = e.NewPageIndex;
            BindDeliverables(GvDeli.ToolTip, GvDeli);
        }

        protected void GvDeli_Sorting(object sender, GridViewSortEventArgs e)
        {
            GridView GvDeli = sender as GridView;
            this.SortExpressionDeli = e.SortExpression;
            this.SortDirectDeli = (ViewState["sortdirectdeli"].ToString() == "ASC") ? "DESC" : "ASC";
            BindDeliverables(GvDeli.ToolTip, GvDeli);
        }
        #endregion

        protected void ImgBtnExport_Click(object sender, ImageClickEventArgs e)
        {
            string fileName = "ClauseOwnershipList.xls";
            const string m_Http_Attachment = "attachment;filename=";
            const string m_Http_Content = "content-disposition";

            Response.ClearContent();
            Response.AddHeader(m_Http_Content, m_Http_Attachment + fileName);
            Response.ContentType = "application/excel";

            GetDataForExcel();

            Response.Flush();
            Response.End();

        }

        private void GetDataForExcel()
        {

            DataTable _dtClause = new DataTable();
            DataSet _dsReq = new DataSet();
            DataSet _dsDeli = new DataSet();
            DateTime _dtDuedate;

            if (ViewState["clausetable"] != null)
            {
                _dtClause = (DataTable)ViewState["clausetable"];

                if (_dtClause.Rows.Count > 0)
                {
                    Response.Write("<table><tr>");
                    Response.Write("<td colspan=3><h2>" + "List of Clauses" + "<h2></td></tr>");
                    Response.Write("</table>");
                    Response.Write("<p>");
                    Response.Write("<table border=1>");
                    Response.Write("<th align='center' valign='bottom' bgcolor=#4b6c9e >" + "<font color='#ffffff'>Clause Number</font>" + "</th>");
                    Response.Write("<th align='center' valign='bottom' bgcolor=#4b6c9e >" + "<font color='#ffffff'>Clause Name</font>" + "</th>");
                    Response.Write("<th align='center' valign='bottom' bgcolor=#4b6c9e >" + "<font color='#ffffff'>Owner</font>" + "</th>");
                    Response.Write("<th align='center' valign='bottom' bgcolor=#4b6c9e>" + "<font color='#ffffff'>Requirement</font>" + "</th>");
                    Response.Write("<th align='center' valign='bottom' bgcolor=#4b6c9e>" + "<font color='#ffffff'>Frequency</font>" + "</th>");
                    Response.Write("<th align='center' valign='bottom' bgcolor=#4b6c9e>" + "<font color='#ffffff'>Track Id</font>" + "</th>");
                    Response.Write("<th align='center' valign='bottom' bgcolor=#4b6c9e>" + "<font color='#ffffff'>Due Date</font>" + "</th>");
                    Response.Write("<th align='center' valign='bottom' bgcolor=#4b6c9e>" + "<font color='#ffffff'>Deliverable</font>" + "</th>");                    
                    Response.Write("<th align='center' valign='bottom' bgcolor=#4b6c9e>" + "<font color='#ffffff'>Owner</font>" + "</th>");
                    Response.Write("<th align='center' valign='bottom' bgcolor=#4b6c9e>" + "<font color='#ffffff'>Sub Owner</font>" + "</th>");
                    Response.Write("<th align='center' valign='bottom' bgcolor=#4b6c9e>" + "<font color='#ffffff'>Status</font>" + "</th>");
                    foreach (DataRow row in _dtClause.Rows)
                    {
                        
                        _dsReq= GetGridRequirementDetails(row["CLAUSE_ID"].ToString());
                        if (_dsReq.Tables["requirement"].Rows.Count > 0)
                        {
                            foreach (DataRow reqrow in _dsReq.Tables["requirement"].Rows)
                            {
                                _dsDeli = GetGridDeliverableDetails(reqrow["REQUIREMENT_ID"].ToString());
                                if (_dsDeli.Tables["del"].Rows.Count > 0)
                                {
                                    foreach (DataRow delirow in _dsDeli.Tables["del"].Rows)
                                    {
                                        Response.Write("<tr>");
                                        Response.Write("<td align=left valign=top>" + row["CLAUSE_NUMBER"] + "</td>");
                                        Response.Write("<td align=left valign=top>" + row["CLAUSE_NAME"] + "</td>");
                                        Response.Write("<td align=left valign=top>" + row["OWNERNAME"] + "</td>");
                                        Response.Write("<td align=left valign=top>" + reqrow["REQUIREMENT"] + "</td>");
                                        Response.Write("<td align=left valign=top>" + reqrow["FREQUENCY"] + "</td>");
                                        Response.Write("<td align=left valign=top>" + delirow["COMPOSITE_KEY"] + "</td>");
                                        if (delirow["DUE_DATE"] != null)
                                        {
                                            _dtDuedate = Convert.ToDateTime(delirow["DUE_DATE"]);
                                            Response.Write("<td align=left valign=top>" + _dtDuedate.ToShortDateString() + "</td>");
                                        }
                                        else Response.Write("<td align=left valign=top>" + "" + "</td>");
                                        Response.Write("<td align=left valign=top>" + delirow["DESCRIPTION"] + "</td>");                                       
                                        Response.Write("<td align=left valign=top>" + delirow["OWNER"] + "</td>");
                                        Response.Write("<td align=left valign=top>" + (this.Page as BasePage).ConvertListToStrArr("so", delirow["DELIVERABLE_ID"].ToString()) + "</td>");
                                        Response.Write("<td align=left valign=top>" + delirow["STATUS_DESC"] + "</td>");
                                        Response.Write("</tr>");
                                    }
                                }
                                else
                                {
                                    Response.Write("<tr>");
                                    Response.Write("<td align=left valign=top>" + row["CLAUSE_NUMBER"] + "</td>");
                                    Response.Write("<td align=left valign=top>" + row["CLAUSE_NAME"] + "</td>");
                                    Response.Write("<td align=left valign=top>" + row["OWNERNAME"] + "</td>");
                                    Response.Write("<td align=left valign=top>" + reqrow["REQUIREMENT"] + "</td>");
                                    Response.Write("<td align=left valign=top>" + reqrow["FREQUENCY"] + "</td>");
                                    Response.Write("<td></td>");
                                    Response.Write("<td></td>");
                                    Response.Write("<td></td>");
                                    Response.Write("<td></td>");
                                    Response.Write("<td></td>");
                                    Response.Write("<td></td>");
                                    Response.Write("</tr>");
                                }
                            }

                        }
                        else
                        {
                            Response.Write("<tr>");
                            Response.Write("<td align=left valign=top>" + row["CLAUSE_NUMBER"] + "</td>");
                            Response.Write("<td align=left valign=top>" + row["CLAUSE_NAME"] + "</td>");
                            Response.Write("<td align=left valign=top>" + row["OWNERNAME"] + "</td>");
                            Response.Write("<td></td>");
                            Response.Write("<td></td>");
                            Response.Write("<td></td>");
                            Response.Write("<td></td>");
                            Response.Write("<td></td>");
                            Response.Write("<td></td>");
                            Response.Write("<td></td>");
                            Response.Write("<td></td>");
                            Response.Write("</tr>");
                        }
                    }
                    Response.Write("</table>");
                }
            }
        }
     

    }
}