﻿//$Header:$
//
//  CMS_Common_Util.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2013 SLAC. All rights reserved.
// U.S. Department of Energy under contract number DE-AC02-76SF00515
// DOE O 241.1B, SCIENTIFIC AND TECHNICAL INFORMATION MANAGEMENT In the performance of Department of Energy(DOE) contracted obligations, each contractor is required to manage scientific and technical information(STI) produced under the contract as a direct and integral part of the work and ensure its broad availability to all customer segments by making STI available to DOE's central STI coordinating office, the Office of Scientific and Technical Information (OSTI).

//
//  This is class with common functions used by other pages
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.OracleClient;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using log4net;
using System.Web.SessionState;



namespace ContractManagement.Business
{
    public class CMS_Common_Util
    {
    
        protected static readonly ILog Log = LogManager.GetLogger(typeof(CMS_Common_Util));
        Data.CMS_DataUtil objData = new Data.CMS_DataUtil();
   
        public void CreateMessageAlert(Page senderPage, string alertMsg, string alertKey, bool loc)
        {
            string strScript;
            string _cleanMsg;

            _cleanMsg = alertMsg.Replace("'", "\\'");
            if (loc)
            {
                strScript = "<script language=JavaScript> alert('" + _cleanMsg + "');   window.location.href ='Default.aspx';</script>";
            }
            else
            {
                strScript = "<script language=JavaScript> alert('" + _cleanMsg + "');</script>";
            }


            if (!(senderPage.ClientScript.IsStartupScriptRegistered(alertKey)))
            {
                senderPage.ClientScript.RegisterStartupScript(senderPage.GetType(), alertKey, strScript);


            }
        }
      
        public void CreateMessageAlertSM(Page senderpage, string alertmsg, string alertkey,bool loc)
        {
            string _cleanMsg;

            _cleanMsg = alertmsg.Replace("'", "\\'");
            string s2 = "<SCRIPT language='javascript'>";
            if (loc)
                s2 = s2 + "alert('" + _cleanMsg + "'); window.location.href ='Default.aspx';";
            else
                s2 = s2 + "alert('" + _cleanMsg + "');";
            s2 = s2 + " </script>";
            ScriptManager.RegisterStartupScript(senderpage, senderpage.GetType(),alertkey, s2, false);
        }

        public void CreateMessageAlertSM(Page senderpage, string alertmsg, string alertkey, string loc)
        {
            string _cleanMsg;

            _cleanMsg = alertmsg.Replace("'", "\\'");
            string s2 = "<SCRIPT language='javascript'>";

            s2 = s2 + "alert('" + _cleanMsg + "'); window.location.href ='" + loc + "';";
          
            s2 = s2 + " </script>";
            ScriptManager.RegisterStartupScript(senderpage, senderpage.GetType(), alertkey, s2, false);
        }
    

        public string WrapNeat(string text)
        {
            if (text.Length == 0)
            {
                return ("&nbsp;");
            }
            else
            {
               return Regex.Replace(text,@"\r", "<br/>");
            }
        }
        
        public void ConfirmMessage(Page senderPage, string alertMsg, string loc)
        {
            string _strScript;
            _strScript = "<script language=JavaScript> var r=confirm('" + alertMsg + "'); if (r==false)  { window.location.href = '" + loc + "'; } </script>";

            if (!(senderPage.ClientScript.IsStartupScriptRegistered("confirm")))
            {
                senderPage.ClientScript.RegisterStartupScript(senderPage.GetType(), "confirm", _strScript);

            }

        }
        public bool IsUrlValid(string url) {return Regex.IsMatch(url, @"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?"); }

        public  string GetEnumDescription(Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return en.ToString();
        }

        public string FixDBNull(object rdrObject)
        {
            if (rdrObject.Equals(DBNull.Value))
            { return ""; }
            else { return rdrObject.ToString(); }
        }

