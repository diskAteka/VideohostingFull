using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.DTOmodels.RequestModel
{
    public class ReactionRequest
    {
        public int UserId { get; set; }
        public int VideoId { get; set; }
        public bool IsLike { get; set; }
    }
}
