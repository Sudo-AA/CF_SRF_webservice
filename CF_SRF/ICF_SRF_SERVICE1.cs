using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace CF_SRF
{
    [ServiceContract]
    public interface ICF_SRF_SERVICE1
    {

        [OperationContract]
        [WebGet(UriTemplate = "getstation_method/{areacode}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        List<station_method> GetStation_Method(String areacode);

        [OperationContract]
        [WebGet(UriTemplate = "cat_method", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        List<cat_method> Getcat_Method();

        [OperationContract]
        [WebGet(UriTemplate = "status", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        List<status_class> Getstatus_class();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "addservicerequest/{SRF_StnCode}/{SRF_CatCode}/{SRF_CatDesc}/{SRF_Problem}/{SRF_User}/{status}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string AddServiceRequest(string SRF_StnCode, string SRF_CatCode, string SRF_CatDesc, string SRF_Problem, string SRF_User, string status);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "check_login/{user}/{pass}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        List<login_access> check_login(string user, string pass);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "retrivestnsrf/{stncode}/{status}/{cat_holder}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        List<srf_get_data> retr_stn_srf(string stncode, string status, string cat_holder);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "editstnsrf/{stncode}/{srfNo}/{srfproblem}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string edi_stn_srf(string stncode, string srfNo, string srfproblem);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "addfile/{stn}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string addfile(string stn);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "FileUpload")]
        string FileUpload(Stream fileStream);


        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "FileDelete/{stn}/{imagename}")]
        string FileDelete(string stn, string imagename);

        [OperationContract]
        [WebGet(UriTemplate = "GetImage/{stn}/{imagename}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Stream GetImage(string stn, string imagename);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "imageret/{stn}/{srfno}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        List<ret_image> imageret(string stn, string srfno);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "editadd/{stn}/{srf_no}")]
        string editadd(string stn, string srf_no);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "oic_login/{android_id}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        List<branch_login> oic_login(string android_id);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "add_oic_device/{firstname}/{surname}/{android_id}/{stn_code}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string add_oic_device(string firstname, string surname, string android_id, string stn_code);

        [OperationContract]
        [WebGet(UriTemplate = "get_action_per_srf/{stn}/{srfno}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        List<action_per_srf> get_action_per_srf(string stn, string srfno);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "add_action/{sraStncode}/{sraNo}/{sraAction}/{srauserID}/{sratechID}/{sraStatus}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string add_action(string sraStncode, string sraNo, string sraAction, string srauserID, string sraStatus, string sratechID = " ");

        [OperationContract]
        [WebGet(UriTemplate = "get_tech/{techcat}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        List<tech_id> get_tech(string techcat);
    }

    [DataContract]
    public class station_method
    {
        String srf_stnCode;
        String srf_station_name;

        [DataMember]
        public String SRF_stnCode
        {
            get { return srf_stnCode; }
            set { srf_stnCode = value; }
        }
        [DataMember]
        public String SRF_station_name
        {
            get { return srf_station_name; }
            set { srf_station_name = value; }
        }
    }

    [DataContract]
    public class cat_method
    {
        String catcode;
        String catdesc;

        [DataMember]
        public String Catcode
        {
            get { return catcode; }
            set { catcode = value; }
        }
        [DataMember]
        public String Catdesc
        {
            get { return catdesc; }
            set { catdesc = value; }
        }
    }

    [DataContract]
    public class srfdata
    {
        String srf_StnCode;
        String srf_CatCode;
        String srf_CatDesc;
        String srf_Problem;
        String srf_User;



        [DataMember]
        public String SRF_StnCode
        {
            get { return srf_StnCode; }
            set { srf_StnCode = value; }
        }
        [DataMember]
        public String SRF_CatCode
        {
            get { return srf_CatCode; }
            set { srf_CatCode = value; }
        }
        [DataMember]
        public String SRF_CatDesc
        {
            get { return srf_CatDesc; }
            set { srf_CatDesc = value; }
        }
        [DataMember]
        public String SRF_Problem
        {
            get { return srf_Problem; }
            set { srf_Problem = value; }
        }
        [DataMember]
        public String SRF_User
        {
            get { return srf_User; }
            set { srf_User = value; }
        }


    }

    [DataContract]
    public class srf_get_data
    {
        String srf_StnCode;
        String srf_No;
        String srf_Date;
        String srf_CatCode;
        String srf_CatDesc;
        String srf_Problem;
        String srf_User;
        String srf_Status;
        String srf_Stn;
        String images_status;
        String updated_by;
        String update_date;
        String action;
        String closed_date;


        [DataMember]
        public String SRF_Stn
        {
            get { return srf_Stn; }
            set { srf_Stn = value; }
        }
        [DataMember]
        public String SRF_StnCode
        {
            get { return srf_StnCode; }
            set { srf_StnCode = value; }
        }
        [DataMember]
        public String SRF_No
        {
            get { return srf_No; }
            set { srf_No = value; }
        }
        [DataMember]
        public String SRF_Date
        {
            get { return srf_Date; }
            set { srf_Date = value; }
        }
        [DataMember]
        public String SRF_CatCode
        {
            get { return srf_CatCode; }
            set { srf_CatCode = value; }
        }
        [DataMember]
        public String SRF_CatDesc
        {
            get { return srf_CatDesc; }
            set { srf_CatDesc = value; }
        }
        [DataMember]
        public String SRF_Problem
        {
            get { return srf_Problem; }
            set { srf_Problem = value; }
        }
        [DataMember]
        public String SRF_User
        {
            get { return srf_User; }
            set { srf_User = value; }
        }
        [DataMember]
        public String SRF_Status
        {
            get { return srf_Status; }
            set { srf_Status = value; }
        }
        [DataMember]
        public String Images_status
        {
            get { return images_status; }
            set { images_status = value; }
        }

        [DataMember]
        public String Updated_by
        {
            get { return updated_by; }
            set { updated_by = value; }
        }
        [DataMember]
        public String Updated_date
        {
            get { return update_date; }
            set { update_date = value; }
        }
        [DataMember]
        public String Action
        {
            get { return action; }
            set { action = value; }
        }
        [DataMember]
        public String Closed_date
        {
            get { return closed_date; }
            set { closed_date = value; }
        }
    }


    [DataContract]
    public class ret_image
    {
        String stn;
        String srfno;
        String page;
        String filename;


        [DataMember]
        public String Stn
        {
            get { return stn; }
            set { stn = value; }
        }
        [DataMember]
        public String Srfno
        {
            get { return srfno; }
            set { srfno = value; }
        }
        [DataMember]
        public String Page
        {
            get { return page; }
            set { page = value; }
        }
        [DataMember]
        public String Filename
        {
            get { return filename; }
            set { filename = value; }
        }
    }

    [DataContract]
    public class branch_login
    {
        String stn_code;
        String stn_name;
        String user_firstname;



        [DataMember]
        public String Stn_code
        {
            get { return stn_code; }
            set { stn_code = value; }
        }
        [DataMember]
        public String Stn_name
        {
            get { return stn_name; }
            set { stn_name = value; }
        }
        [DataMember]
        public String User_firstname
        {
            get { return user_firstname; }
            set { user_firstname = value; }
        }
    }

    [DataContract]
    public class login_access
    {
        String firstname;
        String ops_area;


        [DataMember]
        public String Firstname
        {
            get { return firstname; }
            set { firstname = value; }
        }
        [DataMember]
        public String Ops_area
        {
            get { return ops_area; }
            set { ops_area = value; }
        }

    }

    [DataContract]
    public class status_class
    {
        String status_desc;
        String status_code;


        [DataMember]
        public String Status_desc
        {
            get { return status_desc; }
            set { status_desc = value; }
        }
        [DataMember]
        public String Status_code
        {
            get { return status_code; }
            set { status_code = value; }
        }

    }

    [DataContract]
    public class action_per_srf
    {
        String sra_stn;
        String sra_no;
        String sra_seq;
        String sra_action;
        String sra_userID;
        String sra_SysDate;
        String sra_techID;
        String sra_Status;

        [DataMember]
        public String Sra_stn
        {
            get { return sra_stn; }
            set { sra_stn = value; }
        }

        [DataMember]
        public String Sra_no
        {
            get { return sra_no; }
            set { sra_no = value; }
        }
        [DataMember]
        public String Sra_date
        {
            get { return sra_SysDate; }
            set { sra_SysDate = value; }
        }
        [DataMember]
        public String Sra_seq
        {
            get { return sra_seq; }
            set { sra_seq = value; }
        }

        [DataMember]
        public String Sra_action
        {
            get { return sra_action; }
            set { sra_action = value; }
        }

        [DataMember]
        public String Sra_userID
        {
            get { return sra_userID; }
            set { sra_userID = value; }
        }

        [DataMember]
        public String Sra_techID
        {
            get { return sra_techID; }
            set { sra_techID = value; }
        }
        [DataMember]
        public String Sra_Status
        {
            get { return sra_Status; }
            set { sra_Status = value; }
        }
    }

    [DataContract]
    public class tech_id
    {
        String empcode;
        String techname;


        [DataMember]
        public String Empcode
        {
            get { return empcode; }
            set { empcode = value; }
        }
        [DataMember]
        public String Techname
        {
            get { return techname; }
            set { techname = value; }
        }

    }
}
