using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.DAO;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TransferController : ControllerBase
    {
        private readonly ITransferDao transferDao;

        public TransferController(ITransferDao transferDao)
        {
            this.transferDao = transferDao;
        }


        [HttpPost]
        public ActionResult<Transfer> SendMoney(Transfer transfer)
        {   
            transfer.SenderId = Convert.ToInt32(User.FindFirst("sub")?.Value);
            Transfer result = transferDao.SendMoney(transfer);
            return Ok(result);
        }
        [HttpPost("request")]
        public ActionResult<Transfer> RequestMoney(Transfer transfer)
        {
            transfer.ReceiverId = Convert.ToInt32(User.FindFirst("sub")?.Value);
            Transfer result = transferDao.RequestMoney(transfer);
            return Ok(result);
        }

        [HttpPut()]
        public ActionResult<Transfer> ApproveRejectTransfer(Transfer transfer)
        {
            //transfer.ReceiverId = Convert.ToInt32(User.FindFirst("sub")?.Value);
            bool result = transferDao.ApproveRejectTransfer(transfer);
            if (result)
            {
                return Ok(transfer);
            }
            else
            {
                transfer.TransferStatus = "Pending";
                return BadRequest(transfer);
            }
            
        }

        [HttpGet]
        public ActionResult<List<Transfer>> GetTransfers()
        {
            int userId = Convert.ToInt32(User.FindFirst("sub")?.Value);
            List<Transfer> transfers = transferDao.GetTransfers(userId);

            return Ok(transfers);
        }
    }
}
