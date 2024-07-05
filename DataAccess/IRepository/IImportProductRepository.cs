﻿using AutoMapper.Execution;
using BusinessObject.DTOs;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepository
{
    public interface IImportProductRepository
    {
        Task<List<ImportProductDTO>> GetAllImportProductAsync(int warehouseId, DateTime? from, DateTime? to);
        
        Task<List<ImportProductDTO>> GetImportProductByWarehouseAsync(int warehouseId);
        Task<ImportProductDTO> GetImportProductByIdAsync(int importId);

        
        Task<bool> CheckImportProductExistAsync(int importId);

        Task<bool> CreateImportProductAsync(ImportProductCreateDTO importProductCreateDTO);

        Task<bool> UpdateImportProductAsync(ImportProductDTO importProductDTO);
    
      


    }
}
