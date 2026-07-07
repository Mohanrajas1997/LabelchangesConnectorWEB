using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlexicodeConnectors.Models
{
    public class Connection
    {
        public string connection_gid { get; set; }
        public string connection_code { get; set; }
        public string connection_name { get; set; }
        public string connection_desc { get; set; }
        public string source_db_type { get; set; }
        public string source_host_name { get; set; }
        public string source_port { get; set; }
        public string source_auth_mode { get; set; }
        public string source_db_user { get; set; }
        public string source_db_pwd { get; set; }
        public string source_auth_file_name { get; set; }
        public byte[] source_auth_file_blob { get; set; }
        public string having_auth_url { get; set; }
        public string source_file { get; set; }
        public string ssh_tunneling { get; set; }
        public string ssh_host_name { get; set; }
        public string ssh_port { get; set; }
        public string ssh_user { get; set; }
        public string ssh_pwd { get; set; }
        public string ssh_auth_mode { get; set; }
        public string ssh_file_name { get; set; }
        public byte[] ssh_file_blob { get; set; }
        public string connection_status { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public DateTime updated_date { get; set; }
        public string updated_by { get; set; }

        public int SelectedMasterId { get; set; } = 0;
        public List<SelectListItem> masterDatas { get; set; }
        public DataProcessing dataProcessing { get; set; }
        public DataProcessingHeader dataProcessingheader { get; set; }
    }
    public class ConnectionDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    #region DataProcessing changes done by Muthu
    public class Masters
    {
        public Int64 master_gid { get; set; }
        public string master_code { get; set; }
        public string master_name { get; set; }
        public Int64 parent_gid { get; set; }
        public Int64 depend_gid { get; set; }
        public string parent_code { get; set; }
        public string depend_code { get; set; }
    }
    public class FieldMapping
    {
        public Int64 pplfieldmapping_gid { get; set; }
        public string pipeline_code { get; set; }
        public string ppl_field_name { get; set; }
        public string dataset_field_name { get; set; }
        public string default_value { get; set; }
    }

    public class DataProcessing
    {
        public Int64 dataprocessing_gid { get; set; } = 0;
        public Int64 dataprocessing_header_gid { get; set; } = 0;
        public Int64 dataprocessing_child_master_gid { get; set; } = 0;
        public Int64 dataprocessing_parent_master_gid { get; set; } = 0;
        public Int64 dataprocessing_master_gid { get; set; } = 0;
        public int dataprocessing_pipeline_gid { get; set; } = 0;
        public int dataprocessing_pplfieldmapping_gid { get; set; } = 0;
        public int dataprocessing_orderby { get; set; } = 0;
        public string dataprocessing_param1 { get; set; } = null;
        public string dataprocessing_param2 { get; set; } = null;
        public string dataprocessing_param3 { get; set; } = null;
        public string dataprocessing_child_master_code { get; set; } = null;
        public string dataprocessing_parent_master_code { get; set; } = null;
        public string dataprocessing_master_code { get; set; } = null;
        public string dataprocessing_child_master_name { get; set; } = null;
        public string dataprocessing_parent_master_name { get; set; } = null;
        public string dataprocessing_master_name { get; set; } = null;
        public string dataprocessing_ppl_field_name { get; set; } = null;
        public string dataprocessingheader_dataset_code { get; set; } = null;
        public char delete_flag { get; set; } = 'N';
        public decimal field_Sequenceno { get; set; } = 1;
        public decimal field_gid { get; set; } = 0;
    }

    public class DataProcessingHeader
    {
        public Int64 dataprocessingheader_gid { get; set; }
        public string dataprocessingheader_pipeline_code { get; set; }
        public string dataprocessingheader_dataset_code { get; set; }
        public int dataprocessingheader_seqno { get; set; }
        public string dataprocessingheader_ppl_field_name { get; set; }
        public Int64 dataprocessingheader_pplfieldmapping_gid { get; set; }
        public string delete_flag { get; set; }

    }
    #endregion
    public class DatabaseInfo
    {
        public string Name { get; set; }
    }
    public class SrcExpression
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class TableAndView
    {
        public string Name { get; set; }
    }
    public class TargetTable
    {
        public string dataset_code { get; set; }
        public string dataset_name { get; set; }
    }
    public class TableFields
    {
        public string source_field_name { get; set; }
        public string target_field_name { get; set; }
    }

    public class apiAuthToken
    {
        public string apiauthtoken_gid { get; set; }
        public string connection_code { get; set; }
        public string auth_token_keyname { get; set; }
        public string auth_url { get; set; }
        public string auth_method { get; set; }
        public string auth_token { get; set; }
        public string auth_header_json { get; set; }
        public string auth_payload_type { get; set; }
        public string auth_body_format { get; set; }
        public string auth_payload_json { get; set; }
        public string auth_response { get; set; }
        public string remarks { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public DateTime updated_date { get; set; }
        public string updated_by { get; set; }
    }

}
