﻿using System;
using System.Xml.XPath;
using AlarmWorkflow.Shared.Core;
using MySql.Data.MySqlClient;

namespace AlarmWorkflow.Shared.Jobs
{
    /// <summary>
    /// Implements a job, who saves all the operation data to a MySQL database.
    /// </summary>
    [Export("DatabaseJob", typeof(IJob))]
    public class DatabaseJob : IJob
    {
        #region private members
        /// <summary>
        /// Saves the errormsg, if an error occured.
        /// </summary>
        private string errormsg;

        /// <summary>
        /// The database user.
        /// </summary>
        private string user;

        /// <summary>
        /// The database users passoword.
        /// </summary>
        private string pwd;

        /// <summary>
        /// Name of the database.
        /// </summary>
        private string database;

        /// <summary>
        /// URL o the MySQL server.
        /// </summary>
        private string server;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DatabaseJob class.
        /// </summary>
        public DatabaseJob()
        {
        }

        #endregion

        #region IJob Members

        string IJob.ErrorMessage
        {
            get
            {
                return this.errormsg;
            }
        }

        void IJob.Initialize(IXPathNavigable settings)
        {
            // NOTE: TENTATIVE CODE until settings are stored more dynamical!
            // Navigate to Database settings
            settings = settings.CreateNavigator().SelectSingleNode("DataBase");

            XPathNavigator nav = settings.CreateNavigator();
            this.database = nav.SelectSingleNode("DBName").InnerXml;
            this.user = nav.SelectSingleNode("UserID").InnerXml;
            this.pwd = nav.SelectSingleNode("UserPWD").InnerXml;
            this.server = nav.SelectSingleNode("DBServer").InnerXml;
        }

        bool IJob.DoJob(Operation einsatz)
        {
            this.errormsg = string.Empty;
            try
            {
                MySqlConnection conn = new MySqlConnection("Persist Security Info=False;database=" + this.database + ";server=" + this.server + ";user id=" + this.user + ";Password=" + this.pwd);
                conn.Open();
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    this.errormsg = "Coud not open SQL Connection";
                    return false;
                }

                string cmdText = "INSERT INTO tb_einstaz (Einsatznr, Einsatzort, Einsatzplan, Hinweis, Kreuzung, Meldebild, Mitteiler, Objekt, Ort, Strasse, Stichwort) VALUES ('" + einsatz.Einsatznr + "', '" + einsatz.Einsatzort + "', '" + einsatz.Einsatzplan + "', '" + einsatz.Hinweis + "', '" + einsatz.Kreuzung + "', '" + einsatz.Meldebild + "', '" + einsatz.Mitteiler + "', '" + einsatz.Objekt + "', '" + einsatz.Ort + "', '" + einsatz.Strasse + "', '" + einsatz.Stichwort + "')";
                MySqlCommand cmd = new MySqlCommand(cmdText, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                this.errormsg = ex.ToString();
                return false;
            }

            return true;
        }

        #endregion
    }
}