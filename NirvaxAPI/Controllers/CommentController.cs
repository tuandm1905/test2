﻿using AutoMapper;
using BusinessObject.DTOs;
using BusinessObject.Models;
using DataAccess.DAOs;
using DataAccess.IRepository;
using DataAccess.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public CommentController(IProductRepository productRepository ,ICommentRepository commentRepository,INotificationRepository notificationRepository, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
            _productRepository = productRepository;
            _notificationRepository = notificationRepository;
        }

        // Lấy tất cả comments của một sản phẩm
        [HttpGet]
        public async Task<IActionResult> GetComments(int productId)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null || product.Isdelete == true || product.Isban == true)
                {
                    return NotFound(new { message = "Product is not found." });
                }
                var products = await _commentRepository.GetCommentsByProductIdAsync(productId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddComment([FromForm] CommentDTO commentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var product = await _productRepository.GetByIdAsync(commentDto.ProductId);
                if (product == null || product.Isdelete == true || product.Isban == true)
                {
                    return NotFound(new { message = "Product is not found." });
                }
                var comment = _mapper.Map<Comment>(commentDto);
                
                await _commentRepository.AddCommentAsync(comment);
                var notification = new Notification
                {
                    AccountId = comment.AccountId,
                    OwnerId = null, // Assuming Product model has OwnerId field
                    Content = $"Your product has just been commented.",
                    IsRead = false,
                    Url = null,
                    CreateDate = DateTime.UtcNow
                };

                await _notificationRepository.AddNotificationAsync(notification);               
                return Ok(new { message = "Comment is created successfully." });
                
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateComment(int commentId, string updateComment)
        {
            try
            {
                var comment = await _commentRepository.GetCommentByIdAsync(commentId);
                var product = await _productRepository.GetByIdAsync(comment.ProductId);
                if (product == null || product.Isdelete == true || product.Isban == true)
                {
                    return NotFound(new { message = "Product is not found." });
                }
                
                
                comment.Content = updateComment;
                await _commentRepository.UpdateCommentAsync(comment);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to create the comment." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{commentId}/reply")]
        [Authorize(Roles = "Owner,Staff")]
        public async Task<IActionResult> ReplyComment(int commentId, [FromBody] ReplyCommentDTO replyDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var comment = await _commentRepository.GetCommentByIdAsync(commentId);

                var product = await _productRepository.GetByIdAsync(comment.ProductId);

                if (product == null || product.Isdelete == true || product.Isban == true)
                {
                    return NotFound(new { message = "Product is not found." });
                }
                
                

                _mapper.Map(replyDto, comment);
                comment.ReplyTimestamp = DateTime.UtcNow;

                await _commentRepository.UpdateCommentAsync(comment);
                var notification = new Notification
                {
                    AccountId = comment.AccountId,
                    OwnerId = null, // Assuming Product model has OwnerId field
                    Content = $"Your comment has just been replied.",
                    IsRead = false,
                    Url = null,
                    CreateDate = DateTime.UtcNow
                };

                await _notificationRepository.AddNotificationAsync(notification);
                return Ok(new { message = "Reply successfully." });
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
