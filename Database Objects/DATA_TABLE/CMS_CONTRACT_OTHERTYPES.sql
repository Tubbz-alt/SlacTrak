--------------------------------------------------------
--  File created - Tuesday-August-20-2019   
--------------------------------------------------------
REM INSERTING into CMS_CONTRACT_OTHERTYPES
SET DEFINE OFF;
Insert into CMS_CONTRACT_OTHERTYPES (CONTRACT_ID,PARENT_ID,CONTRACT_NAME,IS_ACTIVE,CREATED_BY,CREATED_ON,MODIFIED_BY,MODIFIED_DATE,GROUP_ID,SHORT_NAME) values (1,null,'Prime Contract','Y','APPS_ADMIN',to_date('03-MAY-13','DD-MON-RR'),'APPS_ADMIN',to_date('03-MAY-13','DD-MON-RR'),2,'PC');
Insert into CMS_CONTRACT_OTHERTYPES (CONTRACT_ID,PARENT_ID,CONTRACT_NAME,IS_ACTIVE,CREATED_BY,CREATED_ON,MODIFIED_BY,MODIFIED_DATE,GROUP_ID,SHORT_NAME) values (2,null,'Appendix E','Y','APPS_ADMIN',to_date('03-MAY-13','DD-MON-RR'),'APPS_ADMIN',to_date('25-JUN-13','DD-MON-RR'),1,'ADE');
Insert into CMS_CONTRACT_OTHERTYPES (CONTRACT_ID,PARENT_ID,CONTRACT_NAME,IS_ACTIVE,CREATED_BY,CREATED_ON,MODIFIED_BY,MODIFIED_DATE,GROUP_ID,SHORT_NAME) values (3,2,'Contractual DOE Directive','Y','APPS_ADMIN',to_date('03-MAY-13','DD-MON-RR'),'APPS_ADMIN',to_date('03-MAY-13','DD-MON-RR'),2,'DOEDIR');
Insert into CMS_CONTRACT_OTHERTYPES (CONTRACT_ID,PARENT_ID,CONTRACT_NAME,IS_ACTIVE,CREATED_BY,CREATED_ON,MODIFIED_BY,MODIFIED_DATE,GROUP_ID,SHORT_NAME) values (4,null,'Data Call','Y','APPS_ADMIN',to_date('03-MAY-13','DD-MON-RR'),'APPS_ADMIN',to_date('25-JUN-13','DD-MON-RR'),3,'DC');
Insert into CMS_CONTRACT_OTHERTYPES (CONTRACT_ID,PARENT_ID,CONTRACT_NAME,IS_ACTIVE,CREATED_BY,CREATED_ON,MODIFIED_BY,MODIFIED_DATE,GROUP_ID,SHORT_NAME) values (5,null,'DOE Request','Y','APPS_ADMIN',to_date('03-MAY-13','DD-MON-RR'),'APPS_ADMIN',to_date('25-JUN-13','DD-MON-RR'),3,'DOEREQ');
COMMIT;
