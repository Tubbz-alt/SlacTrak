--------------------------------------------------------
--  DDL for Table CMS_CLAUSE_AUDIT
--------------------------------------------------------

  CREATE TABLE "APPS_ADMIN"."CMS_CLAUSE_AUDIT" 
   (	"CLAUSE_ID" NUMBER, 
	"CLAUSE_NAME" VARCHAR2(400 BYTE), 
	"CLAUSE_NUMBER" VARCHAR2(120 BYTE), 
	"PARENT_ID" NUMBER, 
	"OWNER" NUMBER, 
	"CREATED_BY" VARCHAR2(50 BYTE), 
	"CREATED_ON" DATE, 
	"CONTRACT_ID" NUMBER, 
	"CLAUSE_AUDIT_ID" NUMBER, 
	"MODIFIED_BY" VARCHAR2(50 BYTE), 
	"MODIFIED_DATE" DATE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "APPS_ADMIN_DATA" ;
