using DbCommands.DbAttributes;
using System.Data;

namespace DbCommands
{
    [DbTable("dbo", "PerformanceCheck")]
    public class PerformanceCheckData
    {
        [DbColumn("Id", DbType.Guid, 0, IsPrimaryKey = true)]
        public Guid Id { get; set; }

        [DbColumn("Code", DbType.Int32, 1)]
        public int Code { get; set; }

        [DbColumn("CreatedDate", DbType.DateTime, 2)]
        public DateTime CreatedDate { get; set; }

        [DbColumn("CreatedBy", DbType.String, 3)]
        public string CreatedBy { get; set; }

        [DbColumn("UpdatedDate", DbType.DateTime, 4)]
        public DateTime? UpdatedDate { get; set; }

        [DbColumn("UpdatedBy", DbType.String, 5)]
        public string? UpdatedBy { get; set; }

        public PerformanceCheckData(Guid id, int code)
        {
            Id = id;
            Code = code;
            CreatedDate = DateTime.Now;
            CreatedBy = "System";
        }
    }
}
