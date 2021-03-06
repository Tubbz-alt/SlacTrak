﻿//$Header:$
//
// U.S. Department of Energy under contract number DE-AC02-76SF00515
// DOE O 241.1B, SCIENTIFIC AND TECHNICAL INFORMATION MANAGEMENT In the performance of Department of Energy(DOE) contracted obligations, each contractor is required to manage scientific and technical information(STI) produced under the contract as a direct and integral part of the work and ensure its broad availability to all customer segments by making STI available to DOE's central STI coordinating office, the Office of Scientific and Technical Information (OSTI).
// DynamicTextBox.ascx.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2013 SLAC. All rights reserved.
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace ContractManagement.User_Controls
{
    public partial class DynamicTextBox : System.Web.UI.UserControl
    {
        Business.CMS_Common_Util objCommon = new Business.CMS_Common_Util();
        protected void Page_Load(object sender, EventArgs e)
        {
            img0.Attributes.Add("onClick", "OpenJQueryDialogSO('dialogowner', 'txt0','y'); return false;");
            cv0.ServerValidate += new ServerValidateEventHandler(cv_ServerValidate);
        }

    

        void Createdynamicctrls(int i)
        {
            HtmlTable tbl = new HtmlTable();
            HtmlTableRow row = new HtmlTableRow();

 
            HtmlTableCell cell = new HtmlTableCell();
            TextBox txt = new TextBox();
            txt.ID = "txt" + (i+1).ToString();
            txt.Width = Unit.Pixel(150);
            txt.ToolTip = "Please use Find to select names!!";
            cell.Controls.Add(txt);
            row.Cells.Add(cell);
  

            CustomValidator cv = new CustomValidator();
            cv.ID = "cv" + (i+1).ToString();
            cv.ControlToValidate = txt.ID;
            cv.ErrorMessage = "Not a valid name/format";
            cv.CssClass = "errlabels";
           // cv.ValidationGroup = "add";


            HtmlTableCell cell1 = new HtmlTableCell();
            ImageButton img = new ImageButton();
            img.ID = "img" + (i+1).ToString();
            img.ImageUrl = "../Images/find.gif";
            img.CausesValidation = false;
            cell1.Controls.Add(img);
            row.Cells.Add(cell1);

            string txtID;
            txtID = txt.ID.ToString();
            txt.ClientIDMode = ClientIDMode.Static;
            img.Attributes.Add("onClick", "OpenJQueryDialogSO('dialogowner', '" + txtID + "','y'); return false;");
            HtmlTableCell cell2 = new HtmlTableCell();
            cell2.Controls.Add(cv);
            row.Cells.Add(cell2);

 
            tbl.Rows.Add(row);
            pnlsubowner.Controls.Add(tbl);
            cv.ServerValidate += new ServerValidateEventHandler(cv_ServerValidate);

        }
      
        public int TBControlsCount
        {
            get
            {
                if ((ViewState["tbCount"] != null))
                {
                    return Convert.ToInt32(ViewState["tbCount"]);
                }
                else
                {
                    return 0;
                }
            }
            set { ViewState["tbCount"] = value; }
        }
        protected void CheckforValidNames(object source, ServerValidateEventArgs args)
        {
            string data = args.Value;
            args.IsValid = false;
            bool bValid;

            bValid = CheckNameValid(data);


            if (bValid)
            {
                args.IsValid = true;
            }

        }
      
       public void cv_ServerValidate(object source, ServerValidateEventArgs args)
        {
         
            CheckforValidNames(source, args);
                   }
 
        protected override void LoadViewState(object savedState)
        {

            base.LoadViewState(((Pair)savedState).First);
            CreateTextBox();
        }
        protected override object SaveViewState()
        {
            return new Pair(base.SaveViewState(), null);
        }

        public void CreateTextBox()
        {
            Panel pnlsubowner = new Panel();
            pnlsubowner = (Panel)this.FindControl("pnlsubowner");
            pnlsubowner.Controls.Clear();
            for (int i = 0; i < TBControlsCount; i++)
            {
                Createdynamicctrls(i);
            }
        }

        protected bool CheckNameValid(string owner)
        {
            int _owner;

            if (owner != "")
            {
                _owner = objCommon.GetEmpid(owner.Trim());
                if (_owner == 0)
                    return false;
                else
                    return true;
            }
            else
            { return false; }
        }

        protected void cmdAddSubOwner_Click(object sender, EventArgs e)
        {
            try
            {
               // if (divsubowner.Visible)
               // {
                    Createdynamicctrls(TBControlsCount);
                    TBControlsCount += 1;
                    if (TBControlsCount >= 7)
                    { cmdAddSubOwner.Enabled = false; }
                    else { cmdAddSubOwner.Enabled = true; }
               // }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }


      

    }
}