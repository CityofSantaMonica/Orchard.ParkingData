using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace CSM.ParkingData
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ApiWriter = new Permission {
            Name = "ApiWriter",
            Description = "POST data to API endpoints",
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
                },
                new PermissionStereotype {
                    Name = "ApiWriter",
                    Permissions = new[] { ApiWriter }
                }
            };
        }
    }
}