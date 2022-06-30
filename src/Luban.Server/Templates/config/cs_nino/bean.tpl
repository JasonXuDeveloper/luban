using System.Collections.Generic;
{{
    name = x.name
    parent_def_type = x.parent_def_type
    parent = x.parent
    export_fields = x.export_fields
    hierarchy_export_fields = x.hierarchy_export_fields
}}

namespace {{x.namespace_with_top_module}}
{

    /// <summary>
    /// {{name}} excel data
    /// </summary>
    [Nino.Serialization.NinoSerialize]
    public partial class {{name}}ConfigData
    {
        public {{name}}ConfigData()
        {
            
        }
        
        [Nino.Serialization.NinoMember(0)]
        public {{name}}[] Data;
        
        public override string ToString()
        {
            return $"{{name}}ConfigData has {Data.Length} entries: \n"
            + string.Join<{{name}}>(",\n", Data);
        }
    }
    
    
    {{~if x.comment != '' ~}}
    /// <summary>
    /// {{x.escape_comment}}
    /// </summary>
    {{~end~}}
    [Nino.Serialization.NinoSerialize]
    public {{x.cs_class_modifier}} partial class {{name}} {{if parent_def_type}} : {{parent}} {{end}}
    {
        public {{name}}()
        {
        
        }
    
        public {{name}}({{~for field in hierarchy_export_fields }}{{cs_define_type field.ctype}} {{field.name}}{{if !for.last}},{{end}} {{end}}) {{if parent_def_type}} : base({{- for field in parent_def_type.hierarchy_export_fields }}{{field.name}}{{if !for.last}},{{end}}{{end}}) {{end}}
        {
            {{~ for field in export_fields ~}}
            this.{{field.convention_name}} = {{field.name}};
            {{~if field.index_field~}}
            foreach(var _v in {{field.convention_name}}) { {{field.convention_name}}_Index.Add(_v.{{field.index_field.convention_name}}, _v); }
            {{~end~}}
            {{~end~}}
            PostInit();
        }
    
        {{~ index = 0 ~}}
        {{~ for field in export_fields ~}}
    {{~if field.comment != '' ~}}
        /// <summary>
        /// {{field.escape_comment}}
        /// </summary>
    {{~end~}}
        [Nino.Serialization.NinoMember({{index++}})]
        public {{cs_define_type field.ctype}} {{field.convention_name}};
        {{~if field.index_field~}} 
        public readonly Dictionary<{{cs_define_type field.index_field.ctype}}, {{cs_define_type field.ctype.element_type}}> {{field.convention_name}}_Index = new Dictionary<{{cs_define_type field.index_field.ctype}}, {{cs_define_type field.ctype.element_type}}>();
        {{~end~}}
        {{~if field.gen_ref~}}
        public {{field.cs_ref_validator_define}}
        {{~end~}}
        {{~if (gen_datetime_mills field.ctype) ~}}
        public long {{field.convention_name}}_Millis => {{field.convention_name}} * 1000L;
        {{~end~}}
        {{~if field.gen_text_key~}}
        public {{cs_define_text_key_field field}} { get; }
        {{~end~}}
        {{~end~}}
    
    {{~if !x.is_abstract_type~}}
        public const int __ID__ = {{x.id}};
        public int GetTypeId() => __ID__;
    {{~end~}}
    
        public {{x.cs_method_modifier}} void TranslateText(System.Func<string, string, string> translator)
        {
            {{~if parent_def_type~}}
            base.TranslateText(translator);
            {{~end~}}
            {{~ for field in export_fields ~}}
            {{~if field.gen_text_key~}}
            {{cs_translate_text field 'translator'}}
            {{~else if field.has_recursive_text~}}
            {{cs_recursive_translate_text field 'translator'}}
            {{~end~}}
            {{~end~}}
        }
    
        public override string ToString()
        {
            return "{{full_name}}{ "
        {{~ for field in hierarchy_export_fields ~}}
            + "{{field.convention_name}}: " + this.{{field.convention_name}}.ToString() + ","
        {{~end~}}
            + "}";
        }
        
        partial void PostInit();
        partial void PostResolve();
    }
}
