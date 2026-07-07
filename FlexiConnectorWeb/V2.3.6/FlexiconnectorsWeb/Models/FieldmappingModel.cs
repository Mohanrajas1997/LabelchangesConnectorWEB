using System;

namespace FlexicodeConnectors.Models
{
    public class FieldmappingModel
    {
            public string pplfieldmapping_gid { get; set; }
            public string pipeline_code { get; set; }
            public string ppl_field_name { get; set; }
            public string dataset_field_name { get; set; }
            public string default_value { get; set; }
            public DateTime created_date { get; set; }
            public string created_by { get; set; }
            public DateTime updated_date { get; set; }
            public string updated_by { get; set; }
            public string delete_flag { get; set; }
    }
}