        public string GetUserID()
        {
            string UserId = null;
            string sCurrentUser = null;

            string sVar = null;
            string sUserName = null;
            int sPOS = 0;
            string sqlSelect = null;
            OracleDataReader drUser = null;
            OracleCommand cmdUser = new OracleCommand();

            UserId = Convert.ToString(HttpContext.Current.Session["EmpID"]); // "355291"; //

            if (!string.IsNullOrEmpty(UserId))
            {
                HttpContext.Current.Session["EmpID"] = UserId;
                sCurrentUser = UserId;
            }
            else
            {

                sVar = HttpContext.Current.Request.ServerVariables["LOGON_USER"];
                sPOS = sVar.LastIndexOf("\\");
                sUserName = sVar.Substring(sPOS + 1);

                sqlSelect = "select max(but_sid) from but where but_kid = upper(:UserName)";

                try
                {
                    cmdUser.Parameters.Add(":UserName", OracleType.VarChar).Value = sUserName;
                    drUser = objData.GetReader(sqlSelect, cmdUser);
                    if (drUser.HasRows)
                    {
                        while (drUser.Read())
                        {
                            UserId = Convert.ToString(drUser[0]);
                        }
                    }
                    Log.Info(" GetUserId" + UserId);
                    
                }
                catch (NullReferenceException exnull)
                {
                   Log.Error(exnull);
                    UserId = "err"; 
                }
                catch(Exception ex)
                {
                    Log.Error(ex);                                      
                }
                finally
                {
                    if (drUser != null)
                    {
                        drUser.Close();
                    }
                }
                HttpContext.Current.Session["EmpID"] = UserId;
                sCurrentUser = UserId;
            }
            return sCurrentUser;
        }

     
        public string GetEmpname(string empid)
   {
       string _empName = "";
       OracleDataReader _empNamedr= null;
       OracleCommand _cmd = new OracleCommand();
 
       try
       {
           string _sqlSelect = "SELECT EMPLOYEE_NAME FROM DW_PEOPLE_CURRENT WHERE EMPLOYEE_ID = :Empid";        
           _cmd.Parameters.Add(":Empid",OracleType.VarChar).Value  = empid;
           _empNamedr = objData.GetReader( _sqlSelect,_cmd);
           
           while (_empNamedr.Read())
           {
               _empName = Convert.ToString(FixDBNull(_empNamedr[0]));
           }
           return _empName;
       }
       finally
       {
           _empNamedr.Close();
           _cmd.Dispose();
       }
   }
       
        public int GetEmpid(string empname) 
{
    int _empId = 0;
    OracleDataReader _empIddr = null;
    OracleCommand _cmdEmpid = new OracleCommand();
    try
    {
        string _sqlEmpid = "SELECT EMPLOYEE_ID FROM DW_PEOPLE_CURRENT WHERE UPPER(EMPLOYEE_NAME) LIKE UPPER(:Empname)";
        _cmdEmpid.Parameters.Add(":Empname",OracleType.VarChar).Value= empname.Trim();
        _empIddr = objData.GetReader(_sqlEmpid, _cmdEmpid);
        while (_empIddr.Read())
        {
            _empId = Convert.ToInt32(FixDBNull(_empIddr[0]));
            
        }
    }
    catch { _empId= 0; }
    finally{
        _empIddr.Close();
        _cmdEmpid.Dispose();
    }
    return _empId;
}

        public string GetFullName(string name)
        {
            string[] mAr = null;
            string temp = null;
            
            name = name.Replace( ",", "");

            mAr = name.Split(' ');
            temp = (mAr[1]) + " " + (mAr[0]);
            return temp;

        }

        public bool tryParse(string input)
        {

            int myIntNumber;




            if (int.TryParse(input, out myIntNumber) == true)
            {

                return true;

            }
            else { return false; }
        }

        public string ReplaceFirstOccurence(string wordToReplace, string replaceWith, string input)
        {

            Regex r = new Regex(wordToReplace, RegexOptions.IgnoreCase);

            return r.Replace(input, replaceWith, 1);

        }

        //For User controls
        public StringBuilder SetSBFilter(StringBuilder sbFilter)
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

      
    }

    //class for all description fields on a new / update operation
    public static class WordCharExtension
    {
        public static string ReplaceWordChars(this string text)
        {
            StringBuilder sb = new StringBuilder(text);
            //sb.Replace("•", "--"); //Replace unordered lists
            //sb.Replace ("","--");
          
     
            var s = sb.ToString();

            // smart single quotes and apostrophe           
            s = Regex.Replace(s, "[\u2018|\u2019|\u201A]", "'");
            // smart double quotes           
            s = Regex.Replace(s, "[\u201C|\u201D|\u201E]", "\"");
            // ellipsis           
            s = Regex.Replace(s, "\u2026", "...");
            // dashes           
            s = Regex.Replace(s, "[\u2013|\u2014]", "-");
            // circumflex        
            s = Regex.Replace(s, "\u02C6", "^");
            // open angle bracket  
            //! angular brackets are not getting inserted into db
            s = Regex.Replace(s, "\u2039", "<");
           //  close angle bracket          
            s = Regex.Replace(s, "\u203A", ">");
           //  spaces           
            s = Regex.Replace(s, "[\u02DC|\u00A0]", " ");
           
            //rest  put it as empty string
            //string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            //Regex.Replace(s, re, "");
            s = Regex.Replace(s, "[^0-9a-zA-Z\\[\\]!\"\\t\\r\\n#$%&'()*+,./:;=?@^_`{|}~-]+", " "); 
            //[\]\[!"#$%&'()*+,./:;<=>?@\^_`{|}~-]
         
            return s;
        }

