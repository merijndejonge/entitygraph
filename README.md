# EntityGraph 

## Description
`EntityGraph` is a library that implements the concept of `EntityGraph` as declarative way to define subsets of related entities in a larger data model. Operations like copying, visiting, change notification, comparison can then be defined in a generic fashion rather than in a data model-specific way, having to adjust your data model to support e.g., a visitor pattern.

`EntityGraph` first implemented in the context of WCF RIA Services (https://riaservicescontrib.codeplex.com/), but has now been ported to the DotNet platform. [Documentation](https://github.com/merijndejonge/entitygraph/wiki) has not yet been fully updated.

`EntityGraph` also includes the `DataValidationFramework`, which as a very generic data validation framework supporting, among others instance-based validation and cross-entity validation. 

Both `EntityGraph` and `DataValidationFramework` are demonstrated in the [Car Park Example](https://github.com/merijndejonge/entitygraph/wiki/EntityGraphCarParkExample).

## More info
* Source code of `EntityGraph` is available at [GitHub](https://github.com/merijndejonge/entitygraph). 
* `EntityGraph` is distributed under the [Apache 2.0 License](https://github.com/merijndejonge/entitygraph/blob/master/LICENSE).

