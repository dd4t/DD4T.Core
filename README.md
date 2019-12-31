[![AppVeyor](https://ci.appveyor.com/api/projects/status/github/dd4t/DD4T.Core?branch=master&svg=true&passingText=master)](https://ci.appveyor.com/project/DD4T/dd4t-core)

[![AppVeyor](https://ci.appveyor.com/api/projects/status/github/dd4t/DD4T.Core?branch=develop&svg=true&passingText=develop)](https://ci.appveyor.com/project/DD4T/dd4t-core)

# DD4T.Core
DD4T - delivery framework for Microsoft.NET


## Release notes for version 2.5

This release rolls up the bugfixes and stabilization and performance improvements that have been released in the various DD4T NuGet packages over the last years. 

This release contains new versions of:
- All NuGet packages that make up the .NET version of DD4T
- The Tridion templates
- The dd4t-cachechannel.jar (which is needed to invalidate items in the DD4T cache when they are republished or unpublished)

The Java version of DD4T is not included in this release. If you want to use DD4T in a Java web application, please use version 2.1.5. However, this version does not include support for the page regions that were introduced by SDL in SDL Tridion Sites 9.0. You can use this version in combination with the 2.5 templates.


## Features / improvements introduced in this release
- Full support for regions in Tridion 9 (see [below](#regions-in-tridion-9)).
- Cache invalidation is now working properly with SDL Web 8.5 as well as Tridion 9.0 / 9.1 (see https://github.com/dd4t/DD4T.Caching.ApacheMQ for installation notes).
- Cache invalidation now removes all dependencies on a Tridion page or component, including indirect dependencies like models 
- Cache invalidation also works for component links now. This means you can cache them for much longer. To do that, add `<add key="DD4T.CacheSettings.Link" value="3600" />` to your Web.config. The value is in seconds, so this would cache links for 1 hour, but you could cache them for longer if you want because they are invalidated anyway.
- Metadata fields on pages and templates work even if they are annotated without 'IsMetadata=true'.
- Experience Manager implementation was optimized. You can now load the page bootstrap code (including the HTML comments required to identify the page) with the following command: <br/> `@Model.InsertXpmPageMarkup()`. <br/>It is also possible to make fields in linked components editable, by using this command in your Razor views: `@Model.Link.StartXpmEditingZone(Model)`<br/> (assuming the property 'Link' points to a linked component).
- Added missing DependencyMapper classes to all providers so they can now all be loaded automatically.
- Upgraded Newtonsoft.Json to 11.0.2 for all DD4T projects



## Bugs fixed in this release
- [BinaryFactory leaves empty folders if there is a BinaryNotFound exception](https://github.com/dd4t/DD4T.Core/issues/117)
- [ComponentPresentationFactory stores DCPs in the cache based on only the Component URI](https://github.com/dd4t/DD4T.Core/issues/96)
- [ComponentFactory throws NotImplementedExceptions](https://github.com/dd4t/DD4T.Core/issues/79)
- [Providers.cs uses IComponentProvider, which is marked as obsolete](https://github.com/dd4t/DD4T.DI.Autofac/issues/14)
- [If a binary is requested with a slash at the end, a folder is created and the binary cannot be stored on the file system anymore](https://github.com/dd4t/DD4T.MVC/issues/44)
- [Fails to return highest priority cp if not in xml format ](https://github.com/dd4t/DD4T.Providers.SDLWeb8.5/issues/2)
- [Binaries with file names longer than 64 characters sometimes fail publishing](https://github.com/dd4t/DD4T.TridionTemplates/issues/43)
- [Bug with KeyFieldName in Resource schema](https://github.com/dd4t/DD4T.TridionTemplates/issues/33)
- [ComponentFactory throws NotImplementedExceptions](https://github.com/dd4t/DD4T.Core/issues/79)


## Getting started with DD4T

If you want to start using DD4T yourself, please check out the [Wiki](https://github.com/dd4t/DD4T.Core/wiki).


## Regions in Tridion 9
Before the introduction of page regions in Tridion 9, the concept of 'regions' of component presentations on a page was commonly implemented using a metadata field called 'region' on the metadata of the component template. In that case, the web application would group component presentations together based on this metadata field.

With DD4T 2.5 you can now upgrade your CM to Tridion 9 / 9.1 and start using regions, without having to change your web application. All you need to do is add the new template building block 'Convert regions to CT metadata' to your page templates. It will put a metadata field called region in each of the component templates, and put the name of the region in it.

