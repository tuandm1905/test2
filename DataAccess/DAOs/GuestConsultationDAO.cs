﻿using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Azure;
using Azure.Core;
using System.Security.Cryptography;

namespace DataAccess.DAOs
{
    public class GuestConsultationDAO
    {

        private readonly NirvaxContext  _context;
        private readonly IMapper _mapper;


        public GuestConsultationDAO(NirvaxContext context, IMapper mapper)
        {

             _context = context;
            _mapper = mapper;
        }

        //create
        public async Task<bool> CheckGuestConsultationAsync(GuestConsultationCreateDTO guestConsultationCreateDTO)
        {

            
            GuestConsultation? guestCreate = new GuestConsultation();
            guestCreate = await _context.GuestConsultations
                   .Include(i => i.Owner)
                .Include(i => i.Ad)
                .Include(i => i.StatusGuest)
                .SingleOrDefaultAsync(i => i.Phone.Trim() == guestConsultationCreateDTO.Phone.Trim() && i.AdId == guestConsultationCreateDTO.AdId);

            if (guestCreate == null)
            {
                return true;
            }
            return false;
        }

      
    //update
        public async Task<bool> CheckGuestConsultationExistAsync(int guestId ) 
        {
            GuestConsultation? sid = new GuestConsultation();

            sid = await _context.GuestConsultations.Include(i => i.Owner)
                .Include(i => i.Ad)
                .Include(i => i.StatusGuest).SingleOrDefaultAsync(i => i.GuestId  == guestId );

            if (sid == null)
            {
                return false;
            }
            return true;
        }



        public async Task<int> ViewGuestConsultationStatisticsAsync()
        {
            GuestConsultation guest = new GuestConsultation();
            var number = await _context.GuestConsultations.CountAsync();
            return number;
        }



        //owner,staff or admin??
        public async Task<List<GuestConsultationDTO>> GetAllGuestConsultationsAsync(string? searchQuery, int page, int pageSize) 
        {
            List<GuestConsultationDTO> listStaffDTO = new List<GuestConsultationDTO>();


            if (!string.IsNullOrEmpty(searchQuery))
            {
                List<GuestConsultation> getList = await _context.GuestConsultations.Include(i => i.Owner)
                .Include(i => i.Ad)
                .Include(i => i.StatusGuest)
                    .Where(i => i.Content.Contains(searchQuery) || i.Phone.Contains(searchQuery) || i.Fullname.Contains(searchQuery))
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                listStaffDTO = _mapper.Map<List<GuestConsultationDTO>>(getList);
            }
            else
            {
                List<GuestConsultation> getList = await _context.GuestConsultations.Include(i => i.Owner).Include(i => i.Ad).Include(i => i.StatusGuest)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                listStaffDTO = _mapper.Map<List<GuestConsultationDTO>>(getList);
            }
            return listStaffDTO;
        }

    
        //owner,staff 
        public async Task<GuestConsultationDTO> GetGuestConsultationsByIdAsync(int guestId )

        {
            GuestConsultationDTO guestConsultationDTO = new GuestConsultationDTO();
            try
            {
                GuestConsultation? sid = await _context.GuestConsultations.Include(i => i.Owner)
                .Include(i => i.Ad)
                .Include(i => i.StatusGuest).SingleOrDefaultAsync(i => i.GuestId  == guestId );
                guestConsultationDTO = _mapper.Map<GuestConsultationDTO>(sid);
                return guestConsultationDTO;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        
        }

  
        public async Task<bool> CreateGuestConsultationAsync(GuestConsultationCreateDTO guestConsultationCreateDTO) 
        {
          
            GuestConsultation guestConsultation = _mapper.Map<GuestConsultation>(guestConsultationCreateDTO);
            
            await _context.GuestConsultations.AddAsync(guestConsultation);
            int i = await _context.SaveChangesAsync();
            if (i > 0)
            {
                return true;
            }
            else { return false; }

        }

        //admin

        //oldPass = xyz
        //newPass =1234
        public async Task<bool> UpdateGuestConsultationAsync(GuestConsultationDTO guestConsultationDTO)
        {
           
            GuestConsultation? staffOrgin = await _context.GuestConsultations
                   .Include(i => i.Owner)
                .Include(i => i.Ad)
                .Include(i => i.StatusGuest)
                .SingleOrDefaultAsync(i => i.GuestId  == guestConsultationDTO.GuestId );
                _mapper.Map(guestConsultationDTO, staffOrgin);
                 _context.GuestConsultations.Update(staffOrgin);
                await _context.SaveChangesAsync();
                return true;
        }

        public async Task<bool> UpdateStatusGuestConsultationtAsync(int guestId , int statusGuestId)
        {

            GuestConsultation? staffOrgin = await _context.GuestConsultations
          .Include(i => i.Owner)
                .Include(i => i.Ad)
                .Include(i => i.StatusGuest)
                .SingleOrDefaultAsync(i => i.GuestId  == guestId );
            staffOrgin.StatusGuestId = statusGuestId;
             _context.GuestConsultations.Update(staffOrgin);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}





