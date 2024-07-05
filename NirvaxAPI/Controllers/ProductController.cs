﻿using AutoMapper;
using BusinessObject.DTOs;
using BusinessObject.Models;
using DataAccess.DAOs;
using DataAccess.IRepository;
using DataAccess.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IImageRepository _imageRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public ProductController(IProductRepository productRepository, IMapper mapper, IImageRepository imageRepository, INotificationRepository notificationRepository)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _imageRepository = imageRepository;
            _notificationRepository = notificationRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var products = await _productRepository.GetAllAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null || product.Isdelete == true || product.Isban == true)
                {
                    return NotFound(new { message = "Product is not found." });
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("owner/{ownerId}")]
        public async Task<IActionResult> GetByOwner(int ownerId)
        {
            try 
            { 
                var products = await _productRepository.GetByOwnerAsync(ownerId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            try
            { 
                var products = await _productRepository.GetByCategoryAsync(categoryId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("brand/{brandId}")]
        public async Task<IActionResult> GetByBrand(int brandId)
        {
            try
            { 
                var products = await _productRepository.GetByBrandAsync(brandId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = ex.Message
                });
            }
        }
        [HttpPost]
        [Authorize(Roles = "Owner,Staff")]
        public async Task<IActionResult> Create([FromForm] ProductDTO productDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var product = _mapper.Map<Product>(productDto);
                var check = await _productRepository.CheckProductAsync(product);
                if (!check)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable, new { message = "The product name has been duplicated." });
                }

                await _productRepository.CreateAsync(product);

                foreach (var link in productDto.ImageLinks)
                {
                        // Save image information to database
                        var image = new BusinessObject.Models.Image
                        {
                            LinkImage = link,
                            ProductId = product.ProductId
                        };
                        await _imageRepository.AddImagesAsync(image);
                }
                return Ok(new { message = "Product is created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Owner,Staff")]
        public async Task<IActionResult> Update(int id, [FromForm] ProductDTO productDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null || product.Isdelete == true)
                {
                    return NotFound(new { message = "Product is not found." });
                }
                
                _mapper.Map(productDto, product);
                var check = await _productRepository.CheckProductAsync(product);
                if (!check)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable, new { message = "The product name has been duplicated." });
                }

                await _productRepository.UpdateAsync(product);

                IEnumerable<BusinessObject.Models.Image> images = await _imageRepository.GetByProductAsync(id);
                foreach(BusinessObject.Models.Image img in images)
                {
                    // Xóa ảnh cũ trước khi cập nhật ảnh mới
                    await _imageRepository.DeleteImagesAsync(img);
                }
                // Handle image upload
                foreach (var link in productDto.ImageLinks)
                {
                    // Save image information to database
                    var image = new BusinessObject.Models.Image
                    {
                        LinkImage = link,
                        ProductId = product.ProductId
                    };
                    await _imageRepository.AddImagesAsync(image);
                }
                return Ok(new { message = "Product is updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner,Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            { 
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null || product.Isdelete == true)
                {
                    return NotFound(new { message = "Product not found." });
                }
                IEnumerable<BusinessObject.Models.Image> images = await _imageRepository.GetByProductAsync(id);
                foreach (BusinessObject.Models.Image img in images)
                {
                    // Xóa ảnh cũ trước khi xóa ảnh mới
                    await _imageRepository.DeleteImagesAsync(img);
                }
                await _productRepository.DeleteAsync(product);
                return Ok(new { message = "Product is deleted successfully"});
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("ban/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BanProduct(int id)
        {
            try
            { 
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null || product.Isdelete == true)
                {
                    return NotFound(new { message = "Product not found." });
                }
                await _productRepository.BanProductAsync(product);
                var notification = new Notification
                {
                    AccountId = null,
                    OwnerId = product.OwnerId, // Assuming Product model has OwnerId field
                    Content = $"Your product '{product.Name}' has been banned.",
                    IsRead = false,
                    Url = null,
                    CreateDate = DateTime.UtcNow
                };

                await _notificationRepository.AddNotificationAsync(notification);
                return Ok(new { message = "Product is banned successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("unban/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnbanProduct(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null || product.Isdelete == true)
                {
                    return NotFound(new { message = "Product not found." });
                }
                await _productRepository.UnbanProductAsync(product);
                var notification = new Notification
                {
                    AccountId = null,
                    OwnerId = product.OwnerId, // Assuming Product model has OwnerId field
                    Content = $"Your product '{product.Name}' has been unbanned.",
                    IsRead = false,
                    Url = null,
                    CreateDate = DateTime.UtcNow
                };

                await _notificationRepository.AddNotificationAsync(notification);
                return Ok(new { message = "Product has been unbanned successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{productId}/rate")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> RateProduct(int productId, [FromBody] int rating)
        {
            try
            {
                if (rating < 1 || rating > 5)
                {
                    return BadRequest(new { message = "Rating must be between 1 and 5." });
                }

                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null || product.Isdelete == true || product.Isban == true)
                {
                    return NotFound(new { message = "Product is not found." });
                }

                await _productRepository.AddRatingAsync(product, rating);

                return Ok(new { message = "Product is rated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("top5/{ownerId}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetTop5SellingProductsByOwner(int ownerId)
        {
            try
            {
                var products = await _productRepository.GetTopSellingProductsByOwnerAsync(ownerId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("top10")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetTop10SellingProducts()
        {
            try
            {
                var products = await _productRepository.GetTopSellingProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("searchs")]
        public async Task<IActionResult> Searchs(string searchTerm, double? minPrice, double? maxPrice, [FromQuery] List<int> categoryIds, [FromQuery] List<int> brandIds, [FromQuery] List<int> sizeIds)
        {
            try
            {
                var result = await _productRepository.SearchProductsAndOwnersAsync(searchTerm, minPrice, maxPrice, categoryIds, brandIds, sizeIds);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = ex.Message
                });
            }   
        }
    }

}
