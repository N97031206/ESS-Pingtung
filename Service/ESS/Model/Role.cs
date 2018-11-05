using System;
using Support.Authorize;

namespace Service.ESS.Model
{
    public class Role
    {
        public Guid Id { get; set; }
        public RoleType Type { get; set; }
    }
}
