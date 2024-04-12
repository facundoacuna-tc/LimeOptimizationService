using System.ComponentModel.DataAnnotations;

namespace LOS.Common.Entities
{
    public class VersionInfo
    {
        [Key]
        public long Version { get; set; }
        public DateTime AppliedOn { get; set; }
        public string Description { get; set; }
    }
}
