--------------------------------------------------------
--  DDL for View VW_CMS_CONTRACT_DETAILS
--------------------------------------------------------

  CREATE OR REPLACE FORCE EDITIONABLE VIEW "APPS_ADMIN"."VW_CMS_CONTRACT_DETAILS" ("CONTRACT_ID", "PARENT_ID", "CONTRACT_NAME", "IS_ACTIVE", "CREATED_BY", "CREATED_ON", "MODIFIED_BY", "MODIFIED_DATE", "GROUP_ID", "SHORT_NAME", "CONTRACTTYPE", "PARENTCONTRACT") AS 
  SELECT CON."CONTRACT_ID",CON."PARENT_ID",CON."CONTRACT_NAME",CON."IS_ACTIVE",CON."CREATED_BY",CON."CREATED_ON",CON."MODIFIED_BY",CON."MODIFIED_DATE",CON."GROUP_ID",CON."SHORT_NAME",
(CASE WHEN CON.PARENT_ID IS NULL THEN (DECODE(CON.GROUP_ID,3,'','Contract')) ELSE 'Subcontract' END) AS CONTRACTTYPE, CON1.CONTRACT_NAME AS PARENTCONTRACT
FROM CMS_CONTRACT_OTHERTYPES CON,CMS_CONTRACT_OTHERTYPES CON1
WHERE CON.PARENT_ID = CON1.CONTRACT_ID(+)  AND CON.IS_ACTIVE='Y'
;
