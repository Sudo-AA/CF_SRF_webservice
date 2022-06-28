using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Web.Hosting;
using System.Drawing;
using System.Drawing.Imaging;
using System.Configuration;
using System.Web;
using System.Drawing.Drawing2D;

namespace CF_SRF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class CF_SRF_SERVICE : ICF_SRF_SERVICE1
    {
        private string conn_string = "Data Source =CF-SQL\\CFSQLSERVER; Initial Catalog=cleanfuel; uid=sa; pwd=Cleanfuel1"; // live
        //private string conn_string = "Data Source =CF-IT\\CFSTNSERVER; Initial Catalog=CFappDatabase; uid=sa; pwd=Cl3@nfu3l"; // testing
        public static string stncode;
        public static string srfno;
        public static string fileloc = "E:\\SRF IMAGES\\";

        public List<station_method> GetStation_Method(String areacode)
        {
            SqlCommand sqlcmd = new SqlCommand();
            List<station_method> stationList = new List<station_method>();
            SqlConnection connection = new SqlConnection(this.conn_string);
            connection.Open();
            if (areacode.Trim() == "9999")
            {

                sqlcmd = new SqlCommand("SELECT stnCode,stnBranch FROM Stations WHERE stnOpenDate IS NOT NULL ORDER BY stnBranch ASC", connection);

            }
            else if (areacode.Trim() == "8888")
            {

                sqlcmd = new SqlCommand("SELECT Stations.stnCode,Stations.stnBranch FROM Stations WHERE stnOpenDate IS NOT NULL AND not exists (select OIC_android_id_login.OIC_stn_code from OIC_android_id_login WHERE Stations.stnCode =  OIC_android_id_login.OIC_stn_code)ORDER BY stnBranch ASC", connection);

            }
            else
            {
                sqlcmd = new SqlCommand("SELECT stnCode,stnBranch FROM Stations WHERE stnOpenDate IS NOT NULL AND stnArea = @areacode ORDER BY stnBranch ASC", connection);
                sqlcmd.Parameters.AddWithValue("@areacode", areacode.Trim());
            }

            sqlcmd.CommandType = System.Data.CommandType.Text;
            SqlDataReader sdr = sqlcmd.ExecuteReader();
            while (sdr.Read())
            {
                station_method station = new station_method();
                station.SRF_stnCode = sdr["stnCode"].ToString();
                station.SRF_station_name = sdr["stnBranch"].ToString();
                stationList.Add(station);

            }
            sdr.Close();
            connection.Close();
            return stationList.ToList();

        }

        // retrieve dafa

        public List<srf_get_data> retr_stn_srf(string stncode, string status, string cat_holder)
        {

            List<srf_get_data> SRF_List = new List<srf_get_data>();
            SqlConnection connection = new SqlConnection(this.conn_string);
            SqlCommand sqlcmd = new SqlCommand();
            string extra = "";
            String sort = "srfSysDate DESC";
            if (status.Equals("0001") || status.Equals("0014"))
            {
                extra = "TOP(20)";
                sort = "SRF.srfUpdateDate DESC";
            }
            else if (status.Equals("9999"))
            {
                extra = "";
                sort = "SRF.srfSysDate DESC";
            }
            else if (status.Equals("8888"))
            {
                extra = "TOP(20)";
                sort = "SRF.srfSysDate DESC";
            }
            else if (status.Equals("0016"))
            {
                extra = "TOP(10)";
            }

            connection.Open();
            sqlcmd = new SqlCommand("SELECT " + extra + "SRF.srfStnCode, ST.stnBranch, SRF.srfNo, convert(varchar,SRF.srfDate, 107) AS srfDate, SRF.srfCatCode, SRF.srfCatDesc, SRF.srfProblem, SRF.srfUser, SS.ssDesc, CASE WHEN SRF.srfUpdateUserID IS NULL THEN 'NO DATA' WHEN SRF.srfUpdateUserID IS NOT NULL THEN SRF.srfUpdateUserID END AS srfUpdateUserID, CASE WHEN SRF.srfUpdateDate IS NULL THEN 'NO UPDATE YET' WHEN SRF.srfUpdateDate IS NOT NULL THEN CONVERT(VARCHAR(20),SRF.srfUpdateDate,100) END AS srfUpdateDate, CASE WHEN SRF.srfCloseDate IS NULL THEN 'NOT CLOSED YET' WHEN SRF.srfCloseDate IS NOT NULL THEN CONVERT(VARCHAR(20),SRF.srfCloseDate,100) END AS srfCloseDate, CASE WHEN SRI.srfaFileName IS NULL THEN 'FALSE' WHEN SRI.srfaFileName IS NOT NULL THEN 'TRUE' END AS srfaFileName, CASE WHEN SRA.sraAction IS NULL THEN 'FALSE' WHEN SRA.sraAction IS NOT NULL THEN 'TRUE' END AS sraAction FROM ServiceRequestForm SRF LEFT JOIN ServiceRequestImages SRI ON SRF.srfStnCode = SRI.srfaStnCode AND SRF.srfNo = SRI.srfaNo LEFT JOIN ServiceReportAction SRA ON SRA.sraStnCode=SRF.srfStnCode AND SRF.srfNo = SRA.sraNo LEFT JOIN Stations ST ON ST.stnCode = SRF.srfStnCode LEFT JOIN SRFStatus SS ON SS.ssCode = SRF.srfStatus WHERE SRF.srfStatus = @SRF_Status AND SRF.srfStnCode= @SRF_StnCode AND SRF.srfCatCode LIKE '%"+cat_holder+"%' ORDER BY " + sort, connection);

            sqlcmd.Parameters.AddWithValue("@SRF_StnCode", stncode);
            sqlcmd.Parameters.AddWithValue("@SRF_Status", status);
            sqlcmd.CommandType = System.Data.CommandType.Text;
            SqlDataReader sdr = sqlcmd.ExecuteReader();
            while (sdr.Read())
            {
                srf_get_data SRF = new srf_get_data();
                SRF.SRF_Stn = sdr["stnBranch"].ToString().Trim();
                SRF.SRF_StnCode = sdr["srfStnCode"].ToString().Trim();
                SRF.SRF_No = sdr["srfNo"].ToString().Trim();
                SRF.SRF_Date = sdr["srfDate"].ToString().Trim();
                SRF.SRF_CatCode = sdr["srfCatCode"].ToString().Trim();
                SRF.SRF_CatDesc = sdr["srfCatDesc"].ToString().Trim();
                SRF.SRF_Problem = sdr["srfProblem"].ToString().Trim();
                SRF.SRF_User = sdr["srfUser"].ToString().Trim();
                SRF.SRF_Status = sdr["ssDesc"].ToString().Trim();
                SRF.Images_status = sdr["srfaFileName"].ToString().Trim();
                SRF.Action = sdr["sraAction"].ToString().Trim();
                SRF.Updated_by = sdr["srfUpdateUserID"].ToString().Trim();
                SRF.Updated_date = sdr["srfUpdateDate"].ToString().Trim();
                SRF.Closed_date = sdr["srfCloseDate"].ToString().Trim();


                SRF_List.Add(SRF);
            }


            return SRF_List.ToList();
        }


        public List<status_class> Getstatus_class()
        {

            List<status_class> status = new List<status_class>();
            SqlConnection connection = new SqlConnection(this.conn_string);
            connection.Open();
            SqlCommand sqlcmd = new SqlCommand("SELECT * FROM SRFStatus", connection);
            sqlcmd.CommandType = System.Data.CommandType.Text;
            SqlDataReader sdr = sqlcmd.ExecuteReader();
            while (sdr.Read())
            {
                status_class sta = new status_class();

                sta.Status_desc = sdr["ssDesc"].ToString();
                sta.Status_code = sdr["ssCode"].ToString();
                status.Add(sta);

            }
            sdr.Close();
            connection.Close();
            return status.ToList();

        }



        // ADD DATA TO SRF

        public string AddServiceRequest(string SRF_StnCode, string SRF_CatCode, string SRF_CatDesc, string SRF_Problem, string SRF_User, string status)
        {
            string s_status;
            SqlConnection connection = new SqlConnection(this.conn_string);
            SqlCommand sqlcmd = new SqlCommand();
            String SRF_No;
            DateTime dt = DateTime.Now;
            String yy = dt.Year.ToString();
            String mm = dt.Month.ToString().PadLeft(2, '0');
            int i = 1;
            connection.Open();
            while (true)
            {
                SRF_No = yy + mm + i.ToString().PadLeft(4, '0');
                sqlcmd = new SqlCommand("SELECT srfNo FROM ServiceRequestForm WHERE srfStnCode=@SRF_StnCode AND srfNo=@SRF_No", connection);
                sqlcmd.Parameters.AddWithValue("@SRF_StnCode", SRF_StnCode);
                sqlcmd.Parameters.AddWithValue("@SRF_No", SRF_No);
                SqlDataReader sdr = sqlcmd.ExecuteReader();

                if (sdr.Read() == false)
                {
                    sdr.Close();
                    break;

                }
                sdr.Close();
                sdr.Dispose();
                i++;
            }
            sqlcmd = new SqlCommand("INSERT INTO ServiceRequestForm (srfStnCode,srfNo,srfDate,srfCatCode,srfCatDesc,srfProblem,srfUser,srfSysDate,srfStatus) VALUES(@SRF_StnCode,@SRF_No,getdate(),@SRF_CatCode,@SRF_CatDesc,@SRF_Problem,@SRF_User,SYSDATETIME(),@status)", connection);
            sqlcmd.Parameters.AddWithValue("@status", status);
            sqlcmd.Parameters.AddWithValue("@SRF_StnCode", SRF_StnCode);
            sqlcmd.Parameters.AddWithValue("@SRF_No", SRF_No);
            sqlcmd.Parameters.AddWithValue("@SRF_CatCode", SRF_CatCode);
            sqlcmd.Parameters.AddWithValue("@SRF_CatDesc", SRF_CatDesc);
            sqlcmd.Parameters.AddWithValue("@SRF_Problem", SRF_Problem.ToUpper());
            sqlcmd.Parameters.AddWithValue("@SRF_User", SRF_User);
            int result = sqlcmd.ExecuteNonQuery();
            if (result == 1)
            {
                s_status = "SUCCESSFULLY INSERTED";
            }
            else
            {
                s_status = null;

            }
            connection.Close();
            return s_status;

        }
        // login 
        public List<login_access> check_login(string user, string pass)
        {
            List<login_access> login = new List<login_access>();
            SqlConnection connection = new SqlConnection(this.conn_string);
            SqlCommand sqlcmd = new SqlCommand();
            try
            {
                connection.Open();
                sqlcmd = new SqlCommand("SELECT UserFname,OPS,OPSArea FROM UserNames WHERE UserID=@User AND UserPassword=@Password", connection);
                sqlcmd.Parameters.AddWithValue("@User", user);
                sqlcmd.Parameters.AddWithValue("@Password", pass);
                SqlDataReader sdr = sqlcmd.ExecuteReader();
                sdr.Read();

                login_access acc = new login_access();
                acc.Firstname = sdr["UserFname"].ToString();
                if (sdr["OPSArea"].ToString().Trim().Equals("") || sdr["OPS"].ToString().Trim().Equals("")) // FOR TEST
                {
                    acc.Ops_area = "9999";
                }
                else if (!sdr["OPSArea"].ToString().Trim().Equals("") && sdr["OPS"].ToString().Trim().Equals("Y"))
                {
                    acc.Ops_area = sdr["OPSArea"].ToString().Trim();
                }
                login.Add(acc);
                sdr.Close();
                connection.Close();
                return login;

            }
            catch
            {
                return null;
            }
        }
        // oiclogin
        public List<branch_login> oic_login(string android_id)
        {
            List<branch_login> login = new List<branch_login>();
            SqlConnection connection = new SqlConnection(this.conn_string);
            SqlCommand sqlcmd = new SqlCommand();
            connection.Open();

            try
            {

                sqlcmd = new SqlCommand("IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='OIC_android_id_login' and xtype='U') CREATE TABLE OIC_android_id_login( OIC_firstname varchar(15), OIC_surname varchar(15), OIC_android_id varchar(50), OIC_stn_code varchar(5),OIC_active_status varchar(1),OIC_acc_date_made datetime)" +
                "SELECT * FROM OIC_android_id_login WHERE OIC_android_id=@android_id AND OIC_active_status = 'Y'", connection);
                sqlcmd.Parameters.AddWithValue("@android_id", android_id);
                SqlDataReader sdr = sqlcmd.ExecuteReader();
                sdr.Read();
                branch_login acc = new branch_login();
                acc.User_firstname = sdr["OIC_firstname"].ToString().Trim() + " (Branch OIC)";
                acc.Stn_code = sdr["OIC_stn_code"].ToString().Trim();
                acc.Stn_name = get_stn(sdr["OIC_stn_code"].ToString().Trim()).Trim();
                login.Add(acc);
                sdr.Close();
                connection.Close();
                return login;
            }
            catch
            {
                return null;
            }

        }
        // edit srf reports
        public string edi_stn_srf(string stncode, string srfNo, string srfproblem)
        {
            string status;
            SqlConnection connection = new SqlConnection(this.conn_string);
            connection.Open();
            SqlCommand sqlcmd = new SqlCommand("UPDATE ServiceRequestForm SET srfProblem = @up_problem WHERE srfStnCode = @srfstncode AND srfNo = @srfNo", connection);
            sqlcmd.Parameters.AddWithValue("@up_problem", srfproblem);
            sqlcmd.Parameters.AddWithValue("@srfstncode", stncode);
            sqlcmd.Parameters.AddWithValue("@srfNo", srfNo);
            int result = sqlcmd.ExecuteNonQuery();
            if (result == 1)
            {
                status = "SUCCESSFULLY UPDATED";
            }
            else
            {
                status = "ERROR OCCURED";

            }
            connection.Close();
            return status;
        }
        public string add_oic_device(string firstname, string surname, string android_id, string stn_code)
        {

            SqlConnection connection = new SqlConnection(this.conn_string);
            SqlCommand sqlcmd = new SqlCommand();
            connection.Open();

            sqlcmd = new SqlCommand("SELECT * FROM OIC_android_id_login WHERE OIC_stn_code = @stn AND OIC_active_status='Y' OR OIC_android_id = @an_code", connection);
            sqlcmd.Parameters.AddWithValue("@stn", stn_code);
            sqlcmd.Parameters.AddWithValue("@an_code", android_id);
            SqlDataReader sdr = sqlcmd.ExecuteReader();
            if (sdr.Read() == true)
            {
                sdr.Close();
                return "FALSE";

            }
            else if (sdr.Read() == false)
            {
                sdr.Close();
            }
            sqlcmd = new SqlCommand("INSERT INTO OIC_android_id_login (OIC_firstname,OIC_surname,OIC_android_id,OIC_stn_code,OIC_acc_date_made,OIC_active_status) VALUES(@firstname,@surname,@android_id,@stn_code,SYSDATETIME(),'Y')", connection);
            sqlcmd.Parameters.AddWithValue("@firstname", firstname.ToUpper());
            sqlcmd.Parameters.AddWithValue("@surname", surname.ToUpper());
            sqlcmd.Parameters.AddWithValue("@android_id", android_id);
            sqlcmd.Parameters.AddWithValue("@stn_code", stn_code);
            int result = sqlcmd.ExecuteNonQuery();
            connection.Close();
            if (result == 1)
            {
                return "TRUE";
            }
            else
            {
                return "FALSE";

            }

        }

        public List<cat_method> Getcat_Method()
        {
            List<cat_method> catmet = new List<cat_method>();
            SqlConnection connection = new SqlConnection(this.conn_string);
            connection.Open();
            SqlCommand sqlcmd = new SqlCommand("SELECT * FROM Class", connection);
            sqlcmd.CommandType = System.Data.CommandType.Text;
            SqlDataReader sdr = sqlcmd.ExecuteReader();
            while (sdr.Read())
            {
                cat_method catcat = new cat_method();
                catcat.Catcode = sdr["ccClassCode"].ToString().Trim() + sdr["ccCatCode"].ToString().Trim();
                catcat.Catdesc = sdr["ccDesc"].ToString();
                catmet.Add(catcat);

            }
            sdr.Close();
            connection.Close();
            return catmet.ToList();
        }

        // add file phase
        public string addfile(string stn)
        {
            SqlConnection connection = new SqlConnection(this.conn_string);
            SqlCommand sqlcmd = new SqlCommand();
            String SRF_No;
            DateTime dt = DateTime.Now;
            String yy = dt.Year.ToString();
            String mm = dt.Month.ToString().PadLeft(2, '0');
            int i = 1;
            connection.Open();
            while (true)
            {
                SRF_No = yy + mm + i.ToString().PadLeft(4, '0');
                sqlcmd = new SqlCommand("SELECT srfNo FROM ServiceRequestForm WHERE srfStnCode=@SRF_StnCode AND srfNo=@SRF_No", connection);
                sqlcmd.Parameters.AddWithValue("@SRF_StnCode", stn);
                sqlcmd.Parameters.AddWithValue("@SRF_No", SRF_No);
                SqlDataReader sdr = sqlcmd.ExecuteReader();

                if (sdr.Read() == false)
                {
                    sdr.Close();
                    break;

                }

                sdr.Close();
                sdr.Dispose();
                i++;
            }
            stncode = stn.Trim();
            srfno = SRF_No.Trim();

            return srfno;
        }


        // stream to array converter
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public string FileUpload(Stream fileStream)
        {

            int page = 0;
            SqlConnection connection = new SqlConnection(this.conn_string);
            SqlCommand sqlcmd = new SqlCommand();
            int i = 1;
            connection.Open();
            while (true)
            {
                sqlcmd = new SqlCommand("SELECT srfaPage FROM ServiceRequestImages WHERE srfaStnCode=@SRF_StnCode AND srfaNo=@SRF_No AND srfaPage = @page", connection);
                sqlcmd.Parameters.AddWithValue("@SRF_StnCode", stncode);
                sqlcmd.Parameters.AddWithValue("@SRF_No", srfno);
                sqlcmd.Parameters.AddWithValue("@page", i);
                SqlDataReader sdr = sqlcmd.ExecuteReader();
                if (sdr.Read() == false)
                {
                    sdr.Close();
                    page = i;
                    break;

                }

                sdr.Close();
                sdr.Dispose();
                i++;
            }
            string imagename = stncode + srfno + "-" + page + ".jpg";
            string filename = fileloc + stncode + "\\";
            Directory.CreateDirectory(Path.GetDirectoryName(filename));

            try
            {
                FileStream fileToupload = File.Open(filename + imagename, FileMode.Create);

                byte[] image_array = ReadFully(fileStream);

                fileToupload.Write(image_array, 0, image_array.Length);
                fileToupload.Close();
                fileToupload.Dispose();
                sqlcmd = new SqlCommand("INSERT INTO ServiceRequestImages (srfaStnCode,srfaNo,srfaPage,srfaFileName) VALUES(@stncode,@srfno,@page,@filename)", connection);
                sqlcmd.Parameters.AddWithValue("@stncode", stncode);
                sqlcmd.Parameters.AddWithValue("@srfno", srfno);
                sqlcmd.Parameters.AddWithValue("@page", page);
                sqlcmd.Parameters.AddWithValue("@filename", imagename);
                int result = sqlcmd.ExecuteNonQuery();
                fileToupload.Close();
                fileToupload.Dispose();
                if (result == 1)
                {

                    connection.Close();

                }

                return "confirm";


            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        // delete images
        public string FileDelete(string stn, string imagename)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.conn_string);
                SqlCommand sqlcmd = new SqlCommand();
                connection.Open();
                sqlcmd = new SqlCommand("DELETE FROM ServiceRequestImages WHERE srfaFileName = @imagename AND srfaStnCode = @stn", connection);
                sqlcmd.Parameters.AddWithValue("@stn", stn.Trim());
                sqlcmd.Parameters.AddWithValue("@imagename", imagename.Trim());
                File.Delete(fileloc + stn.Trim() + "\\" + imagename.Trim());
                int result = sqlcmd.ExecuteNonQuery();
                if (result == 1)
                {
                    connection.Close();

                }

                return "Confirm";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        public Stream GetImage(string stn, string imagename)
        {
            FileStream fs = File.OpenRead(fileloc + stn + "\\" + imagename);
            WebOperationContext.Current.OutgoingResponse.ContentType = "image/jpg";
            return fs;
        }
        // RETRIVE IMAGE LIST

        public List<ret_image> imageret(string stn, string srfno)
        {
            List<ret_image> imgfiles = new List<ret_image>();
            SqlConnection connection = new SqlConnection(this.conn_string);
            connection.Open();
            SqlCommand sqlcmd = new SqlCommand("SELECT * FROM ServiceRequestImages WHERE srfaStnCode = @stn AND srfaNo = @srfno ORDER BY srfaPage ASC", connection);
            sqlcmd.CommandType = System.Data.CommandType.Text;
            sqlcmd.Parameters.AddWithValue("@stn", stn);
            sqlcmd.Parameters.AddWithValue("@srfno", srfno);

            SqlDataReader sdr = sqlcmd.ExecuteReader();
            while (sdr.Read())
            {
                ret_image row = new ret_image();
                row.Stn = sdr["srfaStnCode"].ToString();
                row.Srfno = sdr["srfaNo"].ToString();
                row.Page = sdr["srfaPage"].ToString();
                row.Filename = sdr["srfaFileName"].ToString();
                imgfiles.Add(row);
            }
            sdr.Close();
            connection.Close();
            return imgfiles.ToList();
        }

        // action
        public List<action_per_srf> get_action_per_srf(string stn, string srfno)
        {
            List<action_per_srf> actions = new List<action_per_srf>();
            SqlConnection connection = new SqlConnection(this.conn_string);
            connection.Open();
            SqlCommand sqlcmd = new SqlCommand("SELECT SRA.sraStnCode,SRA.sraNo,convert(varchar,SRA.sraDate, 107) AS sraDate,SRA.sraSeqNo,SRA.sraAction,SS.ssDesc,SRA.sraUserID, CASE WHEN E.empFName IS NULL And E.empLName IS NULL THEN 'NO DATA' WHEN E.empFName IS NOT NULL And E.empLName IS NOT NULL THEN E.empFName +' ' + E.empLName END AS techfullname FROM ServiceReportAction SRA LEFT JOIN Employees E ON E.empNo = SRA.sraTechID LEFT JOIN SRFStatus SS ON SRA.sraStatus = SS.ssCode WHERE SRA.sraStnCode = @stn AND SRA.sraNo = @srfno ORDER BY SRA.sraSeqNo DESC", connection);
            sqlcmd.CommandType = System.Data.CommandType.Text;
            sqlcmd.Parameters.AddWithValue("@stn", stn);
            sqlcmd.Parameters.AddWithValue("@srfno", srfno);

            SqlDataReader sdr = sqlcmd.ExecuteReader();
            while (sdr.Read())
            {
                action_per_srf row = new action_per_srf();
                row.Sra_stn = sdr["sraStnCode"].ToString();
                row.Sra_no = sdr["sraNo"].ToString();
                row.Sra_date = sdr["sraDate"].ToString();
                row.Sra_seq = sdr["sraSeqNo"].ToString();
                row.Sra_userID = sdr["sraUserID"].ToString();
                row.Sra_action = sdr["sraAction"].ToString();
                row.Sra_techID = sdr["techfullname"].ToString().Trim();
                row.Sra_Status = sdr["ssDesc"].ToString();
                actions.Add(row);
            }
            sdr.Close();
            connection.Close();
            return actions.ToList();
        }


        public string add_action(string sraStncode, string sraNo, string sraAction, string srauserID, string sraStatus, string sratechID = "")
        {

            string status;
            string seq = (action_status(sraStncode, sraNo) + 1).ToString();
            SqlConnection connection = new SqlConnection(this.conn_string);
            SqlCommand sqlcmd = new SqlCommand();
            connection.Open();
            sqlcmd = new SqlCommand("INSERT INTO ServiceReportAction ( " +
                                "sraStnCode," +
                                "sraNo," +
                                "sraDate," +
                                "sraSeqNo," +
                                "sraAction," +
                                "sraUserID," +
                                "sraSysDate," +
                                "sraTechID," +
                                "sraStatus" +
                                ") VALUES(" +
                                "@stncode," +
                                "@no," +
                                "getdate()," +
                                "@seq," +
                                "@action," +
                                "@sraUser," +
                                "SYSDATETIME()," +
                                "@techID," +
                                "@status)", connection);
            sqlcmd.Parameters.AddWithValue("@techID", sratechID);
            sqlcmd.Parameters.AddWithValue("@stncode", sraStncode);
            sqlcmd.Parameters.AddWithValue("@no", sraNo);
            sqlcmd.Parameters.AddWithValue("@seq", seq);
            sqlcmd.Parameters.AddWithValue("@action", sraAction.ToUpper());
            sqlcmd.Parameters.AddWithValue("@sraUser", srauserID);
            sqlcmd.Parameters.AddWithValue("@status", sraStatus);
            int result = sqlcmd.ExecuteNonQuery();

            Boolean gg = status_updater(sraStncode, sraNo, sraStatus, srauserID);
            if (result == 1)//&& gg == true)
            {
                status = "ACTION SUCCCEFULLY ADDED";

            }
            else
            {
                status = null;

            }
            connection.Close();
            return status;

        }

        //extras sheeeesh
        public string editadd(string stn, string srf_no)
        {
            stncode = stn.Trim();
            srfno = srf_no.Trim();
            return "confirm";
        }

        public string get_stn(String stn_code)
        {
            String status;
            SqlConnection connection = new SqlConnection(this.conn_string);
            SqlCommand sqlcmd = new SqlCommand();
            connection.Open();
            sqlcmd = new SqlCommand("SELECT stnBranch FROM Stations WHERE stnCode=@stn_code", connection);
            sqlcmd.Parameters.AddWithValue("@stn_code", stn_code);
            SqlDataReader sdr = sqlcmd.ExecuteReader();
            sdr.Read();
            status = sdr["stnBranch"].ToString().Trim();
            sdr.Close();
            connection.Close();
            return status;
        }


        public int action_status(string stn, string srf_no)
        {
            SqlConnection connection = new SqlConnection(this.conn_string);
            SqlCommand sqlcmd = new SqlCommand();
            connection.Open();
            sqlcmd = new SqlCommand("SELECT sraSeqNo FROM ServiceReportAction WHERE sraStnCode = @stn AND sraNo = @srfno", connection);
            sqlcmd.Parameters.AddWithValue("@stn", stn);
            sqlcmd.Parameters.AddWithValue("@srfno", srf_no);
            SqlDataReader sdr = sqlcmd.ExecuteReader();
            int number = 0;
            while (sdr.Read())
            {
                number++;
            }
            sdr.Close();
            connection.Close();
            return number;

        }
        // status updater
        public Boolean status_updater(string stn, string srf_no, string status, string user)
        {
            SqlConnection connection = new SqlConnection(this.conn_string);
            SqlCommand sqlcmd = new SqlCommand();
            connection.Open();
            String cmd;
            if (status.Trim().Equals("0001") || status.Trim().Equals("8888"))
            {

                cmd = "UPDATE ServiceRequestForm SET " +
                    "srfStatus = @status, " +
                    "srfUpdateDate = SYSDATETIME(), " +
                    "srfCloseDate = SYSDATETIME()," +
                    "srfUpdateUserID = @user" +
                    " WHERE " +
                    "srfStnCode =@stn " +
                    "AND " +
                    "srfNo = @no";
            }
            else
            {
                cmd = "UPDATE ServiceRequestForm SET " +
                    "srfStatus = @status, " +
                    "srfUpdateDate = SYSDATETIME()," +
                    "srfUpdateUserID = @user " +
                    "WHERE " +
                    "srfStnCode =@stn " +
                    "AND " +
                    "srfNo = @no";
            }
            sqlcmd = new SqlCommand(cmd, connection);
            sqlcmd.Parameters.AddWithValue("@stn", stn);
            sqlcmd.Parameters.AddWithValue("@no", srf_no);
            sqlcmd.Parameters.AddWithValue("@status", status);
            sqlcmd.Parameters.AddWithValue("@user", user);
            int result = sqlcmd.ExecuteNonQuery();
            if (result == 1)
            {
                return true;
            }
            else
            {
                return false;

            }
        }
        public List<tech_id> get_tech(string techcat)
        {
            List<tech_id> emp = new List<tech_id>();
            SqlConnection connection = new SqlConnection(this.conn_string);
            connection.Open();
            SqlCommand sqlcmd = new SqlCommand("SELECT empNo,empFName,empLName FROM Employees WHERE empTechCode = @techcat AND empDateTerminated IS NULL", connection);
            sqlcmd.Parameters.AddWithValue("@techcat", techcat);
            sqlcmd.CommandType = System.Data.CommandType.Text;
            SqlDataReader sdr = sqlcmd.ExecuteReader();
            while (sdr.Read())
            {
                tech_id tech = new tech_id();

                tech.Empcode = sdr["empNo"].ToString().Trim();
                tech.Techname = sdr["empFName"].ToString().Trim() + " " + sdr["empLName"].ToString().Trim();
                emp.Add(tech);

            }
            sdr.Close();
            connection.Close();
            return emp.ToList();
        }
    }

}

