﻿//$Header:$
//
// U.S. Department of Energy under contract number DE-AC02-76SF00515
// DOE O 241.1B, SCIENTIFIC AND TECHNICAL INFORMATION MANAGEMENT In the performance of Department of Energy(DOE) contracted obligations, each contractor is required to manage scientific and technical information(STI) produced under the contract as a direct and integral part of the work and ensure its broad availability to all customer segments by making STI available to DOE's central STI coordinating office, the Office of Scientific and Technical Information (OSTI).
//  Deliverable.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2013 SLAC. All rights reserved.
//  This is the codebehind for displaying the deliverables list
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OracleClient;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections;
using System.Configuration;

namespace ContractManagement
{
    public partial class Deliverable_List : BasePage
    {
        Data.CMS_DMLUtil objDml = new Data.CMS_DMLUtil();
        Business.CMS_Common_Util objCommon = new Business.CMS_Common_Util();
        DataSet _dsAppr = new DataSet();

        protected void Page_Load(object sender, EventArgs e)
        {
            string _drt;
            string _stat;
            string _days;
            string _ssomy;
            string _fylist;
            string _fynoqt = "";
            string _prevpage = "";
            string _qtrlist;
            
            
            StringBuilder _sbLabel = new StringBuilder();
            txtDeliverable.Attributes.Add("onkeydown", "return onKeypress('cmdFind');");
           
            if (!Page.IsPostBack)
            {
                ChkFY.DataSourceID = "SDSFY";
                ChkFY.DataTextField = "FYDUE";
                ChkFY.DataValueField = "FYDUE";
                ChkFY.DataBind();

                _prevpage = Request.QueryString["page"]; 

                if (_prevpage != "back")
                {
                    _stat = Request.QueryString["stat"];
                    _drt = Request.QueryString["drt"];
                    _days = Request.QueryString["days"];
                    _ssomy = Request.QueryString["ssomy"];
                    _fylist = Request.QueryString["fy"];
                    _qtrlist = Request.QueryString["qtr"];
                    StoreInSession(_stat, _drt, _days, _ssomy, _fylist, _qtrlist);
                }
                else
                {
                    
                    ArrayList _arrQSV = new ArrayList();
                    _arrQSV = GetFromSession();
                    if (_arrQSV != null)
                    {
                        _stat = (string)_arrQSV[0];
                        _drt = (string)_arrQSV[1];
                        _days = (string)_arrQSV[2];
                        _ssomy = (string)_arrQSV[3];
                        _fylist = (string)_arrQSV[4];
                        _qtrlist = (string)_arrQSV[5];
                    }
                    else { _stat = _drt = _days = _ssomy = _fylist = ""; _qtrlist = ""; }
                }
               
               
                CheckIfManager();

                if ((!_cma) && (!_admin) && (!_ald) && (!_diradmin))
                {
                    cmdAdd.Visible = false;
                    if ((!_sso) && (!_ssosuper))
                    {
                        if (null == Session["view"])
                        {
                            Session["view"] = "owner";
                        }
                    }
                    else if ((_sso) || (_ssosuper))
                    {
                        string _page = "";
                        if (!string.IsNullOrEmpty(_ssomy))
                        {
                            if (_ssomy == "Y")
                            {
                                _page = "Viewed deliverables for approval";
                            }

                        }
                        else { _page = "Viewed all deliverables of SSO"; }
                        string _errCode = objDml.AddSSOLog(0, _page, Convert.ToInt32(objCommon.GetUserID()), objCommon.GetUserID());

                        Session["view"] = "sso";


                    }
                   
                }
                else Session["view"] = "";

                bool _selected = false;
                if (string.IsNullOrEmpty(_stat) && string.IsNullOrEmpty(_drt) && string.IsNullOrEmpty(_days) && string.IsNullOrEmpty(_ssomy) && string.IsNullOrEmpty(_fylist))
                {

                    DivFY.Visible = true;
                    string _fyDue = Business.DateTimeExtension.ToFinancialYearShort(DateTime.Today).ToString();
                    //current FY is the default
                    Hdnfylist.Value = "'" + _fyDue + "'";
                    foreach (ListItem li in ChkFY.Items)
                    {
                        if (li.Text.Equals(_fyDue))
                        {
                            li.Selected = true;
                            _selected = true;
                        }
                      }
   
                }
                else
                {
                    DivFY.Visible = false;
                   
                    //Comments MS: Per Requirement, CM Office and Superadmin are the only ones to add a Deliverable
                    if ((_qtrlist != null) && (_qtrlist != ""))
                    {
                        HdnQtr.Value = _qtrlist;
                        _sbLabel.Append(" Fiscal Quarter(s) ");
                        _sbLabel.Append(_qtrlist);
                        _sbLabel.Append(" ");

                    }

                    if ((_fylist != null) && (_fylist != ""))
                    {

                        Hdnfylist.Value = _fylist;
                        if (_fylist != "All")
                        {
                            _fynoqt = _fylist.Replace("'", string.Empty);
                            _sbLabel.Append(_fynoqt);
                            _sbLabel.Append(" ");
                         }

                    }
                   

                    if ((_stat != null) && (_stat != ""))
                    {
                        if (Regex.IsMatch(_stat, "^[0-9]+$"))
                        {
                            HdnStat.Value = _stat;
                            Status _statId = (Status)Convert.ToInt32(HdnStat.Value);

                            _sbLabel.Append(objCommon.GetEnumDescription(_statId));
                            _sbLabel.Append(" Deliverables");
                        }
                        else { RedirectInvalidParam(); }
                    }

                    if ((_drt != null) && (_drt != ""))
                    {
                        if (Regex.IsMatch(_drt, "^[A-Z]$"))
                        {
                            HdnDrt.Value = _drt;
                            _sbLabel.Append(" of ");
                            _sbLabel.Append(GetDirectorateName(_drt));
                            _sbLabel.Append(" Directorate ");
                        }
                        else { RedirectInvalidParam(); }
                    }
                    if ((_days != null) && (_days != ""))
                    {
                        if (Regex.IsMatch(_days, "^[0-9]+$"))
                        {
                            HdnDays.Value = _days;
                            if (_sbLabel.Length == 0) 
                            {
                                _sbLabel.Append(" Deliverables ");
                            }
                            else if (_sbLabel.ToString().TrimEnd().Equals(_fynoqt))
                            {
                                _sbLabel.Append(" Deliverables ");
                            }
                            _sbLabel.Append(" that are due in ");

                            if (_days == "1") { _sbLabel.Append(" 1 day "); }
                            else
                            {
                                _sbLabel.Append(" next ");
                                _sbLabel.Append(_days);
                                _sbLabel.Append(" days");
                            }

                        }
                        else { RedirectInvalidParam(); }
                    }
                   
                   
                    if (_ssomy == "Y")
                    { HdnmySSO.Value = "Y"; }


                }

                if (null != (Session["view"]))
                {
                    if (Session["view"].ToString() == "owner")
                    {
                        SortExpression = "DUE_DATE";
                        SortDirect = "ASC";
                        if (_stat == "2")
                        {
                            _sbLabel.Replace("In Progress", "Unsubmitted");
                            }
                    }

                    if (Session["view"].ToString() == "sso")
                    {
                        if (_stat == "3")
                        {
                            _sbLabel.Replace("Submitted", string.Empty);
                            if (_ssomy == "Y")
                            {
                                _sbLabel.Append(" For Your Approval ");
                            }
                            else { _sbLabel.Append(" For SSO Approval"); }
                        }
                    }
                }
                LblSubtitle.Text = _sbLabel.ToString();
                HdnDesc.Value = txtDeliverable.Text;
                 cmdAdd.Visible = true;
                PnlFY.GroupingText = "";

                if ((_selected == false) && ( DivFY.Visible))
                {
                    if (ChkFY.Items.FindByText("All") != null)
                    {
                        ChkFY.Items.FindByText("All").Selected = true;
                        ChkFY_SelectedIndexChanged(null, null);
                        GetFYListAndBind();
                    }
                    
                }
                else
                {
                    BindGrid();
                }
               
            }
        }

