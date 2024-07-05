﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTOs;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAOs
{
    public class AccountDAO
    {
        private readonly NirvaxContext _context;

        public AccountDAO(NirvaxContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Account>> GetAllAccountAsync()
        {
            return await _context.Accounts.Where( a => !a.Role.Equals("Admin")).ToListAsync();
        }

        public async Task<Account> GetAccountByIdAsync(int id)
        {
            return await _context.Accounts.FindAsync(id);
        }

        public async Task UpdateAccountAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Account>> SearchAccountAsync(string keyword)
        {
            return await _context.Accounts.Where(a => a.Fullname.Contains(keyword) || a.Email.Contains(keyword)).ToListAsync();
        }
    }
}
