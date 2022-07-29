using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using Nino.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.DataExporters
{
    class NinoExportor
    {
        public static NinoExportor Ins { get; } = new();
        
        public void WriteList(DefTable table, List<Record> records, Writer writer)
        {
            writer.CompressAndWrite(records.Count);
            foreach (var record in records)
            {
                Accept(record.Data, ref writer);
            }
        }

        public void Apply(DType type, ref Writer writer)
        {
            switch (type)
            {
                case DInt x: Accept(x, ref writer); break;
                case DString x: Accept(x, ref writer); break;
                case DFloat x: Accept(x, ref writer); break;
                case DBean x: Accept(x, ref writer); break;
                case DBool x: Accept(x, ref writer); break;
                case DEnum x: Accept(x, ref writer); break;
                case DList x: Accept(x, ref writer); break;
                case DArray x: Accept(x, ref writer); break;
                case DLong x: Accept(x, ref writer); break;
                case DMap x: Accept(x, ref writer); break;
                case DByte x: Accept(x, ref writer); break;
                case DDouble x: Accept(x, ref writer); break;
                case DFint x: Accept(x, ref writer); break;
                case DFlong x: Accept(x, ref writer); break;
                case DFshort x: Accept(x, ref writer); break;
                case DShort x: Accept(x, ref writer); break;
                case DDateTime x: Accept(x, ref writer); break;
                default: throw new NotSupportedException($"DType:{type.GetType().FullName} not support");
            }
        }

        public void Accept(DBool type, ref Writer writer)
        {
            writer.Write(type.Value);
        }

        public void Accept(DByte type, ref Writer writer)
        {
            writer.Write(type.Value);
        }

        public void Accept(DShort type, ref Writer writer)
        {
            writer.Write(type.Value);
        }

        public void Accept(DFshort type, ref Writer writer)
        {
            writer.Write(type.Value);
        }

        public void Accept(DInt type, ref Writer writer)
        {
            writer.CompressAndWrite(type.Value);
        }

        public void Accept(DFint type, ref Writer writer)
        {
            writer.CompressAndWrite(type.Value);
        }

        public void Accept(DLong type, ref Writer writer)
        {
            writer.CompressAndWrite(type.Value);
        }

        public void Accept(DFlong type, ref Writer writer)
        {
            writer.CompressAndWrite(type.Value);
        }

        public void Accept(DFloat type, ref Writer writer)
        {
            writer.Write(type.Value);
        }

        public void Accept(DDouble type, ref Writer writer)
        {
            writer.Write(type.Value);
        }

        public void Accept(DEnum type, ref Writer writer)
        {
            writer.CompressAndWriteEnum(typeof(int), (ulong)type.Value);
        }

        public void Accept(DString type, ref Writer writer)
        {
            writer.Write(type.Value);
        }

        public void Accept(DBytes type, ref Writer writer)
        {
            writer.Write(type.Value);
        }
        
        public void Accept(DDateTime type, ref Writer writer)
        {
            writer.Write(type.Time);
        }
        
        public void Accept(DBean type, ref Writer writer)
        {
            var defFields = type.ImplType.HierarchyFields;
            int index = 0;
            foreach (var field in type.Fields)
            {
                var defField = (DefField)defFields[index++];
                if (!defField.NeedExport)
                {
                    continue;
                }
                if (defField.CType.IsNullable)
                {
                    if (field != null)
                    {
                        Apply(field, ref writer);
                    }
                    else
                    {
                        throw new InvalidOperationException("Nino does not support writing null");
                    }
                }
                else
                {
                    Apply(field, ref writer);
                }
            }
        }

        public void Accept(DArray type, ref Writer writer)
        {
            writer.CompressAndWrite(type.Datas.Count);
            foreach (var d in type.Datas)
            {
                Apply(d, ref writer);
            }
        }

        public void Accept(DList type, ref Writer writer)
        {
            writer.CompressAndWrite(type.Datas.Count);
            foreach (var d in type.Datas)
            {
                Apply(d, ref writer);
            }
        }

        public void Accept(DMap type, ref Writer writer)
        {
            writer.CompressAndWrite(type.Datas.Count);
            foreach (var d in type.Datas)
            {
                Apply(d.Key, ref writer);
                Apply(d.Value, ref writer);
            }
        }
    }
}