        private void StoreInSession(string stat,string drt,string days,string ssomy, string fyList, string qtr)
        {
            ArrayList _arrQS = new ArrayList();
            _arrQS.Add(stat);
            _arrQS.Add(drt);
            _arrQS.Add(days);
            _arrQS.Add(ssomy);
            _arrQS.Add(fyList);
            _arrQS.Add(qtr);
            Session["arrqs"] = _arrQS;
        }

        private ArrayList GetFromSession()
        {
            if (Session["arrqs"] != null)
            {
                return (ArrayList)Session["arrqs"];
            }
            else return null;
        }
       
        protected void cmdAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("Deliverable.aspx?mode=add");
        }

        protected void cmdFind_Click(object sender, EventArgs e)
        {
            HdnDesc.Value = txtDeliverable.Text.Trim();
            if (DivFY.Visible)
            {
                Hdnfylist.Value = GetChecklistItems();
            }
            BindGrid();
        }

       

        private void BindGrid()
        {
          
            StringBuilder _sbFilter = new StringBuilder();
            List<string> _deliList = new List<string>();
            string _deliIdforUser = "";
            using (OracleCommand _cmdList = new OracleCommand())
            {
                                                             
                if (HdnDesc.Value != "")
                {
                    //Get the list of deliverable ids where subowner is like the text
                     _deliList = objDml.GetDeliverableIdForSO(HdnDesc.Value);
                    _deliIdforUser = string.Join(",", _deliList.ToArray());

                    _sbFilter = SetSBFilter(_sbFilter);
                   _sbFilter.Append(" ((LOWER(REQUIREMENT) LIKE :Reqdesc OR LOWER(DESCRIPTION) LIKE :Reqdesc OR LOWER(TYPENAME) LIKE :Reqdesc OR LOWER(STATUS_DESC) LIKE :Reqdesc OR LOWER(OWNER) LIKE :Reqdesc ");
                   _sbFilter.Append(" OR LOWER(CLAUSENUM) LIKE :Reqdesc OR LOWER(FREQNAME) LIKE :Reqdesc OR LOWER(COMPOSITE_KEY) LIKE :Reqdesc) ");
                   if (_deliIdforUser != "")
                   {
                       _sbFilter.Append(" OR DELIVERABLE_ID IN (" + _deliIdforUser + "))");
                   }
                   else { _sbFilter.Append(" ) "); }
                    _cmdList.Parameters.Add(":Reqdesc", OracleType.VarChar).Value = "%" +  Server.HtmlEncode(HdnDesc.Value.ToLower()) + "%";
                }

                if (HdnStat.Value != "")
                {
                    _sbFilter = SetSBFilter(_sbFilter);
                    if (HdnStat.Value != "0")
                    {
                        //Based on DEV-4240, For SSO view, Approved by default is combined with Approved status
                        if ((HdnStat.Value == "4") && ((Session["view"].Equals("sso")) || ( Session["view"].Equals("owner"))))
                        {
                            _sbFilter.Append(" STATUS_ID IN (4,6) ");
                        }
                        else
                        {
                            _sbFilter.Append("  STATUS_ID = :StatusId");
                            _cmdList.Parameters.Add(":StatusId", OracleType.VarChar).Value = Server.HtmlEncode(HdnStat.Value);
                        }
                    }
                    else { _sbFilter.Append(" STATUS_ID IN (" + (int)Status.New + "," + (int)Status.InProgress + "," + (int)Status.Reopened + ")" + " AND DAYS < 0 "); }
                }
                else
                {
                    bool _ssosuper = (Session["ssosuper"] != null)?(bool)Session["ssosuper"]:false ;
                    if (Session["view"] != null)
                    {
                        if ((Session["view"].Equals("sso")) && (!_ssosuper))
                        {
                            _sbFilter = SetSBFilter(_sbFilter);
                            _sbFilter.Append(" STATUS_ID IN (" + (int)Status.submitted + "," + (int)Status.Approved + "," + (int)Status.ApprovedbyDefault + ")");
                        }
                    }
                }

                if (HdnDrt.Value != "")
                {
                    string _drt = GetDirectorateName(Server.HtmlEncode(HdnDrt.Value));
                    string _orgId = objDml.GetOrgId(_drt.Substring(0, 4));
                    _sbFilter = SetSBFilter(_sbFilter);
                    _sbFilter.Append(" DIRECTORATE_ID = :DirectorateId");
                    _cmdList.Parameters.Add(":DirectorateId", OracleType.VarChar).Value = _orgId;
                }

                if (HdnDays.Value != "")
                {
                    int _startdays = -1;
                    _startdays = ReturnStartDays(Convert.ToInt32(HdnDays.Value));
                    _sbFilter = SetSBFilter(_sbFilter);
                    _sbFilter.Append(" STATUS_ID IN (1,2,5) AND DAYS > ");
                    _sbFilter.Append(_startdays);
                    _sbFilter.Append(" AND DAYS <= :Days ");
                    _cmdList.Parameters.Add(":Days", OracleType.VarChar).Value = Server.HtmlEncode(HdnDays.Value);
                }

                if ((Hdnfylist.Value != "") && (Hdnfylist.Value != "All"))
                {
                    _sbFilter = SetSBFilter(_sbFilter);
                    _sbFilter.Append(" FYDUE IN (");
                    _sbFilter.Append(Hdnfylist.Value);
                    _sbFilter.Append(")");
                  
                }
                
                if ((HdnQtr.Value !="") && (HdnQtr.Value != ""))
                {
                    _sbFilter = SetSBFilter(_sbFilter);
                    _sbFilter.Append(" QUARTER IN (");
                    _sbFilter.Append(HdnQtr.Value);
                    _sbFilter.Append(")");
                }


                FillDeliDetails(_sbFilter.ToString(), _cmdList);
            }

            HdnFilter.Value = _sbFilter.ToString();
            
        }

