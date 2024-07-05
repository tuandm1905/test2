﻿using AutoMapper;
using BusinessObject.DTOs;
using BusinessObject.Models;
using DataAccess.DAOs;
using DataAccess.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IRoomRepository  _repo;
        private readonly IMessageRepository _mess;
        private readonly IMapper _mapper;
    




        private readonly string ok = "successfully";
        private readonly string notFound = "Not found";
        private readonly string badRequest = "Failed!";

        public RoomController(IConfiguration config, IRoomRepository repo, IMessageRepository mess, IMapper mapper)
        {
            _config = config;
             _repo = repo;
            _mess = mess;
            _mapper = mapper;
        }

     
        [HttpGet]
        //  [Authorize]
        public async Task<ActionResult<IEnumerable<Room>>> ViewUserHistoryChatAsync(int accountId)
        {
            var list = await _repo.ViewUserHistoryChatAsync(accountId);
            if (list.Any())
            {
                return StatusCode(200, new
                {                   
                    Message = "Get list room of this account " + ok,
                    Data = list
                });
            }
            return StatusCode(404, new
            {                
                Message = notFound + "any room"
            });
        }

        [HttpGet]
        //  [Authorize]
        public async Task<ActionResult<IEnumerable<Room>>> ViewOwnerHistoryChatAsync(int ownerId)
        {
            var list = await _repo.ViewOwnerHistoryChatAsync(ownerId);
            if (list.Any())
            {
                return StatusCode(200, new
                {               
                    Message = "Get list room of this owner " + ok,
                    Data = list
                });
            }
            return StatusCode(404, new
            {               
                Message = notFound + "any room"
            });
        }


        [HttpGet]
        //  [Authorize]
        public async Task<ActionResult> GetRoomIdByAccountIdAndOwnerIdAsync(int accountId, int ownerId)
        {

            var room = await _repo.GetRoomIdByAccountIdAndOwnerIdAsync(accountId, ownerId);
            if (room != null)
            {
                return StatusCode(200, new
                {                 
                    Message = "Get room" + ok,
                    Data = room
                });
            }
            return StatusCode(404, new
            {       
                Message = notFound + "any room"
            });
        }

        [HttpGet]
        //  [Authorize]
        public async Task<ActionResult> GetRoomByAccountIdAndOwnerIdAsync(int accountId, int ownerId)
        {
            
                var room = await _repo.GetRoomByAccountIdAndOwnerIdAsync(accountId, ownerId);
            if (room != null)
            {
                return StatusCode(200, new
                {                  
                    Message = "Get room" + ok,
                    Data = room
                });
            }
            return StatusCode(404, new
            {              
                Message = notFound + "any room"
            });
        }

        [HttpGet("{roomId}")]
        //  [Authorize]
        public async Task<ActionResult> GetRoomByIdAsync(int roomId)
        {
           
                var size = await _repo.GetRoomByIdAsync(roomId);
            if (size != null)
            {
                return StatusCode(200, new
                {                  
                    Message = "Get room by id" + ok,
                    Data = size
                });
            }
            return StatusCode(404, new
            {              
                Message = notFound + "any room"
            });
        }

        [HttpPost]
        public async Task<ActionResult> CreateRoomAsync(RoomCreateDTO roomCreateDTO)
        {
            try
            {
              if (ModelState.IsValid)
              {
                    var checkSize = await _repo.CheckRoomAsync(roomCreateDTO.AccountId, roomCreateDTO.OwnerId);
                if (checkSize == true)
                {
                    var size1 = await _repo.CreateRoomAsync(roomCreateDTO);
                    if (size1)
                    {
                        return StatusCode(200, new
                        {
                            Message = "Create new room " + ok,
                            Data = size1
                        });
                    }
                    else
                    {
                        return StatusCode(500, new
                        { 
                            Message = "Server error",                        
                        });
                    }
                }
                return StatusCode(400, new
                {
                    Message = "Owner or User is is not exist!",
                });
              }

                return StatusCode(400, new
                { 
                    Message = "Dont't accept empty information!",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred: " + ex.Message
                });
            }

        }

       
    }
}
