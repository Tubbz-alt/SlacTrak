﻿//$Header:$
//
// U.S. Department of Energy under contract number DE-AC02-76SF00515
// DOE O 241.1B, SCIENTIFIC AND TECHNICAL INFORMATION MANAGEMENT In the performance of Department of Energy(DOE) contracted obligations, each contractor is required to manage scientific and technical information(STI) produced under the contract as a direct and integral part of the work and ensure its broad availability to all customer segments by making STI available to DOE's central STI coordinating office, the Office of Scientific and Technical Information (OSTI).
//  BasePage.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2013 SLAC. All rights reserved.
//
//  This is the base code for some of the pages.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data.OracleClient;
using System.Data;
using System.Text;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections;


namespace ContractManagement
{
    public class BasePage: System.Web.UI.Page
    {
        public bool _admin = false;
        public bool _cma = false;
        public bool _sso = false;
        public bool _ald = false;
        public bool _diradmin = false;
        public bool _owner = false;
        public bool _subowner = false;
        public bool _ssoallow = false;
        public bool _ssosuper = false;

        Business.UserRoles objUser = new Business.UserRoles();
        Business.CMS_Common_Util objCommon = new Business.CMS_Common_Util();
        Data.CMS_DMLUtil objDml = new Data.CMS_DMLUtil();
        Business.EmailSetting objEmail = new Business.EmailSetting();
        # region "Enums"

        public enum Type
        {
            PrimaryContract = 1,
            DOEDirective = 3,
            DataCall = 4,
            DOERequest = 5
        }

        public enum MstrMnu
        {
            MyDashboard = 0,
            Deliverable=1,
            Admin=2,
            Reports=3
        }

        public enum Status
        {
            [Description("New")]
            New = 1,
            [Description("In Progress")]
            InProgress =2,
            [Description("Submitted")]
            submitted=3,
            [Description("Approved")]
            Approved=4,
            [Description("Re-opened")]
            Reopened=5,
            [Description("Approved by Default")]
            ApprovedbyDefault=6,
            [Description("Overdue")]
            Overdue = 0
        }

        public enum UserType
        {
            CMA=1,
            Admin=2,
            ALD=3,
            DIRADMIN=4,
            SSO=5,
            OTHER=6
        }

     
        public enum Directorates
        {
            Accelerator,
            Director,
            Infrastructure,
            Particle,
            LCLS,
            Photon,
            SSRL,
            Other,
            NA,
            All,
            onlydrts = Directorates.Accelerator | Directorates.Director | Directorates.Infrastructure
        }

     

        # endregion



        protected virtual Control FindControlRecursive(string id)
        {
            return FindControlRecursive(id, this);
        }
        protected virtual Control FindControlRecursive(string id, Control parent)
        {
            // If parent is the control we're looking for, return it
            if (string.Compare(parent.ID, id, true) == 0)
                return parent;
            // Search through children
            foreach (Control child in parent.Controls)
            {
                Control match = FindControlRecursive(id, child);
                if (match != null)
                    return match;
            }
            // If we reach here then no control with id was found
            return null;
        }

        public void CheckIfManager()
        {
            objUser.GetUserRole(objCommon.GetUserID());
            if (Session["admin"] != null)
            {
                _admin = (bool)Session["admin"];
            }
          
            if (Session["cma"] != null)
            {
                _cma = (bool)Session["cma"];
            }

            if (Session["sso"] != null)
            {
                _sso = (bool)Session["sso"];
            }
            if (Session["ald"] != null)
            {
                _ald = (bool)Session["ald"];
             }
            if (Session["diradmin"] != null)
            {
                _diradmin = (bool)Session["diradmin"];
            }

            if (Session["ssosuper"] != null)
            {
                _ssosuper = (bool)Session["ssosuper"];
            }
        }

        public bool IsSpecial()
        {
            bool _isspecial = false;
            if (ViewState["isspecial"] != null)
            {
                _isspecial = (bool)ViewState["isspecial"];

            }
            else
            {
                CheckIfManager();
                if ((!_admin) && (!_cma) && (!_ald) && (!_diradmin))
                    _isspecial = false;
                else _isspecial = true;
                ViewState["isspecial"] = _isspecial;

            }
            return _isspecial;

        }

        public bool IsSpecialForReport()
        {
            bool _isspecialRep = false;
            if (ViewState["isspecialRep"] != null)
            {
                _isspecialRep = (bool)ViewState["isspecialRep"];

            }
            else
            {
                CheckIfManager();
                if ((!_admin) && (!_cma) && (!_ald) && (!_diradmin) && (!_sso) && (! _ssosuper))
                    _isspecialRep = false;
                else _isspecialRep = true;
                ViewState["isspecialRep"] = _isspecialRep;

            }
            return _isspecialRep;

        }

