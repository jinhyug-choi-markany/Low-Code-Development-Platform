IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IU_CtButtonHlp')
    DROP INDEX CtButtonHlp.IU_CtButtonHlp 

CREATE  UNIQUE INDEX IU_CtButtonHlp ON CtButtonHlp(CultureId, ButtonTypeId)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_CtCulture')
    DROP INDEX CtCulture.IX_CtCulture 

CREATE  UNIQUE INDEX IX_CtCulture ON CtCulture(CultureTypeName)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_CtCultureLbl')
    DROP INDEX CtCultureLbl.IX_CtCultureLbl 

CREATE  UNIQUE INDEX IX_CtCultureLbl ON CtCultureLbl(CultureId, CultureTypeId)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_CtEvent')
    DROP INDEX CtEvent.IX_CtEvent 

CREATE  UNIQUE INDEX IX_CtEvent ON CtEvent(EventCode)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_CtPermKey')
    DROP INDEX CtPermKey.IX_CtPermKey 

CREATE  UNIQUE INDEX IX_CtPermKey ON CtPermKey(CtPermKeyName)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_DbKey_KeyName')
    DROP INDEX DbKey.IX_DbKey_KeyName 

CREATE  UNIQUE INDEX IX_DbKey_KeyName ON DbKey(KeyName)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IU_DbTable')
    DROP INDEX DbTable.IU_DbTable 

CREATE  UNIQUE INDEX IU_DbTable ON DbTable(SystemId, TableName)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Label')
    DROP INDEX Label.IX_Label 

CREATE  UNIQUE INDEX IX_Label ON Label(CultureId, LabelCat, LabelKey, CompanyId)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IU_MemTrans')
    DROP INDEX MemTrans.IU_MemTrans 

CREATE  UNIQUE INDEX IU_MemTrans ON MemTrans(InStr, CultureTypeId)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_MenuHlp_MenuId')
    DROP INDEX MenuHlp.IX_MenuHlp_MenuId 

CREATE  UNIQUE INDEX IX_MenuHlp_MenuId ON MenuHlp(CultureId, MenuId)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_MsgCenter')
    DROP INDEX MsgCenter.IX_MsgCenter 

CREATE  UNIQUE INDEX IX_MsgCenter ON MsgCenter(CultureId, MsgId)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Num2Word_Digits')
    DROP INDEX Num2Word.IX_Num2Word_Digits 

CREATE  UNIQUE INDEX IX_Num2Word_Digits ON Num2Word(CultureId, Digit10, Digit09, Digit08, Digit07, Digit06, Digit05, Digit04, Digit03, Digit02, Digit01)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ReportCriHlp_ReportCriId')
    DROP INDEX ReportCriHlp.IX_ReportCriHlp_ReportCriId 

CREATE  UNIQUE INDEX IX_ReportCriHlp_ReportCriId ON ReportCriHlp(CultureId, ReportCriId)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ReportHlp_ReportId')
    DROP INDEX ReportHlp.IX_ReportHlp_ReportId 

CREATE  UNIQUE INDEX IX_ReportHlp_ReportId ON ReportHlp(CultureId, ReportId)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ReportLstCri')
    DROP INDEX ReportLstCri.IX_ReportLstCri 

CREATE  UNIQUE INDEX IX_ReportLstCri ON ReportLstCri(UsrId, ReportId, ReportCriId)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ReportObjHlp_ReportObjId')
    DROP INDEX ReportObjHlp.IX_ReportObjHlp_ReportObjId 

CREATE  UNIQUE INDEX IX_ReportObjHlp_ReportObjId ON ReportObjHlp(CultureId, ReportObjId)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_RptMemCri')
    DROP INDEX RptMemCri.IX_RptMemCri 

CREATE INDEX IX_RptMemCri ON RptMemCri(UsrId, ReportId)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_RptTemplate')
    DROP INDEX RptTemplate.IX_RptTemplate 

CREATE INDEX IX_RptTemplate ON RptTemplate(MasterId, DocName)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ScrAudit')
    DROP INDEX ScrAudit.IX_ScrAudit 

CREATE INDEX IX_ScrAudit ON ScrAudit(ScreenId, ChangedOn)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ScrAuditDtl')
    DROP INDEX ScrAuditDtl.IX_ScrAuditDtl 

CREATE INDEX IX_ScrAuditDtl ON ScrAuditDtl(ScrAuditId)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ScreenCriHlp_ScreenCriId')
    DROP INDEX ScreenCriHlp.IX_ScreenCriHlp_ScreenCriId 

CREATE  UNIQUE INDEX IX_ScreenCriHlp_ScreenCriId ON ScreenCriHlp(CultureId, ScreenCriId)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ScreenFilter_Id_Name')
    DROP INDEX ScreenFilter.IX_ScreenFilter_Id_Name 

