--------------------------------------------------------
--  DDL for Package CMS_FILE_UPLOAD_PKG
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE "APPS_ADMIN"."CMS_FILE_UPLOAD_PKG" AS

  PROCEDURE PROC_INS_FILEDATA
  (
      PI_DELIVERABLE_ID IN CMS_ATTACHMENT.DELIVERABLE_ID%TYPE,
      PI_FILE_NAME IN CMS_ATTACHMENT.FILE_NAME%TYPE,
      PI_FILE_SIZE IN CMS_ATTACHMENT.FILE_SIZE%TYPE,
      PI_FILE_CONTENT_TYPE IN CMS_ATTACHMENT.FILE_CONTENT_TYPE%TYPE,
      PI_FILEDATA IN CMS_ATTACHMENT.FILE_DATA%TYPE,
      PI_UPLOADED_BY IN CMS_ATTACHMENT.UPLOADED_BY%TYPE,
      PO_ATTACHMENT_ID OUT NUMBER
  );


  PROCEDURE PROC_DEL_FILEDATA
  (
    PI_ATTACHMENT_ID IN CMS_ATTACHMENT.ATTACHMENT_ID%TYPE,
     PI_CHANGED_BY IN CMS_ATTACHMENT.CHANGED_BY%TYPE,
    PO_RETURN_CODE OUT NUMBER
  );


END CMS_FILE_UPLOAD_PKG;

/
