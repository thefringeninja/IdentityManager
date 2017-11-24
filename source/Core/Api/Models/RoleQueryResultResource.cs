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

namespace IdentityManager.Api.Models
{
    public class RoleQueryResultResource
    {
        public RoleQueryResultResource(QueryResult<RoleSummary> result, RoleMetadata meta)
        {
            if (result == null) throw new ArgumentNullException("result");
            if (meta == null) throw new ArgumentNullException("meta");

            Data = new RoleQueryResultResourceData(result, meta);
            
            var links = new Dictionary<string, object>();
            if (meta.SupportsCreate)
            {
                links["create"] = new CreateRoleLink(meta);
            };
            Links = links;
        }

        public RoleQueryResultResourceData Data { get; set; }
        public object Links { get; set; }
    }

    public class RoleQueryResultResourceData : QueryResult<RoleSummary>
    {
        static RoleQueryResultResourceData()
        {
            AutoMapper.Mapper.CreateMap<QueryResult<RoleSummary>, RoleQueryResultResourceData>()
                .ForMember(x => x.Items, opts => opts.MapFrom(x => x.Items));
            AutoMapper.Mapper.CreateMap<RoleSummary, RoleResultResource>()
                .ForMember(x => x.Data, opts => opts.MapFrom(x => x));
        }

        public RoleQueryResultResourceData(QueryResult<RoleSummary> result, RoleMetadata meta)
        {
            if (result == null) throw new ArgumentNullException("result");
            if (meta == null) throw new ArgumentNullException("meta");

            AutoMapper.Mapper.Map(result, this);

            foreach (var role in this.Items)
            {
                var links = new Dictionary<string, string>();
                links.Add("detail", LinkFormatter.Role(role.Data.Subject));
                if (meta.SupportsDelete)
                {
                    links.Add("delete", LinkFormatter.Role(role.Data.Subject));
                }
                role.Links = links;
            }
        }

        public new IEnumerable<RoleResultResource> Items { get; set; }
    }

    public class RoleResultResource
    {
        public RoleSummary Data { get; set; }
        public object Links { get; set; }
    }
}