CREATE  UNIQUE INDEX IX_ScreenFilter_Id_Name ON ScreenFilter(ScreenId, ScreenFilterName)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ScreenFilterHlp_FilterId')
    DROP INDEX ScreenFilterHlp.IX_ScreenFilterHlp_FilterId 

CREATE  UNIQUE INDEX IX_ScreenFilterHlp_FilterId ON ScreenFilterHlp(CultureId, ScreenFilterId)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ScreenLstCri')
    DROP INDEX ScreenLstCri.IX_ScreenLstCri 

CREATE  UNIQUE INDEX IX_ScreenLstCri ON ScreenLstCri(UsrId, ScreenId, ScreenCriId)
if exists (select * from dbo.sysobjects where id = object_id(N'dbo.FK_ScreenLstCri_UsrId') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE dbo.ScreenLstCri DROP CONSTRAINT FK_ScreenLstCri_UsrId 


ALTER TABLE ScreenLstCri ADD
CONSTRAINT FK_ScreenLstCri_UsrId FOREIGN KEY
(
ScreenId)
 REFERENCES [dbo].[Screen]
(
ScreenId)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ScreenLstInf')
    DROP INDEX ScreenLstInf.IX_ScreenLstInf 

CREATE INDEX IX_ScreenLstInf ON ScreenLstInf(UsrId, ScreenId)
GO

if exists (select * from dbo.sysobjects where id = object_id(N'dbo.FK_ScreenObj_Screen') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE dbo.ScreenObj DROP CONSTRAINT FK_ScreenObj_Screen 


ALTER TABLE ScreenObj ADD
CONSTRAINT FK_ScreenObj_Screen FOREIGN KEY
(
ScreenId)
 REFERENCES [dbo].[Screen]
(
ScreenId)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ScreenTabHlp_ScreenTabId')
    DROP INDEX ScreenTabHlp.IX_ScreenTabHlp_ScreenTabId 

CREATE  UNIQUE INDEX IX_ScreenTabHlp_ScreenTabId ON ScreenTabHlp(CultureId, ScreenTabId)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IU_SctGrpCol')
    DROP INDEX SctGrpCol.IU_SctGrpCol 

CREATE  UNIQUE INDEX IU_SctGrpCol ON SctGrpCol(SectionCd, GroupColId)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IU_SctGrpRow')
    DROP INDEX SctGrpRow.IU_SctGrpRow 

CREATE  UNIQUE INDEX IU_SctGrpRow ON SctGrpRow(SectionCd, GroupRowId)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ServerRule')
    DROP INDEX ServerRule.IX_ServerRule 

CREATE INDEX IX_ServerRule ON ServerRule(ScreenId, RuleOrder)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IU_SredMebr')
    DROP INDEX SredMebr.IU_SredMebr 

CREATE  UNIQUE INDEX IU_SredMebr ON SredMebr(MemberId, UserId)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Template_TmplDefault')
    DROP INDEX Template.IX_Template_TmplDefault 

CREATE  UNIQUE INDEX IX_Template_TmplDefault ON Template(TmplDefault DESC, TemplateName)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Usage_UsrId')
    DROP INDEX Usage.IX_Usage_UsrId 

CREATE INDEX IX_Usage_UsrId ON Usage(UsrId, EntityTitle)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IU_Usr')
    DROP INDEX Usr.IU_Usr 

CREATE  UNIQUE INDEX IU_Usr ON Usr(LoginName)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_UsrAudit')
    DROP INDEX UsrAudit.IX_UsrAudit 

CREATE INDEX IX_UsrAudit ON UsrAudit(LoginName)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_UsrGroup_UsrGroupName')
    DROP INDEX UsrGroup.IX_UsrGroup_UsrGroupName 

CREATE  UNIQUE INDEX IX_UsrGroup_UsrGroupName ON UsrGroup(UsrGroupName)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_UsrGroupAuth')
    DROP INDEX UsrGroupAuth.IX_UsrGroupAuth 

CREATE  UNIQUE INDEX IX_UsrGroupAuth ON UsrGroupAuth(UsrGroupId, CompanyId, ProjectId, SystemId)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_UtReport_ProgramName')
    DROP INDEX UtReport.IX_UtReport_ProgramName 

CREATE  UNIQUE INDEX IX_UtReport_ProgramName ON UtReport(ProgramName)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IU_Wizard')
    DROP INDEX Wizard.IU_Wizard 

CREATE  UNIQUE INDEX IU_Wizard ON Wizard(ProgramName)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_WizardObj_WizardId')
    DROP INDEX WizardObj.IX_WizardObj_WizardId 

CREATE INDEX IX_WizardObj_WizardId ON WizardObj(WizardId, TabIndex)
GO

IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_WizardRule_WizardId')
    DROP INDEX WizardRule.IX_WizardRule_WizardId 

CREATE  UNIQUE INDEX IX_WizardRule_WizardId ON WizardRule(WizardId, RuleOrder)
GO