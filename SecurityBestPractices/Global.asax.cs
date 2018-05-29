﻿using System;
using DevExpress.DashboardWeb;
using DevExpress.DataAccess.Web;
using DevExpress.XtraReports.Web.QueryBuilder;
using DevExpress.XtraReports.Web.QueryBuilder.Native;
using DevExpress.XtraReports.Web.ReportDesigner;
using SecurityBestPractices.Authorization;
using SecurityBestPractices.Authorization.Dashboards;
using SecurityBestPractices.Authorization.Reports;

namespace SecurityBestPractices
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e) {
            #region Query builder
            DefaultQueryBuilderContainer.Register<IDataSourceWizardConnectionStringsProvider, DataSourceWizardConnectionStringsProvider>();
            DefaultQueryBuilderContainer.RegisterDataSourceWizardDBSchemaProviderExFactory<DataSourceWizardDBSchemaProviderExFactory>();
            
            //DevExpress.XtraReports.Web.ASPxQueryBuilder.StaticInitialize();
            #endregion            

            #region Reports
            DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension.RegisterExtensionGlobal(new ReportStorageWithAccessRules());
            DefaultReportDesignerContainer.RegisterDataSourceWizardConnectionStringsProvider<DataSourceWizardConnectionStringsProvider>(); // provide connections for report designer
            DefaultReportDesignerContainer.RegisterDataSourceWizardDBSchemaProviderExFactory<DataSourceWizardDBSchemaProviderExFactory>(); // provide only nessesary dbtables

            DevExpress.XtraReports.Web.WebDocumentViewer.Native.WebDocumentViewerBootstrapper.SessionState = System.Web.SessionState.SessionStateBehavior.Required;
            DevExpress.XtraReports.Web.ASPxReportDesigner.StaticInitialize();
            #endregion           

            #region Dashboards
            DashboardConfigurator.Default.SetDashboardStorage(new DashboardStorageWithAccessRules());
            DashboardConfigurator.Default.CustomParameters += (o, args) => {
                if (!new DashboardStorageWithAccessRules().IsAuthorized(args.DashboardId))
                    throw new UnauthorizedAccessException();
            };

            DashboardConfigurator.Default.SetConnectionStringsProvider(new DataSourceWizardConnectionStringsProvider()); // provide connections for dashboard designer
            DashboardConfigurator.Default.SetDBSchemaProvider(new DBSchemaProviderEx()); // provide only nessesary dbtables
            #endregion            
        }
    }
}