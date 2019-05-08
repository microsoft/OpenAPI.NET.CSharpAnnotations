using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models
{
    public class SchemaInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public OpenApiSchema schema { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GenerationError error { get; set; }
    }
}