        public bool IsNotSSO()
        {
            if ((!_cma) && (!_admin))
            {
                if ((_sso) || (_ssosuper))
                    return false;
                else return true;
            }
            else return true;

        }

        
        protected void CheckIfValidUser(string objId)
        {

            _owner = objDml.IsOwner(objId, true);
            _subowner = objDml.IsOwner(objId, false);
            _ssoallow = objDml.AllowSSO(objId);

        }

        public void DeliverFile(byte[] Data, string Type, int Length, string DownloadFileName)
        {
                Page.Response.ClearHeaders();
                Page.Response.Clear();
                Page.Response.ContentType = Type;
                if (!string.IsNullOrEmpty(DownloadFileName))
                {
                    //Add filename to header 
                     
                    Page.Response.AddHeader("Connection", "keep-alive");
                    Page.Response.AddHeader("Content-Length", Convert.ToString(Length));

                    Response.AddHeader("Content-Disposition", "attachment; filename=\"" + DownloadFileName + "\"");

                    switch (Type)
                    {
                        case "False":
                            Page.Response.ContentType = "application/octet-stream";
                            Page.Response.Charset = "UTF-8";
                            break;
                        default:
                            Page.Response.ContentType = Type;
                            break;
                    }
                }
                Page.Response.OutputStream.Write(Data, 0, Length);
                Page.Response.End();
            }

        public void FileData(int id)
        {
            int sfileid = 0;
            string sfilename = null;
            int sfilesize = 0;
            string scontent = null;
            byte[] sfiledata = null;
            int _deliverableId = 0;
            //string[] scontentsplit = null; 
            using (OracleDataReader drFileinfo = objDml.GetFileInfoById(id))
            {

                while (drFileinfo.Read())
                {
                    sfileid = Convert.ToInt32(drFileinfo["ATTACHMENT_ID"]);
                    sfilename = (string)drFileinfo["FILE_NAME"];
                    sfilesize = Convert.ToInt32(drFileinfo["FILE_SIZE"]);
                    scontent = (string)drFileinfo["FILE_CONTENT_TYPE"];
                    sfiledata = (byte[])drFileinfo["FILE_DATA"];
                    //scontentsplit = S.Split((string)drFileinfo["CONTENTTYPE"], "/"); 
                    _deliverableId = Convert.ToInt32(drFileinfo["DELIVERABLE_ID"]);
                    SSOLog(_deliverableId, sfilename);
                    DeliverFile(sfiledata, scontent, sfilesize, sfilename);
                }

            }
        }

        protected void SSOLog(int deliverableId, string fileName)
        {
            if (Session["sso"] != null)
            {
                _sso = (bool)Session["sso"];
            }
            if (_sso)
            {
                string _errCode = objDml.AddSSOLog(deliverableId, "Downloaded file " + fileName, Convert.ToInt32(objCommon.GetUserID()), objCommon.GetUserID());
            }

        }

        protected void AddRemoveItemsInList(string DdId, string itemValue, string itemText,string action = "add")
        {
            DropDownList _ddId;

            _ddId = (DropDownList)FindControlRecursive(DdId);

            ListItem _ligen = new ListItem();
            _ligen.Text = itemText;
            _ligen.Value = itemValue;

             //Comments MS: If items need to be removed, check if it is already present in the dropdown
             if (action == "remove")
             {
                 if (_ddId.Items.FindByValue(itemValue) != null)
                 {
                     _ddId.Items.Remove(_ligen);
                 }
             }
             //Comments MS: If items need to be added, make sure it is not already in the dropdown
             else 
             {
                 if (_ddId.Items.FindByValue(itemValue) == null)               
                    {
                        _ddId.Items.Insert(0, _ligen);
                    }
            }
        }

        protected bool ValidateAllSubowners(int tbcount)
        {

            User_Controls.DynamicTextBox _ucdb;
            Panel _pnlSO = new Panel();

            _ucdb = (User_Controls.DynamicTextBox)FindControlRecursive("TB1");
            _pnlSO = (Panel)_ucdb.FindControl("pnlsubowner");
            bool _result = true;
            for (int i = 0; i < tbcount; i++)
            {
                TextBox txtbox = new TextBox();
                CustomValidator _cv = new CustomValidator();
                txtbox = (TextBox)_pnlSO.FindControl("txt" + i.ToString());
                _cv = (CustomValidator)_pnlSO.FindControl("cv" + i.ToString());
                if (txtbox.Text != "")
                {
                    bool _isValid = objDml.CheckValidName(txtbox.Text);
                    if (!_isValid)
                    {
                        _cv.IsValid = false;
                        _result = false;
                    }
                }
            }
            return _result;

        }

