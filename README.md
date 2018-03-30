# InfusionsoftOAuthUsingREST

This project is based on the work of [Bradley Ullery](https://github.com/bradleyullery/InfusionsoftOAuth). However, the code has not been updated in over 4 years and is using [Infusionsofts Legacy XML-RPC API](https://developer.infusionsoft.com/docs/xml-rpc/). The FindContact View has been updated to [Infusionsofts REST API](https://developer.infusionsoft.com/docs/rest/#!/Account_Info/getAccountProfileUsingGET).

## Getting Started

* Clone Repo
* Open the controller "HomeController.cs"
* Enter in your "DeveloperAppKey" and "DeveloperAppSecret"
* Run with IIS Express

### Changes

These are the changes that were made from [Bradley Ullery](https://github.com/bradleyullery/InfusionsoftOAuth).
* Updated all Packages to use at least 4.5 Framework *(async requires it)*
* Updated Web.config to work with newer Framework
* Completely reworked FindContact to use REST

## Deployment

This project is for research/resource purposes ONLY. It will not be supported for production environments.

## Authors

* **Bradley Ullery** - *Initial work* - [Bradley Ullery](https://github.com/bradleyullery)
* **Shane Marcotte** - *Re-work* - [Shane Marcotte](https://github.com/smarcotte4g)
