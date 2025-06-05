using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NuGet.Packaging.Signing;
using Swashbuckle.AspNetCore.Annotations;

namespace TodoApi.Models
{
    [SwaggerSchema("待辦事項模型", Description = "待辦事項的model，包含ID、名稱和完成狀態。")]
    public class TodoItem
    {
        [Key]
        [SwaggerSchema("待辦事項ID", ReadOnly = false)]
        public int Id { get; set; }

        [SwaggerSchema("待辦事項名稱", ReadOnly = false, Nullable = true)]
        public string? Title { get; set; }

        [SwaggerSchema("待辦事項是否完成", ReadOnly = false)]
        public bool IsDone { get; set; }

        [SwaggerSchema("待辦事項內容")]
        public string? Comment { get; set; }

        [SwaggerSchema("新增時間")]
        public DateTime CreatedTime { get; set; }

        [SwaggerSchema("異動時間")]
        public DateTime UpdateTime { get; set; }
    }
}