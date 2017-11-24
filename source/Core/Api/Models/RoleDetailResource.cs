﻿/*
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
    public class RoleDetailResource
    {
        public RoleDetailResource(RoleDetail role, RoleMetadata meta)
        {
            if (role == null) throw new ArgumentNullException("role");
            if (meta == null) throw new ArgumentNullException("meta");

            Data = new RoleDetailDataResource(role, meta);

            var links = new Dictionary<string, string>();
            if (meta.SupportsDelete)
            {
                links["Delete"] = LinkFormatter.Role(role.Subject);
            }
            this.Links = links;
        }

        public RoleDetailDataResource Data { get; set; }
        public object Links { get; set; }
    }

    public class RoleDetailDataResource : Dictionary<string, object>
    {
        public RoleDetailDataResource(RoleDetail role, RoleMetadata meta)
        {
            if (role == null) throw new ArgumentNullException("role");
            if (meta == null) throw new ArgumentNullException("meta");

            this["Name"] = role.Name;
            this["Subject"] = role.Subject;

            if (role.Properties != null)
            {
                var props =
                    from p in role.Properties
                    let m = (from m in meta.UpdateProperties where m.Type == p.Type select m).SingleOrDefault()
                    where m != null
                    select new
                    {
                        Data = m.Convert(p.Value),
                        Meta = m,
                        Links = new
                        {
                            update = LinkFormatter.RoleProperty(role.Subject, p.Type)
                        }
                    };
                
                if (props.Any())
                {
                    this["Properties"] = props.ToArray();
                }
            }
        }
    }
}
