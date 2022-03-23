using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDao
    {
        Transfer SendMoney(Transfer transfer);

        Transfer RequestMoney(Transfer transfer);

        bool ApproveRejectTransfer(Transfer transfer);
        List<Transfer> GetTransfers(int userId);
    }
}
