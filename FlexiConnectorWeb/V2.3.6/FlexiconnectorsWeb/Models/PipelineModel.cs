using Microsoft.AspNetCore.Http;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace FlexicodeConnectors.Models
{
    public class PipelineModel
    {
        public string pipeline_gid { get; set; }
        public string pipeline_code { get; set; }
        public string pipeline_name { get; set; }
        public string source_db_type { get; set; }
        public string connector_name { get; set; }
        public string pipeline_desc { get; set; }
        public string connection_code { get; set; }
        public string connection_name { get; set; }
        public string db_name { get; set; }
        public string file_path { get; set; }
        public string source_file_name { get; set; }
        public string sheet_name { get; set; }
        public string table_view_query_type { get; set; }
        public string table_name { get; set; }
        public string table_view_query_desc { get; set; }
        public string target_dataset_code { get; set; }
        public string dataset_name { get; set; }
        public string pplFieldNames { get; set; }
        public string default_value { get; set; }
        public string run_type { get; set; }
        public string cron_expression { get; set; }
        public string upload_mode { get; set; }
        public string key_field { get; set; }
        public string updated_time_stamp { get; set; }
        public string pull_days { get; set; }
        public string pipeline_status { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public DateTime updated_date { get; set; }
        public string updated_by { get; set; }
        public string dataset_count { get; set; }
        public Boolean haveActiveDataset { get; set; }
        public string result_name { get; set; }
    }
    public class SourcetblFields
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
    public class TargettblKeyFields
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
    public class TargettblFields
    {
        public string dataset_field_name { get; set; }
        public string dataset_field_desc { get; set; }
    }
    public class pplFieldMapping
    {
        public string pplfieldmapping_gid { get; set; }
        public string pipeline_code { get; set; }
        public int pplfieldmapping_flag { get; set; }
        public string dataset_code { get; set; }
        public string ppl_field_name { get; set; }
        public string dataset_field_name { get; set; }
        public string dataset_field_desc { get; set; }
        public string default_value { get; set; }
        public string field_mandatory { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public DateTime updated_date { get; set; }
        public string updated_by { get; set; }
        public string delete_flag { get; set; }
    }
    public class pplFieldMappinglist
    {
        public List<pplFieldMapping> fieldmaplist { get; set; }
    }
    public class pplSourceExpression
    {
        public string pplsourcefield_gid { get; set; }
        public string pipeline_code { get; set; }
        public string dataset_code { get; set; }
        public string sourcefield_name { get; set; }
        public string sourcefield_format { get; set; }
        public string sourcefield_datatype { get; set; }
        public string sourcefield_expression { get; set; }
        public string source_type { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public DateTime updated_date { get; set; }
        public string updated_by { get; set; }
        public string delete_flag { get; set; }
    }

    public class pplSourceCsv
    {
        public string pplsourcefield_gid { get; set; }
        public string pipeline_code { get; set; }
        public string dataset_code { get; set; }
        public string sourcefield_name { get; set; }
        public int sourcefield_sno { get; set; }
        public string sourcefield_datatype { get; set; }
        public string source_type { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public DateTime updated_date { get; set; }
        public string updated_by { get; set; }
        public string delete_flag { get; set; }
    }
    public class pplCondition
    {
        public string pplcondition_gid { get; set; }
        public string dataset_code { get; set; }
        public string pipeline_code { get; set; }
        public string condition_type { get; set; }
        public string condition_name { get; set; }
        public string condition_text { get; set; }
        public string condition_msg { get; set; }
        public string sys_flag { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public DateTime updated_date { get; set; }
        public string updated_by { get; set; }
        public string delete_flag { get; set; }
    }
    public class pplFinalization
    {
        public string finalization_gid { get; set; }
        public string pipeline_code { get; set; }
        public string dataset_code { get; set; }
        public string run_type { get; set; }
        public string run_trigger { get; set; }
        public string cron_expression { get; set; }
        public string extract_mode { get; set; }
        public string upload_mode { get; set; }
        public string key_field { get; set; }
        public string extract_condition { get; set; }
        public string last_incremental_val { get; set; }
        public string parent_dataset_code { get; set; }
        public int pull_days { get; set; } = 0;
        public string reject_duplicate_flag { get; set; }
        public string error_mode { get; set; }
        public string duplicate_mode { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public DateTime updated_date { get; set; }
        public string updated_by { get; set; }
        public string delete_flag { get; set; }
        public string pipeline_status { get; set; }
        public string scheduler_file_path { get; set; }
    }
    public class SourceToTargetPushdata
    {
        public string connection_code { get; set; }
        public string databasename { get; set; }
        public string sourcetable { get; set; }
        public string source_field_columns { get; set; }
        public string targettable { get; set; }
        public string defaultvalue { get; set; }
        public string upload_mode { get; set; }
        public string primary_key { get; set; }
        public string updated_time_stamp { get; set; }
        public string pull_days { get; set; }

    }
    public class pipelineclone
    {
        public string in_pipeline_name { get; set; }
        public string in_pipeline_code { get; set; }
        public string in_dataset_code { get; set; }
        public string out_srcfile_name { get; set; }
        public string out_dstfile_name { get; set; }
        public string out_msg { get; set; }
        public int out_result { get; set; }
    }
    public class ErrorLog
    {
        public string in_errorlog_pipeline_code { get; set; }
        public int in_errorlog_scheduler_gid { get; set; }
        public string in_errorlog_type { get; set; }
        public string in_errorlog_exception { get; set; }
        public string in_created_by { get; set; }
    }
    public class PipelinecloneResult
    {
        public string SrcFileName { get; set; }
        public string DstFileName { get; set; }
        public string Message { get; set; }
        public int Result { get; set; }
    }
    public class IncrementalRecord
    {
        public int serialNumber { get; set; }
        public int incremental_gid { get; set; }
        public string pipeline_code { get; set; }
        public string incremental_field { get; set; }
        public string incremental_value { get; set; }
        public string delete_flag { get; set; }
    }
    public class saveIncrementalRedordModel
    {
        public string jsondata { get; set; }
        public string pipeline_code { get; set; }
    }
    public class pplCsvHeader
    {
        public string pplcsvheader_gid { get; set; }
        public string pipeline_code { get; set; }
        public string column_separator { get; set; }
        public int number_ofcolumns { get; set; }
        public int number_oflines_toskip { get; set; }
        public string csvfile_dateformat { get; set; }
        public string csvfile_datetimeformat { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public DateTime updated_date { get; set; }
        public string updated_by { get; set; }
        public string delete_flag { get; set; }
    }

    public class pplApiHeader
    {
        public string api_gid { get; set; }
        public string pipeline_code { get; set; }
        public string api_url { get; set; }
        public string api_method { get; set; }
        public string json_response { get; set; }
        public string api_payload { get; set; }
        public string api_payload_type { get; set; }
        public string api_header { get; set; }
        public string have_auth_url { get; set; }
        public string auth_token_keyname { get; set; }
        public string auth_url { get; set; }
        public string auth_user_name { get; set; }
        public string auth_user_pswd { get; set; }
        public string auth_token { get; set; }
        public string auth_type { get; set; }
        public string remarks { get; set; }
        public string inclusion_filter_cond { get; set; }
        public string rejection_filter_cond { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public DateTime updated_date { get; set; }
        public string updated_by { get; set; }
        public string delete_flag { get; set; }
    }

    public class pplSourceApi
    {
        public string pplsourcefield_gid { get; set; }
        public string pipeline_code { get; set; }
        public string sourcefield_name { get; set; }
        public int sourcefield_sno { get; set; }
        public string sourcefield_datatype { get; set; }
        public string source_type { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public DateTime updated_date { get; set; }
        public string updated_by { get; set; }
        public string delete_flag { get; set; }
    }


    public class ppldatasetHeader
    {
        public string dataset_gid { get; set; }
        public string dataset_code { get; set; }
        public string dataset_name { get; set; }
        public string dataset_category { get; set; }
        public string system_flag { get; set; }
        public string active_status { get; set; }
        public string inactive_reason { get; set; }
        public DateTime insert_date { get; set; }
        public string insert_by { get; set; }
        public DateTime updated_date { get; set; }
        public string updated_by { get; set; }
        public string delete_flag { get; set; }
    }

    public class DatasetModel
    {
        public string pipeline_code { get; set; }
        public string dataset_code { get; set; }
        public string dataset_name { get; set; }
        public string active_status { get; set; }
        public string pipelinedet_gid { get; set; }
        public int dataset_gid { get; set; }
        public string Status { get; set; }
        public string dataset_category { get; set; }
        public string scheduler_path { get; set; }

    }

    public class DatasetfieldModel
    {
        public string dataset_gid { get; set; }
        public string dataset_code { get; set; }
        public string datasetfield_gid { get; set; }
        public string dataset_code1 { get; set; }
        public string dataset_seqno { get; set; }
        public string field_name { get; set; }
        public string field_type { get; set; }
        public string field_length { get; set; }
        public string precision_length { get; set; }
        public string scale_length { get; set; }
        public string field_mandatory { get; set; }
        public string dataset_field_sno { get; set; }
        public string dataset_table_field { get; set; }
        public string Status { get; set; }
        public string pipeline_code { get; set; }
        public string pplfieldmapping_gid { get; set; }
        public string pplfieldmapping_flag { get; set; }
        public string ppl_field_name { get; set; }
        public string dataset_field_name { get; set; }
        public string default_value { get; set; }
        public string pplsourcefield_format { get; set; }
        public string pplsourcefield_gid { get; set; }
        public string parent_pipeline_code { get; set; }
        public string parent_pipeline_name { get; set; }
        public string has_saved { get; set; }
        public string system_flag { get; set; }
        public string mapped_ppl_field_name { get; set; }
        public string field_mapped { get; set; }
        public char its_parent_pipeline { get; set; }
        public string source_type { get; set; }
        public string pipelinedet_dataset_type { get; set; }
        public string previous_mapped_field_name { get; set; }
        public string datasetdetail_id { get; set; }
    }

    public class datasetHeader
    {
        public string dataset_systemflag { get; set; }
        public string pipeline_code { get; set; }
        public Int32 pipelinedet_id { get; set; }
        public string? dataset_name { get; set; }
        public string? datasetCode { get; set; }
        public Int32 dataset_id { get; set; }
        public string? dataset_category { get; set; }
        public string? clone_dataset { get; set; }
        public string? active_status { get; set; }
        public string? inactive_reason { get; set; }
        public string pipelinedet_dataset_type { get; set; }
        public string? in_action { get; set; }
        public string? in_action_by { get; set; }
        public string? out_msg { get; set; }
        public string? out_result { get; set; }
    }

    public class sourcefieldModel
    {
        public Int32 pipeline_gid { get; set; }
        public string pipeline_code { get; set; }
        public Int32 pplsourcefield_gid { get; set; }
        public string sourcefield_name { get; set; }
        public string source_type { get; set; }
    }

    public class datasetDetail
    {
        public string? datasetCode { get; set; }
        public int datasetdetail_id { get; set; }
        public string? field_name { get; set; }
        public string? field_type { get; set; }
        public string? field_length { get; set; }
        public int? precision_length { get; set; }
        public int? scale_length { get; set; }
        public string? field_mandatory { get; set; }
        public decimal? dataset_seqno { get; set; }
        public string? in_action { get; set; }
        public string? in_action_by { get; set; }


    }

    public class CloneDatasetModel
    {
        public string in_pipeline_code { get; set; }
        public string in_clone_dataset_code { get; set; }
        public string in_new_dataset_code { get; set; }
        public string in_user_code { get; set; }
        public string in_dataset_type { get; set; }
    }

    //---------------------Pandiaraj support doc add 25-08-2025-------------------------//
    public class SupportingDoc
    {
        public List<IFormFile> supportfileuser { get; set; }
        public int supportingdoc_gid { get; set; }
        public string pipeline_code { get; set; }
        public string supportingdoc_name { get; set; }
        public string supportingdoc_extension { get; set; }
        public string supportingdoc_remarks { get; set; }
        public string supportingdoc_size { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public DateTime update_date { get; set; }
        public string updated_by { get; set; }
        public string delete_flag { get; set; }
        public string action { get; set; }
        public string out_msg { get; set; }
        public int sl_no { get; set; }

    }

    //---------------------Pandiaraj support doc add 25-08-2025-------------------------//

    public class JsonInputModel
    {
        public string JsonData { get; set; }
    }

    public class ValidationResult1
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
    }

    public class ProxyRequest
    {
        public string Url { get; set; }
        public string Method { get; set; } = "GET";
        // bind body as JsonElement so we can accept object/string/primitive from client
        public JsonElement Body { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        // helper to know if body was provided
        public bool HasBody => Body.ValueKind != JsonValueKind.Undefined;
    }

    public class apinodeModel
    {
        public string node { get; set; }
        public int level { get; set; }
        public string parent_node { get; set; }
        public string child_node { get; set; }
        public string sibling { get; set; }
    }

    public class apifilterCondition
    {
        public string JsonData { get; set; }
        public string Query { get; set; }
    }

    public class getAllDatasetFieldsModel
    {
        public string in_pipeline_code { get; set; }
        public string in_dataset_code { get; set; }
    }

    public class parentchildRelation
    {
        public string parentchild_rel_gid { get; set; }
        public string pipeline_code { get; set; }
        public string seq_no { get; set; }
        public string parent_ds_code { get; set; }
        public string parent_dskeyfield { get; set; }
        public string child_ds_code { get; set; }
        public string child_dskeyfield { get; set; }
        public string remarks { get; set; }
        public string child_ds_name { get; set; }
        public string parent_field_name { get; set; }
        public string child_field_name { get; set; }

        //public DateTime created_date { get; set; }
        //public string created_by { get; set; }
        //public DateTime update_date { get; set; }
        //public string updated_by { get; set; }
        // public string delete_flag { get; set; }
    }
}