        //TODO: get it from db with current drtcode - replace character with drt code 
        protected string GetDirectorateName(string dcode)
        {
            string _drtName = "";
            switch (dcode)
            {
                case "A":
                    _drtName = "Accelerator";
                    break;
                case "D":
                    _drtName = "Director's Office";
                    break;
                case "L":
                    _drtName = "LCLS";
                    break;
                case "F":
                    _drtName = "Infrastructure & Safety";
                    break;
                case "X":
                    _drtName = "Photon Science";
                    break;
                case "S":
                    _drtName = "SSRL";
                    break;
                case "P":
                    _drtName = "Particle Physics and Astro";
                    break;
                case "O":
                    _drtName = "Other Costs";
                    break;

            }
            return _drtName;
        }

        public void SetOrgBasedOnUser(string user)
        {
            string _deptCode = "";
            string _dirCode = "";
            //set the directorate
            using (OracleDataReader _drDrt = objDml.GetDirectorateDept(objCommon.GetEmpid(user).ToString()))
            {
                while (_drDrt.Read())
                {
                    _deptCode = objCommon.FixDBNull(_drDrt[0]);
                    _dirCode = objCommon.FixDBNull(_drDrt[1]);
                }

            }
         
                SetOrg(_dirCode, _deptCode);


        }

        protected void RedirectInvalidParam(string errtype="pminvld")
        {
            string _redirecturl;
            string _msg="pminvld";

            if (errtype == "noobj")
            {
                _msg = "noobj";
            }
            if (Session["urlhost"] != null)
            {
                _redirecturl = Session["urlhost"] + Request.ApplicationPath + "/" + "Error.aspx?msg=" + _msg;

            }
            else { _redirecturl = "Error.aspx?msg=" + _msg; }
            Response.Redirect(_redirecturl);
        }

        public void SetOrg(string dirId, string deptId)
        {
            DropDownList DdlDirectorate;
            DdlDirectorate = (DropDownList)FindControlRecursive("DdlDirectorate");

            DropDownList DdlDepartment;
            DdlDepartment = (DropDownList)FindControlRecursive("DdlDepartment");

            DdlDirectorate.ClearSelection();
            AddRemoveItemsInList("DdlDirectorate", "0", "--Choose One--", "remove");
            //Comment MS: if in future the directorate becomes inactive, checking it for null will prevent it from crashing
            if (DdlDirectorate.Items.FindByValue(dirId) != null)
            {
                DdlDirectorate.Items.FindByValue(dirId).Selected = true;
                AddRemoveItemsInList("DdlDirectorate", "0", "--Choose One--");
            }
            else
            {
                AddRemoveItemsInList("DdlDirectorate", "0", "--Choose One--");
                DdlDirectorate.SelectedValue = "0";
            }
            FillDepartment();
           //DdlDirectorate_SelectedIndexChanged(null, null);
            AddRemoveItemsInList("DdlDepartment", "0", "--Choose One--", "remove");
            if (DdlDepartment.Items.FindByValue(deptId) != null)
            {
                DdlDepartment.Items.FindByValue(deptId).Selected = true;
                AddRemoveItemsInList("DdlDepartment", "0", "--Choose One--");
            }
            else
            {
                AddRemoveItemsInList("DdlDepartment", "0", "--Choose One--");
                DdlDepartment.SelectedValue = "0";
            }


        }

        protected StringBuilder SetSBFilter(StringBuilder sbFilter)
        {
            if ((!sbFilter.Equals(string.Empty)) && (sbFilter.ToString() != ""))
            {
                sbFilter.Append(" AND ");
            }
            else
            {
                sbFilter.Append(" WHERE ");
            }
            return sbFilter;
        }


        public void FillDepartment()
        {
            string _drtId = "";

            DataSet _dsDept = new DataSet();
            DropDownList DdlDirectorate;
            DdlDirectorate = (DropDownList)FindControlRecursive("DdlDirectorate");

            DropDownList DdlDepartment;
            DdlDepartment = (DropDownList)FindControlRecursive("DdlDepartment");


            if (DdlDirectorate.SelectedIndex != 0)
            {
                _drtId = DdlDirectorate.SelectedValue.ToString();
            }


            DdlDepartment.Items.Clear();
            _dsDept = objDml.GetDepartment(_drtId);
            DdlDepartment.DataValueField = "ORG_ID";
            DdlDepartment.DataTextField = "DESCRIPTION";
            DdlDepartment.DataSource = _dsDept.Tables["dept"];
            DdlDepartment.DataBind();
            AddRemoveItemsInList("DdlDepartment", "0", "--Choose One--");

        }