        protected void FillDeliDetails(string filter, OracleCommand cmdList)
        {
            DataSet _dsDeli = new DataSet();
            filter += " ORDER BY " + SortExpression + " " + SortDirect;
           
            _dsDeli = objDml.GetDeliverableInfo(filter, cmdList);
            ViewState["deli"] = _dsDeli.Tables["deli"];
            

            DataView _newdv = new DataView(ViewState["deli"] as DataTable);
            string _view;
            string _deliIds = "";
            string _deliIdforUser = "";
            int _count = 0;

            if (!(null == Session["view"]))
            {
                _view = Session["view"].ToString();
                //Session["view"] = null;
                if ((_view.Equals("owner")) || (_view.Equals("sso")))
                {
                    if (_view.Equals("owner"))
                    {
                        LblSearch.Text = "Search my Requirements or Deliverables:";
                        _deliIdforUser = GetDeliverablesAsList(objCommon.GetUserID());
                        if (_deliIdforUser != "")
                        {
                            if (LblSubtitle.Text == "")
                            {
                                LblSubtitle.Text = " Deliverables";
                            }
                            if (!LblSubtitle.Text.Contains("Your"))
                            {
                                LblSubtitle.Text = " Your " + LblSubtitle.Text;
                            }
                            _deliIds = " DELIVERABLE_ID IN (" + _deliIdforUser + ")";
                            _newdv.RowFilter = _deliIds;
                        }
                        else { _newdv.RowFilter = " DELIVERABLE_ID = 0 "; }
                    }
                    else if (_view.Equals("sso") )
                    {
                        _deliIdforUser = GetDeliverablesAsList(objCommon.GetUserID(), userType: "appvr");
                        ViewState["deliappr"] = _deliIdforUser;
                        if (HdnmySSO.Value == "Y")
                        {
                            if (_deliIdforUser != "")
                            {
                                if (LblSubtitle.Text == "")
                                {
                                    LblSubtitle.Text = " Deliverables that need your approval ";
                                }
                                _deliIds = " DELIVERABLE_ID IN (" + _deliIdforUser + ")";
                                _newdv.RowFilter = _deliIds;
                            }
                        }
                       
                    }
                    
                }

            }
             _count = _newdv.Count; 

             if (_count > 0)
            {
                
                GVDeli.DataSource = _newdv;
                GVDeli.DataBind();
                if (LblSubtitle.Text == "")
                {
                    LblSubtitle.Text = "Deliverables";
                }
            }
            else
            {

                LblInfo.Visible = false;

                if (LblSubtitle.Text != "")
                {

                    GVDeli.EmptyDataText = "No Deliverables found that match your criteria ";
                    LblSubtitle.Text = "";
                }
                GVDeli.DataSource = null;
                GVDeli.DataBind();
               
            }

        }

