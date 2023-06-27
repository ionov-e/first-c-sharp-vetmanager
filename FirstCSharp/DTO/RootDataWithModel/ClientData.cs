﻿using FirstCSharp.DTO.RootDataWithModel.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FirstCSharp.DTO.RootDataWithModel
{
    public class ClientData : AbstractRootDataWithOneModel
    {
        [JsonPropertyName("client")]
        public required Client Model { get; set; }

        public override Client GetModel()
        {
            return Model;
        }
    }
}