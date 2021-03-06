--------------------------------------------------------
--  DDL for View VW_CMS_CLAUSE_DETAILS
--------------------------------------------------------

  CREATE OR REPLACE FORCE EDITIONABLE VIEW "APPS_ADMIN"."VW_CMS_CLAUSE_DETAILS" ("CLAUSE_ID", "CLAUSE_NAME", "CLAUSE_NUMBER", "PARENT_ID", "OWNER", "CREATED_BY", "CREATED_ON", "IS_ACTIVE", "CONTRACT_ID", "MODIFIED_BY", "MODIFIED_DATE", "OWNERNAME", "CONTRACT_NAME", "PARENTOWNER", "PARENTCONTRACT", "PARENTCLAUSE", "PARENTCLAUSENUM", "PARENTCONTRACTNAME", "CLAUSETYPE") AS 
  SELECT CL."CLAUSE_ID",
CL."CLAUSE_NAME",
CL."CLAUSE_NUMBER",
CL."PARENT_ID",
CL."OWNER",
CL."CREATED_BY",
CL."CREATED_ON",
CL."IS_ACTIVE",
CL."CONTRACT_ID",
CL."MODIFIED_BY",
CL."MODIFIED_DATE",
PC.EMPLOYEE_NAME AS OWNERNAME,
CONT.CONTRACT_NAME,
CL2.OWNER AS PARENTOWNER,
CL2.CONTRACT_ID AS PARENTCONTRACT,
CL2.CLAUSE_NAME AS PARENTCLAUSE,
CL2.CLAUSE_NUMBER AS PARENTCLAUSENUM,
CONT1.CONTRACT_NAME AS PARENTCONTRACTNAME,
CASE WHEN CL.PARENT_ID IS NULL THEN 'Clause' ELSE 'Subclause' END as CLAUSETYPE
FROM CMS_CLAUSE CL LEFT JOIN DW_PEOPLE PC ON CL.OWNER = PC.EMPLOYEE_ID
LEFT JOIN CMS_CONTRACT_OTHERTYPES CONT ON CONT.CONTRACT_ID = CL.CONTRACT_ID LEFT JOIN CMS_CLAUSE CL2 ON CL.PARENT_ID = CL2.CLAUSE_ID
LEFT JOIN CMS_CONTRACT_OTHERTYPES CONT1 ON CONT1.CONTRACT_ID = CL2.CONTRACT_ID WHERE CL.IS_ACTIVE='Y'
;