        /// <summary>
        /// Replaces the characters (" ' ...) Micrsoft Word special characters
        /// with their UTF-8 standard character entities.
        /// </summary>
        /// <param name="sToReplace">The string to search and replace these characters in.</param>
        public static string ReplaceMicrosoftWordCharacters(string sToReplace)
        {
            sToReplace = sToReplace.Replace((char)130, '\'');
            sToReplace = sToReplace.Replace((char)132, '"');
            sToReplace = sToReplace.Replace(((char)133).ToString(), "...");
            sToReplace = sToReplace.Replace((char)145, '\'');
            sToReplace = sToReplace.Replace((char)146, '\'');
            sToReplace = sToReplace.Replace((char)147, '"');
            sToReplace = sToReplace.Replace((char)148, '"');
            return sToReplace;
        }


    }

    //class for all multiline fields to break to nextline if there is no space between the words
    //for all view items
    public static class WordWrapExtension
    {
        public const string _newline = "\r\n";

        /// <summary>
        /// Word wraps the given text to fit within the specified width.
        /// </summary>
        /// <param name="text">Text to be word wrapped</param>
        /// <param name="width">Width, in characters, to which the text
        /// should be word wrapped</param>
        /// <returns>The modified text</returns>
        public static string WordWrap(string text, int width)
        {
            int pos, next;
            StringBuilder sb = new StringBuilder();

            // Lucidity check
            if (width < 1)
                return text;

            // Parse each line of text
            for (pos = 0; pos < text.Length; pos = next)
            {
                // Find end of line
                int eol = text.IndexOf(_newline, pos);
                if (eol == -1)
                    next = eol = text.Length;
                else
                    next = eol + _newline.Length;

                // Copy this line of text, breaking into smaller lines as needed
                if (eol > pos)
                {
                    do
                    {
                        int len = eol - pos;
                        if (len > width)
                            len = BreakLine(text, pos, width);

                        sb.Append(text, pos, len);
                        sb.Append(_newline);
                        // Trim whitespace following break
                        pos += len;
                        while (pos < eol && Char.IsWhiteSpace(text[pos]))
                            pos++;
                    } while (eol > pos);
                }
                else sb.Append(_newline); // Empty line
            }
            return sb.ToString();
        }

        /// <summary>
        /// Locates position to break the given line so as to avoid
        /// breaking words.
        /// </summary>
        /// <param name="text">String that contains line of text</param>
        /// <param name="pos">Index where line of text starts</param>
        /// <param name="max">Maximum line length</param>
        /// <returns>The modified line length</returns>
        public static int BreakLine(string text, int pos, int max)
        {
            // Find last whitespace in line
            int i = max - 1;
            while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
                i--;
            if (i < 0)
                return max; // No whitespace found; break at maximum length

            // Find start of whitespace
            while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
                i--;

            // Return length of text before whitespace
            return i + 1;


        }
    }


    public class SessionVar
    {
        static HttpSessionState Session
        {
            get
            {
                if (HttpContext.Current == null)
                    throw new ApplicationException("No Http Context, no Session to Get!");
                return HttpContext.Current.Session;
            }

        }

        public static T Get<T>(string key)
        {
            if (Session[key] == null)
                return default(T);
            else
                return (T)Session[key];

        }

        public static void Set<T>(string key, T value)
        {
            Session[key] = value;
        }

        //Getters/setters for specific types, for e.g string
        public static string GetString(string key)
        {
            string s = Get<string>(key);
            return s == null ? string.Empty : s;
        }

        public static void SetString(string key, string value)
        {
            Set<string>(key, value);
        }
    }

    public static class DateTimeExtension
    {
         public static string ToFinancialYearShort(this DateTime dateTime)
        {
            return "FY" + (dateTime.Month >= 10 ? dateTime.AddYears(1).ToString("yy") : dateTime.ToString("yy"));
        }
    }
    
    
}