

[![Build status](https://ci.appveyor.com/api/projects/status/capxc7p5cvyrq21w/branch/master?svg=true)](https://ci.appveyor.com/project/MicrosoftOpenAPINETAdmin/openapi-net-csharpcomment/branch/master)

![C# Annotation Document Generator](docs/images/banner.png "Convert /// C# Comments --> OpenAPI.NET")

# C# Annotation Document Generator for OpenAPI.NET [Preview]
[Disclaimer: This repository is in a preview state. Expect to see some iterating as we work towards the final release candidate slated for late summer 2018. Feedback is welcome!]


### Welcome!
This component is the first by-product of Microsoft's supported base [OpenAPI.NET](http://aka.ms/openapi) object model. The module is designed to convert your native annotation XML from your API code into a OpenAPI document object. All you need to do is follow a simple annotation schema for your API controller comments, and you automatically get all the benefits of the OpenAPI and its related Swagger tooling.

_Remarks: While reading through the readme and wiki please know that_

_C# Comments refers to the /// csharp comments in code_

e.g.
```csharp
        /// <summary>
        /// Sample Get 1
        /// </summary>
        /// <group>Sample V1</group>
        /// <verb>GET</verb>
        /// <url>http://localhost:9000/V1/samples/{id}?queryBool={queryBool}</url>
        /// <param name="sampleHeaderParam1" cref="float" in="header">Header param 1</param>
        /// <param name="id" cref="string" in="path">The object id</param>
        /// <param name="queryBool" required="true" cref="bool" in="query">Sample query boolean</param>
        /// <response code="200"><see cref="SampleObject1"/>Sample object retrieved</response>
        /// <returns>The sample object 1</returns>
```

_Annotation refers to the compiler built version of the above /// csharp comments_

e.g.
```xml
        <member name="M:Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.Controllers.SampleControllerV1.SampleGet1(System.String,System.Boolean)">
            <summary>
            Sample Get 1
            </summary>
            <group>Sample V1</group>
            <verb>GET</verb>
            <url>http://localhost:9000/V1/samples/{id}?queryBool={queryBool}</url>
            <param name="sampleHeaderParam1" cref="T:System.Object" in="header">Header param 1</param>
            <param name="id" cref="T:System.String" in="path">The object id</param>
            <param name="queryBool" required="true" cref="T:System.Boolean" in="query">Sample query boolean</param>
            <response code="200"><see cref="T:Microsoft.OpenApi.CSharpComment.Reader.Tests.Contracts.SampleObject1"/>Sample object retrieved</response>
            <returns>The sample object 1</returns>
        </member>
```

_This Document Generator consumes the above annotations (outputted from MSBuild.exe) to create OpenAPI.NET objects._

### Annotations (C# Comments)
We've made an effort to develop an annotation model that maps very closely to the native .NET comment structure for the C# language. In general, the below image describes the general concept of how this utility can parse your annotation XML and generate your OpenAPI.NET document.
![Convert Comments to OpenAPI](docs/images/comment-oai-map.png "Map /// C# Comments --> OpenAPI.NET")

Consult our [Wiki](https://github.com/Microsoft/OpenAPI.NET.CSharpAnnotations/wiki) for specific guidance and examples on how to annotate your controllers.

### Mechanics
The items needed to use this component as shown in the sample below.
- The "XML documentation file(s)" from your MSBuild.exe output. (List<string>)
- Any DLL's that contain the data types of your API's request/response contracts. (List<string>)
- OpenAPI Document Version (string)
- Filterset Version (enum)

After you've correctly annotated your C# code, you'll need to build your solution and then retrieve the output XML file where MSBuild.exe aggregates the projects comments. This file is what this utility will use to convert your comments into an OpenAPI.NET object.
![Enable Comment Output](docs/images/vs-enable.png "Output comments from MSBuild.exe")

### Simple Example Code
Here's a simple exampled of how you'd use this component. The utility takes in two lists. The first shown below is the paths to your post-MSbuild.exe xml documentation files. The second being the paths to any DLL's that have classes that you reference in those XML comments.

For example, if you have an annotation for a response type as follows:
```csharp
/// <response code="200"><see cref="SampleObject1"/>Sample object retrieved</response>
```
You'd need to include the path to the .dll that contains the SampleObject1 class. 

Generating your OAI document should look something like this:
```csharp
var input = new OpenApiGeneratorConfig(
    annotationXmlDocuments: new List<XDocument>()
    {
        XDocument.Load(@"C:\TestData\Annotation.xml"),
        XDocument.Load(@"C:\TestData\Contracts.xml"),
    },
    assemblyPaths: new List<string>()
    {
        @"C:\TestData\Service.dll",
        @"C:\TestData\Contract.dll"
    },
    openApiDocumentVersion: "V1",
    filterSetVersion: FilterSetVersion.V1
);

GenerationDiagnostic result;

var generator = new OpenApiGenerator();

IDictionary<DocumentVariantInfo,OpenApiDocument> openApiDocuments =   generator.GenerateDocuments(
    openApiGeneratorConfig: input,
    generationDiagnostic: out result
);
```
In this example the generated should contain a valid OpenAPI.NET document for your API based on the annotation XML and contract dll's you included.

### Newtonsoft (JSON.Net)
C# Document Generator supports fetching Newtonsoft.Json "JsonProperty" Attribute, so if your service contracts use Newtonsoft please include the same version of Newtonsoft.Json.dll as used by service contracts in the assembly paths.

### Optional Advanced Configuration

Document generator also allows you to provide an optional advanced configuration as input in "OpenApiGeneratorConfig.AdvancedConfigurationXmlDocument"
which enables:

- Specifying annotations that logically apply to either the entire document or to certain set of operations.
- Generate multiple documents based on the variant information provided.

The configuration XML is handcrafted (NOT generated from Visual Studio build).

Consult our [Wiki](https://github.com/Microsoft/OpenAPI.NET.CSharpAnnotations/wiki/Advanced-Configuration-XML) for specific guidance and examples on how to draft this XML.

# Contributing
This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
