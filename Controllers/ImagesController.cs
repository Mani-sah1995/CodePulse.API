using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }

        //GET: {apiBaseUrl}/api/Images When you come to this address with the http Get keyword you will get all the images back in result
        [HttpGet]
        public async Task<IActionResult> GetAllImages()
        {
            //Call Image repository to get all the images
            var images = await imageRepository.GetAll();
            //Before I can return the response I should convert this domain model back to DTO

            //Convert this domain model to DTO
            var response = new List<BlogImageDto>();

            //Now we can iterate through the images domain Model and convert it into the DTO and store it inside the list
            foreach (var image in images) 
            {
                response.Add(new BlogImageDto
                {
                    Id = image.Id,
                    Title = image.Title,
                    DateCreated = DateTime.Now,
                    FileExtension = image.FileExtension,
                    FileName = image.FileName,
                    Url = image.Url,
                });
            }
            return Ok(response);
        }


        //POST: {apiBaseUrl}/api/Images
        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file,
            [FromForm] string fileName, [FromForm] string title)
        {
            ValidateFileUpdate(file);
            if (ModelState.IsValid)
            {
                //File Upload
                //we will use this domain model then go to the repository and upload this file to API and also db table
                var blogImage = new BlogImage
                {
                    FileExtension = Path.GetExtension(file.FileName).ToLower(),
                    FileName = fileName,
                    Title = title,
                    DateCreated = DateTime.Now,


                };
                 blogImage = await imageRepository.Upload(file, blogImage);

                //we can not directly return the domain model so first convert it to the DTO 
                //because we are following the clean coding technique
                //return Ok(blogImage);

                //Convert Domain Model DTO before we expose it to the out side world
                //Now we can use this response instead of Domain Model and pass it back as result back to client
                var response = new BlogImageDto
                {
                    Id = blogImage.Id,
                    Title = blogImage.Title,
                    DateCreated = DateTime.Now,
                    FileExtension= blogImage.FileExtension,
                    FileName = blogImage.FileName,
                    Url = blogImage.Url,

                };

                return Ok(response);
            }

            return BadRequest(ModelState);
        }

        private void ValidateFileUpdate(IFormFile file)
        {
            var allowedExtension = new string[] { ".jpg", "jpge", ".png" };

            if (!allowedExtension.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                ModelState.AddModelError("file", "Unsupported file formate");
            }

            if (file.Length > 10485760)
            {
                ModelState.AddModelError("file", "File Size can not be more than 10MB");
            }
         }
    }
}
