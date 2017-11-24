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
    public class UserQueryResultResource
    {
        public UserQueryResultResource(QueryResult<UserSummary> result, UserMetadata meta)
        {
            if (result == null) throw new ArgumentNullException("result");
            if (meta == null) throw new ArgumentNullException("meta");

            Data = new UserQueryResultResourceData(result, meta);
            
            var links = new Dictionary<string, object>();
            if (meta.SupportsCreate)
            {
                links["create"] = new CreateUserLink(meta);
            };
            Links = links;
        }

        public UserQueryResultResourceData Data { get; set; }
        public object Links { get; set; }
    }

    public class UserQueryResultResourceData : QueryResult<UserSummary>
    {
        static UserQueryResultResourceData()
        {
            AutoMapper.Mapper.CreateMap<QueryResult<UserSummary>, UserQueryResultResourceData>()
                .ForMember(x => x.Items, opts => opts.MapFrom(x => x.Items));
            AutoMapper.Mapper.CreateMap<UserSummary, UserResultResource>()
                .ForMember(x => x.Data, opts => opts.MapFrom(x => x));
        }

        public UserQueryResultResourceData(QueryResult<UserSummary> result, UserMetadata meta)
        {
            if (result == null) throw new ArgumentNullException("result");
            if (meta == null) throw new ArgumentNullException("meta");

            AutoMapper.Mapper.Map(result, this);

            foreach (var user in this.Items)
            {
                var links = new Dictionary<string, string> {
                    {"Detail", LinkFormatter.User(user.Data.Subject)}
                };
                if (meta.SupportsDelete)
                {
                    links.Add("delete", LinkFormatter.User(user.Data.Subject));
                }
                user.Links = links;
            }
        }

        public new IEnumerable<UserResultResource> Items { get; set; }
    }

    public class UserResultResource
    {
        public UserSummary Data { get; set; }
        public object Links { get; set; }
    }
}
