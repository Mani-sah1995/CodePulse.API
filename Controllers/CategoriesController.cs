using CodePulse.API.Data;
using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers
{
    //https://localhost:xxxx/api/categories
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        //We are not directly injecting dbContext directly here in the controller instead we are using Repository
        //private readonly ApplicationDbContext dbContext;
        private readonly ICategoryRepository categoryRepository;
        public CategoriesController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public ApplicationDbContext DbContext { get; }

        //
        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateCategory([FromBody]CreateCategoryRequestdto request)
        {
            //Map DTO to domain Models
            var category = new Category
            {
                Name = request.Name,
                UrlHandle = request.UrlHandle
            };

            //Removed this code from controller and Move to Repository
            //await dbContext.Categories.AddAsync(category);
            //await dbContext.SaveChangesAsync();

            //Instead of using dbcontext directly in the controller to save the data
            //we are using Repository
            await categoryRepository.CreateAsync(category);

            //Domain model to DTO
            var response = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle
            };
            return Ok(response);
        }

        //Get: /api/categories?query=html&sortBy=name&sprtDirecton=desc
        [HttpGet]        
        public async Task<IActionResult> GetAllCategories(
            [FromQuery] string? query, 
            [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            var categories=await categoryRepository
                .GetAllAsync(query ,sortBy, sortDirection, pageNumber, pageSize);
            //Map Domain Model to DTO
            var response = new List<CategoryDto>();
            foreach(var category in categories)
            {
                response.Add(new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    UrlHandle = category.UrlHandle
                });
            }

            return Ok(response);
        }

        //Get: https://localhost:xxxx/api/categories/{id}
        [HttpGet]
        [Route("{id=Guid}")]
        public async Task<IActionResult> GetCategoryId([FromRoute]Guid id)
        {
            var existingCategory =await categoryRepository.GetById(id);
            if (existingCategory is null)
            {
                return NotFound();
            }
            var response = new CategoryDto
            {
                Id = existingCategory.Id,
                Name = existingCategory.Name,
                UrlHandle = existingCategory.UrlHandle
            };
            return Ok(response);
        }

        //Put: https://localhost:7072/api/categories/{id}
        [HttpPut]
        [Route("{id=Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> EditCategory([FromRoute] Guid id, UpdateCategoryRequestDto request)
        {
            //Convert DTO to domain Model
            var category = new Category
            {
                Id = id,
                Name = request.Name,
                UrlHandle = request.UrlHandle
            };
            category= await categoryRepository.UpdateAsync(category);
            if(category == null)
            {
                return NotFound();
            }
            //Convert Domain Model to DTO
            var response = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle
            };
            return Ok(response);

        }

        //DELETE : https://localhost:7072/api/categories/{id}
        [HttpDelete]
        [Route("{id=Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
        {
            var category = await categoryRepository.DeleteAsync(id);
            if(category is null)
            {
                return NotFound();
            }
            //Convert Domain model to DTO
            var response = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle

            };
            return Ok(response);
        }

        // GET: https://localhost:7226/api/categories/count
        [HttpGet]
        [Route("count")]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> GetCategoriesTotal()
        {
            var count = await categoryRepository.GetCount();

            return Ok(count);
        }
    }
}