        protected string SortExpression
        {
            get
            {
                if (null == ViewState["sort"])
                {                                   
                    ViewState["sort"] = "DELIVERABLE_ID";
                }
                return ViewState["sort"].ToString();
            }

            set { ViewState["sort"] = value; }
        }

        protected string SortDirect
        {
            get
            {
                if (null == ViewState["sortdirection"])
                {
                    ViewState["sortdirection"] = "DESC";
                }
                return ViewState["sortdirection"].ToString();
            }
            set
            {
                ViewState["sortdirection"] = value;
            }
        }

    

        protected void GVDeli_Sorting(object sender, GridViewSortEventArgs e)
        {
            this.SortExpression = e.SortExpression;
            if (ViewState["sortdirection"].ToString() == "ASC")
            {
                this.SortDirect = "DESC";
            }
            else
            {
                this.SortDirect = "ASC";
            }
            BindGrid();
        }

        protected void GVDeli_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GVDeli.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        

        protected void GVDeli_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if ((e.Row.RowType == DataControlRowType.DataRow) || (e.Row.RowType == DataControlRowType.Header))
            {
                 string _deliId ="";
                 string _status = "";
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    DataRowView rowView = (DataRowView)e.Row.DataItem;
                    
                    string _requirement;
                    string _description;
                    string _reqId;                       

                    _requirement = rowView["REQUIREMENT"].ToString();
                    _description = rowView["DESCRIPTION"].ToString();
                    _deliId = rowView["DELIVERABLE_ID"].ToString();
                    _status = rowView["STATUS_DESC"].ToString();
                    _reqId = rowView["REQUIREMENT_ID"].ToString();

                
                }

