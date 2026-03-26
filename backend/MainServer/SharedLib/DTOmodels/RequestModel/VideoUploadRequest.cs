using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SharedLib.DTOmodels.RequestModel
{
    public class VideoUploadRequest
    {
        [FromForm(Name = "title")]
        public string Title { get; set; }

        [FromForm(Name = "description")]
        public string? Description { get; set; }

        [FromForm(Name = "video")]
        public IFormFile Video {  get; set; }
    }
}
