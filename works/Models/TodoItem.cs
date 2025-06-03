using Swashbuckle.AspNetCore.Annotations;

namespace TodoApi.Models
{
    [SwaggerSchema("待辦事項模型", Description = "待辦事項的model，包含ID、名稱和完成狀態。")]
    public class TodoItem
    {
        [SwaggerSchema("待辦事項ID", ReadOnly = false)]
        public int Id { get; set; }

        [SwaggerSchema("待辦事項名稱", ReadOnly = false, Nullable = true)]
        public string? Title { get; set; }

        [SwaggerSchema("待辦事項是否完成", ReadOnly = false)]
        public bool IsDone { get; set; }
    }
}