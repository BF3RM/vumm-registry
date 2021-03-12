using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VUModManagerRegistry.Binders;

namespace VUModManagerRegistry.Models
{
    public class ModVersionForm
    {
        [ModelBinder(BinderType = typeof(JsonModelBinder))]
        public ModVersionDto Attributes { get; set; }
        public IFormFile Archive { get; set; }
    }
}