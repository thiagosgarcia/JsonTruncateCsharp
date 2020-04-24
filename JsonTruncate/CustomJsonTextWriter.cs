﻿using System.IO;
using Newtonsoft.Json;

namespace JsonTruncate
{
    public class CustomJsonTextWriter : JsonTextWriter
    {
        public CustomJsonTextWriter(TextWriter textWriter) : base(textWriter) { }

        public int CurrentDepth { get; private set; }

        public override void WriteStartObject()
        {
            CurrentDepth++;
            base.WriteStartObject();
        }

        public override void WriteEndObject()
        {
            CurrentDepth--;
            base.WriteEndObject();
        }
    }
}
