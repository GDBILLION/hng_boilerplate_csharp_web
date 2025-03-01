using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Hng.Application.Features.UserManagement.Dtos
{
    public class DeactivateUserDto
    {
        [JsonPropertyName("reason")]
        public string? Reason { get; set; }
    }
}
