using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace CSM.ParkingData
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ApiWriter = new Permission {
            Description = "POST data to API endpoints",
            Name = "ApiWriter"
        };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                ApiWriter
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] { ApiWriter }
                }
            };
        }
    }
}