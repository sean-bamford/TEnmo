using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace TenmoClient.Models
{
    public class Transfer
    {
        public decimal Amount { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public int ReceiverId { get; set; }
        public int TransferId { get; set; }
        public string TransferStatus { get; set; }
        public string TransferType { get; set; }


    }
}


