Startitecture.Orm (ST/orm)
======================
ST/orm is an object-relational mapper that combines the speed of ORMs like [Dapper](https://github.com/StackExchange/Dapper) and [PetaPoco](https://github.com/CollaboratingPlatypus/PetaPoco/wiki) with the rich POCO composition and query features of traditional ORMs.

Initially based on the work done in PetaPoco, ST/orm represents an 8 year journey in schema modeling, query language, data design, and object hyrdration, built (and re-built) for use in private corporations. [Startitecture](https://startitecture.com) has gained the rights to redistribute those substantial modifications from the original PetaPoco library as a fully open source package.   

Vision
---
ST/orm is designed with a single purpose - to overcome the inherent problems in using ORM entties as dual purpose domain models and data representations while increasing productivity of development.

In any complex application, domain (runtine) models and data (persistence) models will diverge, and developers are faced with a variety of unpleasant choices:

1. Extend ORM data entities to support domain model methods and properties and deal with the performace issues of traditonal ORMs
2. Build anemic domain models based on entities, increasing coupling and decreasing cohesion in the application layer
3. Build fully separate domain and data models, and rely either on significant customization or application-layer aggregation for hyrdating complex entity aggregates

ST/orm eliminates these issues by:

* Maintaining the original IL hydration model found in PetaPoco and Dapper
* Extending it to support an unlimited object graph
* Avoiding object cloning with primary key-based item caches
* Reverse POCO generation based on [System.ComponentModel.DataAnnotations](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations?view=netcore-3.1) and [System.ComponentModel.DataAnnotations.Schema](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.schema?view=netcore-3.1).
* Out-of-the-box support for CRUD operations using the repository pattern
* Providing a fluent query interface for unlimited LEFT and INNER JOINS
* Supporting an ever-increasing subset of set-based SQL predicates
* Supporting MERGE and INSERT operations with table valued parameters (SQL Server) or JSON (PostgreSQL, coming soon)

In future releases, ST/orm will add support for:

* Paging queries
* Query results with dynamics
* MySQL, Oracle, and SQLite 
* Oracle

In addition to functionality, ST/orm is fully customizable, with interfaces for every facet of operation, including Object-Object mapping. ST/orm has been tested with version 9.0.0 of [AutoMapper](https://automapper.org/) with object graph self-referencing intact. That means if it can be mapped, it can be directly hydrated by a ST/orm Entity Repository. (To integrate AutoMapper, install the [Startitecture.Orm.AutoMapper](https://www.nuget.org/packages/Startitecture.Orm.AutoMapper/) package.)

Quick Start Guide
---

To get started, install the [Startitecture.Orm](https://www.nuget.org/packages/Startitecture.Orm/) NuGet package. For setting up POCO entities, refer to the readme.txt in the project.

Contributions
---
To raise an issue or feature request, please visit the [Issues section](https://github.com/Startitecture/Core/issues) of our [GitHub](https://github.com/Startitecture/Core) repository. For faster resolution, please consider a [pull request](https://github.com/Startitecture/Core/pulls) that includes unit tests.

License
---
The code and API documentation for ST/orm are licensed under [Apache 2.0](https://www.apache.org/licenses/LICENSE-2.0).