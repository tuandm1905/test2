﻿using AutoMapper.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccess.Repository.StaffRepository;
using BusinessObject.DTOs;
using BusinessObject.Models;
using DataAccess.DAOs;
using DataAccess.IRepository;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Azure;
using Pipelines.Sockets.Unofficial.Buffers;

namespace DataAccess.Repository
{
    public class VoucherRepository : IVoucherRepository
    {
       
        private readonly VoucherDAO _voucherDAO;
        public VoucherRepository(VoucherDAO voucherDAO)
        {
            _voucherDAO = voucherDAO;
        }
        public Task<int> QuantityVoucherUsedStatisticsAsync(int ownerId)
        {
            return _voucherDAO.QuantityVoucherUsedStatisticsAsync(ownerId);

        }
       public Task<Voucher> GetVoucherById(string voucherId)
        {
            return _voucherDAO.GetVoucherById(voucherId);
        }
        public Task<bool> PriceAndQuantityByOrderAsync( string voucherId)
        {
            return _voucherDAO.PriceAndQuantityByOrderAsync(voucherId);
        }
        public Task<double> PriceVoucherUsedStatisticsAsync(int ownerId)
        {
            return _voucherDAO.PriceVoucherUsedStatisticsAsync(ownerId);

        }
        public Task<bool> CheckVoucherByIdAsync(string voucherId)
        {
            return _voucherDAO.CheckVoucherByIdAsync(voucherId);
        }
        public Task<bool> CheckVoucherAsync(DateTime startDate, DateTime endDate, string voucherId)
        {
            return _voucherDAO.CheckVoucherAsync(startDate, endDate, voucherId);
        }
        public Task<bool> CheckVoucherExistAsync(VoucherDTO voucherDTO)
        {
            return _voucherDAO.CheckVoucherExistAsync(voucherDTO);
        }

       
        public Task<List<VoucherDTO>> GetAllVouchersAsync(string? searchQuery, int page, int pageSize)
        {
            return _voucherDAO.GetAllVouchersAsync(searchQuery, page, pageSize);
        }

       public Task<List<VoucherDTO>> GetAllVoucherForUserAsync()
        {
            return _voucherDAO.GetAllVoucherForUserAsync();
        }

     
        public Task<VoucherDTO> GetVoucherDTOByIdAsync(string voucherId)
        {
            return _voucherDAO.GetVoucherDTOByIdAsync(voucherId);
        }

        public Task<bool> CreateVoucherAsync(VoucherCreateDTO voucherCreateDTO)
        {
            return _voucherDAO.CreateVoucherAsync(voucherCreateDTO);
        }
        
        public Task<bool> UpdateVoucherAsync(VoucherDTO voucherDTO)
        {
            return _voucherDAO.UpdateVoucherAsync(voucherDTO);
        }
        public Task<bool> DeleteVoucherAsync(string voucherId)
        {
            return _voucherDAO.DeleteVoucherAsync(voucherId);
        }

    }
}
