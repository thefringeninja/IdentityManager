/*
 * Copyright 2014 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using IdentityManager.Extensions;

namespace IdentityManager.Api.Models
{
    public class UserDetailResource
    {
        public UserDetailResource(UserDetail user, IdentityManagerMetadata idmMeta, RoleSummary[] roles)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (idmMeta == null) throw new ArgumentNullException("idmMeta");

            Data = new UserDetailDataResource(user, idmMeta, roles);

            var links = new Dictionary<string, string>();
            if (idmMeta.UserMetadata.SupportsDelete)
            {
                links["Delete"] = LinkFormatter.User(user.Subject);
            }
            this.Links = links;
        }

        public UserDetailDataResource Data { get; set; }
        public object Links { get; set; }
    }

    public class UserDetailDataResource : Dictionary<string, object>
    {
        public UserDetailDataResource(UserDetail user, IdentityManagerMetadata meta, RoleSummary[] roles)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (meta == null) throw new ArgumentNullException("meta");

            this["Username"] = user.Username;
            this["Name"] = user.Name;
            this["Subject"] = user.Subject;

            if (user.Properties != null)
            {
                var props =
                    from p in user.Properties
                    let m = (from m in meta.UserMetadata.UpdateProperties where m.Type == p.Type select m).SingleOrDefault()
                    where m != null
                    select new
                    {
                        Data = m.Convert(p.Value),
                        Meta = m,
                        Links = new
                        {
                            update = LinkFormatter.UserProperty(user.Subject, p.Type)
                        }
                    };

                if (props.Any())
                {
                    this["Properties"] = props.ToArray();
                }
            }

            if (roles != null && user.Claims != null)
            {
                var roleClaims = user.Claims.Where(x => x.Type == meta.RoleMetadata.RoleClaimType);
                var query =
                    from r in roles
                    orderby r.Name
                    let add = LinkFormatter.UserRole(user.Subject, r.Name)
                    let remove = add
                    select new
                    {
                        data = roleClaims.Any(x => x.Value == r.Name),
                        meta = new
                        {
                            type = r.Name,
                            description = r.Description,
                        },
                        links = new
                        {
                            add,
                            remove
                        }
                    };
                this["Roles"] = query.ToArray();
            }

            if (meta.UserMetadata.SupportsClaims && user.Claims != null)
            {
                var claims =
                    from c in user.Claims.ToArray()
                    select new
                    {
                        Data = c,
                        Links = new
                        {
                            delete = LinkFormatter.UserClaim(user.Subject, c.Type, c.Value)
                        }
                    };

                this["Claims"] = new
                {
                    Data = claims.ToArray(),
                    Links = new
                    {
                        create = LinkFormatter.UserClaims(user.Subject)
                    }
                };
            }
        }
    }
}