        public string GetDeliverablesAsList(string userId, string userType = "all", string lookupId = "")
        {
            string _deliId;
            StringBuilder _sbDeli = new StringBuilder();

            List<string> _deliList = new List<string>();

            if (userType == "appvr")
            {
                _deliList = objDml.GetDeliverableIdForApprovers(userId);
            }
            else
            {
                if (lookupId != "")
                {
                    _deliList = objDml.GetDeliverableIdonNotification(lookupId: lookupId);
                }
                else
                {
                    _deliList = objDml.GetDeliverableIdForUser(userId, userType);
                }
            }
            _deliId = string.Join(",", _deliList.ToArray());

            return _deliId;

        }

        protected string GetClauseNameNo(string reqId)
        {
            string _clauseDet = "";
            using (OracleDataReader _drClause = objDml.GetClauseInfo(reqId))
            {
                while (_drClause.Read())
                {
                    _clauseDet = objCommon.FixDBNull(_drClause["CLAUSE_NUMBER"]) + " " + objCommon.FixDBNull(_drClause["CLAUSE_NAME"]);

                }
            }
            return _clauseDet;
        }
      
        protected string ValidateListForNumbers(string stringList)
        {
            string[] _list = stringList.Split(',');
            StringBuilder _sbList = new StringBuilder();
            foreach (string _item in _list)
            {
                if (Regex.IsMatch(_item, "^[0-9]+$"))
                {
                    _sbList.Append(_item);
                    _sbList.Append(",");
                }
            }
            _sbList.Remove(_sbList.Length - 1, 1);
            return _sbList.ToString();
        }

        public string ConvertListToStrArr(string fieldName, string deliId)
        {
            StringBuilder _sbList = new StringBuilder();

            if (fieldName == "so")
            {
                List<Business.SubOwners> searchresult = objDml.GetSubowners(deliId, true);
                foreach (Business.SubOwners objSO in searchresult)
                {
                    _sbList.Append(objSO.Name);
                    _sbList.Append(", ");
                }
            }
            else if (fieldName == "appr")
            {
                List<Business.DeliverableApprovers> searchresult = objDml.GetApprovers(deliId);
                foreach (Business.DeliverableApprovers objApp in searchresult)
                {
                    _sbList.Append(objApp.ApproverName);
                    _sbList.Append(", ");
                }
            }
            else if (fieldName == "notify")
            {
                List<Business.DeliverableNotification> searchresult = objDml.GetNotificationSchedule(deliId);
                foreach (Business.DeliverableNotification objNotify in searchresult)
                {
                    _sbList.Append(objNotify.LookupDesc);
                    _sbList.Append(", ");
                }
            }

            if (_sbList.Length > 0)
            {
                _sbList.Remove(_sbList.Length - 2, 2);
            }
            return _sbList.ToString();

        }

        protected ArrayList ConvertStringToArray(string inputString)
        {
            ArrayList SplitToArray = new ArrayList(inputString.Split(new char[] { ',' }));
            return SplitToArray;


        }

        protected int ReturnStartDays(int dueDays)
        {
            int _startdays = -1;

            if (dueDays == 1)
            {
                _startdays = -1;
            }
            else if (dueDays == 10)
            {
                _startdays = 1;
            }
            else if (dueDays == 30)
            {
                _startdays = 10;
            }
            else if (dueDays == 60)
            {
                _startdays = 30;
            }
            else if (dueDays == 90)
            {
                _startdays = 60;
            }
            return _startdays;
        }

        protected void InsertSubownerFileAction(bool isDone, string emailSent, int deliID)
        {
            string _emailSent = "N";
            string _isDone = "N";

            _emailSent = (emailSent == "0") ? "Y" : "N";
            _isDone = (isDone) ? "Y" : "N";

            objDml.AddSOFileAction(deliID, objCommon.GetUserID(), _emailSent, _isDone);



        }

        public void RemoveHyperLink(Control grid)
        {
            Literal literal = new Literal();

            for (int i = 0; i < grid.Controls.Count; i++)
            {
                if (grid.Controls[i] is HyperLink)
                {
                    literal.Text = (grid.Controls[i] as HyperLink).Text;
                    grid.Controls.Remove(grid.Controls[i]);
                    grid.Controls.AddAt(i, literal);
                }

                if (grid.Controls[i].HasControls())
                {
                    RemoveHyperLink(grid.Controls[i]);
                }

            }
        }

        protected string CheckIfEmailOn()
        {
            objEmail = objDml.GetEmailSetting();
            if (objEmail != null)
            {
                return  objEmail.SendEmail.ToString();
            }
            else return "";

        }
    }
}