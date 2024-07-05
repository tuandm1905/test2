﻿using BusinessObject.DTOs;
using BusinessObject.Models;
using DataAccess.IRepository;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[Action]")]
        [ApiController]
    public class MessageController : ControllerBase
    {
        
       
            private readonly IConfiguration _config;
            private readonly IMessageRepository  _repo;
        private readonly IRoomRepository _room;

        private readonly string ok = "successfully";
            private readonly string notFound = "Not found";
            private readonly string badRequest = "Failed!";

            public MessageController(IConfiguration config, IMessageRepository repo, IRoomRepository room)
            {
                _config = config;
            _room = room;
                 _repo = repo;
            }


            [HttpGet]
            //  [Authorize]
            public async Task<ActionResult<IEnumerable<Message>>> ViewUserHistoryChatAsync(int roomId)
            {
                var list = await _repo.ViewAllMessageByRoomAsync(roomId);
                if (list.Any())
                {
                    return StatusCode(200, new
                    {
                        Message = "Get all message of this room " + ok,
                        Data = list
                    });
                }
                return StatusCode(404, new
                {                   
                    Message = notFound + "all message of this room"
                });
            }

        [HttpPost]
        public async Task<ActionResult> CreateMessageAsync(MessageCreateDTO messageCreateDTO)
        {
            if (ModelState.IsValid)
            {
                var checkMessage = await _repo.CheckMessageAsync(messageCreateDTO);
            if (checkMessage == true)
            {
                var message1 = await _repo.CreateMessageAsync(messageCreateDTO);
                if (message1)
                {
                    var result = await _room.UpdateContentRoomAsync(messageCreateDTO.RoomId);
                      if (result)
                      {
                          return StatusCode(200, new
                          {
                            Message = "Create messgae " + ok,
                            Data = message1
                          });
                      }
                
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
                Message = "Please enter the word!",
            });
        }
               
            return StatusCode(400, new
            {               
                Message = "Dont't accept empty information!",
            });

        }
    }
}
