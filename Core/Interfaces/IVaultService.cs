using Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IVaultService
    {
        public Task<List<VaultResponse>> GetUserVaultsByIdAsync(int Id);

    }
}
