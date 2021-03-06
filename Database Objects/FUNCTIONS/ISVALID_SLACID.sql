--------------------------------------------------------
--  DDL for Function ISVALID_SLACID
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE FUNCTION "APPS_ADMIN"."ISVALID_SLACID" 
(
p_SLACID VARCHAR2
)
RETURN BOOLEAN
IS
l_COUNT NUMBER;
BEGIN
SELECT COUNT(*) INTO l_COUNT FROM DW_PEOPLE_CURRENT WHERE EMPLOYEE_ID = p_SLACID;
IF (l_COUNT = 0) THEN
RETURN FALSE;
ELSE
RETURN TRUE;
END IF;
EXCEPTION
WHEN OTHERS THEN
    RETURN FALSE;

END ISVALID_SLACID;

/
