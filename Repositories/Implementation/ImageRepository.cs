using CodePulse.API.Data;
using CodePulse.API.Models.Domain;
using CodePulse.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.API.Repositories.Implementation
{
    public class ImageRepository : IImageRepository
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public IWebHostEnvironment webHostEnvironment { get; }
        public ApplicationDbContext dbContext { get; }

        public ImageRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor,
            ApplicationDbContext dbContext)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = dbContext;
        }

       
        //To use this Upload method which is in the repository inside the controler first we have to inject inside the service 
        //Program.cs
        public async Task<BlogImage> Upload(IFormFile file, BlogImage blogImage)
        {
            //1- Upload the image to API/Images
            //Image will be stored to ContentRootPath i.e ProjectFile then we have Images Folder
            //Then create a file with FileName and Extension
            var localPath = Path.Combine(webHostEnvironment.ContentRootPath, "Images", $"{blogImage.FileName}{blogImage.FileExtension}");

            using var steam = new FileStream(localPath, FileMode.Create);

            await file.CopyToAsync(steam);
            //2- Update the database
            //https://codepulse.com/images/somefilename.jpg

            var httpRequest = httpContextAccessor.HttpContext.Request;
            var urlPath = $"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.PathBase}/Images/{blogImage.FileName}{blogImage.FileExtension}";

            blogImage.Url= urlPath;
            await dbContext.BlogImages.AddAsync(blogImage);
            await dbContext.SaveChangesAsync();

            return blogImage;
        }

        public async Task<IEnumerable<BlogImage>> GetAll()
        {
            //BlogImages is a collection/Table Name that we are getting 
            return await dbContext.BlogImages.ToListAsync();
        }
    }
}