                if (Session["view"] != null)
                {
                    if (Session["view"].ToString() == "sso")
                    {
                        e.Row.Cells[12].Visible = true;
                        e.Row.Cells[11].Visible = true;
                        e.Row.Cells[8].Visible = false;                      
                    } //End of Session["view"] sso check
                    else { e.Row.Cells[12].Visible = false;
                    e.Row.Cells[11].Visible = false;
                    e.Row.Cells[8].Visible = true;
                        if ((Session["view"].ToString() == "owner") && (_status.Equals("Approved by Default")))
                        {
                            e.Row.Cells[8].Text = "Approved";
                        }
                    }
                } //End of Session["view"] null check
                else { e.Row.Cells[12].Visible = false;
                e.Row.Cells[11].Visible = false;
                e.Row.Cells[8].Visible = true;
                }
            }
           
        }
            

        protected void GVDeli_RowCreated(object sender, GridViewRowEventArgs e)
        {
            string _view = "";
            _view = Business.SessionVar.GetString("view");


            if (_view == "sso")
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    GridView GVAppr = e.Row.FindControl("GVAppr") as GridView;
                    int _deliId = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "DELIVERABLE_ID"));
                    GVAppr.DataSource = objDml.GetApproversDeli(_deliId);
                    GVAppr.DataBind();
                }
            }
        }

        #region "Checkbox list events/related"

        protected void ChkFY_DataBound(object sender, EventArgs e)
        {
             ChkFY.Items.Insert(ChkFY.Items.Count, new ListItem("All", "0"));
         }

        protected void ChkFY_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ChkFY.Items.FindByText("All").Selected)
            {
                for (int i = 0; i < ChkFY.Items.Count - 1; i++)
                {
                    ChkFY.Items[i].Selected = true;
                }
            }
        }

        protected void BtnRefresh_Click(object sender, EventArgs e)
        {
           
            if (Page.IsValid)
            {
                GetFYListAndBind();
            }
        }

        private void GetFYListAndBind()
        {
            string _fylist = "";
            _fylist = GetChecklistItems();
            Hdnfylist.Value = _fylist;
            HdnDesc.Value = txtDeliverable.Text;
            HdnQtr.Value = GetSelectedQuarter();
            BindGrid();
        }

        private string GetSelectedQuarter()
        {
            StringBuilder _sbQtr = new StringBuilder();
            foreach (ListItem li in LstQtr.Items)
            {
                if (li.Selected)
                {
                    if (li.Value != "0")
                    {
                       _sbQtr.Append(li.Text);
                       _sbQtr.Append(",");
                    }
   
                }

            }

            if  (!string.IsNullOrEmpty(_sbQtr.ToString()))
            {
                _sbQtr.Remove(_sbQtr.Length - 1, 1);
                return _sbQtr.ToString();
            }
            else return "";
           
        }

        protected string GetChecklistItems()
        {
            StringBuilder _sbfy = new StringBuilder();

            foreach (ListItem li in ChkFY.Items)
            {
                if (li.Selected)
                {
                    if (li.Text != "All")
                    {
                        _sbfy.Append("'");
                        _sbfy.Append(li.Text.Substring(0, 4));
                        _sbfy.Append("'");
                        _sbfy.Append(",");
                    }
                    else
                    {
                        _sbfy.Clear();
                        _sbfy.Append("All"); _sbfy.Append(",");
                    }
                }
            }
            _sbfy.Remove(_sbfy.Length - 1, 1);
            return _sbfy.ToString();

        }

        protected void CheckBoxRequired_ServerValidate(object source, ServerValidateEventArgs args)
        {
            int counter = 0;
            for (int i = 0; i < ChkFY.Items.Count; i++)
            {
                if (ChkFY.Items[i].Selected)
                {
                    counter++;
                }
                args.IsValid = (counter == 0) ? false : true;
            }
        }
        #endregion

        protected void LstQtr_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (ListItem li in LstQtr.Items)
            {
                if ((li.Value == "0") && (li.Selected))
                {
                    li.Selected = false;
                }
            }
            if (LstQtr.GetSelectedIndices().Count() == 0)
            {
                LstQtr.SelectedValue = "0";
            }
        }


    }
}