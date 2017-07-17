# DocumentDB HL7 FHIR Api Connector

Microsoft Azure's DocumentDB is a great PaaS NoSQL database. Currently, there isn't a Microsoft DocumentDB Api that can be used efficiently within a Logic App.



**Functionality:**

- CRUD Operations on Documents
 - Create New Document allows for IndexingDirective and Time-To-Live 
- Custom Query for Documents in a Collection.
 - The SQL syntax of DocumentDB provides hierarchical, relational, and spatial query operators
- Execute Stored-procedure 
- List of Stored-procedure Id's in a collection.
  - Stored Procedure names  
- Create Attachment 
  - Post Attachment (Maanaged)
  - Post Attachment MetaData (UnManaged)
- Get List of Attachments for a Document
- Convert DateTime to Unix format
- Create new Collection
- Support for Change Feed


- ## **Support for multiple Partition Keys in all operations**##

----------


## Configure Web.Config ##
You will need to modify the Web.Config file as shown below:


```xml
    <appSettings>
      <add key="masterKey"
           value="Your DocumentDB Primary Key" />
      <add key="endpoint" value="https://<YourAccount>.documents.azure.com:443" />
    </appSettings>
``` 


### Requirements
- DocumentDB Account
  - Creatation of one Database


----------

## Changelog ##

* 1.0.0 - 2016-08-23 - Initial Release
* 1.0.1 - 2016-08-26 - added Unix DateTime Conversion Controller
* 1.0.2 - 2017-02-07 - added ChangeFeed Controller

----------



