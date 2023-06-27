﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FirstCSharp.DTO.RootDataWithModel.Model
{
    public class Color : ModelInterface
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        public required string combo_manual_id { get; set; }
        public required string title { get; set; }
        public required string value { get; set; }
        public required string dop_param1 { get; set; }
        public required string dop_param2 { get; set; }
        public required string dop_param3 { get; set; }
        public required string is_active { get; set; }
    }
}