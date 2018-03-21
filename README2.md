

[![Build status](https://ci.appveyor.com/api/projects/status/capxc7p5cvyrq21w/branch/master?svg=true)](https://ci.appveyor.com/project/MicrosoftOpenAPINETAdmin/openapi-net-csharpcomment/branch/master)

![C# Comment Reader Banner](docs/images/banner.png "Convert /// C# Comments --> OpenAPI.NET")

# C# Comment Reader [Preview]
[Disclaimer: This repository is in a preview state. Expect to see some iterating as we work towards the final release candidate slated for mid 2018. Feedback is welcome!]


### Welcome!
This reader is the first by-product of Microsoft's supported base [OpenAPI.NET](http://aka.ms/openapi) object model. This reader is designed to convert your native C# comments from your API code into a V3 OpenAPI document. All you need to do is follow a simple annotation schema for your API controller comments, and you automatically get all the benefits of the OpenAPI and it's related Swagger tooling.

### Annotations (C# Comments)
We've made an effort to develop an annotation model that maps very closely to the native .Net comment satructure for the C# language. In general, the below image describes the general concept of how this utility parse your C# comments and generate your OpenAPI.NET document.
![Convert Comments to OpenAPI](docs/images/comment-oai-map.png "Map /// C# Comments --> OpenAPI.NET")

### Mechanics
After you've corrrectly annotated your C# code, you'll need to build your solution and then retrieve the output XML file where MSBuild.exe aggegates the projects comments.
![Enable Comment Output](docs/images/vs-enable.png "Output comments from MSBuild.exe")


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