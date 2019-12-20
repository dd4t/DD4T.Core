[![AppVeyor](https://ci.appveyor.com/api/projects/status/github/dd4t/DD4T.Core?branch=master&svg=true&passingText=master)](https://ci.appveyor.com/project/DD4T/dd4t-core)

[![AppVeyor](https://ci.appveyor.com/api/projects/status/github/dd4t/DD4T.Core?branch=develop&svg=true&passingText=develop)](https://ci.appveyor.com/project/DD4T/dd4t-core)

# DD4T.Core
DD4T - delivery framework for Microsoft.NET


## Release notes for version 2.5

- Field annotations for page and keyword models no longer require 'IsMetadata=true' (128)
- Cache invalidation works now with all supported Tridion versions (110)
- Cache invalidation now removes all dependencies (including models), removes links, removes DCPs even if they are included in a page (126)
- ComponentFactory no longer throws NotImplementedExceptions (79)
- Upgraded Newtonsoft.Json to 11.0.2